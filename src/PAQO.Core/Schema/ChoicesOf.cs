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
using System.Text;
using Yaapii.Atoms;
using Yaapii.Atoms.Bytes;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Available choices for a prop.
    /// These might depend on the other current props.
    /// </summary>
    public sealed class ChoicesOf : ListEnvelope<IKvp>
    {
        /// <summary>
        /// Available choices for a prop.
        /// These might depend on the other current props.
        /// (dependings are defined in the schema)
        /// </summary>
        public ChoicesOf(ISchema schema, string propName, string elementType, IProps props) : this(() =>
            schema.AsXML(), propName, elementType, props
        )
        { }

        /// <summary>
        /// Available choices for a prop.
        /// These might depend on the other current props.
        /// (dependings are defined in the schema)
        /// </summary>
        public ChoicesOf(IXML schemaXml, string propName, string elementType, IProps props) : this(() =>
            schemaXml, propName, elementType, props
        )
        { }

        /// <summary>
        /// Available choices for a prop.
        /// These might depend on the other current props.
        /// (dependings are defined in the schema)
        /// </summary>
        public ChoicesOf(Func<IXML> schemaXml, string propName, string elementType, IProps props) : base(() =>
            {
                var result = new List<IKvp>();
                var schema = schemaXml();
                var always =
                    schema.Nodes(
                        $"/schema/{elementType}/attributes/always/options[id/text() = '{propName}']"
                    );

                //"always" node has priority over "depending" node.
                if (always.Count > 0)
                {
                    foreach (var options in always)
                    {
                        if (new XMLString(options, "./id/text()").Value() == propName)
                        {
                            result.AddRange(
                                new Yaapii.Atoms.Enumerable.Mapped<IXML, IKvp>(option =>
                                    new KvpOf(
                                        new XMLString(option, "./value/text()").Value(),
                                        new XMLString(option, "./name/text()").Value()
                                    ),
                                    options.Nodes("./option")
                                )
                            );
                            break;
                        }
                    }
                }
                else
                {
                    var dependings =
                        schema.Nodes(
                            $"/schema/{elementType}/attributes/depending[branch/options/id/text() = '{propName}']"
                        );

                    foreach (var depending in dependings)
                    {
                        //Name of prop we are depending on
                        var dependantName = new XMLString(depending, "./on/text()").Value();

                        //If props contain this, search a branch matching the value
                        //and return options.
                        if (props.Contains(dependantName))
                        {
                            var dependantValue =
                                new TextOf(
                                    new BytesOf(
                                        props.Content(dependantName)
                                    ),
                                    Encoding.UTF8
                                ).AsString();

                            //Find matching branch for existing prop
                            result.AddRange(
                                new Mapped<IXML, IKvp>(
                                    option =>
                                    new KvpOf(
                                        new XMLString(option, "./value/text()").Value(),
                                        new XMLString(option, "./name/text()").Value()
                                    ),
                                    depending.Nodes($"./branch[when/text() = '{dependantValue}']/options[id/text()='{propName}']/option")
                                )
                            );
                        }
                    }
                }
                return result;
            },
            false
        )
        { }
    }
}
