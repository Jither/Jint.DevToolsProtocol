using Jint.DevToolsProtocol.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public abstract class Domain
    {
        private readonly Agent _agent;
        private static readonly HashSet<string> EXCLUDE_METHODS = new HashSet<string> { "HandleMessage", "ToString", "GetHashCode", "Equals", "GetType", "get_Name" };
        private readonly Dictionary<string, DomainMethodHandler> _handlersByName = new Dictionary<string, DomainMethodHandler>();

        protected Domain(Agent agent)
        {
            this._agent = agent;
            RegisterHandlers();
        }

        public abstract string Name { get; }

        public bool HandleMessage(DevToolsRequest message, out DevToolsResponse result)
        {
            if (_handlersByName.TryGetValue(message.Function, out DomainMethodHandler handler))
            {
                var returnValue = handler.Invoke(message.Params);
                result = new DevToolsResponse(message, returnValue);
                return true;
            }
            result = null;
            return false;
        }

        protected void TriggerEvent(string method, DevToolsEventParameters parameters)
        {
            _agent.TriggerEvent(new DevToolsEvent($"{Name}.{method}", parameters));
        }

        private void RegisterHandlers()
        {
            var methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var methodInfo in methodInfos)
            {
                if (EXCLUDE_METHODS.Contains(methodInfo.Name))
                {
                    continue;
                }
                var name = Char.ToLower(methodInfo.Name[0]) + methodInfo.Name[1..];
                var parameterInfos = methodInfo.GetParameters();
                var parameters = new List<DomainMethodParameter>(parameterInfos.Length);
                foreach (var parameterInfo in parameterInfos)
                {
                    string parameterName = parameterInfo.Name;
                    Type parameterType = parameterInfo.ParameterType;
                    parameters.Add(new DomainMethodParameter(parameterName, parameterType));
                }
                var returnType = methodInfo.ReturnType;

                var action = new DomainMethodAction(args => methodInfo.Invoke(this, args));

                var handler = new DomainMethodHandler(name, action, parameters, returnType);
                _handlersByName.Add(name, handler);
                Logger.Info($"Registered {Name}.{handler}");
            }
        }


    }
}
