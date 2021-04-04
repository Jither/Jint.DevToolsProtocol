using Jint.DevToolsProtocol.Protocol.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jint.DevToolsProtocol.Helpers
{
    public static class StringExtensions
    {
        public static ScriptPosition FindEndPosition(this string str)
        {
            int lines = 0;
            int lastLineStart = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    lines++;
                    lastLineStart = i + 1;
                }
            }
            return new ScriptPosition(lines, str.Length - lastLineStart);
        }
    }
}
