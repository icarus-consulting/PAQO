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
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Find
{
    /// <summary>
    /// A match.
    /// </summary>
    public sealed class MatchOf : IMatch
    {
        private readonly IText kind;
        private readonly Func<IProps, bool> matches;

        /// <summary>
        /// A match.
        /// </summary>
        public MatchOf(Func<IMatch> match) : this(new ScalarOf<IMatch>(match))
        { }

        /// <summary>
        /// A match.
        /// </summary>
        public MatchOf(string kind, Func<IProps, bool> matches) : this(
            new TextOf(kind),
            matches
        )
        { }

        /// <summary>
        /// A match.
        /// </summary>
        public MatchOf(IScalar<IMatch> match) : this(
            new TextOf(() => match.Value().Kind()),
            (props) => match.Value().Matches(props)
        )
        { }

        /// <summary>
        /// A match.
        /// </summary>
        private MatchOf(IText kind, Func<IProps, bool> matches)
        {
            this.kind = kind;
            this.matches = matches;
        }

        public string Kind()
        {
            return this.kind.AsString();
        }

        public bool Matches(IProps props)
        {
            return this.matches(props);
        }
    }
}
