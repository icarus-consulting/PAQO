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

using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;

namespace PAQO.Memory.LiteDB.Facets.Test
{
    public sealed class LDBCollectionIDTests
    {
        [Fact]
        public void BuildsID()
        {
            Assert.Equal(
                "col_3432654663472F63582F6C2F704775792B614546437A55446464513D",
                new LDBSingleProgramCollectionID("!\"§$%&/()=?`´°^ß\\*+~'#-:.;,><|").AsString()
            );
        }

        [Fact]
        public void IsReproducable()
        {
            Assert.Equal(
                new LDBSingleProgramCollectionID("same").AsString(),
                new LDBSingleProgramCollectionID("same").AsString()
            );
        }

        [Fact]
        public void HasNoObviousCollisions()
        {
            var current = 0;
            var hashes =
                new ListOf<string>(
                    new Repeated<string>(
                        () => new LDBSingleProgramCollectionID(current++.ToString()).AsString(),
                        64
                    )
                );

            hashes.GetEnumerator();

            foreach (var entry in hashes)
            {
                Assert.Equal(1,
                    new LengthOf(
                        new Filtered<string>(
                            hash => entry == hash,
                            hashes
                        )
                    ).Value()
                );
            }
        }
    }
}
