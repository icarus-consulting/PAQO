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

namespace PAQO.Core.Prop
{
    /// <summary>
    /// Props which is threadsafe.
    /// </summary>
    public sealed class SyncProps : IProps
    {
        private readonly IProps origin;
        private readonly object locking;

        /// <summary>
        /// Props which is threadsafe.
        /// </summary>
        public SyncProps(IProps origin, object locking)
        {
            this.origin = origin;
            this.locking = locking;
        }

        public IList<IProp> All()
        {
            lock (this.locking)
            {
                return this.origin.All();
            }
        }

        public bool Contains(IProp prop)
        {
            lock (this.locking)
            {
                return this.origin.Contains(prop);
            }
        }

        public bool Contains(string propName)
        {
            lock (this.locking)
            {
                return this.origin.Contains(propName);
            }
        }

        public byte[] Content(string propName)
        {
            lock (this.locking)
            {
                return this.origin.Content(propName);
            }
        }

        public IProp Prop(string propName)
        {
            lock (this.locking)
            {
                return new SyncProp(this.origin.Prop(propName));
            }
        }
    }
}
