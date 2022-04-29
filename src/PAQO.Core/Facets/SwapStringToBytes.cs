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

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Converts string data to bytes based on a given type.
    /// </summary>
    public sealed class SwapStringToBytes : ISwap<string, string, byte[]>
    {
        private readonly SwapSwitch<string, byte[]> conversions;

        public SwapStringToBytes()
        {
            this.conversions =
                new SwapSwitch<string, byte[]>(
                    new SwapIf<string, byte[]>(
                        "decimal", (text) =>
                        {
                            var result = new byte[0];
                            var dbl = 0.0;
                            if (double.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dbl))
                            {
                                result = BitConverter.GetBytes(dbl);
                            }
                            return result;
                        }
                    ),
                    new SwapIf<string, byte[]>(
                        "integer", (text) =>
                        {
                            var result = new byte[0];
                            var integer = 0;
                            if (int.TryParse(text, out integer))
                            {
                                result = BitConverter.GetBytes(integer);
                            }
                            return result;
                        }
                    ),
                    new SwapIf<string, byte[]>(
                        "switch", (data) => new BytesOf(data, Encoding.UTF8).AsBytes()
                    ),
                    new SwapIf<string, byte[]>(
                        "text", (data) => new BytesOf(data, Encoding.UTF8).AsBytes()
                    ),
                    new SwapIf<string, byte[]>(
                        "options", (data) => new BytesOf(data, Encoding.UTF8).AsBytes()
                    ),
                    new SwapIf<string, byte[]>(
                        "complex", (data) => new BytesOf(data, Encoding.UTF8).AsBytes()
                    )
                );

        }

        public byte[] Flip(string type, string input)
        {
            return this.conversions.Flip(type, input);
        }
    }
}
