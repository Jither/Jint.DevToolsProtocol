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
        private static readonly JsonSerializerOptions _jsonOptions = CreateJsonOptions();
        private readonly Agent _agent;
        private readonly IDTPServer _server;

        public WebSocketHandler(Agent agent, IDTPServer server)
        {
            _agent = agent;
            _server = server;
        }

        public bool HandleRequest(string requestJson, out string responseJson)
        {
            var message = JsonSerializer.Deserialize<DevToolsRequest>(requestJson, _jsonOptions);
            Logger.Info(message.ToString());
            if (_agent.CallMethod(message, out DevToolsResponse response))
            {
                responseJson = JsonSerializer.Serialize<object>(response, _jsonOptions);
                return true;
            }
            Logger.Warning($"{message.Method} not supported.");
            responseJson = null;
            return false;
        }

        public async Task SendEventAsync(DevToolsEvent evt)
        {
            var json = JsonSerializer.Serialize(evt, _jsonOptions);
            await _server.SendEventAsync(json);
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var result = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
            result.Converters.Add(new StringEnumMemberConverter());
            return result;
        }
    }
}
