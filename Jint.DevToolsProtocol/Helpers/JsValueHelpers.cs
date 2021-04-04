using Jint.DevToolsProtocol.Logging;
using Jint.DevToolsProtocol.Protocol.Domains;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Date;
using Jint.Native.Error;
using Jint.Native.Function;
using Jint.Native.Map;
using Jint.Native.Proxy;
using Jint.Native.RegExp;
using Jint.Native.Set;
using Jint.Native.Symbol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Helpers
{
    public static class JsValueHelpers
    {
        public static (ObjectType type, ObjectSubType? subtype, object value, string description) GetObjectInfo(JsValue jsValue)
        {
            if (jsValue == null)
            {
                jsValue = JsValue.Undefined;
            }
            ObjectType type;
            ObjectSubType? subtype = null;
            object value;
            string description = null;

            switch (jsValue.Type)
            {
                case Runtime.Types.Boolean:
                    type = ObjectType.Boolean;
                    value = jsValue.AsBoolean();
                    description = jsValue.ToString();
                    break;

                case Runtime.Types.Null:
                    type = ObjectType.Object;
                    subtype = ObjectSubType.Null;
                    value = null;
                    description = jsValue.ToString();
                    break;

                case Runtime.Types.Number:
                    type = ObjectType.Number;
                    value = jsValue.AsNumber();
                    description = jsValue.ToString();
                    break;

                case Runtime.Types.Object:
                    // TODO: ArrayBuffer, DataView, Generator, Iterator, (Node), Promise, TypedArray, WasmValue, WeakMap, WeakSet, WebAssemblyMemory
                    type = jsValue is FunctionInstance ? ObjectType.Function : ObjectType.Object;

                    if (type == ObjectType.Object)
                    {
                        if (jsValue is ArrayInstance)
                        {
                            subtype = ObjectSubType.Array;
                        }
                        else if (jsValue is DateInstance)
                        {
                            subtype = ObjectSubType.Date;
                        }
                        else if (jsValue is RegExpInstance)
                        {
                            subtype = ObjectSubType.RegExp;
                        }
                        else if (jsValue is ErrorInstance)
                        {
                            subtype = ObjectSubType.Error;
                        }
                        else if (jsValue is MapInstance)
                        {
                            subtype = ObjectSubType.Map;
                        }
                        else if (jsValue is SetInstance)
                        {
                            subtype = ObjectSubType.Set;
                        }
                        else if (jsValue is ProxyInstance)
                        {
                            subtype = ObjectSubType.Proxy;
                        }
                    }
                    value = null;
                    // TODO: Not sure why SymbolPrototype.ToString() crashes
                    if (!(jsValue is SymbolPrototype))
                    {
                        description = jsValue.ToString();
                    }
                    break;

                case Runtime.Types.String:
                    type = ObjectType.String;
                    value = jsValue.AsString();
                    description = jsValue.ToString();
                    break;

                case Runtime.Types.Symbol:
                    type = ObjectType.Symbol;
                    value = null;
                    description = jsValue.ToString();
                    break;

                case Runtime.Types.Undefined:
                    type = ObjectType.Undefined;
                    value = JsValue.Undefined;
                    description = jsValue.ToString();
                    break;

                default:
                    throw new ArgumentException($"Unimplemented JsValue type: {jsValue.Type}");
            }

            return (type, subtype, value, description);
        }
    }
}
