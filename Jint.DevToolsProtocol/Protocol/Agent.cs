using Jint.DevToolsProtocol.Handlers;
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

        public Agent(IDTPServer server)
        {
            this._httpHandler = new HttpHandler(server.Options);
            this._wsHandler = new WebSocketHandler(this, server);
            server.HttpRequestHandler = _httpHandler.HandleRequest;
            server.WebSocketRequestHandler = _wsHandler.HandleRequest;

            RegisterDomain(new DebuggerDomain(this));
            RegisterDomain(new RuntimeDomain(this));
        }

        public async void TriggerEvent(DevToolsEvent evt)
        {
            await this._wsHandler.SendEventAsync(evt);
        }

        public bool CallMethod(DevToolsRequest message, out DevToolsResponse response)
        {
            if (!_domainsByName.TryGetValue(message.Function, out var domain))
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
