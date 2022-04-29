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

using PAQO.Core.Element;
using System;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Prop.Tests
{
    public sealed class OptionsPropTests
    {
        [Fact]
        public void HasName()
        {
            Assert.Equal(
                "Käse",
                new OptionsProp(
                    "Käse",
                    "Cheddar"
                ).Name()
            );
        }

        [Fact]
        public void DeliversContent()
        {
            Assert.Equal(
                "Cheddar",
                new TextOf(
                    new OptionsProp(
                        "Käse",
                        "Cheddar"
                    ).Content()
                ).AsString()
            );
        }

        [Fact]
        public void DeliversValueName()
        {
            var schema = new VehiclesTestSchema();
            Assert.Equal(
                "Combustion",
                new OptionsProp.ValueName("DriveType",
                    new SimpleElement("1",
                        new OptionsProp("DriveType", "1")
                    ),
                    schema,
                    "bike"
                ).AsString()
            );
        }

        [Fact]
        public void DeliversFallbackForEmptyOption()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionsProp.ValueName("DriveType",
                        new SimpleElement("1"),
                        new VehiclesTestSchema(),
                        "bike"
                ).AsString()
            );
        }
    }
}
