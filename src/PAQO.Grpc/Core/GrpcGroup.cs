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

using Grpc.Net.Client;
using PAQO.Core;
using PAQO.Core.Facets;
using PAQO.Core.Find;
using PAQO.Core.Prop;
using PAQO.Grpc.Core.DTO;
using PAQO.Grpc.Core.Group;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;

namespace PAQO.Grpc.Core
{
    public abstract class GrpcGroup
    {
        /// <summary>
        /// Group which is queried and updated via GRPC.
        /// (Rcv = Receiving)
        /// </summary>
        public sealed class Rcv : IGroup
        {
            private readonly string elementType;
            private readonly IEnumerable<IElement> elements;
            private readonly ScalarOf<Group.Group.GroupClient> service;
            private readonly PAQO.Core.ISwap<IQuery, IGroup> toNewGroup;

            /// <summary>
            /// Group which is queried and updated via GRPC.#
            /// (Rcv = Receiving)
            /// </summary>
            public Rcv(string elementType, GrpcChannel channel, CancellationToken cancelToken) : this(
                elementType,
                new ALL(),
                0,
                int.MaxValue,
                new ScalarOf<GrpcChannel>(channel),
                cancelToken
            )
            { }

            /// <summary>
            /// Group which is queried and updated via GRPC.
            /// (Rcv = Receiving)
            /// </summary>
            private Rcv(string elementType, IQuery query, int start, int amount, IScalar<GrpcChannel> channel, CancellationToken cancelToken)
            {
                this.elements =
                    LiveMany.New(() =>
                        Task.Run(async () =>
                        {
                            Debug.WriteLine($"Fetching {elementType}");
                            var incoming =
                                this.service
                                    .Value()
                                    .Find(
                                        new FindParams()
                                        {
                                            ElementType = elementType,
                                            Query = new GrpcQuery.Tmt(query).Value(),
                                            Start = start,
                                            Amount = amount
                                        }
                                    ).ResponseStream;

                            var result = new List<IElement>();
                            while (await incoming.MoveNext(cancelToken))
                            {
                                result.Add(
                                    new GrpcElement.Rcv(
                                        incoming.Current,
                                        this
                                    )
                                );
                            }
                            return result;
                        })
                        .GetAwaiter()
                        .GetResult()
                    );
                this.service =
                    new ScalarOf<Group.Group.GroupClient>(() =>
                        new Group.Group.GroupClient(channel.Value())
                    );
                this.elementType = elementType;
                this.toNewGroup =
                    new SwapOf<IQuery, IGroup>(newQuery =>
                        new Rcv(
                            elementType,
                            new AND(query, newQuery),
                            start,
                            amount,
                            channel,
                            cancelToken
                        )
                    );
            }

            public IEnumerable<IElement> Elements()
            {
                return this.elements;
            }

            public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
            {
                if (start != 0 || amount != int.MaxValue)
                {
                    throw new InvalidOperationException("Cannot change query range (start, amount) after first call of Find().");
                }
                return this.toNewGroup.Flip(query);
            }

            public void Update(IQuery query, params IProp[] props)
            {
                var updateParams = new UpdateParams();
                updateParams.ElementType = this.elementType;
                updateParams.Query = new GrpcQuery.Tmt(query).Value();
                updateParams.Props.AddRange(
                    new GrpcProps.Tmt(
                        new PropsOf(props)
                    )
                );
                this.service
                    .Value()
                    .Update(updateParams);
            }

            public void Add(IEnumerable<IElement> elements)
            {
                var addParams = new AddParams();
                addParams.ElementType = this.elementType;
                addParams.Elements.AddRange(
                    Yaapii.Atoms.Enumerable.Mapped.New(
                        element => new GrpcElement.Tmt(element).Value(),
                        elements
                    )
                );
                this.service
                    .Value()
                    .Add(addParams);
            }

            public void Remove(IQuery query)
            {
                var removeParams = new RemoveParams();
                removeParams.ElementType = this.elementType;
                removeParams.Query = new GrpcQuery.Tmt(query).Value();
                this.service
                    .Value()
                    .Remove(removeParams);
            }

            /// <summary>
            /// A transmittable list of <see cref="RpcElement"/>s from <see cref="IGroup"/>
            /// </summary>
            public sealed class Tmt : ListEnvelope<RpcElement>
            {
                /// <summary>
                /// A transmittable list of <see cref="RpcElement"/>s from <see cref="IElement"/>
                /// </summary>
                public Tmt(IEnumerable<IElement> elements) : base(() =>
                    new Yaapii.Atoms.List.Mapped<IElement, RpcElement>(
                        t =>
                        {
                            var newElement = new RpcElement();
                            newElement.Id = t.ID();
                            newElement.Props.AddRange(new GrpcProps.Tmt(t.Props()));
                            return newElement;

                        },
                        elements
                    ),
                    false
                )
                { }
            }
        }
    }
}
