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


using PAQO.Core.Facets;
using PAQO.Core.Prop;
using System;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Find
{
    /// <summary>
    /// Matches if given prop is equal.
    /// Type is important and checked against the given schema.
    /// </summary>
    public sealed class EQMatch : MatchEnvelope
    {
        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public EQMatch(string prop, double value, IDictionary<string, string> propTypes) : this(
            prop, () => BitConverter.GetBytes(value), propTypes
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public EQMatch(string prop, int value, IDictionary<string, string> propTypes) : this(
            prop, () => BitConverter.GetBytes(Convert.ToDouble(value)), propTypes
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public EQMatch(string prop, long value, IDictionary<string, string> propTypes) : this(
            prop, () => BitConverter.GetBytes(value), propTypes
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="propName">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public EQMatch(string propName, string value, IDictionary<string, string> propTypes, ISwap<string, string, byte[]> stringToBytes) : this(
            propName,
            () =>
            stringToBytes.Flip(
                propTypes[propName],
                value
            ),
            propTypes
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        private EQMatch(string propName, Func<byte[]> value, IDictionary<string, string> propTypes) : this(
            propName,
            value,
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes[propName]),
            new SwapSwitch<byte[], bool>(
                fallback: (unknown, propValue) => false,
                new SwapIf<byte[], bool>(
                    "decimal", (propValue) => new DecimalProp.AsDouble(value()).Value() == new DecimalProp.AsDouble(propValue).Value()
                ),
                new SwapIf<byte[], bool>(
                    "integer", (propValue) => new IntProp.AsInt(value()).Value() == new IntProp.AsInt(propValue).Value()
                ),
                new SwapIf<byte[], bool>(
                    "date", (propValue) => new IntProp.AsLong(value()).Value() == new IntProp.AsLong(propValue).Value()
                ),
                new SwapIf<byte[], bool>(
                    "switch", (propValue) => new SwitchProp.IsOn(value()).Value() == new SwitchProp.IsOn(propValue).Value()
                ),
                new SwapIf<byte[], bool>(
                    "text", (propValue) =>
                    String.Compare(
                        new TextProp.AsText(propValue).AsString(),
                        new TextProp.AsText(value()).AsString()
                    ) == 0
                ),
                new SwapIf<byte[], bool>(
                    "options", (propValue) =>
                    String.Compare(
                        new TextProp.AsText(propValue).AsString(),
                        new TextProp.AsText(value()).AsString()
                    ) == 0
                ),
                new SwapIf<byte[], bool>(
                    "complex", (propValue) =>
                    String.Compare(
                        new TextProp.AsText(propValue).AsString(),
                        new TextProp.AsText(value()).AsString()
                    ) == 0
                )
            )
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        private EQMatch(string propName, Func<byte[]> value, IScalar<bool> propExists, IText propType, SwapSwitch<byte[], bool> comparison) : base(
            new MatchOf("EQ", props =>
                propExists.Value()
                &&
                props.Contains(propName)
                &&
                comparison
                    .Flip(
                        propType.AsString(),
                        props.Content(propName)
                    )
            )
        )
        { }
    }
}
