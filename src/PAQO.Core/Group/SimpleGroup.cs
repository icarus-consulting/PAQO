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


using PAQO.Core.Find;
using PAQO.Core.Prop;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;

namespace PAQO.Core.Group
{
    /// <summary>
    /// Simple element group.
    /// </summary>
    public sealed class SimpleGroup : IGroup
    {
        private readonly List<IElement> elements;
        private readonly List<string> elementIDs;
        private readonly ISchema schema;
        private readonly Func<IElement, string> makeID;
        private readonly string context;

        /// <summary>
        /// Simple element group.
        /// </summary>
        public SimpleGroup(string elementType, ISchema schema, params IElement[] group) : this(
            elementType, schema, element => element.ID(), group
        )
        { }

        /// <summary>
        /// Simple element group.
        /// </summary>
        public SimpleGroup(string elementType, ISchema schema, Func<IElement, string> makeID, params IElement[] group) : this(
            elementType,
            schema,
            new ManyOf<IElement>(group),
            makeID
        )
        { }

        /// <summary>
        /// Simple element group.
        /// </summary>
        public SimpleGroup(string elementType, ISchema schema, IEnumerable<IElement> elements) : this(
            elementType, schema, elements, element => element.ID()
        )
        { }

        /// <summary>
        /// Simple element group.
        /// </summary>
        public SimpleGroup(string elementType, ISchema schema, IEnumerable<IElement> elements, Func<IElement, string> makeID)
        {
            this.elements = new List<IElement>(elements);
            this.elementIDs = new List<string>(Mapped.New(element => element.ID(), elements));
            this.schema = schema;
            this.makeID = makeID;
            this.context = elementType;
        }

        public IEnumerable<IElement> Elements()
        {
            return this.elements;
        }

        public void Add(IEnumerable<IElement> elements)
        {
            var withIDs = WithGeneratedIDs(elements);

            var existing =
                Union.New(
                    this.elementIDs,
                    Mapped.New(element => element.ID(), withIDs)
                );

            if (new MoreThan(1, existing).Value())
            {
                throw new InvalidOperationException($"Could not add elements [{String.Join(", ", existing)}] because they already exist.");
            }
            this.elements.AddRange(elements);
            this.elementIDs.AddRange(
                Mapped.New(
                    element => element.ID(),
                    this.elements
                )
            );
        }

        public void Remove(IQuery query)
        {
            var toRemove = this.Find(query).Elements();
            foreach (var element in toRemove)
            {
                this.elementIDs.Remove(element.ID());
                this.elements.Remove(element);
            }
        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            var match = new QueryMatch(query, this.schema, this.context);
            var result = new List<IElement>();
            var skip = -start;
            foreach (var element in this.elements)
            {
                if (match.Matches(element.Props()))
                {
                    if (skip < 0) skip++;
                    else
                    {
                        result.Add(element);
                        if (result.Count == amount)
                        {
                            break;
                        }
                    }
                }
            }
            return new SimpleGroup(this.context, this.schema, result, this.makeID);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            foreach (var element in this.Find(query).Elements())
            {
                element.Update(props);
            }
        }

        private IEnumerable<IElement> WithGeneratedIDs(IEnumerable<IElement> elements)
        {
            return
                Mapped.New(element =>
                    {
                        var newID = this.makeID(element);
                        if (!newID.Equals(element.ID()))
                        {
                            element.Update(ManyOf.New(new TextProp("id", newID)));
                        }
                        return element;
                    },
                    elements
                );
        }
    }
}
