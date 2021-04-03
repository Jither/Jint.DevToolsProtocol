using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jint.DevToolsProtocol.Server
{
    public class ConnectedClient
    {
        private readonly SemaphoreSlim sendMutex = new SemaphoreSlim(1, 1);

        public ConnectedClient(int socketId, WebSocket socket)
        {
            SocketId = socketId;
            Socket = socket;
        }

        public int SocketId { get; private set; }
        public WebSocket Socket { get; private set; }

        public async Task SendJsonAsync(string json)
        {
            if (Socket.State == WebSocketState.Open)
            {
                await sendMutex.WaitAsync().ConfigureAwait(false);
                try
                { 
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    await Socket.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
                }
                finally
                {
                    sendMutex.Release();
                }
            }
        }
    }
}
