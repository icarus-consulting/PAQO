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
using PAQO.Core.Facets;
using PAQO.Core.Find;
using PAQO.Memory.LiteDB.Facets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;
using Yaapii.Xml;

namespace PAQO.Memory.LiteDB.Find
{
    /// <summary>
    /// A PAQO Element query as LitedDB BsonExpression.
    /// </summary>
    public sealed class LDBElementQuery : IScalar<BsonExpression>
    {
        private readonly string elementType;
        private readonly ISchema schema;
        private readonly ScalarOf<BsonExpression> expression;
        private readonly SwapSwitch<string, IXML, BsonExpression> expressionSwap;
        private readonly SwapStringToBsonValue bsonSwap;

        /// <summary>
        /// A PAQO element query as LitedDB expression.
        /// <param name="origin">Original query.</param>
        /// <param name="schemata">The schemata of the element-types the props may belong to. It is needed to convert the
        /// prop values to the correct type (int, string) to enable type specific queries like GT (greater than).</param>
        /// </summary>
        public LDBElementQuery(IQuery origin, string elementType, ISchema schema)
        {
            this.bsonSwap = new SwapStringToBsonValue();
            this.elementType = elementType;
            this.schema = schema;
            this.expression =
                new ScalarOf<BsonExpression>(() =>
                    Expression(
                        new FirstOf<IXML>(
                            new StrictAmount<IXML>(
                                new QueryXML(origin).Nodes("/paq"),
                                new ArgumentException($"There is more than one expression on root level. Only one is allowed.")
                            )
                        ).Value()
                    )
                );

            this.expressionSwap =
                new SwapSwitch<string, IXML, BsonExpression>(
                    new SwapIf<string, IXML, BsonExpression>("ALL", (propName, paq) =>
                        Query.Not("_id", null)
                    ),
                    new SwapIf<string, IXML, BsonExpression>("NONE", (propName, paq) =>
                        Query.EQ("_id", Guid.NewGuid().ToString()) //This will never be true. But if it is, buy a lottery ticket RIGHT NOW
                    ),
                    new SwapIf<string, IXML, BsonExpression>("AND", (propName, paq) =>
                        Query.And(
                            new Yaapii.Atoms.Enumerable.Mapped<IXML, BsonExpression>(
                                pq => Expression(pq),
                                paq.Nodes("./paqs/paq")
                            ).ToArray()
                        )
                    ),
                    new SwapIf<string, IXML, BsonExpression>("OR", (propName, paq) =>
                        Query.Or(
                            new Yaapii.Atoms.Enumerable.Mapped<IXML, BsonExpression>(
                                pq => Expression(pq),
                                paq.Nodes("./paqs/paq")
                            ).ToArray()
                        )
                    ),
                    new SwapIf<string, IXML, BsonExpression>("EQ", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.EQ(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("NOT", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.Not(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("GT", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.GT(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("GTE", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.GTE(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("IN", (propName, paq) =>
                        INMatch(propName, paq)
                    ),
                    new SwapIf<string, IXML, BsonExpression>("LT", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.LT(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("LTE", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.LTE(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("STARTS", (propName, paq) =>
                        MultiMatch(propName, paq, (pName, bsonValue) => Query.StartsWith(propName, bsonValue))
                    ),
                    new SwapIf<string, IXML, BsonExpression>("CONTAINS", (propName, paq) =>
                        ContainsMatch(propName, paq) //, (pName, bsonValue) => Query.Contains(propName, bsonValue))
                    )
                );
        }

        public BsonExpression Value()
        {
            Debug.WriteLine(this.expression.Value().ToString());
            return this.expression.Value();
        }

        private BsonExpression Expression(IXML paq)
        {
            var queryType = new XMLString(paq, "./kind/text()").Value();
            var propName = new XMLString(paq, "./paqs/paq/field/text()", String.Empty).Value();
            return
                this.expressionSwap
                    .Flip(queryType, propName, paq);
        }

        private IList<string> PropTypes(string propName)
        {
            return
                new ListOf<string>(
                    this.schema.For(this.elementType).Types()[propName]
                );
        }

        /// <summary>
        /// If a prop has the same name but different types in multiple schemata,
        /// it results in an OR match of multiple expression, of which each one respects a different type.
        /// This ensures, that queries like "EQ" can test if a number is equal ("1.0" == 1 = true)
        /// while a string in another program with a different schema might not ("1.0" == "1" = false)
        /// This match is built by this method.
        /// </summary>
        private BsonExpression MultiMatch(string propName, IXML paq, Func<string, BsonValue, BsonExpression> build)
        {
            var strValue = new XMLString(paq, "./paqs/paq/value/text()").Value();
            var types = PropTypes(propName);
            BsonExpression result = Query.EQ("_id", null); //an id will never be null. This is used as a workaround for having a query that has always a negative result.
            if (types.Count > 0)
            {
                result = build(propName, this.bsonSwap.Flip(types[0], strValue));
                if (types.Count > 1)
                {
                    foreach (var type in new Skipped<string>(types, 1))
                    {
                        result =
                            Query.Or(result, build(propName, this.bsonSwap.Flip(type, strValue))
                        );
                    }
                }
            }
            return result;
        }

        private BsonExpression ContainsMatch(string propName, IXML paq)
        {
            var strValue =
                new XMLString(
                    paq,
                    "./paqs/paq/value/text()"
                ).Value();

            return Query.Contains($"{propName}_asText", strValue); //all properties are stored as text in parallel to normal.
        }

        private BsonExpression INMatch(string propName, IXML paq)
        {
            var strValues = new XMLStrings("./paqs/paq/values/value/text()", paq);
            var types = PropTypes(propName);
            BsonExpression result = Query.EQ("_id", null); //an id will never be null. This is used as a workaround for having a query that has always a negative result.
            if (types.Count > 0)
            {
                result =
                    Query.In(
                        propName,
                        new BsonArray(
                            new Yaapii.Atoms.Enumerable.Mapped<string, BsonValue>(
                                strValue => this.bsonSwap.Flip(types[0], strValue),
                                strValues
                            )
                        )
                    );

                if (types.Count > 1)
                {
                    foreach (var type in new Skipped<string>(types, 1))
                    {
                        result =
                            Query.Or(
                                result,
                                Query.In(
                                    propName,
                                    new BsonArray(
                                        new Yaapii.Atoms.Enumerable.Mapped<string, BsonValue>(
                                            strValue => this.bsonSwap.Flip(type, strValue),
                                            strValues
                                        )
                                    )
                                )
                            );
                    }
                }
            }
            return result;
        }
    }
}
