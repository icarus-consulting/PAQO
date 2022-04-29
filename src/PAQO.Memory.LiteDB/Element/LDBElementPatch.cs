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
using PAQO.Core.Schema;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;

namespace PAQO.Memory.LiteDB.Element
{
    /// <summary>
    /// A patch that can update contents of an element in bsondocument format.
    /// </summary>
    public sealed class LDBElementPatch : IPatch<BsonDocument, IProp>
    {
        private const string RAW_DOC_ADDRESS = "_";

        private readonly ILiteCollection<BsonDocument> collection;
        private readonly ISchema schema;
        private readonly ISwap<string, byte[], BsonValue> swapPropToBsonValue;
        private readonly ISwap<string, byte[], BsonValue> swapPropToBsonStringValue;
        private readonly string elementType;

        /// <summary>
        /// A patch that can update contents of an element in bsondocument format.
        /// </summary>
        public LDBElementPatch(
            ILiteCollection<BsonDocument> collection,
            ISchema schema,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue,
            string elementType
        )
        {
            this.collection = collection;
            this.schema = schema;
            this.swapPropToBsonValue = swapPropToBsonValue;
            this.swapPropToBsonStringValue = swapPropToBsonStringValue;
            this.elementType = elementType;
        }

        public void Apply(BsonDocument destination, IEnumerable<IProp> props)
        {
            foreach (var prop in
                new StrictProps(props, schema, elementType,
                    (illegal) => throw new ArgumentException($"Cannot update non existing prop '{illegal.Name()}' of a '{elementType}'.")
                )
            )
            {
                //Convert the value to typed bson value so
                //it can be queried.
                destination[prop.Name()] =
                    this.swapPropToBsonValue.Flip(
                        schema
                            .For(this.elementType)
                            .Types()[prop.Name()],
                        prop.Content()
                    );

                //Convert the value to string bson value so
                //it can be queried by CONTAINS statement for numbers seen as text. (12345 CONTAINS 34)
                destination[$"{prop.Name()}_asText"] =
                    this.swapPropToBsonStringValue.Flip(
                        schema
                            .For(this.elementType)
                            .Types()[prop.Name()],
                        prop.Content()
                    );

                //Store the raw value to save conversion time (typed to byte) when accessing it.
                destination[RAW_DOC_ADDRESS][prop.Name()] = prop.Content();
                this.collection.Update(destination);
            }
        }

        public void Apply(BsonDocument destination, params IProp[] data)
        {
            this.Apply(destination, new ManyOf<IProp>(data));
        }
    }
}
