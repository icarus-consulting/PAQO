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
using PAQO.Core.Prop;
using PAQO.Memory.LiteDB.Element;
using PAQO.Memory.LiteDB.Facets;
using System;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Func;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Group.Test
{
    public sealed class LDBQueryingGroupTests
    {
        [Fact]
        public void QueriesDatabase()
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
                    1,
                    new LengthOf(
                        group.Find(new EQ("Name", "Car #8")).Elements()
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
                    "Car #12",
                    new TextProp.AsText(
                        FirstOf.New(
                            group
                                .Find(new CONTAINS("Name", "1"))
                                .Find(new CONTAINS("Name", "2"))
                                .Elements()
                        )
                        .Value()
                        .Props()
                        .Content("Name")
                    ).AsString()
                );
            }
        }

        [Fact]
        public void WorksWithEmptyValues()
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
                            new TextProp("Name", $"Car #{id}"),
                            new TextProp("ModelName", id == 5 ? "" : "model")
                        ),
                        16
                    )
                ).Invoke();

                Assert.Single(
                    group
                        .Find(new EQ("ModelName", ""))
                        .Elements()
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

        [Fact]
        public void RejectsUpdate()
        {
            using (var engine = new LDBMemoryEngine())
            {
                Assert.Throws<InvalidOperationException>(() =>
                    new LDBQueryingGroup("bike",
                        new VehiclesTestSchema(),
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    ).Update(new ALL(), new TextProp("Name", "Database-Justus"))
                );
            }
        }

        [Fact]
        public void RejectsAdding()
        {
            using (var engine = new LDBMemoryEngine())
            {
                Assert.Throws<InvalidOperationException>(() =>
                    new LDBQueryingGroup("bike",
                        new VehiclesTestSchema(),
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    ).Add(ManyOf.New(new SimpleElement("1")))
                );
            }
        }

        [Fact]
        public void RejectsRemoval()
        {
            using (var engine = new LDBMemoryEngine())
            {
                Assert.Throws<InvalidOperationException>(() =>
                    new LDBQueryingGroup("bike",
                        new VehiclesTestSchema(),
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue()
                    ).Remove(new ALL())
                );
            }
        }
    }
}
