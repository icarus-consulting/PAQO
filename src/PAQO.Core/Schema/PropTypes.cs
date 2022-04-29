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
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// All props for a context in the given schema, ordered.
    /// </summary>
    public sealed class PropTypes : MapEnvelope
    {
        /// <summary>
        /// All props for a context in the given schema, ordered.
        /// </summary>
        public PropTypes(ISchema schema, string context) : this(() => schema.AsXML(), context)
        { }

        /// <summary>
        /// All props for a context in the given schema, ordered.
        /// </summary>
        public PropTypes(IXML schema, string context) : this(() => schema, context)
        { }

        /// <summary>
        /// All props for a context in the given schema, ordered.
        /// </summary>
        public PropTypes(Func<IXML> schema, string context) : base(() =>
        {
            var fixedSchema = schema();
                var props =
                    new MapOf(
                        new Mapped<IXML, IKvp>(node =>
                            new KvpOf(
                                new NotIllegalPropID(
                                    new XMLString(node, "./id/text()").Value()
                                ),
                                new XMLString(node, "name(.)").Value().ToLower()
                            ),
                            fixedSchema.Nodes(
                                $"/schema/{context}/attributes"
                                + "//*[self::integer"
                                + " or self::text"
                                + " or self::decimal"
                                + " or self::options"
                                + " or self::switch"
                                + " or self::complex]"
                            )
                        )
                    );
                var result =
                    new FallbackMap(props, unknown =>
                        throw new ArgumentException(
                            $"Unknown property '{unknown}'. Known properties are: {new Yaapii.Atoms.Text.Joined(", ", props.Keys).AsString()}"
                        )
                    );
                return result;
            },
            false
        )
        { }
    }
}
