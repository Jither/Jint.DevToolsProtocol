using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jint.DevToolsProtocol.Handlers
{
    public static class Serialization
    {
        public static JsonSerializerOptions JsonOptions { get; } = CreateJsonOptions();

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var result = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
            result.Converters.Add(new StringEnumMemberConverter());
            return result;
        }
    }
}
