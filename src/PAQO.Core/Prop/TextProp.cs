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

using System.Text;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// a text prop
    /// </summary>
    public sealed class TextProp : PropEnvelope
    {
        /// <summary>
        /// a text prop
        /// </summary>
        public TextProp(string name, string content) : base(() =>
            new SimpleProp(
                name,
                new BytesOf(content, Encoding.UTF8).AsBytes()
            )
        )
        { }

        /// <summary>
        /// Prop as text.
        /// </summary>
        public sealed class AsText : TextEnvelope
        {
            /// <summary>
            /// Prop as text.
            /// </summary>
            public AsText(byte[] prop) : base(() =>
                Encoding.UTF8.GetString(prop),
                false
            )
            { }

            /// <summary>
            /// Prop as text.
            /// </summary>
            public AsText(string propName, IElement element) : base(() =>
                Encoding.UTF8.GetString(element.Props().Content(propName)),
                false
            )
            { }

            /// <summary>
            /// Prop as text.
            /// </summary>
            public AsText(string propName, IProps props) : base(() =>
                Encoding.UTF8.GetString(props.Content(propName)),
                false
            )
            { }

            /// <summary>
            /// Prop as text.
            /// </summary>
            public AsText(IProp prop) : base(() =>
                Encoding.UTF8.GetString(prop.Content()),
                false
            )
            { }
        }
    }
}
