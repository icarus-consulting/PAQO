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
using PAQO.Memory.LiteDB.Element;
using PAQO.Memory.LiteDB.Facets;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Func;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Group.Test
{
    public sealed class LDBActiveUpdateGroupTests
    {
        [Fact]
        public void AddsToDatabase()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var innerGroup = new SimpleGroup("bike", schema);

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike", schema,
                        new LDBQueryingGroup( //This is necessary because the LDB Group objects assume that the inner groups are all LDB based.
                            "bike",
                            schema,
                            innerGroup,
                            engine,
                            swapToBson,
                            swapToBsonString
                        ),
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                activeUpdateGroup.Add(
                    ManyOf.New(
                        new SimpleElement("1",
                            new TextProp("Name", $"Combustion-Apparatus")
                        )
                    )
                );

                Assert.Equal(
                    "Combustion-Apparatus",
                    FirstOf.New(
                        new LiteDatabase(engine)
                            .GetCollection("bike")
                            .Find(Query.EQ("id", "1"))
                    ).Value()
                    .RawValue["Name"]
                    .AsString
                );
            }
        }

        [Fact]
        public void AddsToInnerGroup()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var innerGroup = new SimpleGroup("bike", schema);

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike",
                        schema,
                        innerGroup,
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    );

                activeUpdateGroup.Add(
                    ManyOf.New(
                        new SimpleElement("1",
                            new TextProp("Name", $"Combustion-Apparatus")
                        )
                    )
                );

                Assert.Equal(
                    "Combustion-Apparatus",
                    new TextProp.AsText("Name",
                        FirstOf.New(
                            innerGroup.Find(new EQ("id", "1")).Elements()
                        )
                        .Value()
                        .Props()
                    ).AsString()
                );
            }
        }

        [Fact]
        public void RemovesFromDatabase()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var innerGroup =
                    new SimpleGroup("bike", schema,
                        new SimpleElement("1",
                            new TextProp("Name", $"Combustion-Apparatus")
                        )
                    );

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike", schema,
                        new LDBQueryingGroup( //This is necessary because the LDB Group objects assume that the inner groups are all LDB based.
                            "bike",
                            schema,
                            innerGroup,
                            engine,
                            swapToBson,
                            swapToBsonString
                        ),
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                activeUpdateGroup.Remove(
                    new EQ("id", "1")
                );

                Assert.Empty(
                    new LiteDatabase(engine)
                        .GetCollection("bike")
                        .Find(Query.EQ("id", "1"))
                );
            }
        }

        [Fact]
        public void RemovesFromInnerGroup()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var innerGroup =
                    new SimpleGroup("bike", schema,
                        new SimpleElement("1",
                            new TextProp("Name", $"Combustion-Apparatus")
                        )
                    );

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike",
                        schema,
                        innerGroup,
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    );

                activeUpdateGroup.Remove(
                    new EQ("id", "1")
                );

                Assert.Empty(
                    innerGroup.Find(new EQ("id", "1")).Elements()
                );
            }
        }

        [Fact]
        public void UpdatesDatabase()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();

                var id = 1;
                var innerGroup =
                    new SimpleGroup(
                        "bike",
                        schema,
                        Repeated.New(() =>
                            new SimpleElement(id++.ToString(),
                                new TextProp("Name", $"Car #{id}")
                            ),
                            16
                        )
                    );

                var db = new LiteDatabase(engine).GetCollection("bike");
                new Each<IElement>(element =>
                    db.Insert(
                        new LDBElementBson(
                            "bike",
                            element,
                            schema,
                            swapToBson,
                            swapToBsonString
                        ).Value()
                    ),
                    innerGroup.Elements()
                ).Invoke();

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike", schema,
                        new LDBQueryingGroup( //This is necessary because the LDB Group objects assume that the inner groups are all LDB based.
                            "bike",
                            schema,
                            innerGroup,
                            engine,
                            swapToBson,
                            swapToBsonString
                        ),
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                activeUpdateGroup.Update(new EQ("id", "8"), new TextProp("Name", "Combustion-Apparatus #8"));

                Assert.Equal(
                    "Combustion-Apparatus #8",
                    FirstOf.New(
                        db.Find(Query.EQ("id", "8"))
                    ).Value()
                    .RawValue["Name"]
                    .AsString
                );
            }
        }

        [Fact]
        public void UpdatesInnerGroup()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();

                var id = 1;
                var innerGroup =
                    new SimpleGroup("bike", schema,
                        Repeated.New(() =>
                            new SimpleElement(id++.ToString(),
                                new TextProp("Name", $"Car #{id}")
                            ),
                            16
                        )
                    );

                var activeUpdateGroup =
                    new LDBActiveUpdateGroup("bike", schema,
                        innerGroup,
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    );

                activeUpdateGroup.Update(new EQ("id", "8"), new TextProp("Name", "Combustion-Apparatus #8"));

                Assert.Equal(
                    "Combustion-Apparatus #8",
                    new TextProp.AsText("Name",
                        FirstOf.New(
                            innerGroup.Find(new EQ("id", "8")).Elements()
                        )
                        .Value()
                        .Props()
                    ).AsString()
                );
            }
        }
    }
}
