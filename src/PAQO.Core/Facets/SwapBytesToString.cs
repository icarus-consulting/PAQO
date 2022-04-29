// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using PAQO.Core.Facets;
using System;
using System.Globalization;
using System.Text;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Converts bytes to string data based on a given type.
    /// </summary>
    public sealed class SwapBytesToString : ISwap<string, byte[], string>
    {
        private readonly SwapSwitch<byte[], string> conversions;

        /// <summary>
        /// Converts bytes to string data based on a given type.
        /// </summary>
        public SwapBytesToString(int roundedDecimals = 4)
        {
            this.conversions =
                new SwapSwitch<byte[], string>(
                    new SwapIf<byte[], string>(
                        "decimal", (data) => data.Length == 0 ? "0" : Math.Round(BitConverter.ToDouble(data, 0), roundedDecimals).ToString(CultureInfo.InvariantCulture)
                    ),
                    new SwapIf<byte[], string>(
                        "integer", (data) => data.Length == 0 ? "0" : BitConverter.ToInt32(data, 0).ToString()
                    ),
                    new SwapIf<byte[], string>(
                        "switch", (data) => new TextOf(new BytesOf(data), Encoding.UTF8).AsString()
                    ),
                    new SwapIf<byte[], string>(
                        "text", (data) => new TextOf(new BytesOf(data), Encoding.UTF8).AsString()
                    ),
                    new SwapIf<byte[], string>(
                        "options", (data) => new TextOf(new BytesOf(data), Encoding.UTF8).AsString()
                    ),
                    new SwapIf<byte[], string>(
                        "complex", (data) => new TextOf(new BytesOf(data), Encoding.UTF8).AsString()
                    )
                );

        }

        public string Flip(string type, byte[] input)
        {
            return this.conversions.Flip(type, input);
        }
    }
}
