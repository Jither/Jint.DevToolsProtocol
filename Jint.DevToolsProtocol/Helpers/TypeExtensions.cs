using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jint.DevToolsProtocol.Helpers
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> _typeDictionary = new Dictionary<Type, string>
        {
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(bool), "bool"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(object), "object"},
            {typeof(void), "void"}
        };

        public static string GetFriendlyName(this Type type, Dictionary<Type, string> translations = null)
        {
            if (translations?.ContainsKey(type) == true)
            {
                return translations[type];
            }
            else if (_typeDictionary.ContainsKey(type))
            {
                return _typeDictionary[type];
            }
            else if (type.IsArray)
            {
                var rank = type.GetArrayRank();
                string commas = new string(',', rank - 1);
                return GetFriendlyName(type.GetElementType(), translations) + $"[{commas}]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].GetFriendlyName() + "?";
            }
            else if (type.IsGenericType)
            {
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x))) + ">";
            }
            else
            {
                return type.Name;
            }
        }
    }
}
