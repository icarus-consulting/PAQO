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
using System;
using System.Globalization;

namespace PAQO.Memory.LiteDB.Facets
{
    /// <summary>
    /// Converts a string to a BsonValue.
    /// </summary>
    public sealed class SwapStringToBsonValue : ISwap<string, string, BsonValue>
    {
        private readonly SwapSwitch<string, BsonValue> conversions;

        /// <summary>
        /// Converts a string to a BsonValue.
        /// </summary>
        public SwapStringToBsonValue()
        {
            this.conversions =
                new SwapSwitch<string, BsonValue>(
                    new SwapIf<string, BsonValue>(
                        "decimal", (str) => DoubleBsonValue(str)
                    ),
                    new SwapIf<string, BsonValue>(
                        "integer", (str) => IntBsonValue(str)
                    ),
                    new SwapIf<string, BsonValue>(
                        "switch", (str) => BooleanBsonValue(str)
                    ),
                    new SwapIf<string, BsonValue>(
                        "text", (str) => new BsonValue(str)
                    ),
                    new SwapIf<string, BsonValue>(
                        "options", (str) => new BsonValue(str)
                    ),
                    new SwapIf<string, BsonValue>(
                        "complex", (str) => new BsonValue(str)
                    )
                );

        }

        public BsonValue Flip(string type, string data)
        {
            return this.conversions.Flip(type, data);
        }

        private BsonValue IntBsonValue(string input)
        {
            var result = new BsonValue(Guid.NewGuid()); //will never match (intended if prop not convertible)
            var parsed = 0;
            if (Int32.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
            {
                result = new BsonValue(parsed);
            }
            return result;
        }

        private BsonValue DoubleBsonValue(string input)
        {
            var result = new BsonValue(Guid.NewGuid()); //will never match (intended if prop not convertible)
            var parsed = 0.0;
            if (Double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
            {
                result = new BsonValue(parsed);
            }
            return result;
        }

        private BsonValue BooleanBsonValue(string input)
        {
            var result = new BsonValue(Guid.NewGuid()); //will never match (intended if prop not convertible)
            var parsed = false;
            if (Boolean.TryParse(input, out parsed))
            {
                result = new BsonValue(parsed);
            }
            return result;
        }
    }
}
