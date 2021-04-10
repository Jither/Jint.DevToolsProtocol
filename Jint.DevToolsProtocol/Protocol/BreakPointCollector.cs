using Esprima.Ast;
using Esprima.Utils;
using Jint.DevToolsProtocol.Protocol.Domains;
using System.Collections.Generic;

namespace Jint.DevToolsProtocol.Protocol
{
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
