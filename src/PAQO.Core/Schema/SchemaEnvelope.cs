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
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Xml;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Envelope for schemata.
    /// </summary>
    public abstract class SchemaEnvelope : ISchema
    {
        private readonly IScalar<ISchema> schema;

        /// <summary>
        /// Envelope for schemata.
        /// </summary>
        public SchemaEnvelope(Func<ISchema> schema)
        {
            this.schema = new ScalarOf<ISchema>(schema);
        }

        public IXML AsXML()
        {
            return this.schema.Value().AsXML();
        }

        public IDefinitions For(string context)
        {
            return this.schema.Value().For(context);
        }

        public IList<string> ElementTypes()
        {
            return this.schema.Value().ElementTypes();
        }
    }
}
