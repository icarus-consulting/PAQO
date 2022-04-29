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
    /// An integer prop.
    /// </summary>
    public sealed class IntProp : PropEnvelope
    {
        /// <summary>
        /// An integer prop.
        /// </summary>
        public IntProp(string name, int content) : base(() =>
            new SimpleProp(
                name,
                BitConverter.GetBytes(content)
            )
        )
        { }

        /// <summary>
        /// Prop as int.
        /// Defaults to 0 on error.
        /// </summary>
        public sealed class AsInt : ScalarEnvelope<int>
        {
            /// <summary>
            /// Prop as int.
            /// Defaults to 0 on error.
            /// </summary>
            public AsInt(IProp prop) : this(prop, mismatched => 0)
            { }

            /// <summary>
            /// Prop as int.
            /// Defaults to 0 on error.
            /// </summary>
            public AsInt(byte[] prop) : this(() => prop, mismatched => 0)
            { }

            /// <summary>
            /// Prop as int.
            /// Defaults to 0 on error.
            /// </summary>
            public AsInt(IProp prop, Func<byte[], int> fallback) : this(() =>
                prop.Content(),
                fallback
            )
            { }

            /// <summary>
            /// Prop as int.
            /// Defaults to 0 on error.
            /// </summary>
            public AsInt(string propName, IElement element) : this(() =>
                element.Props().Content(propName),
                mismatched => 0
            )
            { }

            /// <summary>
            /// Prop as int.
            /// Defaults to 0 on error.
            /// </summary>
            private AsInt(Func<byte[]> prop, Func<byte[], int> fallback) : base(() =>
            {
                var bytes = prop();
                var result = 0;
                if (bytes.Length == 4)
                {
                    result = BitConverter.ToInt32(bytes, 0);
                }
                else if (bytes.Length == 8)
                {
                    result = Convert.ToInt32(BitConverter.ToDouble(bytes, 0));
                }
                else
                {
                    result = fallback(bytes);
                }
                return result;
            }
            )
            { }
        }

        /// <summary>
        /// Prop as long.
        /// Defaults to 0 on error.
        /// </summary>
        public sealed class AsLong : ScalarEnvelope<long>
        {
            /// <summary>
            /// Prop as long.
            /// Defaults to 0 on error.
            /// </summary>
            public AsLong(IProp prop) : this(prop, mismatched => 0)
            { }

            /// <summary>
            /// Prop as long.
            /// Defaults to 0 on error.
            /// </summary>
            public AsLong(byte[] prop) : this(() => prop, mismatched => 0)
            { }

            /// <summary>
            /// Prop as long.
            /// Defaults to 0 on error.
            /// </summary>
            public AsLong(IProp prop, Func<byte[], long> fallback) : this(() =>
                prop.Content(),
                fallback
            )
            { }

            /// <summary>
            /// Prop as long.
            /// Defaults to 0 on error.
            /// </summary>
            public AsLong(string propName, IElement element) : this(() =>
                element.Props().Content(propName),
                mismatched => 0
            )
            { }

            /// <summary>
            /// Prop as long.
            /// Defaults to 0 on error.
            /// </summary>
            private AsLong(Func<byte[]> prop, Func<byte[], long> fallback) : base(() =>
            {
                var bytes = prop();
                long result;
                if (bytes.Length == 4 || bytes.Length == 8)
                {
                    result = BitConverter.ToInt64(bytes, 0);
                }
                else
                {
                    result = fallback(bytes);
                }
                return result;
            }
            )
            { }
        }
    }
}
