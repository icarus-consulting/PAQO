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
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// A switch prop.
    /// </summary>
    public sealed class SwitchProp : PropEnvelope
    {
        /// <summary>
        /// A switch prop.
        /// </summary>
        public SwitchProp(string name, bool on) : base(
            new SimpleProp(name, BitConverter.GetBytes(on))
        )
        { }

        /// <summary>
        /// Prop as bool.
        /// </summary>
        public sealed class IsOn : ScalarEnvelope<bool>
        {
            /// <summary>
            /// Prop as bool.
            /// </summary>
            public IsOn(byte[] prop) : base(() =>
                prop.Length > 0 ? BitConverter.ToBoolean(prop, 0) : false
            )
            { }

            /// <summary>
            /// Prop as bool.
            /// </summary>
            public IsOn(IProp prop) : base(() =>
                prop.Content().Length > 0 ? BitConverter.ToBoolean(prop.Content(), 0) : false
            )
            { }
        }
    }
}
