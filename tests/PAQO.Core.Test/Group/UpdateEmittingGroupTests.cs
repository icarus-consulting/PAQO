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
using PAQO.Core.Find;
using PAQO.Core.Prop;
using PAQO.Core.Pulse;
using System.Collections.Generic;
using Test.PAQO.Schema;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Pulse;

namespace PAQO.Core.Group.Test
{
    public sealed class UpdateEmittingGroupTests
    {
        [Fact]
        public void InformsAboutUpdate()
        {
            IEnumerable<string> result = new None();
            var pulse = new SyncPulse();
            pulse.Connect(new SnsElementsUpdated("not-from-my-test", "bike", ids => result = ids));
            new UpdateEmittingGroup("my-test", "bike",
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement("1",
                        new TextProp("name", "Lightning McQueen")
                    ),
                    new SimpleElement("2",
                        new TextProp("name", "Airwolf")
                    )
                ),
                pulse
            ).Update(
                new EQ("id", "1"),
                new TextProp("name", "bad-property")
            );

            Assert.Equal(new string[] { "1" }, result);
        }

        [Fact]
        public void InformsAboutRemoval()
        {
            IEnumerable<string> result = new None();
            var pulse = new SyncPulse();
            pulse.Connect(new SnsElementRemoval("not-from-my-test", "bike", ids => result = ids));
            new UpdateEmittingGroup("my-test", "bike",
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement("1",
                        new TextProp("name", "Lightning McQueen")
                    ),
                    new SimpleElement("2",
                        new TextProp("name", "Airwolf")
                    )
                ),
                pulse
            ).Remove(
                new EQ("id", "1")
            );
            Assert.Equal(new string[] { "1" }, result);
        }

        [Fact]
        public void InformsAboutAdding()
        {
            IEnumerable<string> result = new None();
            var pulse = new SyncPulse();
            pulse.Connect(new SnsElementAdding("not-from-my-test", "bike", ids => result = ids));
            new UpdateEmittingGroup("my-test", "bike",
                new SimpleGroup("bike",
                    new VehiclesTestSchema(),
                    new SimpleElement("1",
                        new TextProp("name", "Lightning McQueen")
                    )
                ),
                pulse
            ).Add(
                ManyOf.New(
                    new SimpleElement("2",
                        new TextProp("name", "Airwolf")
                    )
                )
            );
            Assert.Equal(new string[] { "2" }, result);
        }
    }
}
