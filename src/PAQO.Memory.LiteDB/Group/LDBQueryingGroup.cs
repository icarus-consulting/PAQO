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
using PAQO.Core.Element;
using PAQO.Core.Facets;
using PAQO.Core.Find;
using PAQO.Core.Group;
using PAQO.Memory.LiteDB.Element;
using PAQO.Memory.LiteDB.Find;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Group
{
    /// <summary>
    /// Group which finds elements using LiteDB.
    /// Note that this grop does not import anything into the database,
    /// it just uses the given database for queries.
    /// If you want to import things or add things, inject the appropriate
    /// objects via the origin IGroup parameter into the constructor.
    /// </summary>
    public sealed class LDBQueryingGroup : IGroup
    {
        private readonly ISchema schema;
        private readonly SwapOf<BsonDocument, IElement> swapToElement;
        private readonly string elementType;
        private readonly IGroup origin;
        private readonly IScalar<ILiteCollection<BsonDocument>> collection;

        /// <summary>
        /// Group which finds elements using LiteDB.
        /// Note that this grop does not support changing anything in the database,
        /// it just uses the given database for queries.
        /// If you want to import things or add things, inject the appropriate
        /// objects via the origin IGroup parameter into the constructor.
        /// </summary>
        public LDBQueryingGroup(
            string elementType,
            ISchema schema,
            ILiteEngine memory,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        ) : this(
            elementType,
            schema,
            new DeadGroup(),
            memory,
            swapPropToBsonValue,
            swapPropToBsonStringValue,
            hasOriginGroup: false //since there is no origin group, it should not be queried.
        )
        { }

        /// <summary>
        /// Group which finds elements using LiteDB.
        /// Note that this grop does not import anything into the database,
        /// it just uses the given database for queries.
        /// If you want to import things or add things, inject the appropriate
        /// objects via the origin IGroup parameter into the constructor.
        /// </summary>
        public LDBQueryingGroup(
            string elementType,
            ISchema schema,
            IGroup origin,
            ILiteEngine memory,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        ) : this(
            elementType,
            schema,
            origin,
            memory,
            swapPropToBsonValue,
            swapPropToBsonStringValue,
            hasOriginGroup: true //if there is an origin group, it should be woken up. This triggers imports for example.
        )
        { }

        /// <summary>
        /// Group which finds elements using LiteDB.
        /// Note that this grop does not import anything into the database,
        /// it just uses the given database for queries.
        /// If you want to import things or add things, inject the appropriate
        /// objects via the origin IGroup parameter into the constructor.
        /// </summary>
        private LDBQueryingGroup(
            string elementType,
            ISchema schema,
            IGroup origin,
            ILiteEngine memory,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            Core.ISwap<string, byte[], BsonValue> swapPropToBsonStringValue,
            bool hasOriginGroup
        )
        {
            this.elementType = elementType;
            this.schema = schema;
            this.origin = origin;
            this.collection =
                ScalarOf.New(() =>
                {
                    if (hasOriginGroup)
                    {
                        this.origin.Find(new ALL()); //this ensures the origin group performs its tasks like importing into the DB, if necessary.
                    }
                    return new LiteDatabase(memory).GetCollection(elementType);
                });
            this.swapToElement =
                new SwapOf<BsonDocument, IElement>(doc =>
                        new LDBElement(
                            doc,
                            this.schema,
                            swapPropToBsonValue,
                            swapPropToBsonStringValue,
                            this.collection.Value(),
                            this.elementType
                        )
                );
        }

        public IEnumerable<IElement> Elements()
        {
            return Elements(new ALL(), 0, int.MaxValue);
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
            return
                new LimitedGroup(this,
                    Yaapii.Atoms.Enumerable.Mapped.New(
                        element => element.ID(),
                        Elements(query, start, amount)
                    )
                );
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.origin.Update(query, props);
        }

        public IEnumerable<IElement> Elements(IQuery query, int start = 0, int maxValue = int.MaxValue)
        {
            var elements =
                new ElementList(
                    new Yaapii.Atoms.Enumerable.Mapped<BsonDocument, IElement>(doc =>
                        this.swapToElement
                            .Flip(doc),
                        this.collection
                            .Value()
                            .Find(
                                new LDBElementQuery(query, this.elementType, this.schema).Value(),
                                start,
                                maxValue
                            )
                    )
                );
            elements.GetEnumerator().MoveNext(); //build here and now
            return elements;
        }
    }
}
