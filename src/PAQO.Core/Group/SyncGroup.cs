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


using System.Collections.Generic;

namespace PAQO.Core.Group
{
    /// <summary>
    /// Group being threadsafe.
    /// </summary>
    public sealed class SyncGroup : IGroup
    {
        private readonly IGroup origin;
        private readonly object locking;

        /// <summary>
        /// Group which is threadsafe.
        /// </summary>
        public SyncGroup(IGroup origin, object locking)
        {
            this.origin = origin;
            this.locking = locking;
        }

        public IEnumerable<IElement> Elements()
        {
            lock (locking)
            {
                var result = new List<IElement>();
                result.AddRange(this.origin.Elements());
                return result;
            }
        }

        public void Add(IEnumerable<IElement> elements)
        {
            lock (this.locking)
            {
                this.origin.Add(elements);
            }
        }

        public void Remove(IQuery query)
        {
            lock (this.locking)
            {
                this.origin.Remove(query);
            }
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            lock (this.locking)
            {
                return this.origin.Find(query, start, amount);
            }
        }

        public void Update(IQuery query, params IProp[] props)
        {
            lock(this.locking)
            {
                this.origin.Update(query, props);
            }
        }
    }
}
