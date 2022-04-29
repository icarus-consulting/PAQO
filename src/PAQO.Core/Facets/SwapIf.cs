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
using Yaapii.Atoms.Map;

namespace PAQO.Core.Facets
{
    /// <summary>
    /// A conversion which can be used with <see cref="SwapSwitch{TInput, TOutput}"/>
    /// </summary>
    public sealed class SwapIf<TInput, TOutput> : KvpEnvelope<ISwap<TInput, TOutput>>
    {
        /// <summary>
        /// A conversion which can be used with <see cref="SwapSwitch{TInput, TOutput}"/>
        /// </summary>
        public SwapIf(string key, Func<TInput, TOutput> swap) : base(
            new KvpOf<ISwap<TInput, TOutput>>(key, new SwapOf<TInput, TOutput>(swap))
        )
        { }

        /// <summary>
        /// A conversion which can be used with <see cref="SwapSwitch{TInput, TOutput}"/>
        /// </summary>
        public SwapIf(string key, ISwap<TInput, TOutput> swap) : base(
            new KvpOf<ISwap<TInput, TOutput>>(key, swap)
        )
        { }
    }

    /// <summary>
    /// A conversion which can be used with <see cref="SwapSwitch{TInput, TOutput}"/>
    /// </summary>
    public sealed class SwapIf<TInput1, TInput2, TOutput> : KvpEnvelope<ISwap<TInput1, TInput2, TOutput>>
    {
        /// <summary>
        /// A conversion which can be used with <see cref="SwapSwitch{TInput1, TInput2, TOutput}"/>
        /// </summary>
        public SwapIf(string key, Func<TInput1, TInput2, TOutput> swap) : base(
            new KvpOf<ISwap<TInput1, TInput2, TOutput>>(key, new SwapOf<TInput1, TInput2, TOutput>(swap))
        )
        { }

        /// <summary>
        /// A conversion which can be used with <see cref="Conversions{TInput1, TInput2, TOutput}"/>
        /// </summary>
        public SwapIf(string name, ISwap<TInput1, TInput2, TOutput> swap) : base(
            new KvpOf<ISwap<TInput1, TInput2, TOutput>>(name, swap)
        )
        { }
    }
}
