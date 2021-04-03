using System;
using System.Threading.Tasks;

namespace Jint.DevToolsProtocol.Server
{
    public delegate bool RequestHandlerDelegate(string request, out string response);
    public interface IDTPServer
    {
        ServerOptions Options { get; }
        RequestHandlerDelegate HttpRequestHandler { get; set; }
        RequestHandlerDelegate WebSocketRequestHandler { get; set; }

        Task SendEventAsync(string json);
    }
}
