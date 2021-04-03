using Jint.Runtime.Debugger;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol.Domains
{
    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-BreakLocation
    public enum BreakLocationType
    {
        [EnumMember(Value = "debuggerStatement")]
        DebuggerStatement,
        [EnumMember(Value = "call")]
        Call,
        [EnumMember(Value = "return")]
        Return
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-ScriptLanguage
    public enum ScriptLanguage
    {
        JavaScript,
        WebAssembly
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-DebugSymbols
    public enum DebugSymbolsType
    {
        None,
        SourceMap,
        EmbeddedDWARF,
        ExternalDWARF
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#event-paused
    public enum PauseReason
    {
        [EnumMember(Value = "ambiguous")]
        Ambiguous,
        [EnumMember(Value = "assert")]
        Assert,
        CSPViolation,
        [EnumMember(Value = "debugCommand")]
        DebugCommand,
        DOM,
        EventListener,
        [EnumMember(Value = "exception")]
        Exception,
        [EnumMember(Value = "instrumentation")]
        Instrumentation,
        OOM,
        [EnumMember(Value = "other")]
        Other,
        [EnumMember(Value = "promiseRejection")]
        PromiseRejection,
        XHR
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-Scope
    public enum ScopeType
    {
        [EnumMember(Value = "global")]
        Global,
        [EnumMember(Value = "script")]
        Script,
        [EnumMember(Value = "local")]
        Local,
        [EnumMember(Value = "block")]
        Block,
        [EnumMember(Value = "catch")]
        Catch,
        [EnumMember(Value = "closure")]
        Closure,
        [EnumMember(Value = "with")]
        With,
        [EnumMember(Value = "eval")]
        Eval,
        [EnumMember(Value = "module")]
        Module,
        [EnumMember(Value = "wasm-expression-stack")]
        WasmExpressionStack
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#method-setInstrumentationBreakpoint
    public enum Instrumentation
    {
        [EnumMember(Value = "beforeScriptExecution")]
        BeforeScriptExecution,
        [EnumMember(Value = "beforeScriptWithSourceMapExecution")]
        BeforeScriptWithSourceMapExecution
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#method-continueToLocation
    public enum TargetCallFrames
    {
        [EnumMember(Value = "any")]
        Any,
        [EnumMember(Value = "current")]
        Current
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#method-setPauseOnExceptions
    public enum ExceptionsState
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "uncaught")]
        Uncaught,
        [EnumMember(Value = "all")]
        All
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#event-consoleAPICalled
    public enum ConsoleCallType
    {
        [EnumMember(Value = "log")]
        Log,
        [EnumMember(Value = "debug")]
        Debug,
        [EnumMember(Value = "info")]
        Info,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "warning")]
        Warning,
        [EnumMember(Value = "dir")]
        Dir,
        [EnumMember(Value = "dirxml")]
        Dirxml,
        [EnumMember(Value = "table")]
        Table,
        [EnumMember(Value = "trace")]
        Trace,
        [EnumMember(Value = "clear")]
        Clear,
        [EnumMember(Value = "startGroup")]
        StartGroup,
        [EnumMember(Value = "startGroupCollapsed")]
        StartGroupCollapsed,
        [EnumMember(Value = "endGroup")]
        EndGroup,
        [EnumMember(Value = "assert")]
        Assert,
        [EnumMember(Value = "profile")]
        Profile,
        [EnumMember(Value = "profileEnd")]
        ProfileEnd,
        [EnumMember(Value = "count")]
        Count,
        [EnumMember(Value = "timeEnd")]
        TimeEnd
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-RemoteObject
    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-ObjectPreview
    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-PropertyPreview
    public enum ObjectType
    {
        [EnumMember(Value = "object")]
        Object,
        [EnumMember(Value = "function")]
        Function,
        [EnumMember(Value = "undefined")]
        Undefined,
        [EnumMember(Value = "string")]
        String,
        [EnumMember(Value = "number")]
        Number,
        [EnumMember(Value = "boolean")]
        Boolean,
        [EnumMember(Value = "symbol")]
        Symbol,
        [EnumMember(Value = "bigint")]
        Bigint
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-RemoteObject
    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-ObjectPreview
    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-PropertyPreview
    public enum ObjectSubType
    {
        [EnumMember(Value = "array")]
        Array,
        [EnumMember(Value = "null")]
        Null,
        [EnumMember(Value = "node")]
        Node,
        [EnumMember(Value = "regexp")]
        RegExp,
        [EnumMember(Value = "date")]
        Date,
        [EnumMember(Value = "map")]
        Map,
        [EnumMember(Value = "set")]
        Set,
        [EnumMember(Value = "weakmap")]
        WeakMap,
        [EnumMember(Value = "weakset")]
        WeakSet,
        [EnumMember(Value = "iterator")]
        Iterator,
        [EnumMember(Value = "generator")]
        Generator,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "proxy")]
        Proxy,
        [EnumMember(Value = "promise")]
        Promise,
        [EnumMember(Value = "typedarray")]
        TypedArray,
        [EnumMember(Value = "arraybuffer")]
        ArrayBuffer,
        [EnumMember(Value = "dataview")]
        DataView,
        [EnumMember(Value = "webassemblymemory")]
        WebAssemblyMemory,
        [EnumMember(Value = "wasmvalue")]
        WasmValue
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-BreakLocation
    public class BreakLocation : Location
    {
        public BreakLocationType Type { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-DebugSymbols
    public class DebugSymbols
    {
        public DebugSymbolsType Type { get; set; }
        public string ExternalURL { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-Location
    public class Location : ScriptPosition
    {
        public string ScriptId { get; set; }

        public Location()
        {

        }

        public Location(Esprima.Position esprimaPosition, string source) : base(esprimaPosition)
        {
            ScriptId = source;
        }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-Scope
    public class Scope
    {
        public ScopeType Type { get; set; }
        public RemoteObject Object { get; set; }
        public string Name { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }

        public Scope()
        {

        }

        public Scope(DebugScope scope)
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
            };
            //Object = RuntimeHelpers.GetRemoteObject(scope);
        }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-SearchMatch
    public class SearchMatch
    {
        public int LineNumber { get; set; }
        public string LineContent { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-LocationRange
    public class LocationRange
    {
        public string ScriptId { get; set; }
        public ScriptPosition Start { get; set; }
        public ScriptPosition End { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-ScriptPosition
    public class ScriptPosition
    {
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

        public ScriptPosition()
        {

        }

        public ScriptPosition(Esprima.Position esprimaPosition) : this(esprimaPosition.Line - 1, esprimaPosition.Column)
        {

        }

        public ScriptPosition(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }
    }

    /***
     * Runtime
     **/

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-CallArgument
    public class CallArgument
    {
        public object Value { get; set; }
        public string UnserializableValue { get; set; }
        public string ObjectId { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Debugger/#type-CallFrame
    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-CallFrame
    public class CallFrame
    {
        public string CallFrameId { get; set; }
        public string FunctionName { get; set; }
        public Location FunctionLocation { get; set; }
        public Location Location { get; set; }
        public string Url { get; set; }
        public Scope[] ScopeChain { get; set; }
        public RemoteObject This { get; set; }
        public RemoteObject ReturnValue { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-ExceptionDetails
    public class ExceptionDetails
    {
        public int ExceptionId { get; set; }
        public string Text { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string ScriptId { get; set; }
        public string Url { get; set; }
        public StackTrace StackTrace { get; set; }
        public RemoteObject Exception { get; set; }
        public int ExecutionContextId { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-ExecutionContextDescription
    public class ExecutionContextDescription
    {
        public string Id { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public object AuxData { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-InternalPropertyDescriptor
    public class InternalPropertyDescriptor
    {
        public string Name { get; set; }
        public RemoteObject Value { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-PropertyDescriptor
    public class PropertyDescriptor
    {
        public string Name { get; set; }
        public RemoteObject Value { get; set; }
        public bool Writable { get; set; }
        public RemoteObject Get { get; set; }
        public RemoteObject Set { get; set; }
        public bool Configurable { get; set; }
        public bool Enumerable { get; set; }
        public bool WasThrown { get; set; }
        public bool IsOwn { get; set; }
        public RemoteObject Symbol { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-RemoteObject
    public class RemoteObject
    {
        public ObjectType Type { get; set; }
        public ObjectSubType Subtype { get; set; }
        public string ClassName { get; set; }
        public object Value { get; set; }
        public string UnserializableValue { get; set; }
        public string Description { get; set; }
        public string ObjectId { get; set; }
        public ObjectPreview ObjectPreview { get; set; }
        public CustomPreview CustomPreview { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-StackTrace
    public class StackTrace
    {
        public string Description { get; set; }
        public CallFrame[] CallFrames { get; set; }
        public StackTrace Parent { get; set; }
        public StackTraceId ParentId { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-CustomPreview
    public class CustomPreview
    {
        public string Header { get; set; }
        public string BodyGetterId { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-EntryPreview
    public class EntryPreview
    {
        public ObjectPreview Key { get; set; }
        public ObjectPreview Value { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-ObjectPreview
    public class ObjectPreview
    {
        public ObjectType Type { get; set; }
        public ObjectSubType Subtype { get; set; }
        public string Description { get; set; }
        public bool Overflow { get; set; }
        public PropertyPreview[] Properties { get; set; }
        public EntryPreview[] Entries { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-PrivatePropertyDescriptor
    public class PrivatePropertyDescriptor
    {
        public string Name { get; set; }
        public RemoteObject Value { get; set; }
        public RemoteObject Get { get; set; }
        public RemoteObject Set { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-PropertyPreview
    public class PropertyPreview
    {
        public string Name { get; set; }
        public ObjectType Type { get; set; }
        public string Value { get; set; }
        public ObjectPreview ValuePreview { get; set; }
        public ObjectSubType Subtype { get; set; }
    }

    // https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#type-StackTraceId
    public class StackTraceId
    {
        public string Id { get; set; }
        public string UniqueDebuggerId { get; set; }
    }
}
