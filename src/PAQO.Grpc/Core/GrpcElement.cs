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
using PAQO.Core.Find;
using PAQO.Core.Prop;
using PAQO.Grpc.Core.DTO;
using System.Collections.Generic;
using System.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Grpc.Core
{
    public abstract class GrpcElement
    {
        /// <summary>
        /// An element which can update via GRPC.
        /// Rcv = Receiving
        /// </summary>
        public sealed class Rcv : IElement
        {
            private readonly IText id;
            private readonly IDictionary<string, IProp> props;
            private readonly IGroup grpcUpdateGroup;

            /// <summary>
            /// An element which can update via GRPC.
            /// Rcv = Receiving
            /// </summary>
            public Rcv(RpcElement rpcElement, IGroup grpcUpdateGroup)
            {
                this.props =
                    new MutableMap<IProp>(
                        new Mapped<RpcProp, IKvp<IProp>>(
                            rpcProp => new KvpOf<IProp>(rpcProp.Name, new GrpcProp.Rcv(rpcProp)),
                            rpcElement.Props
                        )
                    );
                this.id =
                    new TextOf(() =>
                        new TextProp.AsText(
                            this.props["id"].Content()
                        ).AsString()
                    );
                this.grpcUpdateGroup = grpcUpdateGroup;
            }

            public string ID()
            {
                return this.id.AsString();
            }

            public IProps Props()
            {
                return new PropsOf(this.props.Values);
            }

            public void Update(IEnumerable<IProp> props)
            {
                //Update usign the group.
                this.grpcUpdateGroup
                    .Update(new EQ("id", this.id.AsString()), props.ToArray());
                foreach (var prop in props)
                {
                    this.props[prop.Name()] = prop;
                }
            }
        }

        /// <summary>
        /// An element as GRPC DTO.
        /// Tmt = Transmittable
        /// </summary>
        public sealed class Tmt : ScalarEnvelope<RpcElement>
        {
            /// <summary>
            /// An element as GRPC DTO.
            /// Tmt = Transmittable
            /// </summary>
            public Tmt(IElement element) : base(() =>
            {
                var result =
                    new RpcElement()
                    {
                        Id = element.ID(),
                    };
                result.Props.AddRange(new GrpcProps.Tmt(element.Props()));
                return result;
            })
            { }
        }
    }
}
