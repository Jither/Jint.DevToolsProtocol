using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Jint.DevToolsProtocol.Server
{
    public static class ListenerContextExtensions
    {
        public static void ReturnStatus(this HttpListenerContext context, HttpStatusCode statusCode, string description)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.StatusDescription = description;
        }
    }
}
