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

namespace PAQO.Core.Find.Test
{
    public sealed class INTests
    {
        [Fact]
        public void CompilesText()
        {
            Assert.Equal(
                "<paq><kind>IN</kind><paqs><paq><field>year</field><values><value>1980</value><value>2021</value></values></paq></paqs></paq>",
                new QueryXML(
                    new IN("year", "1980", "2021")
                )
                .AsNode()
                .ToString(System.Xml.Linq.SaveOptions.DisableFormatting)
            );
        }

        [Fact]
        public void CompilesNumberWithoutThousandsSeparator()
        {
            Assert.Equal(
                "<paq><kind>IN</kind><paqs><paq><field>year</field><values><value>1980</value><value>2021</value></values></paq></paqs></paq>",
                new QueryXML(
                    new IN("year", 1980, 2021)
                )
                .AsNode()
                .ToString(System.Xml.Linq.SaveOptions.DisableFormatting)
            );
        }

        [Fact]
        public void CompilesDecimalWithDotSeparator()
        {
            Assert.Equal(
                "<paq><kind>IN</kind><paqs><paq><field>year</field><values><value>13.36</value><value>13.37</value></values></paq></paqs></paq>",
                new QueryXML(
                    new IN("year", 13.36, 13.37)
                )
                .AsNode()
                .ToString(System.Xml.Linq.SaveOptions.DisableFormatting)
            );
        }
    }
}
