using System;

namespace Jint.DevToolsProtocol.Server
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message)
        {
        }
    }
}
