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


using System;
using System.Collections.Generic;

namespace PAQO.Core.Find
{
    /// <summary>
    /// Matches if given prop is not equal.
    /// Type is important and checked against the given schema.
    /// </summary>
    public sealed class NOTMatch : MatchEnvelope
    {
        /// <summary>
        /// Matches if given prop is not equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public NOTMatch(string prop, double value, IDictionary<string, string> propTypes) : this(
            (props) => !new EQMatch(prop, value, propTypes).Matches(props)
        )
        { }

        /// <summary>
        /// Matches if given prop is not equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public NOTMatch(string prop, int value, IDictionary<string, string> propTypes) : this(
            (props) => !new EQMatch(prop, value, propTypes).Matches(props)
        )
        { }

        /// <summary>
        /// Matches if given prop is not equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public NOTMatch(string prop, string value, IDictionary<string, string> propTypes, ISwap<string, string, byte[]> stringToBytes) : this(
            (props) => !new EQMatch(prop, value, propTypes, stringToBytes).Matches(props)
        )
        { }

        /// <summary>
        /// Matches if given prop is not equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        private NOTMatch(Func<IProps, bool> matches) : base(
            new MatchOf("NOT", props => matches(props))
        )
        { }
    }
}
