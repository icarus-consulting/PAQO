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
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Pulse;
using Yaapii.Pulse.Signal;

namespace PAQO.Core.Pulse
{
    /// <summary>
    /// Signals updates of elements.
    /// </summary>
    public sealed class SigElementsUpdated : SignalEnvelope
    {
        /// <summary>
        /// Signals updates of elements.
        /// </summary>
        public SigElementsUpdated(string updateSource, string elementType, bool isPublic = false, params string[] ids) : this(
            updateSource, elementType, new ManyOf(ids), isPublic)
        { }

        /// <summary>
        /// Signals updates of elements.
        /// </summary>
        public SigElementsUpdated(string updateSource, string elementType, IEnumerable<string> ids, bool isPublic = false) : base(() =>
            new SignalOf(
                isPublic ?
                    new SigHead("group", "elements", "updated", new SigPublic(), new KvpOf("element-type", elementType), new KvpOf("initiator", updateSource))
                    :
                    new SigHead("group", "elements", "updated", new KvpOf("element-type", elementType), new KvpOf("initiator", updateSource)),
                new SigProps("ids", String.Join(",", ids))
            )
        )
        { }

        /// <summary>
        /// Tells if a signal is an element update signal.
        /// </summary>
        public sealed class Is : ScalarEnvelope<bool>
        {
            /// <summary>
            /// Tells if a signal is an element update signal.
            /// </summary>
            public Is(ISignal signal, string elementType) : base(() =>
                new SigHead.Is(signal, "group", "elements", "updated").Value()
                &&
                new SigHead.Includes(signal, "element-type", elementType).Value()
            )
            { }
        }

        /// <summary>
        /// Tells if a signal is from given initiator. Can prevent that a sender
        /// digests a signal.
        /// </summary>
        public sealed class IsFrom : ScalarEnvelope<bool>
        {
            /// <summary>
            /// Tells if a signal is from given initiator. Can prevent that a sender
            /// digests a signal.
            /// </summary>
            public IsFrom(string initiator, ISignal origin) : base(() =>
                origin.Head()["initiator"] == initiator
            )
            { }
        }

        /// <summary>
        /// Initiator of the signal.
        /// </summary>
        public sealed class Initiator : TextEnvelope
        {
            /// <summary>
            /// Initiator of the signal.
            /// </summary>
            public Initiator(ISignal origin) : base(() =>
                origin.Head()["initiator"], false
            )
            { }

            public static implicit operator string(Initiator initiator) => initiator.AsString();
        }

        /// <summary>
        /// Updated elements.
        /// </summary>
        public sealed class Elements : ListEnvelope<string>
        {
            /// <summary>
            /// Updated elements.
            /// </summary>
            public Elements(ISignal origin) : base(() =>
                origin.Props()["ids"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), false
            )
            { }
        }
    }
}
