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
using PAQO.Core.Pulse;
using PAQO.Memory.LiteDB.Element;
using PAQO.Memory.LiteDB.Facets;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Func;
using Yaapii.Atoms.Scalar;
using Yaapii.Pulse;

namespace PAQO.Memory.LiteDB.Group.Test
{
    public sealed class LDBPassiveUpdateGroupTests
    {
        [Fact]
        public void UpdatesDatabaseOnSignal()
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

                var pulse = new SyncPulse();
                var passiveUpdateGroup =
                    new LDBPassiveUpdateGroup("bike", schema,
                        innerGroup,
                        engine,
                        swapToBson,
                        swapToBsonString,
                        pulse
                    );

                passiveUpdateGroup.Elements();

                innerGroup.Update(new EQ("id", "4"), new TextProp("Name", "Golf 4"));

                pulse.Send(new SigElementsUpdated("UnitTest", "bike", new ManyOf("4")));

                Assert.Equal(
                    "Golf 4",
                    FirstOf.New(
                        db.Find(Query.EQ("id", "4"))
                    ).Value()
                    .RawValue["Name"]
                    .AsString
                );
            }
        }

        [Fact]
        public void AddsToDatabaseOnSignal()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();

                var innerGroup = new SimpleGroup("bike", schema);

                var db = new LiteDatabase(engine).GetCollection("bike");

                var pulse = new SyncPulse();
                var passiveUpdateGroup =
                    new LDBPassiveUpdateGroup("bike", schema,
                        innerGroup,
                        engine,
                        swapToBson,
                        swapToBsonString,
                        pulse
                    );

                passiveUpdateGroup.Elements();

                innerGroup.Add(
                    ManyOf.New(
                        new SimpleElement("4",
                            new TextProp("Name", $"Golf 5")
                        )
                    )
                );

                pulse.Send(new SigElementsAdded("UnitTest", "bike", new ManyOf("4")));

                Assert.Equal(
                    "Golf 5",
                    FirstOf.New(
                        db.Find(Query.EQ("id", "4"))
                    ).Value()
                    .RawValue["Name"]
                    .AsString
                );
            }
        }

        [Fact]
        public void RemovesFromDatabaseOnSignal()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var swapToBson = new SwapBytesToBsonValue();
                var swapToBsonString = new SwapBytesToBsonStringValue();

                var innerGroup =
                    new SimpleGroup("bike", schema,
                        new SimpleElement("4",
                                new TextProp("Name", $"Golf 5")
                            )
                    );

                var db = new LiteDatabase(engine).GetCollection("bike");

                var pulse = new SyncPulse();
                var passiveUpdateGroup =
                    new LDBPassiveUpdateGroup("bike", schema,
                        new LDBImportingGroup(
                            "bike",
                            innerGroup,
                            schema,
                            engine,
                            swapToBson,
                            swapToBsonString
                        ),
                        engine,
                        swapToBson,
                        swapToBsonString,
                        pulse
                    );

                passiveUpdateGroup.Elements(); //trigger import

                innerGroup.Remove(
                    new EQ("id", "4")
                );

                pulse.Send(new SigElementsRemoved("UnitTest", "bike", new ManyOf("4")));

                Assert.Empty(
                    db.Find(Query.EQ("id", "4"))
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

                var passiveUpdateGroup =
                    new LDBPassiveUpdateGroup("bike", schema,
                        innerGroup,
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue(),
                        new SyncPulse()
                    );

                passiveUpdateGroup.Elements();

                passiveUpdateGroup.Add(
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
        public void UpdatesInnerGroup()
        {
            using (var engine = new LDBMemoryEngine())
            {
                var schema = new VehiclesTestSchema();
                var innerGroup =
                    new SimpleGroup("bike", schema,
                        new SimpleElement("1",
                            new TextProp("Name", $"Car(l)")
                        )
                    );

                var passiveUpdateGroup =
                    new LDBPassiveUpdateGroup("bike", schema,
                        innerGroup,
                        engine,
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue(),
                        new SyncPulse()
                    );

                passiveUpdateGroup.Update(new EQ("id", "1"), new TextProp("Name", "Combustion-Apparatus"));

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
    }
}
