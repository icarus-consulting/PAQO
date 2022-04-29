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

using PAQO.Core;
using PAQO.Core.Prop;
using PAQO.Grpc.Core.DTO;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;

namespace PAQO.Grpc.Core
{
    public abstract class GrpcProps
    {
        /// <summary>
        /// Props received from GRPC DTO props.
        /// </summary>
        public sealed class Rcv : PropsEnvelope
        {
            /// <summary>
            /// Props received from GRPC DTO element.
            /// </summary>
            public Rcv(RpcElement element) : this(
                new ManyOf<RpcProp>(() => element.Props)
            )
            { }

            /// <summary>
            /// Props received from GRPC DTO props.
            /// </summary>
            public Rcv(IEnumerable<RpcProp> rpcProps) : base(() =>
                new PropsOf(
                    new Yaapii.Atoms.Enumerable.Mapped<RpcProp, IProp>(prop =>
                        new SimpleProp(prop.Name, prop.Content.ToByteArray()),
                        rpcProps
                    )
                )
            )
            { }
        }

        /// <summary>
        /// A list of <see cref="RpcProp"/> from <see cref="IProps"/>
        /// </summary>
        public sealed class Tmt : ListEnvelope<RpcProp>
        {
            /// <summary>
            /// A transmittable list of props.
            /// </summary>
            public Tmt(IProps props) : this(
                new ManyOf<IProp>(() => props.All())
            )
            { }

            /// <summary>
            /// A transmittable list of props.
            /// </summary>
            public Tmt(IEnumerable<IProp> props) : base(() =>
                new ListOf<RpcProp>(
                    new Yaapii.Atoms.Enumerable.Mapped<IProp, RpcProp>(
                        p => new RpcProp()
                        {
                            Name = p.Name(),
                            Content = Google.Protobuf.ByteString.CopyFrom(p.Content())
                        },
                        props
                    )
                ),
                false
            )
            { }
        }
    }
}
