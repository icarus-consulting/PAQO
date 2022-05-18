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


using PAQO.Core.Facets;
using PAQO.Core.Schema;
using System;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;
using Yaapii.Xml;

namespace PAQO.Core.Find
{
    /// <summary>
    /// A match built from a PAQO query.
    /// </summary>
    public sealed class QueryMatch : IMatch
    {
        private readonly IMatch match;
        private readonly SwapSwitch<IXML, IMatch> matchConversion;
        private readonly ISwap<string, string, byte[]> stringToBytes;
        private readonly IDictionary<string, string> propTypes;

        /// <summary>
        /// A match built from a PAQO query.
        /// </summary>
        public QueryMatch(IQuery origin, ISchema schema, string context) : this(
            origin, schema, context, (matchKind, props, matched) => { }, false
        )
        { }

        /// <summary>
        /// A match built from a PAQO query.
        /// </summary>
        public QueryMatch(IQuery origin, ISchema schema, string context, Action<string, IProps, bool> tellResult) : this(
            origin, schema, context, tellResult, true
        )
        { }

        /// <summary>
        /// A match built from a PAQO query.
        /// </summary>
        private QueryMatch(IQuery origin, ISchema schema, string context, Action<string, IProps, bool> tellResult, bool verbose)
        {
            this.match =
                new MatchOf(() =>
                    AsMatch(
                        new FirstOf<IXML>(
                            new StrictAmount<IXML>(
                                new QueryXML(origin).Nodes("/paq"),
                                new ArgumentException($"There is more than one expression on root level in query. Only one is allowed.")
                            )
                        ).Value()
                    )
                );
            this.propTypes = new MapOf(() => schema.For(context).Types());
            this.stringToBytes = new SwapStringToBytes();
            this.matchConversion =
                new SwapSwitch<IXML, IMatch>(
                    new SwapIf<IXML, IMatch>("ALL", input =>
                    {
                        IMatch result = new ALLMatch();
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("NONE", input =>
                        new NONEMatch()
                    ),
                    new SwapIf<IXML, IMatch>("AND", input =>
                        {
                            IMatch result =
                                new ANDMatch(
                                    new Mapped<IXML, IMatch>(
                                        paq =>
                                        {
                                            IMatch inner = AsMatch(paq);
                                            if (verbose) inner = new VerboseMatch(inner, tellResult);
                                            return inner;
                                        },
                                        input.Nodes("./paqs/paq")
                                    )
                                );
                            if (verbose) result = new VerboseMatch(result, tellResult);
                            return result;
                        }
                    ),
                    new SwapIf<IXML, IMatch>("OR", input =>
                    {
                        IMatch result =
                            new ORMatch(
                                new Mapped<IXML, IMatch>(
                                    paq =>
                                    {
                                        IMatch inner = AsMatch(paq);
                                        if (verbose) inner = new VerboseMatch(inner, tellResult);
                                        return inner;
                                    },
                                    input.Nodes("./paqs/paq")
                                )
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("EQ", input =>
                    {
                        IMatch result =
                            new EQMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("NOT", input =>
                    {
                        IMatch result =
                            new NOTMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("GT", input =>
                    {
                        IMatch result =
                            new GTMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("GTE", input =>
                    {
                        IMatch result =
                            new GTEMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("IN", input =>
                    {
                        IMatch result =
                            new INMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                input.Values("./paqs/paq/values/value/text()"),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("LT", input =>
                    {
                        IMatch result =
                            new LTMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("LTE", input =>
                    {
                        IMatch result =
                            new LTEMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                this.stringToBytes
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    }),
                    new SwapIf<IXML, IMatch>("CONTAINS", input =>
                    {
                        IMatch result =
                            new CONTAINSMatch(
                                new XMLString(input, "./paqs/paq/field/text()").Value(),
                                new XMLString(input, "./paqs/paq/value/text()").Value(),
                                this.propTypes,
                                new SwapBytesToString()
                            );
                        if (verbose) result = new VerboseMatch(result, tellResult);
                        return result;
                    })
                );
        }

        public string Kind()
        {
            return this.match.Kind();
        }

        public bool Matches(IProps props)
        {
            return this.match.Matches(props);
        }

        private IMatch AsMatch(IXML xmlExpression)
        {
            var kind = new XMLString(xmlExpression, "./kind/text()").Value();
            return
                this.matchConversion
                    .Flip(kind, xmlExpression);
        }
    }
}
