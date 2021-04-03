using Jint.DevToolsProtocol.Helpers;
using System;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public class DomainMethodParameter
    {
        public DomainMethodParameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }

        public override string ToString()
        {
            return $"{Type.GetFriendlyName()} {Name}";
        }
    }
}
