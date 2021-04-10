using Esprima.Ast;
using Esprima.Utils;
using Jint.DevToolsProtocol.Helpers;
using Jint.DevToolsProtocol.Protocol.Domains;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jint.DevToolsProtocol.Protocol
{

    public class RuntimeData
    {
        private readonly Engine _engine;
        /// <summary>
        /// Dictionary of SourceData by Source ID (value of Source property on nodes in Jint/Esprima)
        /// </summary>
        private readonly Dictionary<string, SourceData> _sourcesBySourceId = new Dictionary<string, SourceData>();
        /// <summary>
        /// Dictionary of SourceData by Script ID (ID used in communication with devtools)
        /// </summary>
        private readonly Dictionary<string, SourceData> _sourcesByScriptId = new Dictionary<string, SourceData>();

        private readonly ObjectDataMap _objectData = new ObjectDataMap();

        private int _nextBreakPointId = 1;

        private readonly Dictionary<string, BreakPoint> _breakPoints = new Dictionary<string, BreakPoint>();
        private readonly Dictionary<string, PendingBreakPoint> _pendingBreakPoints = new Dictionary<string, PendingBreakPoint>();

        /// <summary>
        /// Current DebugInformation (when paused - null when running)
        /// </summary>
        public DebugInformation DebugInformation { get; set; }

        public RuntimeData(Engine engine)
        {
            _engine = engine;
        }

        public SourceData AddSource(string sourceId, string url, string source, Script ast)
        {
            if (!_sourcesBySourceId.TryGetValue(sourceId, out var data))
            {
                string scriptId = $"jint:{_sourcesByScriptId.Count + 1}";
                data = new SourceData(scriptId, sourceId, url, source, ast);
                _sourcesBySourceId.Add(sourceId, data);
                _sourcesByScriptId.Add(scriptId, data);
            }
            return data;
        }

        public SourceData GetSourceByScriptId(string scriptId)
        {
            if (_sourcesByScriptId.TryGetValue(scriptId, out var result))
            {
                return result;
            }
            return null;
        }

        public IEnumerable<SourceData> GetAllSources()
        {
            return _sourcesByScriptId.Values;
        }

        public IEnumerable<SourceData> FindSources(PendingBreakPoint breakPoint)
        {
            return _sourcesBySourceId.Values.Where(s => breakPoint.Matches(s));
        }

        public string AddBreakPoint(BreakPoint breakPoint)
        {
            string id = _nextBreakPointId.ToString();
            _nextBreakPointId++;
            _breakPoints.Add(id, breakPoint);
            return id;
        }

        public string AddPendingBreakPoint(PendingBreakPoint breakPoint)
        {
            string id = (_nextBreakPointId++).ToString();
            _pendingBreakPoints.Add(id, breakPoint);
            return id;
        }

        public IEnumerable<BreakPoint> RemoveBreakPoint(string id)
        {
            if (_pendingBreakPoints.TryGetValue(id, out var pendingBreakPoint))
            {
                _pendingBreakPoints.Remove(id);
                return pendingBreakPoint.BreakPoints;
            }
            if (_breakPoints.TryGetValue(id, out var breakPoint))
            {
                _breakPoints.Remove(id);
                return new[] { breakPoint };
            }
            return Enumerable.Empty<BreakPoint>();
        }

        public string SourceIdToScriptId(string sourceId)
        {
            if (!_sourcesBySourceId.TryGetValue(sourceId, out var data))
            {
                return null;
            }
            return data.ScriptId;
        }

        public RemoteObject GetRemoteObject(DebugScope scope)
        {
            if (scope.BindingObject != null)
            {
                return GetRemoteObject(scope.BindingObject, generatePreview: true, isScope: true);
            }

            // Create transient object for scope object
            ObjectInstance obj = _engine.Object.Construct(Array.Empty<JsValue>());
            foreach (var name in scope.BindingNames)
            {
                obj.FastAddProperty(
                    name, 
                    // let/const may be null until initialization - Chromium shows them as undefined
                    scope.GetBindingValue(name) ?? JsValue.Undefined,
                    false, false, false);
            }
            return GetRemoteObject(obj, generatePreview: true, isScope: true);
        }

        public RemoteObject GetRemoteObject(JsValue value, bool generatePreview = false, string objectGroup = null, bool isScope = false)
        {
            int? id = null;
            if (value is ObjectInstance obj)
            {
                var data = _objectData.GetOrAdd(obj, objectGroup, isScope);
                id = data.Id;
            }

            return new RemoteObject(value, generatePreview)
            {
                ObjectId = id?.ToString()
            };
        }

        public ObjectData GetObject(string id)
        {
            return _objectData[Int32.Parse(id)];
        }

        public void ReleaseObject(string id)
        {
            _objectData.Release(Int32.Parse(id));
        }

        public void ReleaseObjectGroup(string objectGroup)
        {
            _objectData.ReleaseGroup(objectGroup);
        }
    }

    public class SourceData
    {
        private List<Domains.BreakLocation> _breakLocations;

        public SourceData(string scriptId, string sourceId, string url, string source, Script ast)
        {
            SourceId = sourceId;
            ScriptId = scriptId;
            Url = url;
            Source = source;
            Ast = ast;
            End = source.FindEndPosition();
            Hash = ScriptHashHelper.Hash(source);
        }

        /// <summary>
        /// ID used in communication with devtools
        /// </summary>
        public string ScriptId { get; }

        /// <summary>
        /// ID used in Jint (Source property on Esprima nodes)
        /// </summary>
        public string SourceId { get; }
        public string Url { get; }
        public string Source { get; }
        public Script Ast { get; }
        public ScriptPosition End { get; }
        public string Hash { get; }
        public int Length => Source.Length;
        public List<Domains.BreakLocation> BreakLocations => _breakLocations ??= CollectBreakLocations();

        public bool Sent { get; set; }

        private List<Domains.BreakLocation> CollectBreakLocations()
        {
            var collector = new BreakPointCollector(ScriptId);
            collector.Visit(Ast);
            return collector.Positions;
        }

        public Location FindNearestBreak(Location location)
        {
            var locations = BreakLocations;
            int index = locations.BinarySearch(new Domains.BreakLocation { ColumnNumber = location.ColumnNumber, LineNumber = location.LineNumber });
            if (index < 0)
            {
                // Get the first break after the location
                index = ~index;
            }
            return locations[index];
        }
    }

    public class BreakPointCollector : AstVisitor
    {
        private readonly List<Domains.BreakLocation> _positions = new List<Domains.BreakLocation>();
        private readonly string _scriptId;

        public List<Domains.BreakLocation> Positions => _positions;

        public BreakPointCollector(string scriptId)
        {
            _scriptId = scriptId;
        }

        protected override void VisitStatement(Statement statement)
        {
            _positions.Add(new Domains.BreakLocation
            {
                LineNumber = statement.Location.Start.Line - 1,
                ColumnNumber = statement.Location.Start.Column,
                ScriptId = _scriptId
            });

            base.VisitStatement(statement);
        }

        protected override void VisitArrowFunctionExpression(ArrowFunctionExpression arrowFunctionExpression)
        {
            base.VisitArrowFunctionExpression(arrowFunctionExpression);

            var position = arrowFunctionExpression.Body.Location.End;
            _positions.Add(new Domains.BreakLocation
            {
                Type = BreakLocationType.Return,
                LineNumber = position.Line - 1,
                ColumnNumber = position.Column,
                ScriptId = _scriptId
            });
        }

        protected override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            base.VisitFunctionDeclaration(functionDeclaration);

            var position = functionDeclaration.Body.Location.End;
            _positions.Add(new Domains.BreakLocation
            {
                Type = BreakLocationType.Return,
                LineNumber = position.Line - 1,
                ColumnNumber = position.Column,
                ScriptId = _scriptId
            });
        }

        protected override void VisitFunctionExpression(IFunction function)
        {
            base.VisitFunctionExpression(function);

            var position = function.Body.Location.End;
            _positions.Add(new Domains.BreakLocation
            {
                Type = BreakLocationType.Return,
                LineNumber = position.Line - 1,
                ColumnNumber = position.Column,
                ScriptId = _scriptId
            });
        }
    }
}
