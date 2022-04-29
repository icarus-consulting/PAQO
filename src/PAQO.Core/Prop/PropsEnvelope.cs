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
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// Envelope for props.
    /// </summary>
    public abstract class PropsEnvelope : IProps
    {
        private readonly IScalar<IProps> origin;

        /// <summary>
        /// Envelope for props.
        /// </summary>
        public PropsEnvelope(Func<IProps> origin)
        {
            this.origin = new ScalarOf<IProps>(origin);
        }

        public IList<IProp> All()
        {
            return this.origin.Value().All();

        }

        public bool Contains(IProp prop)
        {
            return this.origin.Value().Contains(prop);
        }

        public bool Contains(string propName)
        {
            return this.origin.Value().Contains(propName);
        }

        public byte[] Content(string propName)
        {
            return this.origin.Value().Content(propName);
        }

        public IProp Prop(string propName)
        {
            return this.origin.Value().Prop(propName);
        }
    }
}
