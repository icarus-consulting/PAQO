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
using Yaapii.Pulse;

namespace PAQO.Core.Pulse
{
    /// <summary>
    /// Sensor detecting updates of elements.
    /// </summary>
    public sealed class SnsElementsUpdated : ISensor
    {
        private readonly string owner;
        private readonly string elementType;
        private readonly Action<IEnumerable<string>> reaction;

        /// <summary>
        /// Sensor detecting updates of elements.
        /// </summary>
        public SnsElementsUpdated(string owner, string elementType, Action<IEnumerable<string>> reaction)
        {
            this.owner = owner;
            this.elementType = elementType;
            this.reaction = reaction;
        }

        public bool IsActive()
        {
            return true;
        }

        public bool IsDead()
        {
            return false;
        }

        public void Trigger(ISignal signal)
        {
            if (new SigElementsUpdated.Is(signal, this.elementType).Value() && !new SigElementsUpdated.IsFrom(this.owner, signal).Value())
            {
                reaction(new SigElementsUpdated.Elements(signal));
            }
        }
    }
}
