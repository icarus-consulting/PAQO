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
using System.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PAQO.Core.Find
{
    /// <summary>
    /// Matches if given prop is equal.
    /// Type is important and checked against the given schema.
    /// </summary>
    public sealed class INMatch : IMatch
    {
        private readonly string propName;
        private readonly IScalar<bool> propExists;
        private readonly IText propType;
        private readonly SwapSwitch<byte[], bool> check;

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public INMatch(string propName, IEnumerable<double> values, IDictionary<string, string> propTypes) : this(
            propName,
            () =>
            new Mapped<double, byte[]>(
                value => BitConverter.GetBytes(Convert.ToDouble(value)),
                values
            ).ToArray(),
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty)
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="prop">name of the prop.</param>
        /// <param name="schema">value of the prop as string.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public INMatch(string propName, IEnumerable<int> values, IDictionary<string, string> propTypes) : this(
            propName,
            () =>
            new Mapped<int, byte[]>(
                value => BitConverter.GetBytes(Convert.ToDouble(value)),
                values
            ).ToArray(),
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty)
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        /// <param name="propName">name of the prop.</param>
        /// <param name="values">values to test.</param>
        /// <param name="schema">schema the prop belongs to.</param>
        /// <param name="stringToBytes">swap to turn string value into bytes, type based on given schema.</param>
        public INMatch(string propName, IEnumerable<string> values, IDictionary<string, string> propTypes, ISwap<string, string, byte[]> stringToBytes) : this(
            propName,
                () =>
                new Mapped<string, byte[]>(value =>
                    stringToBytes.Flip(
                        propTypes[propName],
                            value
                        ),
                        values
                    ).ToArray(),
            new ScalarOf<bool>(() => propTypes.ContainsKey(propName)),
            new TextOf(() => propTypes.ContainsKey(propName) ? propTypes[propName] : String.Empty)
        )
        { }

        /// <summary>
        /// Matches if given prop is equal.
        /// Type is important and checked against the given schema.
        /// </summary>
        private INMatch(string propName, Func<byte[][]> values, IScalar<bool> propExists, IText propType)
        {
            this.propName = propName;
            this.propExists = propExists;
            this.propType = propType;
            this.check =
                new SwapSwitch<byte[], bool>(
                    fallback: (unknown, propValue) => false,
                    new SwapIf<byte[], bool>(
                        "decimal", (propValue) => Doubles(values()).Contains(new DecimalProp.AsDouble(propValue).Value())
                    ),
                    new SwapIf<byte[], bool>(
                        "integer", (propValue) => Ints(values()).Contains(new IntProp.AsInt(propValue).Value())
                    ),
                    new SwapIf<byte[], bool>(
                        "switch", (propValue) => Bools(values()).Contains(new SwitchProp.IsOn(propValue).Value())
                    ),
                    new SwapIf<byte[], bool>(
                        "text", (propValue) => Texts(values()).Contains(new TextProp.AsText(propValue).AsString())
                        
                    ),
                    new SwapIf<byte[], bool>(
                        "options", (propValue) => Texts(values()).Contains(new TextProp.AsText(propValue).AsString())
                    ),
                    new SwapIf<byte[], bool>(
                        "complex", (propValue) => Texts(values()).Contains(new TextProp.AsText(propValue).AsString())
                    )
                );
        }

        public bool Matches(IProps props)
        {
            return
                this.propExists.Value()
                &&
                props.Contains(this.propName)
                &&
                this.check
                    .Flip(
                        this.propType.AsString(),
                        props.Content(this.propName)
                    );
        }

        private double[] Doubles(byte[][] bytes)
        {
            return
                new Mapped<byte[], double>(
                    item => new DecimalProp.AsDouble(item).Value(),
                    bytes
                ).ToArray();
        }

        private int[] Ints(byte[][] bytes)
        {
            return
                new Mapped<byte[], int>(
                    item => new IntProp.AsInt(item).Value(),
                    bytes
                ).ToArray();
        }

        private string[] Texts(byte[][] bytes)
        {
            return
                new Mapped<byte[], string>(
                    item => new TextProp.AsText(item).AsString(),
                    bytes
                ).ToArray();
        }

        private bool[] Bools(byte[][] bytes)
        {
            return
                new Mapped<byte[], bool>(
                    item => new SwitchProp.IsOn(item).Value(),
                    bytes
                ).ToArray();
        }

        public string Kind()
        {
            return "IN";
        }
    }
}
