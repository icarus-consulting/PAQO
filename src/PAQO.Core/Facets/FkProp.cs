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
using System.Globalization;
using System.Text;
using Yaapii.Atoms;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.Map;

namespace PAQO.Core.Facets
{
    /// <summary>
    /// A fake prop.
    /// </summary>
    public sealed class FkProp : IProp
    {
        private readonly MapOf texts;
        private readonly Action<byte[]> update;
        private readonly IBytes rawContent;

        /// <summary>
        /// A fake prop - type can be set to "option" or "text".
        /// </summary>
        public FkProp(string name, bool onOrOff) : this(
            name, onOrOff.ToString().ToLower(), new BytesOf(Convert.ToByte(onOrOff))
        )
        { }

        /// <summary>
        /// A fake prop - type can be set to "option" or "text".
        /// </summary>
        public FkProp(string name, string text) : this(
            name, text, new BytesOf(text, Encoding.UTF8)
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, double decimalNumber) : this(
            name, () => decimalNumber.ToString(CultureInfo.InvariantCulture), new BytesOf(decimalNumber)
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, int integer) : this(
            name, () => integer.ToString(), new BytesOf(integer)
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, string contentAsString, IBytes content) : this(
            name, () => contentAsString, content, (updatedData) => { }
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, Func<string> contentAsString, IBytes content) : this(
            name, contentAsString, content, (updatedData) => { }
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, string contentAsString, IBytes content, Action<byte[]> update) : this(
            name, () => contentAsString, content, update
        )
        { }

        /// <summary>
        /// A fake prop.
        /// </summary>
        public FkProp(string name, Func<string> contentAsString, IBytes content, Action<byte[]> update)
        {
            this.texts =
                new MapOf(
                    new KvpOf("name", name),
                    new KvpOf("content", () => contentAsString())
                );
            this.update = update;
            this.rawContent = content;
        }

        public string AsString()
        {
            return this.texts["content"];
        }

        public byte[] Content()
        {
            return this.rawContent.AsBytes();
        }

        public string Name()
        {
            return this.texts["name"];
        }

        public void Update(byte[] content)
        {
            this.update(content);
        }
    }
}
