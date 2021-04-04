using Jint.DevToolsProtocol.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jint.DevToolsProtocol.Server
{
    public class MinimalServer : IDTPServer
    {
        private const int CloseSocketTimeout = 2500;

        private HttpListener _listener;
        private bool _isRunning = true;

        private CancellationTokenSource _listenerLoopTokenSource;
        private CancellationTokenSource _socketLoopTokenSource;

        private readonly ConcurrentDictionary<int, ConnectedClient> _clients = new ConcurrentDictionary<int, ConnectedClient>();
        private int _socketCounter = 0;

        public MinimalServer(ServerOptions options = null)
        {
            Options = options ?? new ServerOptions();
        }

        public ServerOptions Options { get; }
        public RequestHandlerDelegate HttpRequestHandler { get; set; }
        public RequestHandlerDelegate WebSocketRequestHandler { get; set; }

        public void Start()
        {
            if (HttpRequestHandler == null)
            {
                throw new InvalidOperationException("No HttpRequestHandler");
            }
            if (WebSocketRequestHandler == null)
            {
                throw new InvalidOperationException("No WebSocketRequestHandler");
            }

            _listenerLoopTokenSource = new CancellationTokenSource();
            _socketLoopTokenSource = new CancellationTokenSource();

            _listener = new HttpListener();

            _listener.Prefixes.Add(Options.HttpUri);

            _listener.Start();

            if (!_listener.IsListening)
            {
                throw new ServerException("Server failed to start");
            }

            Logger.Info($"Listening on {Options.HttpUri}...");

            Task.Run(() => ListenerProcessingLoopAsync().ConfigureAwait(false));
        }

        public async Task StopAsync()
        {
            if (_listener?.IsListening == true && _isRunning)
            {
                Logger.Info("Server is stopping...");

                _isRunning = false;
                await CloseAllSocketsAsync();
                _listenerLoopTokenSource.Cancel();
                _listener.Stop();
                _listener.Close();
            }
        }

        public async Task SendEventAsync(string json)
        {
            foreach (var client in _clients)
            {
                await client.Value.SendJsonAsync(json);
            }
        }

        private async Task ListenerProcessingLoopAsync()
        {
            var token = _listenerLoopTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();

                if (!_isRunning)
                {
                    context.ReturnStatus(HttpStatusCode.Conflict, "Server is shutting down");
                    return;
                }

                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext = null;
                    try
                    {
                        wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        int socketId = Interlocked.Increment(ref _socketCounter);
                        var client = new ConnectedClient(socketId, wsContext.WebSocket);
                        _clients.TryAdd(socketId, client);
                        Logger.Info($"Socket {socketId} got new connection");
                        _ = Task.Run(() => SocketProcessingLoopAsync(client).ConfigureAwait(false));
                    }
                    catch (Exception)
                    {
                        context.ReturnStatus(HttpStatusCode.InternalServerError, "Websocket upgrade failed");
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await ProcessHttpRequestAsync(context);
                    }
                    finally
                    {
                        try
                        {
                            await context.Response.OutputStream.FlushAsync();
                            context.Response.Close();
                        }
                        catch (HttpListenerException ex)
                        {
                            Logger.Error($"Error closing response: {ex.Message}");
                        }
                    }
                }
            }
        }

        private async Task SocketProcessingLoopAsync(ConnectedClient client)
        {
            var socket = client.Socket;
            var token = _socketLoopTokenSource.Token;
            try
            {
                var utf8decoder = Encoding.UTF8.GetDecoder();
                var builder = new StringBuilder();
                byte[] buffer = new byte[4096];
                char[] chars = new char[Encoding.UTF8.GetMaxCharCount(buffer.Length)];

                while (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted && !token.IsCancellationRequested)
                {
                    // Read incoming message:
                    builder.Clear();
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        receiveResult = await client.Socket.ReceiveAsync(buffer, token);
                        if (receiveResult.MessageType == WebSocketMessageType.Binary)
                        {
                            await socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary message", CancellationToken.None);
                            return;
                        }
                        int decodedCount = utf8decoder.GetChars(buffer, 0, receiveResult.Count, chars, 0, flush: receiveResult.EndOfMessage);
                        builder.Append(chars, 0, decodedCount);
                    }
                    while (!receiveResult.EndOfMessage && !token.IsCancellationRequested);

                    // Handle message
                    if (!token.IsCancellationRequested)
                    {
                        if (socket.State == WebSocketState.CloseReceived && receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            Logger.Info($"Socket {client.SocketId}: Acknowledging close request from client");
                            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledging close", CancellationToken.None);
                        }

                        if (socket.State == WebSocketState.Open)
                        {
                            await ProcessWebSocketRequestAsync(client, builder.ToString());
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal when token is cancelled
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during websocket communication: {ex.Message}");
            }
            finally
            {
                if (socket.State != WebSocketState.Closed)
                {
                    socket.Abort();
                }

                if (_clients.TryRemove(client.SocketId, out _))
                {
                    socket.Dispose();
                }
            }
        }

        private async Task ProcessHttpRequestAsync(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
            {
                context.ReturnStatus(HttpStatusCode.MethodNotAllowed, "Only HTTP GET requests are allowed");
                return;
            }

            string path = context.Request.Url.AbsolutePath;
            try
            {
                if (!HttpRequestHandler(path, out string json))
                {
                    context.ReturnStatus(HttpStatusCode.NotFound, $"Resource {path} not found");
                    return;
                }

                await WriteJsonAsync(context, json);
            }
            catch (Exception ex)
            {
                context.ReturnStatus(HttpStatusCode.InternalServerError, $"Error generating response: {ex.Message}");
            }
        }

        private async Task ProcessWebSocketRequestAsync(ConnectedClient client, string messageJson)
        {
            if (!WebSocketRequestHandler(messageJson, out string responseJson))
            {
                return;
            }

            await client.SendJsonAsync(responseJson);
        }

        private async Task WriteJsonAsync(HttpListenerContext context, string json)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json; charset=UTF-8";
            context.Response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
            byte[] output = Encoding.UTF8.GetBytes(json);
            context.Response.ContentLength64 = output.Length;
            try
            {
                await context.Response.OutputStream.WriteAsync(output);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error returning result: {ex.Message}");
            }
        }

        private async Task CloseAllSocketsAsync()
        {
            var queue = new List<WebSocket>(_clients.Count);

            while (_clients.Count > 0)
            {
                var client = _clients.ElementAt(0).Value;
                Logger.Info($"Closing socket {client.SocketId}");

                if (client.Socket.State != WebSocketState.Open)
                {
                    Logger.Info($"  not open: {client.Socket.State}");
                }
                else
                {
                    var timeout = new CancellationTokenSource(CloseSocketTimeout);
                    try
                    {
                        Logger.Info("  starting closing handshake");
                        await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", timeout.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        Logger.Info(ex.Message);
                    }
                }

                if (_clients.TryRemove(client.SocketId, out _))
                {
                    queue.Add(client.Socket);
                }

                Logger.Info("  done.");
            }

            _socketLoopTokenSource.Cancel();

            foreach (var socket in queue)
            {
                socket.Dispose();
            }
        }
    }
}
