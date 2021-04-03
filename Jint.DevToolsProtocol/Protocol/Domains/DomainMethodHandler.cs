using Jint.DevToolsProtocol.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public delegate object DomainMethodAction(object[] arguments);

    public class DomainMethodHandler
    {
        public DomainMethodHandler(string name, DomainMethodAction action, List<DomainMethodParameter> parameters, Type returnType)
        {
            Name = name;
            Action = action;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public string Name { get; }
        public List<DomainMethodParameter> Parameters { get; }
        public Type ReturnType { get; }
        public DomainMethodAction Action { get; }

        public object Invoke(DomainMethodArguments arguments)
        {
            // Convert arguments to proper type (by way of JSON deserialization)
            var args = new object[Parameters.Count];
            for (int i = 0; i < Parameters.Count; i++)
            {
                var parameter = Parameters[i];
                if (arguments.TryGetValue(parameter.Name, out JsonElement argument))
                {
                    var json = argument.GetRawText();
                    var obj = JsonSerializer.Deserialize(json, parameter.Type);
                    args[i] = obj;
                }
            }

            return Action(args);
        }

        public override string ToString()
        {
            return $"{Name}({String.Join(", ", Parameters)}) => {ReturnType.GetFriendlyName()}";
        }
    }
}
