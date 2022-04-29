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
using PAQO.Core.Find;
using PAQO.Core.Group;
using PAQO.Core.Pulse;
using PAQO.Memory.LiteDB.Element;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Pulse;

namespace PAQO.Memory.LiteDB.Group
{
    /// <summary>
    /// Group which senses updates/removals via the given pulse,
    /// then updates the database entries accordingly.
    /// When update is called on this group, it is directly
    /// delegated to the origin group, while the database remains
    /// untouched.
    /// </summary>
    public sealed class LDBPassiveUpdateGroup : IGroup
    {
        private readonly IGroup origin;
        private readonly ISchema schema;
        private readonly ILiteEngine engine;
        private readonly ISwap<string, byte[], BsonValue> swapPropToBsonValue;
        private readonly ISwap<string, byte[], BsonValue> swapPropToBsonStringValue;

        /// <summary>
        /// Group which senses updates/removals via the given pulse,
        /// then updates the database entries accordingly.
        /// When update is called on this group, it is directly
        /// delegated to the origin group, while the database remains
        /// untouched.
        /// </summary>
        public LDBPassiveUpdateGroup(
            string elementType,
            ISchema schema,
            IGroup origin,
            ILiteEngine engine,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue,
            IPulse pulse
        )
        {
            this.schema = schema;
            this.engine = engine;
            this.swapPropToBsonValue = swapPropToBsonValue;
            this.swapPropToBsonStringValue = swapPropToBsonStringValue;

            //Ensure sensors are connected before accessing elements.
            this.origin =
                new GroupOf(() =>
                {
                    pulse.Connect(
                        new SnsElementsUpdated("LDBUpdatingGroup", elementType, updatedElements => ReImport(updatedElements, elementType))
                    );

                    pulse.Connect(
                        new SnsElementAdding("LDBUpdatingGroup", elementType, addedElements => Add(addedElements, elementType))
                    );

                    pulse.Connect(
                        new SnsElementRemoval("LDBUpdatingGroup", elementType, removedElements => Remove(removedElements, elementType))
                    );
                    return origin;
                });
        }

        public IEnumerable<IElement> Elements()
        {
            return this.origin.Elements();
        }

        public void Add(IEnumerable<IElement> elements)
        {
            this.origin.Add(elements);
        }

        public void Remove(IQuery query)
        {
            this.origin.Remove(query);
        }

        public IGroup Find(IQuery query)
        {
            return this.origin.Find(query);
        }

        public IGroup Find(IQuery query, int start, int amount)
        {
            return this.origin.Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.origin.Update(query, props);
        }

        private void ReImport(IEnumerable<string> updatedElements, string elementType)
        {
            //load updated elements from origin
            var map =
                MapOf.New(
                    Mapped.New(
                        element => KvpOf.New(element.ID(), element),
                        this.Find(new IN("id", updatedElements)).Elements()
                    )
                );

            if (map.Count > 0)
            {
                var collection = new LiteDatabase(engine).GetCollection(elementType);
                foreach (var element in
                    collection.Find(
                        Query.In("id",
                            Mapped.New(
                                element => new BsonValue(element),
                                map.Keys
                            )
                        )
                    )
                )
                {
                    new LDBElement(
                        element,
                        this.schema,
                        this.swapPropToBsonValue,
                        this.swapPropToBsonStringValue,
                        collection,
                        elementType
                    ).Update(
                        map[element.RawValue["id"].AsString].Props().All()
                    );
                }
            }
        }

        private void Add(IEnumerable<string> addedElements, string elementType)
        {
            var db = new LiteDatabase(this.engine);
            var elements = db.GetCollection(elementType);
            foreach (var element in origin.Find(new IN("id", addedElements)).Elements())
            {
                elements.Insert(
                    new LDBElementBson(
                        elementType,
                        element,
                        schema,
                        swapPropToBsonValue,
                        swapPropToBsonStringValue
                    ).Value()
                );
            }

            foreach (var prop in schema.For(elementType).Types().Keys)
            {
                elements.EnsureIndex(prop);
            }

            db.Commit();
        }

        private void Remove(IEnumerable<string> removedElements, string elementType)
        {
            //remove entries
            var collection = new LiteDatabase(engine).GetCollection(elementType);
            var deleted =
                collection.DeleteMany(
                    Query.In("id",
                        Mapped.New(
                            element => new BsonValue(element),
                            removedElements
                        )
                    )
                );
        }
    }
}
