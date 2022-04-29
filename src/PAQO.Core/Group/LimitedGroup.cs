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
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;

namespace PAQO.Core.Group
{
    /// <summary>
    /// A group which limits every given query to the
    /// injected element IDs.
    /// </summary>
    public sealed class LimitedGroup : IGroup
    {
        private readonly IList<string> allowedIDs;
        private readonly IGroup origin;

        /// <summary>
        /// A group which limits every given query to the
        /// injected element IDs.
        /// </summary>
        public LimitedGroup(IGroup origin, IEnumerable<string> allowedIDs)
        {
            this.origin = origin;
            this.allowedIDs = new ListOf<string>(allowedIDs);
        }

        public IEnumerable<IElement> Elements()
        {
            return
                Filtered.New(
                    element => this.allowedIDs.Contains(element.ID()),
                    this.origin.Elements()
                );
        }

        public void Add(IEnumerable<IElement> elements)
        {
            this.origin
                .Add(
                    Filtered.New(
                        element => this.allowedIDs.Contains(element.ID()),
                        elements
                    )
                );
        }

        public void Remove(IQuery query)
        {
            this.origin
                .Remove(
                    new AND(query,
                        new IN("id", this.allowedIDs)
                    )
                );
        }

        public IGroup Find(IQuery query)
        {
            return
                this.origin
                    .Find(
                        new AND(
                            new IN("id", this.allowedIDs),
                            query
                        )
                    );
        }

        public IGroup Find(IQuery query, int start, int amount)
        {
            return
               this.origin
                   .Find(
                       new AND(
                           new IN("id", this.allowedIDs),
                           query
                        ),
                        start,
                        amount
                   );
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.origin.Update(
                new AND(
                    new IN("id", this.allowedIDs),
                    query
                ),
                props
            );
        }
    }
}
