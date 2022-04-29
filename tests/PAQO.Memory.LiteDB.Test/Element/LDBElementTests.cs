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
using PAQO.Core.Facets;
using PAQO.Memory.LiteDB.Facets;
using System;
using System.Text;
using Test.PAQO.Memory.LiteDB;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Memory.LiteDB.Element.Test
{
    public sealed class LDBElementTest
    {
        [Fact]
        public void UpdatesRawPropInDatabase()
        {
            using (var db = new LiteDatabase(new LDBMemoryEngine()))
            {
                var vehicleBson =
                    new BsonDocument(
                        MapOf.New(
                            //Typed property
                            "MaxSpeed", new BsonValue(180),
                            //Raw property
                            "_",
                            new BsonDocument(
                                MapOf.New(
                                    KvpOf.New("MaxSpeed", new BsonValue(BitConverter.GetBytes(180)))
                                )
                            )
                        )
                    );

                var database = db.GetCollection("bike");
                database.Insert(vehicleBson);

                new LDBElement(
                    vehicleBson,
                    new VehiclesTestSchema(),
                    new SwapBytesToBsonValue(),
                    new SwapBytesToBsonStringValue(),
                    database,
                    "bike"
                ).Update(new ManyOf<IProp>(new FkProp("MaxSpeed", 180)));

                Assert.Equal(
                    BitConverter.GetBytes(180),
                    database.FindOne(Query.All())["_"]["MaxSpeed"].RawValue
                );
            }
        }

        [Fact]
        public void UpdatesTypedPropInDatabase()
        {
            using (var db = new LiteDatabase(new LDBMemoryEngine()))
            {
                var vehicleBson =
                    new BsonDocument(
                        MapOf.New(
                            //Typed property
                            "MaxSpeed", new BsonValue(180),
                            //Raw property
                            "_",
                            new BsonDocument(
                                MapOf.New(
                                    KvpOf.New("MaxSpeed", new BsonValue(BitConverter.GetBytes(180)))
                                )
                            )
                        )
                    );

                var database = db.GetCollection("bike");
                database.Insert(vehicleBson);

                new LDBElement(
                    vehicleBson,
                    new VehiclesTestSchema(),
                    new SwapBytesToBsonValue(),
                    new SwapBytesToBsonStringValue(),
                    database,
                    "bike"
                ).Update(new ManyOf<IProp>(new FkProp("MaxSpeed", 225)));

                Assert.Equal(
                    225,
                    database.FindOne(Query.All())["MaxSpeed"].RawValue
                );
            }
        }

        [Fact]
        public void DeliversUpdatedPropValue()
        {
            using (var db = new LiteDatabase(new LDBMemoryEngine()))
            {
                var vehicleBson =
                    new BsonDocument(
                        MapOf.New(
                            //Typed property
                            "maxspeed", new BsonValue(180),
                            //Raw property
                            "_",
                            new BsonDocument(
                                MapOf.New(
                                    KvpOf.New("MaxSpeed", new BsonValue(BitConverter.GetBytes(180)))
                                )
                            )
                        )
                    );

                var database = db.GetCollection("bike");
                database.Insert(vehicleBson);

                var element =
                    new LDBElement(
                        vehicleBson,
                        new VehiclesTestSchema(),
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue(),
                        database,
                        "bike"
                    );
                element.Update(new ManyOf<IProp>(new FkProp("MaxSpeed", 250)));

                Assert.Equal(
                    BitConverter.GetBytes(250),
                    element.Props().All()[0].Content()
                );
            }
        }

        [Fact]
        public void DeliversIntegerProp()
        {
            Assert.Equal(120,
                BitConverter.ToInt32(
                    new FirstOf<IProp>(
                        prop => prop.Name() == "MaxSpeed",
                        new LDBElement(
                            new BsonDocument(
                                new MapOf<BsonValue>(
                                    new KvpOf<BsonValue>("_",
                                        new BsonDocument(
                                            new MapOf<BsonValue>(
                                                new KvpOf<BsonValue>("MaxSpeed", new BsonValue(BitConverter.GetBytes(120)))
                                            )
                                        )
                                    )
                                )
                            ),
                            new VehiclesTestSchema(),
                            new SwapBytesToBsonValue(),
                            new SwapBytesToBsonStringValue(),
                            new DeadLiteCollection(),
                            "bike"
                        )
                        .Props()
                        .All()
                    )
                    .Value()
                    .Content()
                )
            );
        }

        [Fact]
        public void DeliversDecimalProp()
        {
            Assert.Equal(
                4.9,
                BitConverter.ToDouble(
                    new FirstOf<IProp>(
                        prop => prop.Name() == "MaxGearRatio",
                        new LDBElement(
                            new BsonDocument(
                                new MapOf<BsonValue>(
                                    new KvpOf<BsonValue>("_",
                                        new BsonDocument(
                                            new MapOf<BsonValue>(
                                                new KvpOf<BsonValue>("MaxGearRatio", new BsonValue(BitConverter.GetBytes(4.9)))
                                            )
                                        )
                                    )
                                )
                            ),
                            new VehiclesTestSchema(),
                            new SwapBytesToBsonValue(),
                            new SwapBytesToBsonStringValue(),
                            new DeadLiteCollection(),
                            "bike"
                        )
                        .Props()
                        .All()
                    )
                    .Value()
                    .Content()
                )
            );
        }

        [Fact]
        public void DeliversTextProp()
        {
            Assert.Equal(
                "SmartForOne",
                new TextOf(
                    new BytesOf(
                        new FirstOf<IProp>(
                        prop => prop.Name() == "ModelName",
                        new LDBElement(
                            new BsonDocument(
                                new MapOf<BsonValue>(
                                    new KvpOf<BsonValue>("_",
                                        new BsonDocument(
                                            new MapOf<BsonValue>(
                                                new KvpOf<BsonValue>("ModelName", new BsonValue(Encoding.UTF8.GetBytes("SmartForOne")))
                                            )
                                        )
                                    )
                                )
                            ),
                            new VehiclesTestSchema(),
                            new SwapBytesToBsonValue(),
                            new SwapBytesToBsonStringValue(),
                            new DeadLiteCollection(),
                            "bike"
                        )
                        .Props()
                        .All()
                    )
                    .Value()
                    .Content()
                    ),
                    Encoding.UTF8
                ).AsString()
            );
        }

        [Fact]
        public void DeliversOptionProp()
        {
            Assert.Equal(
                "muscle",
                new TextOf(
                    new BytesOf(
                        new FirstOf<IProp>(
                            prop => prop.Name() == "DriveType",
                            new LDBElement(
                                new BsonDocument(
                                    new MapOf<BsonValue>(
                                        new KvpOf<BsonValue>("_",
                                            new BsonDocument(
                                                new MapOf<BsonValue>(
                                                    new KvpOf<BsonValue>("DriveType", new BsonValue(Encoding.UTF8.GetBytes("muscle")))
                                                )
                                            )
                                        )
                                    )
                                ),
                                new VehiclesTestSchema(),
                                new SwapBytesToBsonValue(),
                                new SwapBytesToBsonStringValue(),
                                new DeadLiteCollection(),
                                "bike"
                            )
                            .Props()
                            .All()
                        )
                        .Value()
                        .Content()
                    ),
                    Encoding.UTF8
                ).AsString()
            );
        }

        [Fact]
        public void DeliversSwitchProp()
        {
            Assert.False(
                BitConverter.ToBoolean(
                    new FirstOf<IProp>(
                        prop => prop.Name() == "Damaged",
                        new LDBElement(
                            new BsonDocument(
                                new MapOf<BsonValue>(
                                    new KvpOf<BsonValue>("_",
                                        new BsonDocument(
                                            new MapOf<BsonValue>(
                                                new KvpOf<BsonValue>("Damaged", new BsonValue(BitConverter.GetBytes(false)))
                                            )
                                        )
                                    )
                                )
                            ),
                            new VehiclesTestSchema(),
                            new SwapBytesToBsonValue(),
                            new SwapBytesToBsonStringValue(),
                            new DeadLiteCollection(),
                            "bike"
                        )
                        .Props()
                        .All()
                    )
                    .Value()
                    .Content()
                )
            );
        }

        [Fact]
        public void DeliversComplexProp()
        {
            Assert.Equal(
                new byte[] { 0x00, 0x01, 0x02, 0x03 },
                new FirstOf<IProp>(
                    prop => prop.Name() == "AntiTheftCode",
                    new LDBElement(
                        new BsonDocument(
                            new MapOf<BsonValue>(
                                new KvpOf<BsonValue>("_",
                                    new BsonDocument(
                                        new MapOf<BsonValue>(
                                            new KvpOf<BsonValue>("AntiTheftCode", new BsonValue(new byte[] { 0x00, 0x01, 0x02, 0x03 }))
                                        )
                                    )
                                )
                            )
                        ),
                        new VehiclesTestSchema(),
                        new SwapBytesToBsonValue(),
                        new SwapBytesToBsonStringValue(),
                        new DeadLiteCollection(),
                        "bike"
                    )
                    .Props()
                    .All()
                )
                .Value()
                .Content()
            );
        }
    }
}
