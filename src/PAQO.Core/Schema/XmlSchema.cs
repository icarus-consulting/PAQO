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
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// A schema.
    /// </summary>
    public sealed class XmlSchema : ISchema
    {
        private readonly IScalar<IXML> xml;
        private readonly IDictionary<string, SchemaDefinitions> definitions;
        private readonly ISwap<string, byte[], string> propSwaps;

        /// <summary>
        /// A schema.
        /// </summary>
        public XmlSchema(IXML xmlSchema)
        {
            this.xml =
                new ScalarOf<IXML>(() =>
                    new XMLCursor(xmlSchema.AsNode())
                );
            this.definitions =
                FallbackMap.New(
                    MapOf.New(
                        Yaapii.Atoms.Enumerable.Mapped.New(
                            name => KvpOf.New(name, new SchemaDefinitions(xmlSchema, name, this.propSwaps)),
                            Yaapii.Atoms.Enumerable.Mapped.New(
                                node => (node.AsNode() as XElement).Name.LocalName,
                                xmlSchema.Nodes("/schema/*")
                            )
                        )
                    ),
                    unknown =>
                        throw new ArgumentException(
                            $"Context '{unknown}' is unknown. Known contexts: "
                            + new Yaapii.Atoms.Text.Joined(
                                ", ",
                                Yaapii.Atoms.Enumerable.Mapped.New(
                                    node => $"'{(node.AsNode() as XElement).Name.LocalName}'",
                                    xmlSchema.Nodes("/schema/*")
                                )
                            ).AsString()
                        )
                );
            this.propSwaps = new SwapBytesToString();
        }

        public IDefinitions For(string context)
        {
            return this.definitions[context];
        }

        public IXML AsXML()
        {
            return this.xml.Value();
        }

        public IList<string> ElementTypes()
        {
            return new ListOf<string>(this.definitions.Keys);
        }
    }
}
