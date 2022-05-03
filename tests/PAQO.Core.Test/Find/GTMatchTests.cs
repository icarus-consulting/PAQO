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
using PAQO.Editor.Tmx.Find;
using System;
using Test.PAQO.Schema;
using Xunit;

namespace PAQO.Core.Find.Test
{
    public sealed class GTMatchTests
    {
        [Fact]
        public void MatchesIntGreaterThanGiven()
        {
            Assert.True(
                new GTMatch(
                    "MaxSpeed",
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
        public void MatchesDateGreaterThanGiven()
        {
            var date = DateTime.Now;
            Assert.True(
                new GTMatch(
                    "BuyDate",
                    date.Ticks,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DateProp("BuyDate", date.AddDays(1))
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchUnknownProp()
        {
            Assert.False(
                new GTMatch(
                    "unknownprop",
                    2,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new IntProp("integerprop", 1)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchMissingProp()
        {
            Assert.False(
                new GTMatch(
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
        [InlineData(0)]
        public void DoesNotMatchIntLessOrEqualToGiven(int value)
        {
            Assert.False(
                new GTMatch(
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
        public void MatchesDecimalGreaterThanGiven()
        {
            Assert.True(
                new GTMatch(
                    "HighestGearRatio",
                    1.8,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", 2.2)
                    )
                )
            );
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(0.5)]
        public void DoesNotMatchDecimalLessOrEqualToGiven(int value)
        {
            Assert.False(
                new GTMatch(
                    "HighestGearRatio",
                    1,
                    new VehiclesTestSchema().For("bike").Types()
                ).Matches(
                    new PropsOf(
                        new DecimalProp("HighestGearRatio", value)
                    )
                )
            );
        }

        [Fact]
        public void MatchesGreaterText()
        {
            Assert.True(
                new GTMatch(
                    "Name",
                    "AB",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapStringToBytes()
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "ABC")
                    )
                )
            );
        }
    }
}
