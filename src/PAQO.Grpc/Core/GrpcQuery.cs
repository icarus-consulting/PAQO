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

using BriX;
using BriX.Media;
using PAQO.Core;
using PAQO.Core.Find;
using PAQO.Grpc.Core.DTO;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Grpc.Core
{
    public abstract class GrpcQuery
    {
        /// <summary>
        /// A <see cref="IQuery"/> received from grpc.
        /// </summary>
        public sealed class Rcv : QueryEnvelope
        {
            /// <summary>
            /// A <see cref="IQuery"/> received from grpc.
            /// </summary>
            public Rcv(RpcQuery query) : base(() =>
                new BxQuery(
                    new BxRebuilt(query.QueryBrix)
                )
            )
            { }
        }

        /// <summary>
        /// A grpc transmittable <see cref="IQuery"/>
        /// </summary>
        public sealed class Tmt : ScalarEnvelope<RpcQuery>
        {
            /// <summary>
            /// A grpc transmittable <see cref="IQuery"/>
            /// </summary>
            public Tmt(IQuery query) : base(() =>
            {
                return new RpcQuery()
                {
                    QueryBrix =
                        new TextOf(
                            query
                                .AsBrix()
                                .Print(new RebuildMedia()).ToString()
                        ).AsString()
                };
            })
            { }
        }
    }
}
