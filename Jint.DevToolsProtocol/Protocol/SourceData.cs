using Esprima.Ast;
using Jint.DevToolsProtocol.Helpers;
using Jint.DevToolsProtocol.Protocol.Domains;
using System;
using System.Collections.Generic;

namespace Jint.DevToolsProtocol.Protocol
{
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
}
