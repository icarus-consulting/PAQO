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

using LiteDB;
using PAQO.Core;
using PAQO.Core.Prop;
using PAQO.Memory.LiteDB.Facets;
using System.Collections.Generic;

namespace PAQO.Memory.LiteDB.Element
{
    /// <summary>
    /// Props of a LiteDB element.
    /// </summary>
    public sealed class LDBElementProps : PropsEnvelope
    {
        private const string RAW_DOC_ADDRESS = "_";

        /// <summary>
        /// Props of a LiteDB element.
        /// </summary>
        public LDBElementProps(BsonDocument elementDoc, ISchema schema, string elementType) : base(() =>
            new PropsOf(
                new Yaapii.Atoms.Enumerable.Mapped<KeyValuePair<string, BsonValue>, IProp>(kvp =>
                    new LDBProp(
                        kvp.Key,
                        kvp.Value,
                        schema,
                        elementType
                    ),
                    elementDoc[RAW_DOC_ADDRESS].AsDocument.GetElements()
                )
            )
        )
        { }
    }
}
