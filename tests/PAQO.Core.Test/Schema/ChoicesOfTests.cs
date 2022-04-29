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

using PAQO.Core.Prop;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;

namespace PAQO.Core.Schema.Test
{
    public sealed class ChoicesOfTests
    {
        [Fact]
        public void DeliversChoicesKeys()
        {
            Assert.Equal(
                new string[] { "0", "1", "2" },
                new Mapped<IKvp, string>(
                    option => option.Key(),
                    new ChoicesOf(
                        new VehiclesTestSchema(),
                        "DriveType",
                        "bike",
                        new PropsOf(
                            new OptionsProp("DriveType", "0") //this is the dependant
                        )
                    )
                )
            );
        }

        [Fact]
        public void DeliversChoicesValues()
        {
            Assert.Equal(
                new string[] { "Electric", "Combustion", "Muscle" },
                new Mapped<IKvp, string>(
                    option => option.Value(),
                    new ChoicesOf(
                        new VehiclesTestSchema(),
                        "DriveType",
                        "bike",
                        new PropsOf(
                            new OptionsProp("DriveType", "0") //this is the dependant
                        )
                    )
                )
            );
        }

        [Fact]
        public void DeliversDependingChoicesKeys()
        {
            Assert.Equal(
                new string[] { "0", "1" },
                new Mapped<IKvp, string>(
                    option => option.Key(),
                    new ChoicesOf(
                        new VehiclesTestSchema(),
                        "EnergySource",
                        "bike",
                        new PropsOf(
                            new OptionsProp("DriveType", "0") //this is the dependant
                        )
                    )
                )
            );
        }

        [Fact]
        public void DeliversDependingChoicesValues()
        {
            Assert.Equal(
                new string[] { "Atomic", "Solar" },
                new Mapped<IKvp, string>(
                    option => option.Value(),
                    new ChoicesOf(
                        new VehiclesTestSchema(),
                        "EnergySource",
                        "bike",
                        new PropsOf(
                            new OptionsProp("DriveType", "0") //this is the dependant
                        )
                    )
                )
            );
        }

        [Fact]
        public void DeliversNoneOnMissingDependant()
        {
            Assert.Equal(
                new string[0],
                new Mapped<IKvp, string>(
                    option => option.Value(),
                    new ChoicesOf(
                        new VehiclesTestSchema(),
                        "Monsieur-Ne-Pas-La",
                        "bike",
                        new PropsOf()
                    )
                )
            );
        }
    }
}
