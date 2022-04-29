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
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;

namespace Yaapii.AtomsTemp.Scalar
{
    /// <summary>
    /// Index of items in given list
    /// If no item matches is found -1 is returned
    /// </summary>
    public sealed class IndexOf<T> : ScalarEnvelope<int>
    {

        /// <summary>
        /// Index of items in given list
        /// If no item matches is found -1 is returned
        /// </summary>
        public IndexOf(T item, IEnumerable<T> items) : base(() =>
            new ListOf<T>(items).IndexOf(item)
        )
        { }

        /// <summary>
        /// Index of items in given list
        /// If no item matches is found -1 is returned
        /// </summary>
        public IndexOf(Func<T, bool> condition, IEnumerable<T> items) : base(() => {
            var result = -1;
            var list = new ListOf<T>(items);
            for (int i = 0; i < list.Count; i++)
            {
                if (condition(list[i]))
                {
                    result = i;
                    break;
                }
            }
            return result;
        })
        { }
    }
}
