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

namespace PAQO.Core.Facets
{
    /// <summary>
    /// Ensures that a collection has only one item.
    /// </summary>
    public sealed class StrictAmount<T> : ManyEnvelope<T>
    {
        /// <summary>
        /// Ensures that a collection has only one item.
        /// </summary>
        public StrictAmount(IEnumerable<T> origin, Exception ex) : this(origin, (tooMuch) => throw ex)
        { }

        /// <summary>
        /// Ensures that a collection has only one item.
        /// </summary>
        public StrictAmount(IEnumerable<T> origin) : this(origin, (tooMuch) => throw new ArgumentException("Found more than one item, but only one is allowed in this enumerable."))
        { }

        /// <summary>
        /// Ensures that a collection has only one item.
        /// </summary>
        public StrictAmount(IEnumerable<T> origin, Action<IEnumerable<T>> onIllegal) : base(() =>
            {
                var enm = origin.GetEnumerator();
                if (enm.MoveNext() && enm.MoveNext())
                {
                    onIllegal(origin);
                }
                return origin;
            },
            false
        )
        { }
    }
}
