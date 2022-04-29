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
using PAQO.Core.Find;
using PAQO.Core.Prop;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Group.Test
{
    public sealed class SimpleGroupTests
    {
        [Fact]
        public void DeliversAllElements()
        {
            Assert.Equal(
                "123-superbike",
                FirstOf.New(
                    new SimpleGroup("bike",
                        new VehiclesTestSchema(),
                        new ManyOf<IElement>(
                            new SimpleElement(
                                "123-superbike",
                                new SimpleProp("name",
                                    new BytesOf("the most wonderful bike").AsBytes()
                                )
                            )
                        )
                    )
                    .Elements()
                )
                .Value()
                .ID()
            );
        }

        [Fact]
        public void AddsElements()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new ManyOf<IElement>()
                );

            group.Add(
                ManyOf.New(
                    new SimpleElement("123-superbike",
                        new TextProp("Name", "the most wonderful bike")
                    )
                )
            );
            Assert.Equal(
                "123-superbike",
                FirstOf.New(
                    group
                    .Find(new EQ("id", "123-superbike"))
                    .Elements()
                )
                .Value()
                .ID()
            );
        }

        [Fact]
        public void RemovesElements()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement("123-superbike",
                        new TextProp("Name", "the most wonderful bike")
                    )
                );

            group.Remove(
                new EQ("id", "123-superbike")
            );

            Assert.Empty(
                group
                .Find(new EQ("id", "123-superbike"))
                .Elements()
            );
        }

        [Fact]
        public void IncludesMatchingElements()
        {
            Assert.Equal(
                "789-ordinary-bike",
                FirstOf.New(
                    new SimpleGroup("bike",
                        new VehiclesTestSchema(),
                        new ManyOf<IElement>(
                            new SimpleElement(
                                "123-superbike",
                                new TextProp("Name", "the most wonderful bike"),
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "789-ordinary-bike",
                                new TextProp("Name", "the most unspectacular bike"),
                                new OptionsProp("DriveType", "muscle")
                            )
                        )
                    )
                    .Find(new EQ("Name", "the most unspectacular bike"))
                    .Elements()
                )
                .Value()
                .ID()
            );
        }

        [Fact]
        public void ChainsQueries()
        {
            Assert.Equal(
                "789-ordinary-bike",
                FirstOf.New(
                    new SimpleGroup("bike",
                        new VehiclesTestSchema(),
                        new ManyOf<IElement>(
                            new SimpleElement(
                                "123-superbike",
                                new TextProp("Name", "the most wonderful bike"),
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "456-lazy-bike",
                                new OptionsProp("DriveType", "eletric")
                            ),
                            new SimpleElement(
                                "789-ordinary-bike",
                                new TextProp("name", "the most unspectacular bike"),
                                new OptionsProp("DriveType", "muscle")
                            )
                        )
                    )
                    .Find(new EQ("DriveType", "muscle"))
                    .Find(new EQ("id", "789-ordinary-bike"))
                    .Elements()
                )
                .Value()
                .ID()
            );
        }

        [Fact]
        public void LimitsMatchingElements()
        {
            Assert.Equal(
                2,
                new LengthOf(
                    new SimpleGroup("bike",
                        new VehiclesTestSchema(),
                        new ManyOf<IElement>(
                            new SimpleElement(
                                "123-superbike",
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "789-ordinary-bike",
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "1001-diesel-bike",
                                new OptionsProp("DriveType", "combustion")
                            ),
                            new SimpleElement(
                                "456-lazy-bike",
                                new OptionsProp("DriveType", "eletric")
                            )
                        )
                    )
                    .Find(new NOT("DriveType", "muscle"), 0, 2)
                    .Elements()
                )
                .Value()
            );
        }

        [Fact]
        public void SkipsMatchingElements()
        {
            Assert.Equal(
                "789-ordinary-bike",
                FirstOf.New(
                    new SimpleGroup("bike",
                        new VehiclesTestSchema(),
                        new ManyOf<IElement>(
                            new SimpleElement(
                                "123-superbike",
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "789-ordinary-bike",
                                new OptionsProp("DriveType", "muscle")
                            ),
                            new SimpleElement(
                                "1001-diesel-bike",
                                new OptionsProp("DriveType", "combustion")
                            ),
                            new SimpleElement(
                                "456-lazy-bike",
                                new OptionsProp("DriveType", "eletric")
                            )
                        )
                    )
                    .Find(new EQ("DriveType", "muscle"), 1, 1)
                    .Elements()
                )
                .Value()
                .ID()
            );
        }

        [Fact]
        public void DoesNotIncludeNonMatchingElements()
        {
            Assert.False(
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new ManyOf<IElement>(
                        new SimpleElement(
                            "123-superbike",
                            new TextProp("name", "the most wonderful bike"),
                            new OptionsProp("DriveType", "muscle")
                        ),
                        new SimpleElement(
                            "789-ordinary-bike",
                            new TextProp("name", "the most unspectacular bike"),
                            new OptionsProp("DriveType", "muscle")
                        )
                    )
                )
                .Find(new EQ("name", "an average bike"))
                .Elements()
                .GetEnumerator()
                .MoveNext()

            );
        }
    }
}
