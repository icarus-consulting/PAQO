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
using System.Globalization;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Find
{
    /// <summary>
    /// Matches if the prop value contains another value.
    /// This is always realized as a string contains - meaning prop values and searched value
    /// are converted to strings despite their type and then tested.
    /// </summary>
    public sealed class CONTAINSMatch : MatchEnvelope
    {
        /// <summary>
        /// Matches if the prop value contains another value.
        /// This is always realized as a string contains - meaning prop values and searched value
        /// are converted to strings despite their type and then tested.
        /// </summary>
        public CONTAINSMatch(string propName, double expectedValuePart, IDictionary<string, string> propTypes, ISwap<string, byte[], string> bytesToString) : this(
            propName,
            () => expectedValuePart.ToString(CultureInfo.InvariantCulture),
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty),
            bytesToString
        )
        { }

        /// <summary>
        /// Matches if the prop value contains another value.
        /// This is always realized as a string contains - meaning prop values and searched value
        /// are converted to strings despite their type and then tested.
        /// </summary>
        public CONTAINSMatch(string propName, int expectedValuePart, IDictionary<string, string> propTypes, ISwap<string, byte[], string> bytesToString) : this(
            propName,
            () => expectedValuePart.ToString(),
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty),
            bytesToString
        )
        { }

        /// <summary>
        /// Matches if the prop value contains another value.
        /// This is always realized as a string contains - meaning prop values and searched value
        /// are converted to strings despite their type and then tested.
        /// </summary>
        public CONTAINSMatch(string propName, string expectedValuePart, IDictionary<string, string> propTypes, ISwap<string, byte[], string> bytesToString) : this(
            propName,
            () => expectedValuePart,
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty),
            bytesToString
        )
        { }

        /// <summary>
        /// Matches if the prop value contains another value.
        /// This is always realized as a string contains - meaning prop values and searched value
        /// are converted to strings despite their type and then tested.
        /// </summary>
        private CONTAINSMatch(string propName, Func<string> expectedValuePart, IScalar<bool> propExists, IText propType, ISwap<string, byte[], string> bytesToString) : base(
                new MatchOf("CONTAINS", props =>
                    propExists.Value()
                    &&
                    props.Contains(propName)
                    &&
                    bytesToString
                        .Flip(
                            propType.AsString(),
                            props.Content(propName)
                    ).Contains(
                        expectedValuePart()
                    )
                )
            )
        { }
    }
}
