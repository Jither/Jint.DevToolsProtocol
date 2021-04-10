using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Helpers
{
	public static class ScriptHashHelper
    {
        private static uint[] _prime = { 0x3FB75161, 0xAB1F4E4F, 0x82675BC5, 0xCD924D35, 0x81ABE279 };
        // These particular "random" constants may look familiar to people who know their SHA-1.
		// However, this is *not* SHA-1 (far from it).
        private static uint[] _random = { 0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };
        private static uint[] _randomOdd = { 0xB4663807, 0xCC322BF5, 0xD4F91BBD, 0xA7BEA11D, 0x8F462907 };

		/// <summary>
		/// Generates script hashes identical to V8.
		/// </summary>
		/// <remarks>
		/// See <see href="https://chromium.googlesource.com/v8/v8.git/+/refs/heads/master/src/inspector/v8-debugger-script.cc"/>
		/// </remarks>
		public static string Hash(string str)
        {
			// These arrays will only ever hold integers within the 32-bit range.
			// So why are they ulong? Solely to avoid a lot of casting below - since multiplications etc.
			// are intended to be 64-bit arithmetic.
			ulong[] hashes = new ulong[5];
			ulong[] zi = new ulong[5] { 1, 1, 1, 1, 1 };
			uint current = 0;

			// Length of substring that can be split into 4 bytes (i.e. round down to even length)
			int int32length = (str.Length / 2) * 2;

			unchecked
			{
				for (int i = 0; i < int32length; i += 2)
				{
					uint v = (uint)str[i + 1] << 16 | str[i];
					uint xi = v * _randomOdd[current] & 0x7fffffff;
					hashes[current] = (hashes[current] + zi[current] * xi) % _prime[current];
					zi[current] = (zi[current] * _random[current]) % _prime[current];
					current++;
					if (current >= hashes.Length)
					{
						current = 0;
					}
				}

				// Process last char in a string with odd length
				if (str.Length > int32length)
				{
					char c = str[^1];
					// Note that V8 handles the last char differently: reversed endianness
					uint v = (uint)((c & 0xff) << 8 | (c & 0xff00) >> 8);
					uint xi = v * _randomOdd[current] & 0x7fffffff;
					hashes[current] = (hashes[current] + zi[current] * xi) % _prime[current];
					zi[current] = (zi[current] * _random[current]) % _prime[current];
				}

				for (int i = 0; i < hashes.Length; i++)
				{
					hashes[i] = (hashes[i] + zi[i] * (_prime[i] - 1)) % _prime[i];
				}
			}

			string result = "";
			for (int i = 0; i < hashes.Length; i++)
			{
				result += hashes[i].ToString("x8");
			}
			return result;
		}
    }
}
