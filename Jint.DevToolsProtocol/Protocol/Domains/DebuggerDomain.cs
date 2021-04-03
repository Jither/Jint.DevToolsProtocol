using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public class DebuggerDomain : Domain
    {
        public DebuggerDomain(Agent agent) : base(agent)
        {
        }

        public override string Name => "Debugger";

        public void ContinueToLocation(Location location, TargetCallFrames? targetCallFrames)
        {

        }

        public void Disable()
        {

        }

        public EnableResponse Enable(double? maxScriptsCacheSize)
        {
            return new EnableResponse();
        }

        public DebuggerEvaluateResponse EvaluateOnCallFrame(string callFrameId, string expression, string objectGroup,
            bool? includeCommandLineAPI, bool? silent, bool? returnByValue, bool? generatePreview, bool? throwOnSideEffect, double? timeout)
        {
            return new DebuggerEvaluateResponse();
        }

        public PossibleBreakpointsResponse GetPossibleBreakpoints(Location start, Location end, bool? restrictToFunction)
        {
            return new PossibleBreakpointsResponse();
        }

        public ScriptSourceResponse GetScriptSource(string scriptId)
        {
            return new ScriptSourceResponse();
        }

        public void Pause()
        {

        }

        public void RemoveBreakpoint(string breakpointId)
        {

        }

        public RestartFrameResponse RestartFrame(string callFrameId)
        {
            return new RestartFrameResponse();
        }

        public void Resume(bool? terminateOnResume)
        {

        }

        public SearchResponse SearchInContent(string scriptId, string query, bool? caseSensitive, bool? isRegex)
        {
            return new SearchResponse();
        }

        public void SetAsyncCallStackDepth(int maxDepth)
        {
            
        }

        public BreakpointResponse SetBreakpoint(Location location, string condition)
        {
            return new BreakpointResponse();
        }

        public BreakpointByUrlResponse SetBreakpointByUrl(int lineNumber, string url, string urlRegex, string scriptHash, int? columnNumber, string condition)
        {
            return new BreakpointByUrlResponse();
        }

        public void SetBreakpointsActive(bool active)
        {

        }

        public BreakpointIdResponse SetInstrumentationBreakpoint(Instrumentation instrumentation)
        {
            return new BreakpointIdResponse();
        }

        public void SetPauseOnExceptions(ExceptionsState state)
        {

        }

        public SetScriptSourceResponse SetScriptSource(string scriptId, string scriptSource, bool? dryRun)
        {
            return new SetScriptSourceResponse();
        }

        public void SetSkipAllPauses(bool skip)
        {

        }

        public void SetVariableValue(int scopeNumber, string variableName, CallArgument newValue, string callFrameId)
        {

        }

        public void StepInto(bool? breakOnAsyncCall, LocationRange[] skipList)
        {

        }

        public void StepOut()
        {

        }

        public void StepOver(LocationRange[] skipList)
        {
            
        }

        public StackTraceResponse GetStackTrace(StackTraceId stackTraceId)
        {
            return new StackTraceResponse();
        }

        public void SetBlackboxedRanges(string scriptId, ScriptPosition[] positions)
        {

        }

        public void SetBlackboxPatterns(string[] patterns)
        {

        }

        public BreakpointIdResponse SetBreakpointOnFunctionCall(string objectId, string condition)
        {
            return new BreakpointIdResponse();
        }

        public void SetReturnValue(CallArgument newValue)
        {
            
        }
    }

    /***
     * Responses
     **/

    public class EnableResponse
    {
        public string UniqueDebuggerId { get; set; }
    }

    public class DebuggerEvaluateResponse
    {
        public RemoteObject Result { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class PossibleBreakpointsResponse
    {
        public BreakLocation[] Locations { get; set; }
    }

    public class ScriptSourceResponse
    {
        public string ScriptSource { get; set; }
        public string ByteCode { get; set; }
    }

    public class RestartFrameResponse
    {
        public CallFrame[] CallFrames { get; set; }
        public StackTrace AsyncStackTrace { get; set; }
        public StackTraceId AsyncStackTraceId { get; set; }
    }

    public class SearchResponse
    {
        public SearchMatch[] Result { get; set; }
    }

    public class BreakpointResponse
    {
        public string BreakpointId { get; set; }
        public Location ActualLocation { get; set; }
    }

    public class BreakpointByUrlResponse
    {
        public string BreakpointId { get; set; }
        public Location[] Locations { get; set; }
    }

    public class BreakpointIdResponse
    {
        public string BreakpointId { get; set; }
    }

    public class SetScriptSourceResponse
    {
        public CallFrame[] CallFrames { get; set; }
        public bool? StackChanged { get; set; }
        public StackTrace AsyncStackTrace { get; set; }
        public StackTraceId AsyncStackTraceId { get; set; }
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class StackTraceResponse
    {
        public StackTrace StackTrace { get; set; }
    }

    /***
     * Events
     **/

    public class BreakpointResolvedEvent : DevToolsEventParameters
    {
        public string BreakpointId { get; set; }
        public Location Location { get; set; }
    }

    public class PausedEvent : DevToolsEventParameters
    {
        public CallFrame[] CallFrames { get; set; }
        public PauseReason Reason { get; set; }
        public object Data { get; set; }
        public string[] HitBreakpoints { get; set; }
        public StackTrace AsyncStackTrace { get; set; }
        public StackTraceId AsyncStackTraceId { get; set; }
    }

    public class ResumedEvent : DevToolsEventParameters
    {
    }

    public class ScriptFailedToParseEvent : DevToolsEventParameters
    {
        public string ScriptId { get; set; }
        public string Url { get; set; }
        public int StartLine { get; set; }
        public int StartColumn { get; set; }
        public int EndLine { get; set; }
        public int EndColumn { get; set; }
        public string ExecutionContextId { get; set; }
        public string Hash { get; set; }
        public object ExecutionContextAuxData { get; set; }
        public string SourceMapURL { get; set; }
        public bool? HasSourceURL { get; set; }
        public bool? IsModule { get; set; }
        public int? Length { get; set; }
        public StackTrace StackTrace { get; set; }
        public int? CodeOffset { get; set; }
        public ScriptLanguage? ScriptLanguage { get; set; }
        public string EmbedderName { get; set; }
    }

    public class ScriptParsedEvent : DevToolsEventParameters
    {
        public string ScriptId { get; set; }
        public string Url { get; set; }
        public int StartLine { get; set; }
        public int StartColumn { get; set; }
        public int EndLine { get; set; }
        public int EndColumn { get; set; }
        public string ExecutionContextId { get; set; }
        public string Hash { get; set; }
        public object ExecutionContextAuxData { get; set; }
        public bool? IsLiveEdit { get; set; }
        public string SourceMapURL { get; set; }
        public bool? HasSourceURL { get; set; }
        public bool? IsModule { get; set; }
        public int? Length { get; set; }
        public StackTrace StackTrace { get; set; }
        public int? CodeOffset { get; set; }
        public ScriptLanguage ScriptLanguage { get; set; }
        public DebugSymbols DebugSymbols { get; set; }
        public string EmbedderName { get; set; }
    }
}
