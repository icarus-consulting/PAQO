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
using System;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Element
{
    /// <summary>
    /// An element as a bson document, with values which are
    /// <see cref="BsonValue"/> - typed based on the given schema.
    /// </summary>
    public sealed class LDBElementBson : IScalar<BsonDocument>
    {
        private const string RAW_DOC_ADDRESS = "_";
        private readonly ScalarOf<BsonDocument> bson;

        /// <summary>
        /// An element as a bson document, with values which are
        /// <see cref="BsonValue"/> which is typed based on the given schema.
        /// </summary>
        public LDBElementBson(
            string elementType,
            IElement element,
            ISchema schema,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        ) : this(
            () => elementType,
            element,
            schema,
            swapPropToBsonValue,
            swapPropToBsonStringValue
        )
        { }

        /// <summary>
        /// An element as a bson document, with values which are
        /// <see cref="BsonValue"/> which is typed based on the given schema.
        /// </summary>
        private LDBElementBson(
            Func<string> elementType,
            IElement element,
            ISchema schema,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        )
        {
            this.bson =
                new ScalarOf<BsonDocument>(() =>
                    AsBson(
                        elementType(),
                        element,
                        schema,
                        swapPropToBsonValue,
                        swapPropToBsonStringValue
                    )
                );
        }

        public BsonDocument Value()
        {
            return this.bson.Value();
        }

        private BsonDocument AsBson(
            string elementType,
            IElement element,
            ISchema schema,
            ISwap<string, byte[], BsonValue> swapToBsonValue,
            ISwap<string, byte[], BsonValue> swapToBsonString
        )
        {
            var doc = new BsonDocument();
            var rawDoc = new BsonDocument();
            doc.Add("id", element.ID());
            foreach (var prop in element.Props().All())
            {
                if (prop.Name() == RAW_DOC_ADDRESS)
                {
                    throw new ArgumentException(
                        $"Cannot import element with id '{element.ID()}': it "
                        + $"has a property named '{RAW_DOC_ADDRESS}', this name is "
                        + $"reserved for internal use."
                    );
                }

                var typedValue =
                    swapToBsonValue
                        .Flip(
                            schema
                                .For(elementType)
                                .Types()[prop.Name()],
                            prop.Content()
                        );
                doc[prop.Name()] = typedValue;

                var stringValue =
                    swapToBsonString
                        .Flip(
                            schema
                                .For(elementType)
                                .Types()[prop.Name()],
                            prop.Content()
                        );

                //For some queries, the property is needed as text (eg: CONTAINS)
                doc[$"{prop.Name()}_asText"] = stringValue;

                //Debug.WriteLine($"{prop.Name()}={stringValue}");

                //Add prop as raw bytes. IProp delivers a type-name and bytes.
                //Later, when the DB is queried, IProps must be delivered.
                //This means, the value is needed as bytes.
                //When we add it to a seperate document as bytes, we save computation
                //time at every single time someone queries the LiteDB.
                //This is 50% of computation time needed for every prop (the
                //other 50% occur when the prop will finally be converted from
                //bytes to typed, if someone needs it - for working with it, like
                //printing)
                rawDoc.Add(
                    prop.Name(),
                    prop.Content()
                );
            }
            doc.Add(RAW_DOC_ADDRESS, rawDoc);
            return doc;
        }
    }
}
