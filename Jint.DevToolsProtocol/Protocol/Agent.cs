using Jint.DevToolsProtocol.Handlers;
using Jint.DevToolsProtocol.Logging;
using Jint.DevToolsProtocol.Protocol.Domains;
using Jint.DevToolsProtocol.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol
{
    public class Agent
    {
        public const string ProtocolVersion = "1.3";
        public static readonly string JintVersion = typeof(Engine).Assembly.GetName().Version.ToString();

        private readonly HttpHandler _httpHandler;
        private readonly WebSocketHandler _wsHandler;
        private readonly Dictionary<string, Domain> _domainsByName = new Dictionary<string, Domain>();

        public Agent(Engine engine, IDTPServer server)
        {
            _httpHandler = new HttpHandler(server.Options);
            _wsHandler = new WebSocketHandler(this, server);
            server.HttpRequestHandler = _httpHandler.HandleRequest;
            server.WebSocketRequestHandler = _wsHandler.HandleRequest;

            RuntimeData = new RuntimeData(engine);
            Debugger = new Debugger(this, engine);
            DebuggerDomain = new DebuggerDomain(this, engine);
            RegisterDomain(DebuggerDomain);
            RegisterDomain(new RuntimeDomain(this));
        }

        public RuntimeData RuntimeData { get; }
        public Debugger Debugger { get; }
        public DebuggerDomain DebuggerDomain { get; }
        public event EventHandler Ready;

        public async void TriggerEvent(string method, DevToolsEventParameters parameters)
        {
            Logger.Info($"Sending event: {method}");
            var evt = new DevToolsEvent(method, parameters);
            await _wsHandler.SendEventAsync(evt);
        }

        public void RunIfWaiting()
        {
            Ready?.Invoke(this, EventArgs.Empty);
        }

        public bool CallMethod(DevToolsRequest message, out DevToolsResponse response)
        {
            if (!_domainsByName.TryGetValue(message.Domain, out var domain))
            {
                response = null;
                return false;
            }
            return domain.HandleMessage(message, out response);
        }

        private void RegisterDomain(Domain domain)
        {
            _domainsByName.Add(domain.Name, domain);
        }
    }
}
