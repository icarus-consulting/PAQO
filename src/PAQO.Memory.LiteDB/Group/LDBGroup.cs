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
using PAQO.Core.Group;
using PAQO.Memory.LiteDB.Facets;
using System.Collections.Generic;

namespace PAQO.Memory.LiteDB.Group
{
    /// <summary>
    /// Group implemented using LiteDB.
    /// </summary>
    public sealed class LDBGroup : IGroup
    {
        private readonly IGroup elements;

        /// <summary>
        /// Group implemented using LiteDB.
        /// This group optionally imports all elements from a given group.
        /// Updates / Adds / Removals will update the database first and
        /// then be delegated to the origin group (if present)
        /// </summary>
        public LDBGroup(
            IGroup origin,
            ISchema schema,
            string elementType,
            ILiteEngine engine
        ) : this(
            origin,
            schema,
            elementType,
            engine,
            new SwapBytesToBsonValue(),
            new SwapBytesToBsonStringValue()
        )
        { }

        /// <summary>
        /// Group implemented using LiteDB.
        /// This group optionally imports all elements from a given group.
        /// Updates / Adds / Removals will update the database first and
        /// then be delegated to the origin group (if present)
        /// </summary>
        public LDBGroup(
            IGroup origin,
            ISchema schema,
            string elementType
        ) : this(
            origin,
            schema,
            elementType,
            new LDBMemoryEngine(),
            new SwapBytesToBsonValue(),
            new SwapBytesToBsonStringValue()
        )
        { }

        /// <summary>
        /// Group implemented using LiteDB.
        /// This group optionally imports all elements from a given group.
        /// Updates / Adds / Removals will update the database first and
        /// then be delegated to the origin group (if present)
        /// </summary>
        public LDBGroup(
            IGroup origin,
            ISchema schema,
            string elementType,
            ILiteEngine memory,
            ISwap<string, byte[], BsonValue> swapPropToBsonValue,
            ISwap<string, byte[], BsonValue> swapPropToBsonStringValue
        )
        {
            this.elements =
                new GroupOf(() =>
                    new LDBQueryingGroup(elementType, schema,
                        new LDBImportingGroup(elementType,
                            new LDBActiveUpdateGroup(
                                elementType,
                                schema,
                                origin,
                                memory,
                                swapPropToBsonValue,
                                swapPropToBsonStringValue
                            ),
                            schema,
                            memory,
                            swapPropToBsonValue,
                            swapPropToBsonStringValue
                        ),
                        memory,
                        swapPropToBsonValue,
                        swapPropToBsonStringValue
                    )
                );
        }

        public IEnumerable<IElement> Elements()
        {
            return this.elements.Elements();
        }

        public void Add(IEnumerable<IElement> elements)
        {
            this.elements.Add(elements);
        }

        public void Remove(IQuery query)
        {
            this.elements.Remove(query);
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return this.elements.Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            this.elements.Update(query, props);
        }
    }
}
