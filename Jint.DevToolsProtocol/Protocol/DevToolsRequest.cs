using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jint.DevToolsProtocol.Protocol
{
    public class DomainMethodArguments : Dictionary<string, JsonElement>
    {
        public override string ToString()
        {
            return String.Join(", ", this.Select(arg => $"{arg.Key}={arg.Value}"));
        }
    }

    public class DevToolsRequest
    {
        private string[] _methodParts;
        private string[] MethodParts => _methodParts ??= Method.Split(".");

        public int Id { get; set; }
        public string Method { get; set; }
        public string Domain => MethodParts[0];
        public string Function => MethodParts[1];
        public DomainMethodArguments Params { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Domain}.{Function}({Params})";
        }
    }
}
