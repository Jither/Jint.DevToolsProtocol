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
        private readonly ServerOptions _options;

        public HttpHandler(ServerOptions options)
        {
            _options = options;
        }

        public bool HandleRequest(string path, out string response)
        {
            switch (path)
            {
                case "/json/version":
                    response = ToJson(new Dictionary<string, string>
                    {
                        ["Browser"] = $"{_options.Name}/{_options.Version}",
                        ["Protocol-Version"] = Agent.ProtocolVersion,
                        ["Jint-Version"] = Agent.JintVersion,
                        ["webSocketDebuggerUrl"] = $"{_options.WebSocketUri}"
                    });
                    return true;
                case "/json":
                case "/json/list":
                    response = ToJson(new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            ["description"] = $"{_options.Name}",
                            ["favIconUrl"] = $"{_options.FavIconUrl}",
                            ["devtoolsFrontendUrl"] = $"devtools://devtools/bundled/inspector.html?ws={_options.WebSocketHost}",
                            ["id"] = null,
                            ["title"] = "Jint Script",
                            ["url"] = null,
                            ["type"] = "other",
                            ["webSocketDebuggerUrl"] = $"{_options.WebSocketUri}"
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
