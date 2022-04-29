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


using PAQO.Core.Prop;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;

namespace PAQO.Core.Facets
{
    /// <summary>
    /// A fake element.
    /// </summary>
    public sealed class FkElement : IElement
    {
        private readonly string id;
        private readonly IDictionary<string, IProp> props;
        private readonly Action remove;
        private readonly Action save;

        /// <summary>
        /// A fake element.
        /// </summary>
        public FkElement(string id, params IProp[] props) : this(
            id, new ManyOf<IProp>(props), () => { }, () => { }
        )
        { }

        /// <summary>
        /// A fake element.
        /// </summary>
        public FkElement(string id, IEnumerable<IProp> props, Action remove, Action save)
        {
            this.id = id;
            this.props =
                FallbackMap.New(
                    MapOf.New(
                        Mapped.New(
                            prop => KvpOf.New(prop.Name(), prop),
                            props
                        )
                    ),
                    missing => throw new ArgumentException($"Cannot update unknown prop '{missing}' of element '{id}'")
                );
            this.remove = remove;
            this.save = save;
        }

        public string ID()
        {
            return this.id;
        }

        public IProps Props()
        {
            return new PropsOf(this.props.Values);
        }

        public void Remove()
        {
            this.remove();
        }

        public void Save()
        {
            this.save();
        }

        public void Update(IEnumerable<IProp> props)
        {
            foreach (var prop in props)
            {
                this.props[prop.Name()] = prop;
            }
        }
    }
}
