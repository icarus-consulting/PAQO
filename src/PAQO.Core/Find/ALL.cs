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


using Yaapii.Atoms.Scalar;
using Yaapii.Xml;

namespace PAQO.Core.Find
{
    /// <summary>
    /// ALL expression for a PAQO query.
    /// </summary>
    public sealed class ALL : QueryEnvelope
    {
        /// <summary>
        /// AND expression for a PAQO query.
        /// </summary>
        public ALL() : base(() =>
            new Expression("ALL")
        )
        { }

        /// <summary>
        /// Checks if a query is of type ALL 
        /// </summary>
        public sealed class Is : ScalarEnvelope<bool>
        {
            /// <summary>
            /// Checks if a query is of type ALL 
            /// </summary>
            public Is(IQuery candidate) : base(() =>
            {
                var xml = new QueryXML(candidate);
                return
                    xml.Nodes("./paq").Count == 1 &&
                    new XMLString(xml, "./paq/kind/text()").Value() == "ALL";
            })
            { }

            public static implicit operator bool(Is check) => check.Value();
        }

    }
}
