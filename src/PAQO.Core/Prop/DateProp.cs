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
    /// A date prop.
    /// </summary>
    public sealed class DateProp : PropEnvelope
    {
        /// <summary>
        /// A date prop.
        /// </summary>
        public DateProp(string name, DateTime content) : base(() =>
            new SimpleProp(
                name,
                BitConverter.GetBytes(content.Ticks)
            )
        )
        { }

        /// <summary>
        /// Prop as date.
        /// </summary>
        public sealed class AsDate : ScalarEnvelope<DateTime>
        {
            /// <summary>
            /// Prop as date.
            /// Defaults to date time min value on error
            /// </summary>
            public AsDate(IProp prop) : this(prop, mismatched => DateTime.MinValue)
            { }

            /// <summary>
            /// Prop as date.
            /// Defaults to date time min value on error
            /// </summary>
            public AsDate(byte[] prop) : this(() => prop, mismatched => DateTime.MinValue)
            { }

            /// <summary>
            /// Prop as date.
            /// Defaults to date time min value on error
            /// </summary>
            public AsDate(IProp prop, Func<byte[], DateTime> fallback) : this(() =>
                prop.Content(),
                fallback
            )
            { }

            /// <summary>
            /// Prop as date.
            /// Defaults to date time min value on error
            /// </summary>
            public AsDate(string propName, IElement element) : this(() =>
                element.Props().Content(propName),
                mismatched => DateTime.MinValue
            )
            { }

            /// <summary>
            /// Prop as date.
            /// Defaults to date time min value on error
            /// </summary>
            private AsDate(Func<byte[]> prop, Func<byte[], DateTime> fallback) : base(() =>
            {
                var bytes = prop();
                DateTime result;
                if (bytes.Length == 4 || bytes.Length == 8)
                {
                    result = new DateTime(BitConverter.ToInt64(bytes, 0));
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
