using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    public class PendingBreakPoint
    {
        public string Url { get; }
        public Regex UrlRegex { get; }
        public string ScriptHash { get; }

        public int LineNumber { get; }
        public int? ColumnNumber { get; }
        public string Condition { get; }

        public List<BreakPoint> BreakPoints { get; } = new List<BreakPoint>();

        public PendingBreakPoint(string url, Regex urlRegex, string scriptHash, int lineNumber, int? columnNumber, string condition)
        {
            Url = url;
            UrlRegex = urlRegex;
            ScriptHash = scriptHash;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Condition = condition;
        }

        public bool Matches(SourceData source)
        {
            if (Url != null && source.Url == Url)
            {
                return true;
            }
            if (ScriptHash != null && source.Hash == ScriptHash)
            {
                return true;
            }
            if (UrlRegex != null && UrlRegex.IsMatch(source.Url))
            {
                return true;
            }
            return false;
        }
    }

    public class DebuggerDomain : Domain
    {
        private bool _isEnabled;
        private Engine _engine;

        public DebuggerDomain(Agent agent, Engine engine) : base(agent)
        {
            _engine = engine;
            agent.Debugger.Paused += Debugger_Paused;
            agent.Debugger.Resumed += Debugger_Resumed;
        }

        public override string Name => "Debugger";

        private void Debugger_Paused(object sender, DebugInformation e)
        {
            _agent.RuntimeData.DebugInformation = e;
            SendPaused(e);
        }

        private void Debugger_Resumed(object sender, EventArgs e)
        {
            _agent.RuntimeData.DebugInformation = null;
            SendResumed();
        }

        /*
        public void ContinueToLocation(Location location, TargetCallFrames? targetCallFrames)
        {

        }
        */

        public void Disable()
        {
            _isEnabled = false;
        }

        public EnableResponse Enable(double? maxScriptsCacheSize)
        {
            _isEnabled = true;
            SendScripts();
            return new EnableResponse
            {
                DebuggerId = _agent.Debugger.Id
            };
        }

        public DebuggerEvaluateResponse EvaluateOnCallFrame(string callFrameId, string expression, string objectGroup,
            bool? includeCommandLineAPI, bool? silent, bool? returnByValue, bool? generatePreview, bool? throwOnSideEffect, double? timeout)
        {
            if (_agent.RuntimeData.DebugInformation == null)
            {
                return null;
            }

            int callFrameIndex = Int32.Parse(callFrameId);
            if (callFrameIndex == 0)
            {
                try
                {
                    JsValue result = _engine.Execute(expression).GetCompletionValue();
                    return new DebuggerEvaluateResponse
                    {
                        Result = _agent.RuntimeData.GetRemoteObject(result, generatePreview == true)
                    };
                }
                catch (JavaScriptException ex)
                {
                    return new DebuggerEvaluateResponse
                    {
                        Result = null,
                        // TODO: This calls RemoteObject constructor directly - rewrite to store the objectId
                        ExceptionDetails = new ExceptionDetails(ex)
                    };
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return new DebuggerEvaluateResponse
                {
                    Result = _agent.RuntimeData.GetRemoteObject(JsValue.FromObject(_engine, "<evaluation on non-top call frames not supported yet>"))
                };
            }
        }

        public PossibleBreakpointsResponse GetPossibleBreakpoints(Location start, Location end, bool? restrictToFunction)
        {
            if (start.ScriptId != end.ScriptId)
            {
                // "scriptId in start and end range locations should be the same."
                return null;
            }

            if (!_agent.RuntimeData.SourcesByScriptId.TryGetValue(start.ScriptId, out var sourceData))
            {
                return null;
            }

            var startIndex = sourceData.BreakLocations.BinarySearch(new BreakLocation { LineNumber = start.LineNumber, ColumnNumber = start.ColumnNumber });
            var endIndex = sourceData.BreakLocations.BinarySearch(new BreakLocation { LineNumber = end.LineNumber, ColumnNumber = end.ColumnNumber });

            if (startIndex < 0)
            {
                // Get the index of the location that's after our search
                startIndex = ~startIndex;
            }
            if (endIndex < 0)
            {
                // Get the index *before* the location that's after our search
                endIndex = ~endIndex;
                if (endIndex > 0)
                {
                    endIndex--;
                }
            }

            // Start and end indices are inclusive
            int count = endIndex - startIndex + 1;

            // TODO: restrictToFunction

            return new PossibleBreakpointsResponse
            {
                Locations = sourceData.BreakLocations.Skip(startIndex).Take(count).ToArray()
            };
        }

        public ScriptSourceResponse GetScriptSource(string scriptId)
        {
            if (!_agent.RuntimeData.SourcesByScriptId.TryGetValue(scriptId, out var sourceData))
            {
                return null;
            }
            return new ScriptSourceResponse
            {
                ScriptSource = sourceData.Source
            };
        }

        public void Pause()
        {
            _agent.Debugger.Pause();
        }

        public void RemoveBreakpoint(string breakpointId)
        {
            var breakPoints = _agent.RuntimeData.RemoveBreakPoint(breakpointId);
            foreach (var breakPoint in breakPoints)
            {
                _engine.DebugHandler.BreakPoints.Remove(breakPoint);
            }
        }

        /*
        public RestartFrameResponse RestartFrame(string callFrameId)
        {
            return new RestartFrameResponse();
        }
        */

        public void Resume(bool? terminateOnResume)
        {
            // TODO: terminateOnResume
            _agent.Debugger.Run();
        }

        /*
        public SearchResponse SearchInContent(string scriptId, string query, bool? caseSensitive, bool? isRegex)
        {
            return new SearchResponse();
        }
        */

        /*
        public void SetAsyncCallStackDepth(int maxDepth)
        {
            
        }
        */

        public BreakpointResponse SetBreakpoint(Location location, string condition)
        {
            var sourceData = _agent.RuntimeData.SourcesByScriptId[location.ScriptId];
            var actualLocation = sourceData.FindNearestBreak(location);

            var breakPoint = new BreakPoint(sourceData.SourceId, actualLocation.LineNumber + 1, actualLocation.ColumnNumber, condition);
            string id = _agent.RuntimeData.AddBreakPoint(breakPoint);

            return new BreakpointResponse
            {
                ActualLocation = actualLocation,
                BreakpointId = id
            };
        }

        public BreakpointByUrlResponse SetBreakpointByUrl(int lineNumber, string url, string urlRegex, string scriptHash, int? columnNumber, string condition)
        {
            // TODO: Make sure the kinds of regex sent are valid in C#. Maybe use Esprima scanner?
            var rx = urlRegex != null ? new Regex(urlRegex) : null;
            var pending = new PendingBreakPoint(url, rx, scriptHash, lineNumber, columnNumber, condition);
            string id = _agent.RuntimeData.AddPendingBreakPoint(pending);

            var breakPoints = SetBreakPoints(pending);

            return new BreakpointByUrlResponse
            {
                BreakpointId = id,
                Locations = breakPoints.Select(bp => new Location { ScriptId = _agent.RuntimeData.GetScriptId(bp.Source), LineNumber = bp.Line - 1, ColumnNumber = bp.Column }).ToArray()
            };
        }

        public void SetBreakpointsActive(bool active)
        {
            _engine.DebugHandler.BreakPoints.Active = active;
        }

        /*
        public BreakpointIdResponse SetInstrumentationBreakpoint(Instrumentation instrumentation)
        {
            return new BreakpointIdResponse();
        }
        */

        /*
        public void SetPauseOnExceptions(ExceptionsState state)
        {

        }
        */

        /*
        public SetScriptSourceResponse SetScriptSource(string scriptId, string scriptSource, bool? dryRun)
        {
            return new SetScriptSourceResponse();
        }
        */

        /*
        public void SetSkipAllPauses(bool skip)
        {

        }
        */

        /*
        public void SetVariableValue(int scopeNumber, string variableName, CallArgument newValue, string callFrameId)
        {

        }
        */

        public void StepInto(bool? breakOnAsyncCall, LocationRange[] skipList)
        {
            _agent.Debugger.StepInto();
        }

        public void StepOut()
        {
            _agent.Debugger.StepOut();
        }

        public void StepOver(LocationRange[] skipList)
        {
            _agent.Debugger.StepOver();
        }

        /*
        public StackTraceResponse GetStackTrace(StackTraceId stackTraceId)
        {
            return new StackTraceResponse();
        }
        */

        /*
        public void SetBlackboxedRanges(string scriptId, ScriptPosition[] positions)
        {

        }
        */

        /*
        public void SetBlackboxPatterns(string[] patterns)
        {

        }
        */

        /*
        public BreakpointIdResponse SetBreakpointOnFunctionCall(string objectId, string condition)
        {
            return new BreakpointIdResponse();
        }
        */

        /*
        public void SetReturnValue(CallArgument newValue)
        {
            
        }
        */

        /***
         * Events
         **/

        internal void SendPaused(DebugInformation info)
        {
            var evt = new PausedEvent
            {
                CallFrames = info.CallStack.Select((frame, index) => new CallFrame
                {
                    CallFrameId = index.ToString(),
                    FunctionName = frame.FunctionName,
                    Location = new Location(frame.Location.Start, _agent.RuntimeData.GetScriptId(frame.Location.Source)),
                    FunctionLocation = frame.FunctionLocation != null ? new Location(frame.FunctionLocation.Value.Start, frame.FunctionLocation.Value.Source) : null,
                    ScopeChain = CreateScopeChain(frame),
                    This = _agent.RuntimeData.GetRemoteObject(frame.This, generatePreview: true)
                }).ToArray(),
                Reason = PauseReason.DebugCommand
            };

            TriggerEvent("paused", evt);
        }

        internal void SendResumed()
        {
            TriggerEvent("resumed", new ResumedEvent());
        }

        internal void SendScripts()
        {
            if (!_isEnabled)
            {
                return;
            }
            var sources = _agent.RuntimeData.SourcesByScriptId;

            foreach (var source in sources.Values)
            {
                if (source.Sent)
                {
                    continue;
                }
                var evt = new ScriptParsedEvent
                {
                    ScriptId = source.ScriptId,
                    Url = source.Url,
                    StartLine = 0,
                    StartColumn = 0,
                    EndLine = source.End.LineNumber,
                    EndColumn = source.End.ColumnNumber,
                    ExecutionContextId = 1, // TODO: Proper execution context
                    Hash = source.Hash,
                    IsModule = false,
                    IsLiveEdit = false,
                    Length = source.Length,
                    ScriptLanguage = ScriptLanguage.JavaScript
                };
                source.Sent = true;

                TriggerEvent("scriptParsed", evt);
            }
        }

        private Scope[] CreateScopeChain(Jint.Runtime.Debugger.CallFrame frame)
        {
            return frame.ScopeChain.Select(scope => new Scope
            {
                Type = scope.ScopeType switch
                {
                    DebugScopeType.Block => ScopeType.Block,
                    DebugScopeType.Catch => ScopeType.Catch,
                    DebugScopeType.Closure => ScopeType.Closure,
                    DebugScopeType.Eval => ScopeType.Eval,
                    DebugScopeType.Global => ScopeType.Global,
                    DebugScopeType.Local => ScopeType.Local,
                    DebugScopeType.Module => ScopeType.Module,
                    DebugScopeType.Script => ScopeType.Script,
                    DebugScopeType.WasmExpressionStack => ScopeType.WasmExpressionStack,
                    DebugScopeType.With => ScopeType.With,
                    _ => throw new NotImplementedException($"Scope type {scope.ScopeType} is not supported")
                },
                // For global and with scopes [Object] represents the actual object;
                // for the rest of the scopes, it is artificial transient object enumerating scope variables as its properties.
                Object = _agent.RuntimeData.GetRemoteObject(scope)
            }).ToArray();
        }

        private List<BreakPoint> SetBreakPoints(PendingBreakPoint definition)
        {
            var result = new List<BreakPoint>();
            var sourceDatas = _agent.RuntimeData.FindSources(definition);
            foreach (var sourceData in sourceDatas)
            {
                var actualLocation = sourceData.FindNearestBreak(new Location { LineNumber = definition.LineNumber, ColumnNumber = definition.ColumnNumber ?? 0 });

                var breakPoint = new BreakPoint(sourceData.SourceId, actualLocation.LineNumber + 1, actualLocation.ColumnNumber, definition.Condition);
                _engine.DebugHandler.BreakPoints.Add(breakPoint);
                result.Add(breakPoint);
            }
            definition.BreakPoints.AddRange(result);
            return result;
        }
    }

    /***
     * Responses
     **/

    public class EnableResponse
    {
        public string DebuggerId { get; set; }
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
        public int ExecutionContextId { get; set; }
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
