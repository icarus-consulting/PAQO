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
using PAQO.Core.Group;
using PAQO.Memory.LiteDB.Element;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;

namespace PAQO.Memory.LiteDB.Group
{
    /// <summary>
    /// This group will import all elements from the origin to the database
    /// when a method is called.
    /// </summary>
    public sealed class LDBImportingGroup : IGroup
    {
        private readonly SwapOf<IElement, BsonDocument> swapToBson;
        private readonly LiteDatabase database;
        private readonly IGroup origin;

        /// <summary>
        /// This group will import all elements from the origin to the database
        /// when a method is called.
        /// </summary>
        public LDBImportingGroup(
            string elementType,
            IGroup origin,
            ISchema schema,
            ILiteEngine memory,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        )
        {
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
            this.database = new LiteDatabase(memory);
            this.origin =
                new GroupOf(() =>
                {
                    var col = this.database.GetCollection(elementType);
                    col.Insert(
                        Mapped.New(
                            element => this.swapToBson.Flip(element),
                            origin.Find(new ALL()).Elements()
                        )
                    );

                    foreach (var prop in schema.For(elementType).Types().Keys)
                    {
                        col.EnsureIndex(prop);
                    }
                    this.database.Commit();
                    return origin;
                });
        }

        public IEnumerable<IElement> Elements()
        {
            return this.origin.Find(new ALL()).Elements();
        }

        public void Add(IEnumerable<IElement> elements)
        {
            this.origin.Add(elements);
        }

        public void Remove(IQuery query)
        {
            this.origin.Remove(query);
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return this.origin.Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.origin.Update(query, props);
        }
    }
}
