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
using System.Linq;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Map;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// Props of an element.
    /// </summary>
    public sealed class PropsOf : IProps
    {
        private readonly IEnumerable<IProp> props;
        private readonly IDictionary<string, IProp> map;

        /// <summary>
        /// Props of an element.
        /// </summary>
        public PropsOf(params IProp[] props) : this(
            new ManyOf<IProp>(props)
        )
        { }

        /// <summary>
        /// Props of an element.
        /// </summary>
        public PropsOf(IEnumerable<IProp> props) : this(props, (unknown)
            => throw new ArgumentException($"Prop '{unknown}' does not exist. Existing props: {new Yaapii.Atoms.Text.Joined(", ", new PropNames(props)).AsString()}")
        )
        { }

        /// <summary>
        /// Props of an element.
        /// </summary>
        public PropsOf(Func<IEnumerable<IProp>> props) : this(props, (unknown) =>
            throw new ArgumentException($"Prop '{unknown}' does not exist. Existing props: {new Yaapii.Atoms.Text.Joined(", ", new PropNames(props())).AsString()}")
        )
        { }

        /// <summary>
        /// Props of an element.
        /// </summary>
        public PropsOf(IEnumerable<IProp> props, Func<string, IProp> fallback) : this(() => props, fallback)
        { }

        /// <summary>
        /// Props of an element.
        /// </summary>
        public PropsOf(Func<IEnumerable<IProp>> props, Func<string, IProp> fallback)
        {
            this.props = new ManyOf<IProp>(props);
            this.map =
                FallbackMap.New(
                    MapOf.New(
                        ManyOf.New(() =>
                            Yaapii.Atoms.Enumerable.Mapped.New(
                                prop => KvpOf.New(prop.Name(), prop),
                                props()
                            )
                        )
                    ),
                    prop => fallback(prop)
                );
        }

        public IList<IProp> All()
        {
            return new ListOf<IProp>(this.props); //faster without building the map if not necessary.
        }

        public bool Contains(IProp prop)
        {
            return
                this.map.ContainsKey(prop.Name())
                &&
                this.map[prop.Name()]
                    .Content()
                    .SequenceEqual(prop.Content());
        }

        public bool Contains(string prop)
        {
            return
                this.map.ContainsKey(prop);
        }

        public IProp Prop(string name)
        {
            return this.map[name];
        }

        public byte[] Content(string name)
        {
            return this.map[name].Content();
        }
    }
}
