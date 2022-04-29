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
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace PAQO.Core.Element
{
    /// <summary>
    /// A simple element.
    /// </summary>
    public sealed class SimpleElement : IElement
    {
        private readonly IText id;
        private readonly IScalar<IDictionary<string, IProp>> props;

        /// <summary>
        /// A simple element.
        /// </summary>
        public SimpleElement(string id, params IProp[] props) : this(
            id,
            new ManyOf<IProp>(props)
        )
        { }

        /// <summary>
        /// A simple element.
        /// </summary>
        public SimpleElement(string id, IEnumerable<IProp> props) : this(
            new Yaapii.Atoms.Enumerable.Joined<IProp>(
                props,
                new TextProp("id", id)
            )
        )
        { }

        /// <summary>
        /// A simple element.
        /// </summary>
        public SimpleElement(IEnumerable<IProp> props) : this(
            new PropsOf(props)
        )
        { }

        /// <summary>
        /// A simple element.
        /// </summary>
        public SimpleElement(IProps props)
        {
            this.id = new TextProp.AsText("id", props);
            this.props =
                new ScalarOf<IDictionary<string, IProp>>(() =>
                     new Dictionary<string, IProp>(
                         MapOf.New(
                            Mapped.New(
                                prop => KvpOf.New(prop.Name(), prop),
                                props.All()
                            )
                        )
                     )
                );
        }

        public string ID()
        {
            return this.id.AsString();
        }

        public IProps Props()
        {
            return new PropsOf(this.props.Value().Values);
        }

        public void Update(IEnumerable<IProp> props)
        {
            foreach (var prop in props)
            {
                this.props.Value()[prop.Name()] = prop;
            }
        }
    }
}
