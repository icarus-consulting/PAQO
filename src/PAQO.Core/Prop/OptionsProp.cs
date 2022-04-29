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
using System.Text;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// An options prop.
    /// </summary>
    public sealed class OptionsProp : PropEnvelope
    {
        /// <summary>
        /// An options prop.
        /// </summary>
        public OptionsProp(string name, string option) : base(
            new TextProp(name, option)
        )
        { }

        /// <summary>
        /// Prop value (usually 0, 1, etc..) as text.
        /// </summary>
        public sealed class Value : TextEnvelope
        {
            /// <summary>
            /// Prop value (usually 0, 1, etc..) as text.
            /// </summary>
            public Value(IProp prop) : base(() =>
                Encoding.UTF8.GetString(prop.Content()),
                false
            )
            { }

            /// <summary>
            /// Prop value (usually 0, 1, etc..) as text.
            /// </summary>
            public Value(string propName, IElement element) : base(() =>
                Encoding.UTF8.GetString(element.Props().Content(propName)),
                false
            )
            { }
        }

        /// <summary>
        /// Option value name.
        /// </summary>
        public sealed class ValueName : TextEnvelope
        {
            /// <summary>
            /// Option value name.
            /// </summary>
            public ValueName(string propName, IElement element, ISchema schema, string elementType) : base(() =>
                new FallbackMap(
                    new MapOf(
                        schema
                        .For(elementType)
                        .Choices(propName, element.Props())
                    ),
                    unknown =>
                        throw new ArgumentException(
                            $"{new Value(propName, element).AsString()} is not a valid option for '{propName}'. Valid options are: {string.Join(",", schema.For(elementType).Choices(propName, element.Props()))}")
                )
                [new Value(propName, element).AsString()],
                false
            )
            { }
        }
    }
}
