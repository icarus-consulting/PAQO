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
using PAQO.Core.Prop;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;
using PAQO.Grpc.Core.Tests;
using PAQO.Core.Element;
using PAQO.Core.Group;
using Test.PAQO.Schema;
using System.Threading;

namespace PAQO.Grpc.Core.Element.Tests
{
    public sealed class GrpcElementTests
    {
        [Fact]
        public void UpdatesPropsInSource()
        {
            var elementSource =
                new SimpleElement(
                    "1", new TextProp("Name", "find me")
                );

            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SimpleGroup("bike",
                                new VehiclesTestSchema(),
                                elementSource
                            )
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(server.Value().Ports).Value().Port;

                //This group is used by the element to apply updates to the remote source
                var grpcElements =
                    new GrpcGroup.Rcv(
                        "bike",
                        GrpcChannel.ForAddress($"http://localhost:{port}"),
                        new CancellationTokenSource().Token
                    );

                var element =
                    new GrpcElement.Rcv(
                        new GrpcElement.Tmt(elementSource).Value(),
                        grpcElements
                    );

                element.Update(
                    new ManyOf<IProp>(
                        new TextProp("Name", "Rumpelstilzchen")
                    )
                );

                Assert.Equal(
                    "Rumpelstilzchen",
                    new TextProp.AsText(
                        elementSource
                            .Props()
                            .Content("Name")
                    ).AsString()
                );
            }
        }

        [Fact]
        public void UpdatesLocalPropBuffer()
        {
            var elementSource =
                new SimpleElement(
                    "1", new TextProp("Name", "find me")
                );

            using (var server =
                new GrpcHost(
                    Group.Group.BindService(
                        new GroupTransfer(
                            new SimpleGroup("bike",
                                new VehiclesTestSchema(),
                                elementSource
                            )
                        )
                    )
                )
            )
            {
                var port = new FirstOf<ServerPort>(server.Value().Ports).Value().Port;

                //This group is used by the element to apply updates to the remote source
                var grpcElements =
                    new GrpcGroup.Rcv(
                        "bike",
                        GrpcChannel.ForAddress($"http://localhost:{port}"),
                        new CancellationTokenSource().Token
                    );

                var element =
                    new GrpcElement.Rcv(
                        new GrpcElement.Tmt(elementSource).Value(),
                        grpcElements
                    );

                element.Update(
                    new ManyOf<IProp>(
                        new TextProp("Name", "Rumpelstilzchen")
                    )
                );

                //Ensure assertion data is not pulled from source
                elementSource.Update(
                    new ManyOf<IProp>(new TextProp("Name", "Horst"))
                );

                Assert.Equal(
                    "Rumpelstilzchen",
                    new TextProp.AsText(
                        element.Props().Content("Name")
                    ).AsString()
                );

            }
        }
    }
}
