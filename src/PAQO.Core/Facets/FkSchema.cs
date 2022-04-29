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


using PAQO.Core.Schema;
using System;
using System.Collections.Generic;
using Yaapii.Xml;

namespace PAQO.Core.Facets
{
    /// <summary>
    /// A fake schema.
    /// </summary>
    public sealed class FkSchema : ISchema
    {
        private readonly IXML schemaXml;

        /// <summary>
        /// A fake schema.
        /// </summary>
        public FkSchema()
            :
        this(
            new XMLCursor("<fake-schema-xml />")
        )
        { }

        /// <summary>
        /// A fake schema.
        /// </summary>
        /// <param name="parameterType">input: parameter-name, output: parameter-type</param>
        /// <param name="parameterAsString">input: parameter-name & parameter as byte-array, output: parameter as string</param>
        public FkSchema(
            IXML schemaXml
        )
        {
            this.schemaXml = schemaXml;
        }

        public IXML AsXML()
        {
            return this.schemaXml;
        }

        public IDefinitions For(string context)
        {
            throw new NotImplementedException();
        }

        public IList<string> ElementTypes()
        {
            throw new NotImplementedException();
        }
    }
}
