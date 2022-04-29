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
using Yaapii.Atoms.Text;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// A strict prop which must be defined in the given schema.
    /// </summary>
    public sealed class StrictProp : PropEnvelope
    {
        /// <summary>
        /// A strict prop which must be defined in the given schema.
        /// </summary>
        public StrictProp(IProp prop, ISchema schema, string elementType) : base(() =>
        {
            if (!schema.For(elementType).Types().ContainsKey(prop.Name()))
            {
                throw new ArgumentException(
                    new Paragraph(
                        $"Invalid prop '{prop.Name()}' for '{elementType}' schema. These props are allowed:",
                        schema.For(elementType).Types().Keys,
                        new string[0]
                    ).AsString()
                );
            }
            return prop;
        })
        { }
    }
}
