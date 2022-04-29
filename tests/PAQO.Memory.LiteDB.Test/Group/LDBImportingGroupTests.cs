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
    public sealed class LDBImportingGroupTests
    {
        [Fact]
        public void ImportsToDatabaseOnFind()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement("1", new TextProp("Name", "Mr In-The-Database-Soon"))
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                group.Find(new ALL());

                var db = new LiteDatabase(engine).GetCollection("bike");

                Assert.Equal(
                    1,
                    new LengthOf(
                        db.Find(Query.EQ("Name", "Mr In-The-Database-Soon"))
                    ).Value()
                );
            }
        }

        [Fact]
        public void ImportsToDatabaseOnListingElements()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement("1", new TextProp("Name", "Mr In-The-Database-Soon"))
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                group.Elements();

                var db = new LiteDatabase(engine).GetCollection("bike");

                Assert.Equal(
                    1,
                    new LengthOf(
                        db.Find(Query.EQ("Name", "Mr In-The-Database-Soon"))
                    ).Value()
                );
            }
        }

        [Fact]
        public void ImportsToDatabaseOnAdd()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement("1", new TextProp("Name", "Mr In-The-Database-Soon"))
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                group.Add(
                    ManyOf.New(
                        new SimpleElement("999",
                            new TextProp("Name", "This will not be in the database because it needs an UpdateGroup")
                        )
                    )
                );

                var db = new LiteDatabase(engine).GetCollection("bike");

                Assert.Equal(
                    1,
                    new LengthOf(
                        db.Find(Query.EQ("Name", "Mr In-The-Database-Soon"))
                    ).Value()
                );
            }
        }

        [Fact]
        public void ImportsToDatabaseOnRemove()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement("1", new TextProp("Name", "Mr In-The-Database-Soon"))
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                group.Remove(new EQ("Name", "This is not existing and only here to trigger import"));

                var db = new LiteDatabase(engine).GetCollection("bike");

                Assert.Equal(
                    1,
                    new LengthOf(
                        db.Find(Query.EQ("Name", "Mr In-The-Database-Soon"))
                    ).Value()
                );
            }
        }

        [Fact]
        public void ImportsToDatabaseOnUpdate()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement("1", new TextProp("Name", "Mr In-The-Database-Soon"))
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                group.Update(
                    new EQ("Name", "Non Existing Name"),
                    new TextProp("Name", "This will not be in the database")
                );

                var db = new LiteDatabase(engine).GetCollection("bike");

                Assert.Equal(
                    1,
                    new LengthOf(
                        db.Find(Query.EQ("Name", "Mr In-The-Database-Soon"))
                    ).Value()
                );
            }
        }

        [Fact]
        public void ChainsQueries()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBImportingGroup("bike",
                        new SimpleGroup(
                            "bike",
                            schema,
                            new SimpleElement(
                                "1",
                                new TextProp("ModelName", "Klaus"),
                                new TextProp("Name", "Mr In-The-Database-Soon")
                            ),
                            new SimpleElement(
                                "2",
                                new TextProp("ModelName", "Herbert"),
                                new TextProp("Name", "Mr In-The-Database-Soon")
                            )
                        ),
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                Assert.Equal(
                    "2",
                    new TextProp.AsText(
                        "id",
                        FirstOf.New(
                            group.Find(new EQ("Name", "Mr In-The-Database-Soon"))
                                .Find(new EQ("ModelName", "Herbert"))
                                .Elements()
                        ).Value()
                        .Props()
                    ).AsString()
                );
            }
        }

        [Fact]
        public void DeliversAllElements()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();
                var group =
                    new LDBQueryingGroup("bike",
                        schema,
                        engine,
                        swapToBson,
                        swapToBsonString
                    );

                var db = new LiteDatabase(engine).GetCollection("bike");

                var id = 1;
                new Each<IElement>(
                    element =>
                        db.Insert(
                            new LDBElementBson(
                                "bike",
                                element,
                                schema,
                                swapToBson,
                                swapToBsonString
                            ).Value()
                        ),
                    Repeated.New(() =>
                        new SimpleElement(id++.ToString(),
                            new TextProp("Name", $"Car #{id}")
                        ),
                        16
                    )
                ).Invoke();

                Assert.Equal(
                    Mapped.New(
                        entry => entry.RawValue["id"].AsString,
                        db.FindAll()
                    ),
                    Mapped.New(
                        element => element.ID(),
                        group.Elements()
                    )
                );
            }
        }
    }
}
