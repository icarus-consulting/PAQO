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
using System.Text;

namespace PAQO.Core.Group
{
    /// <summary>
    /// A group which is dead and rejects any actions.
    /// </summary>
    public sealed class DeadGroup : IGroup
    {
        /// <summary>
        /// A group which is dead and rejects any actions.
        /// </summary>
        public DeadGroup()
        { }

        public void Add(IEnumerable<IElement> elements)
        {
            throw new InvalidOperationException($"Cannot add to a dead group.");
        }

        public void Remove(IQuery query)
        {
            throw new InvalidOperationException($"Cannot remove from a dead group.");
        }

        public IEnumerable<IElement> Elements()
        {
            throw new InvalidOperationException($"Cannot enumerate elements of a dead group.");
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            throw new InvalidOperationException($"Cannot query a dead group.");
        }

        public void Update(IQuery query, params IProp[] props)
        {
            throw new InvalidOperationException($"Cannot update a dead group.");
        }
    }
}
