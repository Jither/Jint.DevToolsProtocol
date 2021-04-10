using Jint.DevToolsProtocol.Protocol;
using Jint.DevToolsProtocol.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jint.DevToolsProtocol.Handlers
{
    public class HttpHandler
    {
        private readonly IDTPServer _server;

        public HttpHandler(IDTPServer server)
        {
            _server = server;
        }

        public bool HandleRequest(string path, out string response)
        {
            switch (path)
            {
                case "/json/version":
                    response = ToJson(new Dictionary<string, string>
                    {
                        ["Browser"] = $"{_server.Name}/{_server.Version}",
                        ["Protocol-Version"] = Agent.ProtocolVersion,
                        ["Jint-Version"] = Agent.JintVersion,
                        ["webSocketDebuggerUrl"] = $"{_server.WebSocketUri}"
                    });
                    return true;
                case "/json":
                case "/json/list":
                    response = ToJson(new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            ["description"] = $"{_server.Name}",
                            ["favIconUrl"] = $"{_server.FavIconUrl}",
                            ["devtoolsFrontendUrl"] = $"devtools://devtools/bundled/js_app.html?ws={_server.WebSocketHost}&v8only=true",
                            ["id"] = null,
                            ["title"] = "Jint Script",
                            ["url"] = null,
                            ["type"] = "other",
                            ["webSocketDebuggerUrl"] = $"{_server.WebSocketUri}"
                        }
                    });
                    return true;
                default:
                    response = null;
                    return false;
            }
        }

        private string ToJson(object response)
        {
            return JsonSerializer.Serialize(response);
        }
    }
}
