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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PAQO.Grpc
{
    /// <summary>
    /// A randomly chosen port which is guaranteed to be free.
    /// </summary>
    public sealed class UnusedPort : ScalarEnvelope<int>
    {
        /// <summary>
        /// A randomly chosen port which is guaranteed to be free.
        /// </summary>
        public UnusedPort() : this(1025, 65535)
        { }

        /// <summary>
        /// A randomly chosen port which is guaranteed to be free.
        /// </summary>
        public UnusedPort(int min, int max) : base(() =>
            {
                var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                var usedPorts =
                    new List<int>(
                        new Joined<int>(
                            new Mapped<TcpConnectionInformation, int>(
                                conn => conn.LocalEndPoint.Port,
                                ipProperties.GetActiveTcpConnections()
                            ),
                            new Mapped<IPEndPoint, int>(
                                listener => listener.Port,
                                new Joined<IPEndPoint>(
                                    ipProperties.GetActiveTcpListeners(),
                                    ipProperties.GetActiveUdpListeners()
                                )
                            )


                        )
                    );

                int? found = null;
                for (int port = min; port <= max; port++)
                {
                    if (!usedPorts.Contains(port))
                    {
                        found = port;
                        break;
                    }
                }

                if (!found.HasValue)
                {
                    throw new ApplicationException($"Cannot find free port in range {min} to {max}, because all ports are occupied.");
                }
                return found.Value;
            }
        )
        { }
    }
}
