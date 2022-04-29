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


using PAQO.Core.Prop;
using System.Collections.Generic;

namespace PAQO.Core.Element
{
    /// <summary>
    /// Element which is threadsafe.
    /// </summary>
    public sealed class SyncElement : IElement
    {
        private readonly IElement origin;
        private readonly object locking;

        /// <summary>
        /// Element which is threadsafe.
        /// </summary>
        public SyncElement(IElement origin, object locking)
        {
            this.origin = origin;
            this.locking = locking;
        }

        public string ID()
        {
            lock(this.locking)
            {
                return this.origin.ID();
            }
        }

        public IProps Props()
        {
            lock (this.locking)
            {
                return new SyncProps(this.origin.Props(), this.locking);
            }
        }

        public void Update(IEnumerable<IProp> props)
        {
            lock(this.locking)
            {
                this.origin.Update(props);
            }
        }
    }
}
