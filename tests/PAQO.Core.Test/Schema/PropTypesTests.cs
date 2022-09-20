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

using System.Collections.Generic;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Collection;

namespace PAQO.Core.Schema.Test
{
    public sealed class PropTypesTests
    {
        [Theory]
        [InlineData("BuyDate", "date")]
        [InlineData("MaxSpeed", "integer")]
        [InlineData("HighestGearRatio", "decimal")]
        [InlineData("Name", "text")]
        [InlineData("ModelName", "text")]
        [InlineData("DriveType", "options")]
        public void ListsPropsAndTypes(string id, string name)
        {
            Assert.Contains($"{id}-{name}",
                new Mapped<KeyValuePair<string, string>, string>(
                    prop => $"{prop.Key}-{prop.Value}",
                    new PropTypes(
                        new VehiclesTestSchema(), "bike"
                    )
                )
            );
        }

        [Fact]
        public void ListsOnlyFromElementType()
        {
            var result =
                new Mapped<KeyValuePair<string, string>, string>(
                    att => $"{att.Key}",
                    new PropTypes(
                        new VehiclesTestSchema(), "bike"
                    )
                );
            Assert.Equal(
                new string[]
                {
                    "id",
                    "ModelName",
                    "Name",
                    "MaxSpeed",
                    "HighestGearRatio",
                    "Damaged",
                    "DriveType",
                    "AntiTheftCode",
                    "Something",
                    "EnergySource",
                    "integerprop2",
                    "FuelType",
                    "textprop2"
                },
                new Mapped<KeyValuePair<string, string>, string>(
                    att => $"{att.Key}",
                    new PropTypes(
                        new VehiclesTestSchema(), "bike"
                    )
                )
            );
        }
    }
}
