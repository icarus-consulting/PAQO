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

using LiteDB;
using PAQO.Core;
using PAQO.Core.Facets;
using PAQO.Core.Prop;
using System;
using System.Text;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Text;

namespace PAQO.Memory.LiteDB.Facets
{
    /// <summary>
    /// Converts raw data to a BsonValue.
    /// </summary>
    public sealed class SwapBytesToBsonValue : ISwap<string, byte[], BsonValue>
    {
        private readonly SwapSwitch<byte[], BsonValue> conversions;

        /// <summary>
        /// Converts raw data to a BsonValue.
        /// </summary>
        public SwapBytesToBsonValue()
        {
            this.conversions =
                new SwapSwitch<byte[], BsonValue>(
                    new SwapIf<byte[], BsonValue>(
                        "date", (data) => new BsonValue(new DateProp.AsDate(data).Value())
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "decimal", (data) => new ByteBsonDouble(data)
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "integer", (data) => new ByteBsonInt(data)
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "switch", (data) => new ByteBsonBool(data)
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "text", (data) => new BsonValue(new TextOf(new BytesOf(data), Encoding.UTF8).AsString())
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "options", (data) => new BsonValue(new TextOf(new BytesOf(data), Encoding.UTF8).AsString())
                    ),
                    new SwapIf<byte[], BsonValue>(
                        "complex", (data) => new BsonValue(data)
                    )
                );

        }
        public BsonValue Flip(string type, byte[] data)
        {
            return this.conversions.Flip(type, data);
        }
    }
}
