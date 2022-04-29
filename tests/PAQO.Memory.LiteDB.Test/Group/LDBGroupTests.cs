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
using PAQO.Core.Element;
using PAQO.Core.Find;
using PAQO.Core.Group;
using PAQO.Core.Prop;
using PAQO.Memory.LiteDB.Facets;
using System.Diagnostics;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Memory.LiteDB.Group.Test
{
    public sealed class LDBGroupTests
    {
        [Fact]
        public void EQMatchesIntToText()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new TextProp("ModelName", "4045")
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("ModelName", 4045)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void EQMatchesIntToInt()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                new LDBGroup(
                    new SimpleGroup(
                        "bike",
                        new VehiclesTestSchema(),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "1"),
                                new IntProp("MaxSpeed", 1200)
                            )
                        )
                    ),
                    new VehiclesTestSchema(),
                    "bike",
                    engine
                );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new EQ("MaxSpeed", 1200)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void EQMatchesTextInText()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new TextProp("ModelName", "Viper")
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new CONTAINS("ModelName", "Viper")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }

        [Fact]
        public void EQMatchesTextToInteger()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                new LDBGroup(
                    new SimpleGroup(
                        "bike",
                        new VehiclesTestSchema(),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "1"),
                                new IntProp("MaxSpeed", 1337)
                            )
                        ),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "2"),
                                new IntProp("MaxSpeed", 8675)
                            )
                        )
                    ),
                    new VehiclesTestSchema(),
                    "bike",
                    engine
                );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new CONTAINS("MaxSpeed", "1337")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }

        [Fact]
        public void EQMatchesTextToDecimal()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                new LDBGroup(
                    new SimpleGroup(
                        "bike",
                        new VehiclesTestSchema(),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "1"),
                                new DecimalProp("HighestGearRatio", 3.45)
                            )
                        )
                    ),
                    new VehiclesTestSchema(),
                    "bike",
                    engine
                );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new EQ("HighestGearRatio", "3.45")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }



        [Fact]
        public void CONTAINSMatchesTextContainingText()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                new LDBGroup(
                    new SimpleGroup(
                        "bike",
                        new VehiclesTestSchema(),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "1"),
                                new TextProp("ModelName", "Viper")
                            )
                        )
                    ),
                    new VehiclesTestSchema(),
                    "bike"
                );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("ModelName", "ipe")).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesTextContainingInt()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new TextProp("ModelName", "Number 4045")
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("ModelName", 45)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesIntContainingIntInBetween()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new IntProp("MaxSpeed", 1302)
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("MaxSpeed", 30)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesIntContainingIntAtStart()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new IntProp("MaxSpeed", 120)
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("MaxSpeed", 12)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesIntContainingIntAtEnd()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                new LDBGroup(
                    new SimpleGroup(
                        "bike",
                        new VehiclesTestSchema(),
                        new SimpleElement(
                            new PropsOf(
                                new TextProp("id", "1"),
                                new IntProp("MaxSpeed", 120)
                            )
                        )
                    ),
                    new VehiclesTestSchema(),
                    "bike",
                    engine
                );

                group.Find(new ALL());

                Debug.WriteLine(
                    "Query Time: " +
                    new ComputationTimeOf(() =>
                        Assert.Equal(
                            1,
                            new LengthOf(
                                group.Find(new CONTAINS("MaxSpeed", 20)).Elements()
                            ).Value()
                        )
                    ).AsDouble()
                    + "ms"
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesTextInText()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new TextProp("ModelName", "Viper")
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new CONTAINS("ModelName", "ipe")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesTextInInteger()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new IntProp("MaxSpeed", 1337)
                                )
                            ),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "2"),
                                    new IntProp("MaxSpeed", 8675)
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new CONTAINS("MaxSpeed", "33")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }

        [Fact]
        public void CONTAINSMatchesTextInDecimal()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                   new LDBGroup(
                       new SimpleGroup(
                           "bike",
                           new VehiclesTestSchema(),
                           new SimpleElement(
                               new PropsOf(
                                   new TextProp("id", "1"),
                                   new DecimalProp("HighestGearRatio", 13.453)
                               )
                           )
                       ),
                       new VehiclesTestSchema(),
                       "bike",
                       engine
                   );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        group.Find(
                            new CONTAINS("HighestGearRatio", "3.45")
                        )
                        .Elements()
                    ).Value()
                );
            }
        }

        [Fact]
        public void CONTAINSDoesNotMatchLeadingZerosOnInteger()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new IntProp("MaxSpeed", 1)
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Assert.Equal(
                    1,
                    new LengthOf(
                        Yaapii.Atoms.Enumerable.Joined.New(
                            group.Find(
                                new CONTAINS("MaxSpeed", "1")
                            ).Elements(),
                            group.Find(
                                new CONTAINS("MaxSpeed", "0001")
                            ).Elements()
                        )
                    ).Value()
                );
            }
        }

        [Fact]
        public void CONTAINSDoesNotMatchMoreThanRoundedDigits()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var group =
                    new LDBGroup(
                        new SimpleGroup(
                            "bike",
                            new VehiclesTestSchema(),
                            new SimpleElement(
                                new PropsOf(
                                    new TextProp("id", "1"),
                                    new DecimalProp("HighestGearRatio", 811.500000000001)
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        "bike",
                        engine
                    );

                group.Find(new ALL());

                Assert.Empty(
                    group.Find(
                        new CONTAINS("HighestGearRatio", "0001")
                    ).Elements()
                );
            }
        }

        [Fact]
        public void UpdatesOrigin()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var start = 0;
                var ids =
                    new ListOf<string>(
                        new Repeated<string>(
                            () => start++.ToString(),
                            32
                        )
                    );

                var origin =
                    new SimpleGroup("bike",
                        schema,
                        Yaapii.Atoms.Enumerable.Mapped.New(id =>
                            new SimpleElement(id,
                                new TextProp("ModelName", "Gerd")
                            ),
                            ids
                        )
                    );

                var group =
                    new LDBGroup(
                        origin,
                        schema,
                        "bike",
                        engine
                    );

                group
                    .Update(
                        new IN("id", ids),
                        new TextProp("ModelName", "Ulf")
                    );

                foreach (var id in ids)
                {
                    Assert.Equal(
                        "Ulf",
                        new TextOf(
                            FirstOf.New(
                                origin
                                    .Find(new EQ("id", id))
                                    .Elements()
                            ).Value()
                            .Props()
                            .Prop("ModelName")
                            .Content()
                        ).AsString()
                    );
                }
            }
        }

        [Fact]
        public void UpdatesDatabaseEntry()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var start = 0;
                var ids =
                    new ListOf<string>(
                        new Repeated<string>(
                            () => start++.ToString(),
                            32
                        )
                    );

                var conversions = new SwapBytesToBsonValue();
                var origin =
                    new SimpleGroup("bike",
                        schema,
                        Yaapii.Atoms.Enumerable.Mapped.New(id =>
                            new SimpleElement(id,
                                new TextProp("ModelName", "Gerd")
                            ),
                            ids
                        )
                    );

                var group =
                    new LDBGroup(
                        origin,
                        schema,
                        "bike",
                        engine
                    );

                group
                    .Update(
                        new IN("id", ids),
                        new TextProp("ModelName", "Ulf")
                    );

                foreach (var id in ids)
                {
                    Assert.Equal(
                        "Ulf",
                        FirstOf.New(
                            new LiteDatabase(engine)
                                .GetCollection("bike")
                                .Find(Query.EQ("id", id))
                        ).Value()
                        .RawValue["ModelName"]
                        .AsString
                    );
                }
            }
        }
    }
}
