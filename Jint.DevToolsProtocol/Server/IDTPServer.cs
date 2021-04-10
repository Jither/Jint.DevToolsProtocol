using System;
using System.Threading.Tasks;

namespace Jint.DevToolsProtocol.Server
{
    public delegate bool RequestHandlerDelegate(string request, out string response);

    public interface IDTPServer
    {
        // Properties mainly used by HttpHandler for metadata:
        string Name { get; }
        string Version { get; }
        string WebSocketUri { get; }
        string WebSocketHost { get; }
        string FavIconUrl { get; }
        
        RequestHandlerDelegate HttpRequestHandler { get; set; }
        RequestHandlerDelegate WebSocketRequestHandler { get; set; }

        Task SendEventAsync(string json);
    }
}
