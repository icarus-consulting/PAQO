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
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Group
{
    /// <summary>
    /// Elements from a given func (sticky).
    /// </summary>
    public sealed class GroupOf : IGroup
    {
        private readonly ScalarOf<IGroup> group;

        /// <summary>
        /// Group from a given func (sticky).
        /// </summary>
        public GroupOf(Func<IGroup> group)
        {
            this.group = new ScalarOf<IGroup>(group);
        }

        /// <summary>
        /// All elements in this group.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IElement> Elements()
        {
            return this.group.Value().Elements();
        }

        /// <summary>
        /// Add new elements to this group.
        /// </summary>
        /// <param name="elements"></param>
        public void Add(IEnumerable<IElement> elements)
        {
            this.group.Value().Add(elements);
        }

        /// <summary>
        /// Remove elements from this group.
        /// </summary>
        /// <param name="elements"></param>
        public void Remove(IQuery query)
        {
            this.group.Value().Remove(query);
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return this.group.Value().Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.group.Value().Update(query, props);
        }
    }
}
