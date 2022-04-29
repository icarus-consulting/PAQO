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
using Test.PAQO.Schema;
using Xunit;

namespace PAQO.Core.Find.Test
{
    public sealed class QueryMatchTests
    {
        [Fact]
        public void BuildsSpecificMatch()
        {
            Assert.True(
                new QueryMatch(
                    new AND(
                        new OR(
                            new EQ("Name", "hollow world"),
                            new EQ("Name", "solid world")
                        ),
                        new NOT("Name", "opaque world"),
                        new GTE("Name", "hollow worl"),
                        new GTE("Name", "hollow world"),
                        new GT("Name", "hollow worl"),
                        new LTE("Name", "hollow world"),
                        new LTE("Name", "hollow worlds"),
                        new LT("Name", "hollow worlds"),
                        new LTE("MaxSpeed", 1000),
                        new LT("MaxSpeed", 1001),
                        new GTE("MaxSpeed", 1000),
                        new GT("MaxSpeed", 999),
                        new EQ("MaxSpeed", 1000),
                        new EQ("HighestGearRatio", 13.36),
                        new LTE("HighestGearRatio", 13.36),
                        new LT("HighestGearRatio", 13.37),
                        new GTE("HighestGearRatio", 13.36),
                        new GT("HighestGearRatio", 13.35),
                        new CONTAINS("Name", "ollo"),
                        new CONTAINS("MaxSpeed", 1),
                        new CONTAINS("HighestGearRatio", 3.3),
                        new IN("Name", "hollow world"),
                        new IN("MaxSpeed", 1000),
                        new IN("HighestGearRatio", 13.36)
                    ),
                    new VehiclesTestSchema(),
                    "bike"
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "hollow world"),
                        new IntProp("MaxSpeed", 1000),
                        new DecimalProp("HighestGearRatio", 13.36)
                    )
                )
            );
        }


        [Fact]
        public void BuildsAllMatch()
        {
            Assert.True(
                new QueryMatch(
                    new ALL(),
                    new VehiclesTestSchema(),
                    "bike"
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "hollow world")
                    )
                )
            );
        }

        [Fact]
        public void BuildsNoneMatch()
        {
            Assert.False(
                new QueryMatch(
                    new NONE(),
                    new VehiclesTestSchema(),
                    "bike"
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "hollow world")
                    )
                )
            );
        }
    }
}
