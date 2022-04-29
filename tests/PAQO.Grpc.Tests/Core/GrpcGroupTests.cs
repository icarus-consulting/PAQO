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

using Grpc.Core;
using Grpc.Net.Client;
using PAQO.Core;
using PAQO.Core.Element;
using PAQO.Core.Facets;
using PAQO.Core.Find;
using PAQO.Core.Group;
using PAQO.Core.Prop;
using PAQO.Memory.LiteDB.Facets;
using PAQO.Memory.LiteDB.Group;
using System.Diagnostics;
using System.Threading;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PAQO.Grpc.Core.Tests
{
    public sealed class GrpcGroupTests
    {
        [Fact]
        public void FindsElementsByQuery()
        {
            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType =>
                                new SimpleGroup(elementType,
                                    new VehiclesTestSchema(),
                                    new SimpleElement(
                                        "1", new TextProp("Name", "find me")
                                    ),
                                    new SimpleElement(
                                        "9", new TextProp("Name", "do not find me")
                                    )
                                )
                            )
                        )
                    )
                )
            )
            {
                Assert.Equal(
                    "1",
                    FirstOf.New(
                        new GrpcGroup.Rcv("bike",
                            GrpcChannel.ForAddress(
                                $"http://localhost:{new FirstOf<ServerPort>(server.Value().Ports).Value().Port}"
                            ),
                            new CancellationTokenSource().Token
                        ).Find(
                            new EQ("Name", "find me")
                        ).Elements()
                    ).Value()
                    .ID()
                );
            }
        }

        [Fact]
        public void AddsElements()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement(
                        "1", new TextProp("Name", "do not find me")
                    )
                );
            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType => group)
                        )
                    )
                )
            )
            {
                var remoteGroup =
                    new GrpcGroup.Rcv(
                            "bike",
                            GrpcChannel.ForAddress(
                                $"http://localhost:{new FirstOf<ServerPort>(server.Value().Ports).Value().Port}"
                            ),
                            new CancellationTokenSource().Token
                        );

                remoteGroup.Add(
                    ManyOf.New(
                        new SimpleElement(
                            "9", new TextProp("Name", "The New One")
                        )
                    )
                );

                Assert.Equal(
                    "9",
                    FirstOf.New(
                        remoteGroup
                            .Find(new EQ("Name", "The New One"))
                            .Elements()
                    ).Value()
                    .ID()
                );
            }
        }

        [Fact]
        public void RemovesElements()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement(
                        "1", new TextProp("Name", "do not find me")
                    )
                );
            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType => group)
                        )
                    )
                )
            )
            {
                var remoteGroup =
                    new GrpcGroup.Rcv(
                            "bike",
                            GrpcChannel.ForAddress(
                                $"http://localhost:{new FirstOf<ServerPort>(server.Value().Ports).Value().Port}"
                            ),
                            new CancellationTokenSource().Token
                        );

                remoteGroup.Remove(
                    new EQ("id", "1")
                );
                Assert.Empty(remoteGroup.Elements());
            }
        }

        [Fact]
        public void ForPerformanceMeasures()
        {
            var amount = 8192;
            var engine = new LDBMemoryEngine();
            var toBson = new SwapBytesToBsonValue();
            var toBsonString = new SwapBytesToBsonStringValue();
            var schema = new VehiclesTestSchema();
            var currentID = 0;
            var group =
                new SimpleGroup("bike", new VehiclesTestSchema(),
                    Repeated.New(() =>
                        new SimpleElement(currentID++.ToString(),
                            new TextProp("Name", $"Element #{currentID}"),
                            new IntProp("MaxSpeed", currentID * 10)
                        ),
                        amount
                    )
                );

            group.Find(new ALL());

            using (var host =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType => group)
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(host.Value().Ports).Value().Port;
                var grpcGroup =
                    new LDBQueryingGroup("bike", schema,
                        new LDBImportingGroup("bike",
                            new GrpcGroup.Rcv("bike",
                                GrpcChannel.ForAddress($"http://localhost:{port}"),
                                new CancellationTokenSource().Token
                            ),
                            new VehiclesTestSchema(),
                            engine,
                            toBson,
                            toBsonString
                        ),
                        engine,
                        toBson,
                        toBsonString
                    );
                grpcGroup.Find(new ALL());

                Debug.WriteLine($"Querytime of {amount} elements:" +
                    new ComputationTimeOf(() =>
                        Assert.Equal(4,
                            new LengthOf(
                                grpcGroup
                                    .Find(new LT("MaxSpeed", 50))
                                    .Elements()
                            ).Value()
                        )
                    ).AsDouble()
                );
            }
        }

        [Fact]
        public void DeliversElementProps()
        {
            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType =>
                                new SimpleGroup(
                                    elementType,
                                    new VehiclesTestSchema(),
                                    new SimpleElement(
                                        "1", new TextProp("Name", "find me")
                                    ),
                                    new SimpleElement(
                                        "9", new TextProp("Name", "do not find me")
                                    )
                                )
                            )
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(server.Value().Ports).Value().Port;
                Assert.Equal("find me",
                    new TextProp.AsText("Name",
                        FirstOf.New(
                            new GrpcGroup.Rcv("bike",
                                GrpcChannel.ForAddress($"http://localhost:{port}"),
                                new CancellationTokenSource().Token
                            ).Find(
                                new EQ("Name", "find me")
                            ).Elements()
                        )
                        .Value()
                        .Props()
                    ).AsString()
                );
            }
        }

        [Fact]
        public void UpdatesElementsViaGroup()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement(
                        "1",
                        new TextProp("Name", "Ye olde Name")
                    ),
                    new SimpleElement(
                        "9", new TextProp("Name", "Untouchable Name")
                    )
                );

            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType =>
                                group
                            )
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(server.Value().Ports).Value().Port;
                var grpcElements =
                    new GrpcGroup.Rcv(
                        "bike",
                        GrpcChannel.ForAddress($"http://localhost:{port}"),
                        new CancellationTokenSource().Token
                    );
                grpcElements.Update(
                    new EQ("id", "1"),
                    new TextProp("name", "Fresh Prince")
                );

                Assert.Equal(
                    "Fresh Prince",
                    new TextProp.AsText(
                        "name",
                        FirstOf.New(
                            grpcElements.Find(
                                new EQ("id", "1")
                            ).Elements()
                        )
                        .Value()
                        .Props()
                    ).AsString()
                );
            }
        }

        [Fact]
        public void UpdatesDeliveredElementObjects()
        {
            var group =
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement(
                        "1",
                        new TextProp("Name", "Ye olde Name")
                    ),
                    new SimpleElement(
                        "9", new TextProp("Name", "Untouchable Name")
                    )
                );

            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SwapOf<string, IGroup>(elementType =>
                                group
                            )
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(server.Value().Ports).Value().Port;
                var grpcElements =
                    new GrpcGroup.Rcv(
                        "bike",
                        GrpcChannel.ForAddress($"http://localhost:{port}"),
                        new CancellationTokenSource().Token
                    );

                var element =
                    FirstOf.New(
                        grpcElements.Find(
                            new EQ("id", "1")
                        )
                        .Elements()
                    ).Value();

                element.Update(
                    ManyOf.New(
                        new TextProp("Name", "Fresh Prince")
                    )
                );

                Assert.Equal(
                    "Fresh Prince",
                    new TextProp.AsText(
                        "Name",
                        FirstOf.New(
                            grpcElements.Find(
                                new EQ("id", "1")
                            ).Elements()
                        )
                        .Value()
                        .Props()
                    ).AsString()
                );
            }
        }
    }
}
