using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Helpers
{
    public static class ObjectExtensions
    {
        public static string HashCodeToId(this object obj)
        {
            return obj.GetHashCode().ToString("x8");
        }
    }
}
