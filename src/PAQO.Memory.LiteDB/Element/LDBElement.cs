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
using System.Collections.Generic;

namespace PAQO.Memory.LiteDB.Element
{
    /// <summary>
    /// An <see cref="IElement"/> implemented using LiteDB.
    /// </summary>
    public sealed class LDBElement : IElement
    {
        private readonly ILiteCollection<BsonDocument> collection;
        private readonly BsonDocument doc;
        private readonly IPatch<BsonDocument, IProp> patch;
        private readonly IProps props;

        /// <summary>
        /// An <see cref="IElement"/> implemented using LiteDB.
        /// </summary>
        public LDBElement(
            BsonDocument doc,
            ISchema schema,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue,
            ILiteCollection<BsonDocument> collection,
            string elementType
        )
        {
            this.collection = collection;
            this.doc = doc;
            this.props = new LDBElementProps(doc, schema, elementType);
            this.patch = new LDBElementPatch(collection, schema, swapPropToBsonValue, swapPropToBsonStringValue, elementType);

        }

        public string ID()
        {
            return this.doc["id"].AsString;
        }

        public IProps Props()
        {
            return this.props;
        }

        public void Remove()
        {
            this.collection.Delete(new BsonValue(this.ID()));
        }

        public void Update(IEnumerable<IProp> props)
        {
            this.patch.Apply(this.doc, props);
        }
    }
}
