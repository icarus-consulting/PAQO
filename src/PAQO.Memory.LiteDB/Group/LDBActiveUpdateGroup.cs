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
using LiteDB.Engine;
using PAQO.Core;
using PAQO.Core.Facets;
using PAQO.Core.Find;
using PAQO.Memory.LiteDB.Element;
using PAQO.Memory.LiteDB.Find;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Func;

namespace PAQO.Memory.LiteDB.Group
{
    /// <summary>
    /// Group which updates the database and then delegates
    /// update to underlying origin group.
    /// </summary>
    public sealed class LDBActiveUpdateGroup : IGroup
    {
        private readonly IGroup origin;
        private readonly string elementType;
        private readonly ISchema schema;
        private readonly LiteDatabase database;
        private readonly SwapOf<IElement, BsonDocument> swapToBson;
        private readonly SwapOf<BsonDocument, IElement> swapToElement;

        /// <summary>
        /// Group which updates the database and then delegates
        /// update to underlying origin group.
        /// </summary>
        public LDBActiveUpdateGroup(
            string elementType,
            ISchema schema,
            IGroup origin,
            ILiteEngine engine,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        )
        {
            this.elementType = elementType;
            this.schema = schema;
            this.database = new LiteDatabase(engine);
            this.swapToBson =
                new SwapOf<IElement, BsonDocument>(element =>
                    new LDBElementBson(
                        elementType,
                        element,
                        schema,
                        swapPropToBsonValue,
                        swapPropToBsonStringValue
                    ).Value()
                );
            this.swapToElement =
                new SwapOf<BsonDocument, IElement>(doc =>
                    new LDBElement(
                        doc,
                        schema,
                        swapPropToBsonValue,
                        swapPropToBsonStringValue,
                        this.database.GetCollection(elementType),
                        elementType
                    )
                );
            this.origin = origin;
        }

        public IEnumerable<IElement> Elements()
        {
            return this.origin.Elements();
        }


        public void Add(IEnumerable<IElement> elements)
        {
            var existing =
                Mapped.New(element => element.RawValue["id"].AsString,
                    this.database
                        .GetCollection(this.elementType)
                        .Find(
                            new LDBElementQuery(
                                new IN("id", Mapped.New(element => element.ID(), elements)),
                                this.elementType,
                                this.schema
                            ).Value()
                        )
                );

            if (new MoreThan(0, existing).Value())
            {
                throw new ArgumentException($"Cannot add already existing elements: [{new Yaapii.Atoms.Text.Joined(", ", existing).AsString()}]");
            }

            this.database
                .GetCollection(this.elementType)
                .Insert(
                    Mapped.New(
                        element => this.swapToBson.Flip(element),
                        elements
                    )
                );
            this.origin.Add(elements);
        }

        public void Remove(IQuery query)
        {
            //remove entries
            this.database
                .GetCollection(this.elementType)
                .DeleteMany(
                    new LDBElementQuery(
                        query,
                        this.elementType,
                        this.schema
                    ).Value()
                );
            this.origin.Remove(query);
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return this.origin.Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            new Each<IElement>(element =>
                element.Update(props),
                Mapped.New(
                    doc => this.swapToElement.Flip(doc),
                    this.database
                        .GetCollection(this.elementType)
                        .Find(
                            new LDBElementQuery(query, this.elementType, schema).Value()
                        )
                )
            ).Invoke();
            this.origin.Update(query, props);
        }
    }
}
