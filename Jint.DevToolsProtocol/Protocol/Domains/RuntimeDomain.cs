using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public class RuntimeDomain : Domain
    {
        public RuntimeDomain(Agent agent) : base(agent)
        {
        }

        public override string Name => "Runtime";

        public RuntimeEvaluateResponse AwaitPromise(string promiseObjectId, bool? returnByValue, bool? generatePreview)
        {
            return new RuntimeEvaluateResponse();
        }

        public RuntimeEvaluateResponse CallFunctionOn(string functionDeclaration, string objectId, CallArgument[] arguments,
            bool? silent, bool? returnByValue, bool? generatePreview, bool? userGesture, bool? awaitPromise, int? executionContextId, string objectGroup)
        {
            return new RuntimeEvaluateResponse();
        }

        public CompileScriptResponse CompileScript(string expression, string sourceURL, bool persistScript, int? executionContextId)
        {
            return new CompileScriptResponse();
        }

        public void Disable()
        {

        }

        public void DiscardConsoleEntries()
        {

        }

        public void Enable()
        {

        }

        public RuntimeEvaluateResponse Evaluate(string expression, string objectGroup, bool? includeCommandLineAPI, bool? silent, int? contextId,
            bool? returnByValue, bool? generatePreview, bool? userGesture, bool? awaitPromise, bool? throwOnSideEffect, double? timeout,
            bool? disableBreaks, bool? replMode, bool? allowUnsafeEvalBlockedByCSP, string uniqueContextId)
        {
            return new RuntimeEvaluateResponse();
        }

        public PropertiesResponse GetProperties(string objectId, bool? ownProperties, bool? accessorPropertiesOnly, bool? generatePreview)
        {
            if (!_agent.RuntimeData.ObjectsById.TryGetValue(objectId, out var obj))
            {
                return null;
            }

            var props = obj.GetOwnProperties().Select(pair => new { Name = pair.Key.ToString(), Value = pair.Value.Value });
            return new PropertiesResponse
            {
                Result = props.Select(p => new PropertyDescriptor
                {
                    IsOwn = true,
                    Configurable = false,
                    Name = p.Name,
                    Value = _agent.RuntimeData.GetRemoteObject(p.Value, generatePreview: true)
                }).ToArray()
            };
        }

        public NamesResponse GlobalLexicalScopeNames(int executionContextId)
        {
            return new NamesResponse();
        }

        public ObjectsResponse QueryObjects(string prototypeObjectId, string objectGroup)
        {
            return new ObjectsResponse();
        }

        public void ReleaseObject(string objectId)
        {

        }

        public void ReleaseObjectGroup(string objectGroup)
        {

        }

        public void RunIfWaitingForDebugger()
        {
            _agent.RunIfWaiting();
        }

        public RuntimeEvaluateResponse RunScript(string scriptId, int? executionContextId, string objectGroup, bool? silent, bool? includeCommandLineAPI,
            bool? returnByValue, bool? generatePreview, bool? awaitPromise)
        {
            return new RuntimeEvaluateResponse();
        }

        public void SetAsyncCallStackDepth(int maxDepth)
        {
            
        }

        public void AddBinding(int? executionContextId, string executionContextName)
        {

        }

        public HeapUsageResponse GetHeapUsage()
        {
            return new HeapUsageResponse();
        }

        public IdResponse GetIsolateId()
        {
            return new IdResponse();
        }

        public void RemoveBinding(string name)
        {
            
        }

        public void SetCustomObjectFormatterEnabled(bool enabled)
        {

        }

        public void SetMaxCallStackSizeToCapture(int size)
        {

        }

        public void TerminateExecution()
        {

        }
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
