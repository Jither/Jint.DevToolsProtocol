using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol
{
    public class DevToolsEvent
    {
        public string Method { get; }
        public object Params { get; } // type Object required for serialization

        public DevToolsEvent(string method, DevToolsEventParameters parameters)
        {
            Method = method;
            Params = parameters;
        }
    }

    public abstract class DevToolsEventParameters
    {
    }
}
