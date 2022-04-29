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

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PAQO.Core;
using PAQO.Core.Element;
using PAQO.Core.Facets;
using PAQO.Grpc.Core.DTO;
using PAQO.Grpc.Core.Group;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yaapii.Atoms.Enumerable;

namespace PAQO.Grpc.Core
{
    /// <summary>
    /// Provides remote access to a local IGroup.
    /// </summary>
    public sealed class GroupTransfer : Group.Group.GroupBase
    {
        private readonly ISwap<string, IGroup> swapTypeToGroup;
        private readonly Action<Exception> onError;

        /// <summary>
        /// Provides remote access to a local IGroup.
        /// </summary>
        public GroupTransfer(IGroup group) : this(
            new SwapOf<string, IGroup>(irrelevant => group),
            (ex) => { }
        )
        { }

        /// <summary>
        /// Provides remote access to multiple local IGroups.
        /// CAREFUL: If you inject a swap which is lazy and you construct a new group,
        /// this will not work because it will construct a new group on every method call of this group.
        /// </summary>
        /// <param name="elementTypeToGroup">Swap element type to group (meaning group with correct schema).</param>
        public GroupTransfer(ISwap<string, IGroup> elementTypeToGroup) : this(
            elementTypeToGroup,
            (ex) => { }
        )
        { }

        /// <summary>
        /// Provides remote access to multiple local IGroups.
        /// CAREFUL: If you inject a swap which is lazy and you construct a new group,
        /// this will not work because it will construct a new group on every method call of this group.
        /// </summary>
        /// <param name="elementTypeToGroup">Swap element type to group (meaning group with correct schema).</param>
        public GroupTransfer(ISwap<string, IGroup> elementTypeToGroup, Action<Exception> onError)
        {
            this.swapTypeToGroup = elementTypeToGroup;
            this.onError = onError;
        }

        public override Task Find(FindParams request, IServerStreamWriter<RpcElement> responseStream, ServerCallContext context)
        {
            return
                ReactOnError(() =>
                {
                    return
                        Task.Run(async () =>
                        {
                            // query elements
                            // write elements to response stream
                            foreach (var element in
                                this.swapTypeToGroup
                                    .Flip(request.ElementType)
                                    .Find(
                                        new GrpcQuery.Rcv(request.Query)
                                    ).Elements()
                            )
                            {
                                await responseStream.WriteAsync(new GrpcElement.Tmt(element).Value());
                            }
                        });
                });
        }

        public override Task<Empty> Update(UpdateParams request, ServerCallContext context)
        {
            return
                ReactOnError(() =>
                {
                    return
                        Task.Run(() =>
                        {
                            this.swapTypeToGroup
                                .Flip(request.ElementType)
                                .Update(
                                    new GrpcQuery.Rcv(request.Query),
                                    new GrpcProps.Rcv(request.Props)
                                        .All()
                                        .ToArray()
                                );
                            return new Empty();
                        });
                });
        }

        public override Task<Empty> Add(AddParams request, ServerCallContext context)
        {
            return
                ReactOnError(() =>
                {
                    return
                        Task.Run(() =>
                        {
                            this.swapTypeToGroup
                                .Flip(request.ElementType)
                                .Add(
                                    Mapped.New(element =>
                                        new SimpleElement(element.Id,
                                            Mapped.New(
                                                prop => new GrpcProp.Rcv(prop),
                                                element.Props
                                            ).ToArray()
                                        ),
                                        request.Elements
                                    )
                                );
                            return new Empty();
                        });
                });
        }

        public override Task<Empty> Remove(RemoveParams request, ServerCallContext context)
        {
            return
                ReactOnError(() =>
                {
                    return
                        Task.Run(() =>
                        {
                            this.swapTypeToGroup
                                .Flip(request.ElementType)
                                .Remove(new GrpcQuery.Rcv(request.Query));
                            return new Empty();
                        });
                });
        }

        private T ReactOnError<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                this.onError(ex);
                throw;
            }
        }
    }
}
