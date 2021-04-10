using Jint.DevToolsProtocol.Helpers;
using System;
using Xunit;

namespace Jint.Tests.DevToolsProtocol
{
    public class ScriptHashTests
    {
        [Theory]
        // Simple ASCII test, testing odd and even lengths and the entire length of the hashes array
        [InlineData("", "3fb75160ab1f4e4e82675bc4cd924d3481abe278")]
        [InlineData("a", "13acd560ab1f4e4e82675bc4cd924d3481abe278")]
        [InlineData("ab", "25db6907ab1f4e4e82675bc4cd924d3481abe278")]
        [InlineData("abc", "25db6907225161c682675bc4cd924d3481abe278")]
        [InlineData("abcd", "25db6907898bf0d482675bc4cd924d3481abe278")]
        [InlineData("abcde", "25db6907898bf0d4319e0fc7cd924d3481abe278")]
        [InlineData("abcdef", "25db6907898bf0d47fa9cc1dcd924d3481abe278")]
        [InlineData("abcdefg", "25db6907898bf0d47fa9cc1d22a0568a81abe278")]
        [InlineData("abcdefgh", "25db6907898bf0d47fa9cc1d61487e3581abe278")]
        [InlineData("abcdefghi", "25db6907898bf0d47fa9cc1d61487e3504acdf89")]
        [InlineData("abcdefghij", "25db6907898bf0d47fa9cc1d61487e358031b6e1")]
        [InlineData("abcdefghijk", "0f39cdcb898bf0d47fa9cc1d61487e358031b6e1")]
        [InlineData("abcdefghijkl", "1544f4b7898bf0d47fa9cc1d61487e358031b6e1")]
        public void ShouldHashASCIIIdenticallyToV8(string input, string expected)
        {
            string actual = ScriptHashHelper.Hash(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        // Test Basic Multilingual Plane support - i.e. U+0000 to U+FFFF
        [InlineData("var x = 'Ḋ'", "17f76298284935e036117f5f0bda0f737f7b2313")]
        [InlineData("var x = 'Ḋ⁈'", "145870f3284935e036117f5f0bda0f737f7b2313")]
        [InlineData("var x = 'Ḋ⁈⽥'", "2e51358f258aca5036117f5f0bda0f737f7b2313")]
        [InlineData("var x = 'Ḋ⁈⽥免'", "2e51358f6702104e36117f5f0bda0f737f7b2313")]
        [InlineData("var x = 'Ḋ⁈⽥免屮'", "2e51358f87b75f3e6ba71dfa0bda0f737f7b2313")]
        public void ShouldHashBMPCharactersIdenticallyToV8(string input, string expected)
        {
            string actual = ScriptHashHelper.Hash(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        // Test Unicode requiring more than 1 codepoint per character - i.e. U+10000 and up
        // 0 at start of string is used to test cases where surrogate pairs are int32 aligned as well
        // as where they're split between two int32's.
        [InlineData("var x = '𐤀'", "37776e1d284935e036117f5f0bda0f734d432313")]
        [InlineData("var x = '0𐤀'", "24ac9d87258aca5036117f5f0bda0f7344852313")]
        [InlineData("var x = '𐤀𠀇'", "17b4747e14ec123b36117f5f0bda0f734d432313")]
        [InlineData("var x = '0𐤀𠀇'", "24ac9d877c8fe3c46ba71dfa0bda0f7344852313")]
        [InlineData("var x = '𐤀𠀇🌂'", "17b4747e6e95ebd63bf6aa9a0bda0f734d432313")]
        [InlineData("var x = '0𐤀𠀇🌂'", "24ac9d877c8fe3c41314e2a40deb034f44852313")]
        [InlineData("var x = '𐤀𠀇🌂🤨'", "17b4747e6e95ebd6100ae39d6eab8c724d432313")]
        [InlineData("var x = '0𐤀𠀇🌂🤨'", "24ac9d877c8fe3c41314e2a400e438ea60208e0c")]
        public void ShouldHashHigherUnicodePlanesIdenticallyToV8(string input, string expected)
        {
            string actual = ScriptHashHelper.Hash(input);
            Assert.Equal(expected, actual);
        }
    }
}
