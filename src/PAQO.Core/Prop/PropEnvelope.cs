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
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// Envelope for props.
    /// </summary>
    public abstract class PropEnvelope : IProp
    {
        private readonly IScalar<IProp> origin;

        /// <summary>
        /// Envelope for props.
        /// </summary>
        /// <param name="origin"></param>
        public PropEnvelope(IProp origin) : this(() => origin)
        { }

        /// <summary>
        /// Envelope for props.
        /// </summary>
        /// <param name="origin"></param>
        public PropEnvelope(Func<IProp> origin)
        {
            this.origin = new ScalarOf<IProp>(origin);
        }

        public byte[] Content()
        {
            return this.origin.Value().Content();
        }

        public string Name()
        {
            return this.origin.Value().Name();
        }
    }
}
