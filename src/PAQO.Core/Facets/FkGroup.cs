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
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;

namespace PAQO.Core.Facets
{
    /// <summary>
    /// Fake group.
    /// </summary>
    public sealed class FkGroup : IGroup
    {
        private readonly IList<IElement> elements;
        private readonly Func<IQuery, IList<IElement>, IGroup> query;

        /// <summary>
        /// Fake group.
        /// </summary>
        public FkGroup() : this(new ListOf<IElement>(), (query, elements) => new FkGroup(elements))
        { }

        /// <summary>
        /// Fake group.
        /// </summary>
        public FkGroup(IEnumerable<IElement> elements) : this(new ListOf<IElement>(elements), (query, elms) => new FkGroup(elms))
        { }

        /// <summary>
        /// Fake group.
        /// </summary>
        public FkGroup(IEnumerable<IElement> elements, Func<IQuery, IList<IElement>, IGroup> query)
        {
            this.elements = new ListOf<IElement>(elements);
            this.query = query;
        }

        public IEnumerable<IElement> Elements()
        {
            return this.elements;
        }

        public void Add(IEnumerable<IElement> elements)
        {
            throw new InvalidOperationException($"Adding to fake group is not supported.");
        }

        public void Remove(IQuery query)
        {
            throw new InvalidOperationException($"Removing from fake group is not supported.");
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return
                new FkGroup(
                    new HeadOf<IElement>(
                        new Skipped<IElement>(this.query(query, this.elements).Elements(),
                            start
                        ),
                        amount
                    )
                );
        }

        public void Update(IQuery query, params IProp[] props)
        {
            foreach (var element in this.Find(query).Elements())
            {
                element.Update(props);
                    
            }
        }
    }
}
