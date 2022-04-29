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

using PAQO.Core.Facets;
using PAQO.Core.Prop;
using System;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Definitions of props in a schema.
    /// </summary>
    public sealed class SchemaDefinitions : IDefinitions
    {
        private readonly IDictionary<string, string> nameToType;
        private readonly PropDisplayNames nameToDisplayName;
        private readonly ISwap<string, byte[], string> swapByteToString;
        private readonly ISwap<string, IProps, IList<IKvp>> swapPropToChoices;
        private readonly AllChoices allChoices;

        /// <summary>
        /// Definitions of props in a schema.
        /// </summary>
        public SchemaDefinitions(ISchema schema, string context, ISwap<string, byte[], string> swaps) : this(
            () => schema.AsXML(), context, swaps
        )
        { }

        /// <summary>
        /// Definitions of props in a schema.
        /// </summary>
        public SchemaDefinitions(IXML schemaXml, string context, ISwap<string, byte[], string> swaps) : this(
            () => schemaXml, context, swaps
        )
        { }

        /// <summary>
        /// Definitions of props in a schema.
        /// </summary>
        public SchemaDefinitions(Func<IXML> schemaXml, string context, ISwap<string, byte[], string> swaps)
        {
            this.nameToType = new PropTypes(schemaXml, context);
            this.nameToDisplayName = new PropDisplayNames(schemaXml, context);
            this.swapByteToString = swaps;
            this.allChoices = new AllChoices(schemaXml);
            this.swapPropToChoices =
                new SwapOf<string, IProps, IList<IKvp>>(
                    (propName, props) => new ChoicesOf(schemaXml(), propName, context, props)
                );
        }

        public IList<IKvp> Choices(string propName, IProps props)
        {
            return this.swapPropToChoices.Flip(propName, props);
        }

        public IDictionary<string, string> Types()
        {
            return this.nameToType;
        }

        public string AsString(string propName, byte[] propValue)
        {
            lock (nameToType)
            {
                nameToType.Keys.Count.ToString();
            }
            string result = String.Empty;
            //Until now, no specific conversions exist - only options. Later this can be extended.
            if (this.nameToType.ContainsKey(propName))
            {
                var type = this.nameToType[propName];
                if (type == "options")
                {
                    var choiceValue = new TextProp.AsText(propValue).AsString();
                    if (this.allChoices.ContainsKey(propName) && this.allChoices[propName].ContainsKey(choiceValue))
                    {
                        result = this.allChoices[propName][choiceValue];
                    }
                }
                else
                {
                    result =
                        this.swapByteToString
                            .Flip(type, propValue);
                }
            }
            else
            {
                result = this.swapByteToString.Flip(this.nameToType[propName], propValue);
            }

            return result;
        }

        public IDictionary<string, string> DisplayNames()
        {
            return this.nameToDisplayName;
        }
    }
}
