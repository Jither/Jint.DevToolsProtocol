using Jint.DevToolsProtocol.Logging;
using Jint.DevToolsProtocol.Protocol;
using Jint.DevToolsProtocol.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jint.DevToolsProtocol.Handlers
{
    public class WebSocketHandler
    {
        private readonly Agent _agent;
        private readonly IDTPServer _server;

        public WebSocketHandler(Agent agent, IDTPServer server)
        {
            _agent = agent;
            _server = server;
        }

        public bool HandleRequest(string requestJson, out string responseJson)
        {
            Logger.Info("");
            var message = JsonSerializer.Deserialize<DevToolsRequest>(requestJson, Serialization.JsonOptions);
            Logger.Info(message.ToString());
            if (_agent.CallMethod(message, out DevToolsResponse response))
            {
                responseJson = JsonSerializer.Serialize<object>(response, Serialization.JsonOptions);
                Logger.Info($"Response: {responseJson}");
                return true;
            }
            Logger.Warning($"{message.Method} not supported.");
            responseJson = null;
            return false;
        }

        public async Task SendEventAsync(DevToolsEvent evt)
        {
            var json = JsonSerializer.Serialize(evt, Serialization.JsonOptions);
            Logger.Info($"Event: {json}");
            await _server.SendEventAsync(json);
        }
    }
}
