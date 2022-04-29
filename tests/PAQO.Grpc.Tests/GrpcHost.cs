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
using System;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PAQO.Grpc.Core.Tests
{
    /// <summary>
    /// A <see cref="Server"/> which shutsdown on dispose
    /// </summary>
    public sealed class GrpcHost : IScalar<Server>, IDisposable
    {
        private static object lockObject = new object();
        private readonly IEnumerable<ServerServiceDefinition> services;
        private readonly ScalarOf<Server> server;

        /// <summary>
        /// A <see cref="Server"/> which shutsdown on dispose
        /// </summary>
        public GrpcHost(params ServerServiceDefinition[] services) : this(
            new ManyOf<ServerServiceDefinition>(services)
        )
        { }

        /// <summary>
        /// A <see cref="Server"/> which shutsdown on dispose
        /// </summary>
        public GrpcHost(IEnumerable<ServerServiceDefinition> services)
        {
            this.services = services;
            this.server = new ScalarOf<Server>(() =>
            {

                lock (lockObject)
                {
                    var port = new UnusedPort().Value();
                    var server =
                       new Server
                       {
                           Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
                       };
                    foreach (var service in this.services)
                    {
                        server.Services.Add(service);
                    }
                    server.Start();
                    return server;
                }
            });
        }

        public async void Dispose()
        {
            await this.server.Value().ShutdownAsync();
        }

        public Server Value()
        {
            return this.server.Value();
        }
    }
}
