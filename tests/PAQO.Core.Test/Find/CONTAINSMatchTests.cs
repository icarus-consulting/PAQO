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

using PAQO.Core.Element;
using PAQO.Core.Prop;
using PAQO.Core.Schema;
using Test.PAQO.Schema;
using Xunit;

namespace PAQO.Core.Find.Test
{
    public sealed class CONTAINSMatchTests
    {
        [Fact]
        public void MatchesContainingText()
        {
            Assert.True(
                new CONTAINSMatch(
                    "Name",
                    "llo w",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "456", new TextProp("Name", "Hello world")
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchUnknownProp()
        {
            Assert.False(
                new CONTAINSMatch(
                    "unknownprop",
                    "llo w",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "456", new TextProp("unknownprop", "BOOM")
                    ).Props()
                )
            );
        }

        [Fact]
        public void NoMatchOnMissingProp()
        {
            Assert.False(
                new CONTAINSMatch(
                    "textprop",
                    "llo w",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "456", new TextProp("unknownprop", "BOOM")
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchNonContainingText()
        {
            Assert.False(
                new CONTAINSMatch(
                    "Name",
                    "llo w",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "456", new TextProp("Name", "Good bye world")
                    ).Props()
                )
            );
        }

        [Fact]
        public void MatchesTextContainingIntegerPart()
        {
            Assert.True(
                new CONTAINSMatch(
                    "Name",
                    456,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new TextProp("Name", "123456")
                    ).Props()
                )
            );
        }

        [Fact]
        public void MatchesTextContainingDecimalPart()
        {
            Assert.True(
                new CONTAINSMatch(
                    "Name",
                    45.6,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new TextProp("Name", "12345.6")
                    ).Props()
                )
            );
        }

        [Fact]
        public void MatchesIntegerContainingIntegerPart()
        {
            Assert.True(
                new CONTAINSMatch(
                    "MaxSpeed",
                    456,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new IntProp("MaxSpeed", 123456)
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchIntegerNotContainingInteger()
        {
            Assert.False(
                new CONTAINSMatch(
                    "MaxSpeed",
                    456,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new IntProp("MaxSpeed", 7890123)
                    ).Props()
                )
            );
        }

        [Fact]
        public void MatchesDecimalContainingIntegerPart()
        {
            Assert.True(
                new CONTAINSMatch(
                    "HighestGearRatio",
                    34,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new DecimalProp("HighestGearRatio", 1234.56)
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchDecimalNotContainingInteger()
        {
            Assert.False(
                new CONTAINSMatch(
                    "HighestGearRatio",
                    34,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new DecimalProp("HighestGearRatio", 123.689)
                    ).Props()
                )
            );
        }

        [Fact]
        public void MatchesDecimalContainingDecimalPart()
        {
            Assert.True(
                new CONTAINSMatch(
                    "HighestGearRatio",
                    4.56,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new DecimalProp("HighestGearRatio", 1234.56)
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchDecimalNotContainingDecimalPart()
        {
            Assert.False(
                new CONTAINSMatch(
                    "HighestGearRatio",
                    4.56,
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new DecimalProp("HighestGearRatio", 12.56)
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchTextNotContainedInDecimal()
        {
            Assert.False(
                new CONTAINSMatch(
                    "HighestGearRatio",
                    "a",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new SimpleElement(
                        "123-element", new DecimalProp("HighestGearRatio", 12.56)
                    ).Props()
                )
            );
        }

        [Fact]
        public void DoesNotMatchTextNotContainedInInteger()
        {
            Assert.False(
                new CONTAINSMatch(
                    "MaxSpeed",
                    "a",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new PropsOf(
                        new IntProp("MaxSpeed", 12)
                    )
                )
            );
        }

        [Fact]
        public void DoesNotMatchEmptyProps()
        {
            Assert.False(
                new CONTAINSMatch(
                    "Name",
                    "a",
                    new VehiclesTestSchema().For("bike").Types(),
                    new SwapBytesToString()
                ).Matches(
                    new PropsOf(
                        new TextProp("Name", "")
                    )
                )
            );
        }
    }
}
