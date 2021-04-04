using Esprima.Ast;
using Jint.DevToolsProtocol.Helpers;
using Jint.DevToolsProtocol.Protocol.Domains;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol
{
    public class RuntimeData
    {
        private readonly Engine _engine;
        public Dictionary<string, SourceData> SourcesById { get; } = new Dictionary<string, SourceData>();
        public Dictionary<string, SourceData> SourcesByDebuggerId { get; } = new Dictionary<string, SourceData>();
        public Dictionary<string, ObjectInstance> ObjectsById { get; } = new Dictionary<string, ObjectInstance>();
        public Dictionary<ObjectInstance, string> IdsByObject { get; } = new Dictionary<ObjectInstance, string>();

        public RuntimeData(Engine engine)
        {
            _engine = engine;
        }

        public SourceData AddSource(string sourceId, string url, string source, Script ast)
        {
            if (!SourcesById.TryGetValue(sourceId, out var data))
            {
                string debuggerId = $"jint:{SourcesByDebuggerId.Count + 1}";
                data = new SourceData(debuggerId, url, source, ast);
                SourcesById.Add(sourceId, data);
                SourcesByDebuggerId.Add(debuggerId, data);
            }
            return data;
        }

        public string GetScriptId(string sourceId)
        {
            if (!SourcesById.TryGetValue(sourceId, out var data))
            {
                return null;
            }
            return data.Id;
        }

        public RemoteObject GetRemoteObject(Jint.Runtime.Debugger.DebugScope scope)
        {
            if (scope.BindingObject != null)
            {
                return GetRemoteObject(scope.BindingObject, generatePreview: true);
            }

            // Create transient object for scope object
            // TODO: Fill it
            ObjectInstance obj = _engine.Object.Construct(Array.Empty<JsValue>());
            foreach (var name in scope.BindingNames)
            {
                obj.FastAddProperty(
                    name, 
                    // let/const may be null until initialization - Chromium shows them as undefined
                    scope.GetBindingValue(name) ?? JsValue.Undefined,
                    false, false, false);
            }
            return GetRemoteObject(obj, generatePreview: true);
        }

        public RemoteObject GetRemoteObject(JsValue value, bool generatePreview = false)
        {
            string id = null;
            if (value is ObjectInstance obj)
            {
                if (!this.IdsByObject.TryGetValue(obj, out id))
                {
                    id = ObjectsById.Count.ToString() + 1;
                    IdsByObject.Add(obj, id);
                    ObjectsById.Add(id, obj);
                }
            }

            return new RemoteObject(value, generatePreview)
            {
                ObjectId = id
            };
        }
    }

    public class SourceData
    {
        public SourceData(string id, string url, string source, Script ast)
        {
            Id = id;
            Url = url;
            Source = source;
            Ast = ast;
            End = source.FindEndPosition();
            // TODO: For now we use GetHashCode - Chrome uses 160 bit hash
            Hash = source.HashCodeToId();
        }

        public string Id { get; }
        public string Url { get; }
        public string Source { get; }
        public Script Ast { get; }
        public ScriptPosition End { get; }
        public string Hash { get; }
        public int Length => Source.Length;

        public bool Sent { get; set; }
    }
}
