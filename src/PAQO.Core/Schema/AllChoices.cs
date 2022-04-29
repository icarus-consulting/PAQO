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
using Yaapii.Atoms.Map;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// All choices in the schema, not regarding any dependencies.
    /// This is for caching all key-value dependencies for options.
    /// </summary>
    public sealed class AllChoices : MapEnvelope<IDictionary<string, string>>
    {
        /// <summary>
        /// All choices in the schema, not regarding any dependencies.
        /// This is for caching all key-value dependencies for options.
        /// </summary>
        public AllChoices(ISchema schema) : this(() => schema.AsXML())
        { }

        /// <summary>
        /// All choices in the schema, not regarding any dependencies.
        /// This is for caching all key-value dependencies for options.
        /// </summary>
        public AllChoices(IXML schemaXml) : this(() => schemaXml)
        { }

        /// <summary>
        /// All choices in the schema, not regarding any dependencies.
        /// This is for caching all key-value dependencies for options.
        /// </summary>
        public AllChoices(Func<IXML> schemaXml) : base(() =>
            {
                var result = new Dictionary<string, IDictionary<string, string>>();

                var options =
                    schemaXml().Nodes(
                        $"//options"
                    );

                foreach (var block in options)
                {
                    var id = new XMLString(block, "./id/text()").Value();
                    if (!result.ContainsKey(id))
                    {
                        result[id] = new Dictionary<string, string>();
                    }
                    foreach (var option in block.Nodes("./option"))
                    {
                        var value = new XMLString(option, "./value/text()").Value();
                        if (!result[id].ContainsKey(value))
                        {
                            result[id][value] = new XMLString(option, "./name/text()").Value();
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
