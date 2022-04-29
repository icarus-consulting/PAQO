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
using PAQO.Core.Pulse;
using System.Collections.Generic;
using Yaapii.Atoms.Enumerable;
using Yaapii.Pulse;

namespace PAQO.Core.Group
{
    /// <summary>
    /// Group which informs about updates using given pulse.
    /// </summary>
    public sealed class UpdateEmittingGroup : IGroup
    {
        private readonly string signalOwner;
        private readonly string elementType;
        private readonly IGroup origin;
        private readonly IPulse pulse;

        /// <summary>
        /// Group which informs about updates using given pulse.
        /// </summary>
        public UpdateEmittingGroup(string signalOwner, string elementType, IGroup origin, IPulse pulse)
        {
            this.signalOwner = signalOwner;
            this.elementType = elementType;
            this.origin = origin;
            this.pulse = pulse;
        }

        public IEnumerable<IElement> Elements()
        {
            return this.origin.Elements();
        }

        public void Add(IEnumerable<IElement> elements)
        {
            this.origin.Add(elements);
            this.pulse
                .Send(
                    new SigElementsAdded(
                        this.signalOwner,
                        this.elementType,
                        new ElementIDs(elements)
                    )
                );
        }

        public void Remove(IQuery query)
        {
            var removal = new ElementIDs(this.Find(query).Elements());
            this.origin.Remove(query);
            this.pulse
                .Send(new SigElementsRemoved(this.signalOwner, this.elementType, removal));

        }

        public IGroup Find(IQuery query, int start = 0, int amount = int.MaxValue)
        {
            return this.origin.Find(query, start, amount);
        }

        public void Update(IQuery query, params IProp[] props)
        {
            var ids = new ElementIDs(this.origin.Find(query).Elements());
            this.origin
                .Update(query, props);
            this.pulse
                .Send(new SigElementsUpdated(this.signalOwner, this.elementType, ids));
        }
    }
}
