using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol
{
    public class DevToolsResponse
    {
        public int Id { get; set; }
        public object Result { get; set; }

        public DevToolsResponse(DevToolsRequest request, object result)
        {
            Id = request.Id;
            Result = result;
        }
    }
}
