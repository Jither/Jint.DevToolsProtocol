using Jint.Native;
using Jint.Native.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public class RuntimeDomain : Domain
    {
        private bool _enabled;

        public RuntimeDomain(Agent agent) : base(agent)
        {
        }

        public override string Name => "Runtime";

        /*
        public RuntimeEvaluateResponse AwaitPromise(string promiseObjectId, bool? returnByValue, bool? generatePreview)
        {
            return new RuntimeEvaluateResponse();
        }
        */

        /*
        public RuntimeEvaluateResponse CallFunctionOn(string functionDeclaration, string objectId, CallArgument[] arguments,
            bool? silent, bool? returnByValue, bool? generatePreview, bool? userGesture, bool? awaitPromise, int? executionContextId, string objectGroup)
        {
            return new RuntimeEvaluateResponse();
        }
        */

        /*
        public CompileScriptResponse CompileScript(string expression, string sourceURL, bool persistScript, int? executionContextId)
        {
            return new CompileScriptResponse();
        }
        */

        public void Disable()
        {
            this._enabled = false;
        }

        /*
        public void DiscardConsoleEntries()
        {

        }
        */

        public void Enable()
        {
            this._enabled = true;
        }

        /*
        public RuntimeEvaluateResponse Evaluate(string expression, string objectGroup, bool? includeCommandLineAPI, bool? silent, int? contextId,
            bool? returnByValue, bool? generatePreview, bool? userGesture, bool? awaitPromise, bool? throwOnSideEffect, double? timeout,
            bool? disableBreaks, bool? replMode, bool? allowUnsafeEvalBlockedByCSP, string uniqueContextId)
        {
            return new RuntimeEvaluateResponse();
        }
        */

        private class Prop
        {
            public string Name { get; }
            public Runtime.Descriptors.PropertyDescriptor Descriptor { get; }
            public bool IsOwn { get; }
            public JsValue Value => Descriptor.Value;

            public Prop(KeyValuePair<JsValue, Runtime.Descriptors.PropertyDescriptor> prop, bool isOwn = true)
            {
                Name = prop.Key.ToString();
                Descriptor = prop.Value;
                IsOwn = isOwn;
            }
        }

        public PropertiesResponse GetProperties(string objectId, bool? ownProperties, bool? accessorPropertiesOnly, bool? generatePreview)
        {
            var data = _agent.RuntimeData.GetObject(objectId);
            if (data == null)
            {
                return null;
            }
            var obj = data.Instance;

            var props = obj.GetOwnProperties().Select(p => new Prop(p, isOwn: true));

            // TODO: Entire prototype chain?
            if (!data.IsScope && ownProperties != true && obj.Prototype != null)
            {
                props = props.Concat(obj.Prototype.GetOwnProperties().Select(p => new Prop(p, isOwn: false)));
            }

            if (accessorPropertiesOnly == true)
            {
                props = props.Where(p => p.Descriptor is Runtime.Descriptors.GetSetPropertyDescriptor);
            }
            return new PropertiesResponse
            {
                Result = props.Select(p => new PropertyDescriptor
                {
                    Name = p.Name,
                    Value = p.Value != null ? _agent.RuntimeData.GetRemoteObject(p.Value, generatePreview: generatePreview == true) : null,
                    IsOwn = p.IsOwn,
                    Configurable = p.Descriptor.Configurable,
                    Enumerable = p.Descriptor.Enumerable,
                    Writable = p.Descriptor.Writable,
                    Get = p.Descriptor.Get != null ? _agent.RuntimeData.GetRemoteObject(p.Descriptor.Get) : null,
                    Set = p.Descriptor.Set != null ? _agent.RuntimeData.GetRemoteObject(p.Descriptor.Set) : null,
                    // TODO: Thrown
                    // TODO: Symbol
                }).ToArray()
            };
        }

        /*
        public NamesResponse GlobalLexicalScopeNames(int executionContextId)
        {
            return new NamesResponse();
        }
        */

        /*
        public ObjectsResponse QueryObjects(string prototypeObjectId, string objectGroup)
        {
            return new ObjectsResponse();
        }
        */

        public void ReleaseObject(string objectId)
        {
            _agent.RuntimeData.ReleaseObject(objectId);
        }

        public void ReleaseObjectGroup(string objectGroup)
        {
            _agent.RuntimeData.ReleaseObjectGroup(objectGroup);
        }

        public void RunIfWaitingForDebugger()
        {
            _agent.RunIfWaiting();
        }

        /*
        public RuntimeEvaluateResponse RunScript(string scriptId, int? executionContextId, string objectGroup, bool? silent, bool? includeCommandLineAPI,
            bool? returnByValue, bool? generatePreview, bool? awaitPromise)
        {
            return new RuntimeEvaluateResponse();
        }
        */

        /*
        public void SetAsyncCallStackDepth(int maxDepth)
        {
            
        }
        */

        /*
        public void AddBinding(int? executionContextId, string executionContextName)
        {

        }
        */

        /*
        public HeapUsageResponse GetHeapUsage()
        {
            return new HeapUsageResponse();
        }
        */

        /*
        public IdResponse GetIsolateId()
        {
            return new IdResponse();
        }
        */

        /*
        public void RemoveBinding(string name)
        {
            
        }
        */

        /*
        public void SetCustomObjectFormatterEnabled(bool enabled)
        {

        }
        */

        /*
        public void SetMaxCallStackSizeToCapture(int size)
        {

        }
        */

        /*
        public void TerminateExecution()
        {

        }
        */
    }

    /***
     * Responses
     **/

    public class RuntimeEvaluateResponse
    {
        public RemoteObject Result { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class CompileScriptResponse
    {
        public string ScriptId { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class PropertiesResponse
    {
        public PropertyDescriptor[] Result { get; set; }
        public InternalPropertyDescriptor[] InternalProperties { get; set; }
        public PrivatePropertyDescriptor[] PrivateProperties { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class NamesResponse
    {
        public string[] Names { get; set; }
    }

    public class ObjectsResponse
    {
        public RemoteObject Objects { get; set; }
    }

    public class HeapUsageResponse
    {
        public double UsedSize { get; set; }
        public double TotalSize { get; set; }
    }

    public class IdResponse
    {
        public string Id { get; set; }
    }

    /***
     * Events
     **/
    public class ConsoleAPICalledEvent : DevToolsEventParameters
    {
        public ConsoleCallType Type { get; set; }
        public CallArgument[] Args { get; set; }
        public string ExecutionContextId { get; set; }
        public double Timestamp { get; set; }
        public StackTrace StackTrace { get; set; }
        public string Context { get; set; }
    }

    public class ExceptionRevokedEvent : DevToolsEventParameters
    {
        public string Reason { get; set; }
        public int ExceptionId { get; set; }
    }

    public class ExceptionThrownEvent : DevToolsEventParameters
    {
        public double Timestamp { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class ExecutionContextCreatedEvent : DevToolsEventParameters
    {
        public ExecutionContextDescription Context { get; set; }
    }

    public class ExecutionContextDestroyedEvent : DevToolsEventParameters
    {
        public int ExecutionContextId { get; set; }
    }

    public class ExecutionContextsClearedEvent : DevToolsEventParameters
    {

    }

    public class InspectRequestedEvent : DevToolsEventParameters
    {
        public RemoteObject Object { get; set; }
        public object Hints { get; set; }
    }

    public class BindingCalledEvent : DevToolsEventParameters
    {
        public string Name { get; set; }
        public string Payload { get; set; }
        public int ExecutionContextId { get; set; }
    }
}
