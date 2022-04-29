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

using PAQO.Core.Prop;
using PAQO.Core.Schema;
using System;
using Test.PAQO.Schema;
using Xunit;

namespace PAQO.Core.Find.Test
{
    public sealed class EQMatchTests
    {
        [Fact]
        public void TextMatchesText()
        {
            Assert.True(
                new EQMatch(
                    "Name",
                    "1",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "1")
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchUnknownProp()
        {
            Assert.False(
                new EQMatch(
                    "unknownprop",
                    "1",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("textprop", "1")
                    )
                )
            );
        }

        [Fact]
        public void NoMatchOnMissingProp()
        {
            Assert.False(
                new EQMatch(
                    "Name",
                    "1",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("unknownprop", "1")
                    )
                )
            );
        }

        [Fact]
        public void TextMatchesInt()
        {
            Assert.True(
                new EQMatch(
                    "MaxSpeed",
                    "1",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }

        [Fact]
        public void TextMatchesDecimal()
        {
            Assert.True(
                new EQMatch(
                    "HighestGearRatio",
                    "1.3",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", 1.3)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchDifferentText()
        {
            Assert.False(
                new EQMatch(
                    "MaxSpeed",
                    "9",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }

        [Fact]
        public void MatchesInt()
        {
            Assert.True(
                new EQMatch(
                    "MaxSpeed",
                    1,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }

        [Fact]
        public void MatchesDate()
        {
            var date = DateTime.Now;
            Assert.True(
                new EQMatch(
                    "BuyDate",
                    date.Ticks,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DateProp("BuyDate", date)
                    )
                )
            );
        }

        [Fact]
        public void MatchesDouble()
        {
            Assert.True(
                new EQMatch(
                    "MaxSpeed",
                    1.0,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }
    }
}
