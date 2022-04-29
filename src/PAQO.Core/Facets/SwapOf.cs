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

namespace PAQO.Core.Facets
{
    /// <summary>
    /// Swaps input to output. Meant to build a conversion.
    /// </summary>
    public sealed class SwapOf<TInput, TOutput> : ISwap<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> swapping;

        /// <summary>
        /// Swaps input to output. Meant to build a conversion.
        /// </summary>
        public SwapOf(Func<TInput, TOutput> swapping)
        {
            this.swapping = swapping;
        }

        public TOutput Flip(TInput input)
        {
            return this.swapping(input);
        }
    }

    /// <summary>
    /// Swaps two inputs to one output. Meant to build a conversion.
    /// </summary>
    public sealed class SwapOf<TInput1, TInput2, TOutput> : ISwap<TInput1, TInput2, TOutput>
    {
        private readonly Func<TInput1, TInput2, TOutput> swapping;

        /// <summary>
        /// Swaps two inputs to one output. Meant to build a conversion.
        /// </summary>
        public SwapOf(Func<TInput1, TInput2, TOutput> swapping)
        {
            this.swapping = swapping;
        }

        public TOutput Flip(TInput1 input1, TInput2 input2)
        {
            return this.swapping(input1, input2);
        }
    }
}
