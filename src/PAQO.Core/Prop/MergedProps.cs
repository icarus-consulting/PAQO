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
using Yaapii.Atoms.Enumerable;

namespace PAQO.Core.Prop
{
    /// <summary>
    /// Props merged by the rule "the last wins".
    /// </summary>
    public sealed class MergedProps : PropsEnvelope
    {
        /// <summary>
        /// Props merged by the rule "the last wins".
        /// </summary>
        public MergedProps(IProps leftProps, IEnumerable<IProp> rightProps) : this(
            new ManyOf<IProp>(() => leftProps.All()), rightProps
        )
        { }

        /// <summary>
        /// Props merged by the rule "the last wins".
        /// </summary>
        public MergedProps(IEnumerable<IProp> leftProps, IEnumerable<IProp> rightProps) : base(() =>
            {
                var overrides =
                    new Mapped<IProp, string>(
                        prop => prop.Name(),
                        rightProps
                    );

                var result =
                    new Joined<IProp>(
                        new Filtered<IProp>(
                            prop => !new Contains<string>(overrides, prop.Name()).Value(),
                            leftProps
                        ),
                        rightProps
                    );

                return new PropsOf(result);
            }
        )
        { }
    }
}
