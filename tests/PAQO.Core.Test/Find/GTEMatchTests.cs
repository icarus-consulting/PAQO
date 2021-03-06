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
    public sealed class GTEMatchTests
    {
        [Fact]
        public void DoesNotMatchIntLessThanGiven()
        {
            Assert.False(
                new GTEMatch(
                    "MaxSpeed",
                    2,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchUnknownProp()
        {
            Assert.False(
                new GTEMatch(
                    "unknownprop",
                    2,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 1)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchMissingProp()
        {
            Assert.False(
                new GTEMatch(
                    "MaxSpeed",
                    2,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("unknownprop", 1)
                    )
                )
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void MatchesIntGreaterOrEqualToGiven(int value)
        {
            Assert.True(
                new GTEMatch(
                    "MaxSpeed",
                    1,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", value)
                    )
                )
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void MatchesDateGreaterOrEqualToGiven(int value)
        {
            var date = DateTime.Now;
            Assert.True(
                new GTEMatch(
                    "BuyDate",
                    date.Ticks,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DateProp("BuyDate", date.AddMilliseconds(value))
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchDecimalLessThanGiven()
        {
            Assert.False(
                new GTEMatch(
                    "HighestGearRatio",
                    1.8,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", 1.7)
                    )
                )
            );
        }

        [Theory]
        [InlineData(1.9)]
        [InlineData(2.5)]
        public void MatchesDecimalGreaterOrEqualToGiven(double value)
        {
            Assert.True(
                new GTEMatch(
                    "HighestGearRatio",
                    1.9,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", value)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchSmallerText()
        {
            Assert.False(
                new GTEMatch(
                    "Name",
                    "AB",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "A")
                    )
                )
            );
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("ABC")]
        public void MatchesGreaterAndEqualText(string propValue)
        {
            Assert.True(
                new GTEMatch(
                    "Name",
                    "AB",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", propValue)
                    )
                )
            );
        }
    }
}
