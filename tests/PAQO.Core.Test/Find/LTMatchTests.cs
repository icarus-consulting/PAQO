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
    public sealed class LTMatchTests
    {
        [Fact]
        public void MatchesIntLessThanGiven()
        {
            Assert.True(
                new LTMatch(
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
        public void MatchesDateLessThanGiven()
        {
            var date = DateTime.Now;
            Assert.True(
                new LTMatch(
                    "BuyDate",
                    date.Ticks,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DateProp("BuyDate", date.AddMonths(-1))
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchUnknownProp()
        {
            Assert.False(
                new LTMatch(
                    "unknownprop",
                    1,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 2)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchMissingProp()
        {
            Assert.False(
                new LTMatch(
                    "MaxSpeed",
                    1,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf()
                )
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void DoesNotMatchIntGreaterOrEqualToGiven(int value)
        {
            Assert.False(
                new LTMatch(
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

        [Fact]
        public void MatchesDecimalLessThanGiven()
        {
            Assert.True(
                new LTMatch(
                    "HighestGearRatio",
                    2.5,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", 2.3)
                    )
                )
            );
        }

        [Theory]
        [InlineData(1.9)]
        [InlineData(100.5)]
        public void DoesNotMatchDecimalGreaterOrEqualToGiven(double value)
        {
            Assert.False(
                new LTMatch(
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
        public void MatchesSmallerText()
        {
            Assert.True(
                new LTMatch(
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
        [InlineData("A")]
        [InlineData("AB")]
        public void DoesNotMatchGreaterOrEqualText(string propValue)
        {
            Assert.False(
                new LTMatch(
                    "Name",
                    "A",
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
