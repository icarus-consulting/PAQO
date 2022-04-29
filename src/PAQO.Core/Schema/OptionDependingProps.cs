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

using Yaapii.Atoms.List;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Names of all props in the schema which are depending on the given
    /// prop.
    /// </summary>
    public sealed class OptionDependingProps : ListEnvelope<string>
    {
        /// <summary>
        /// Names of all props in the schema which are depending on the given
        /// prop.
        /// </summary>
        public OptionDependingProps(string propName, string propValue, ISchema schema, string elementType) : base(() =>
            {
                return
                    schema.AsXML().Values(
                        $"/schema/{elementType}/attributes/"
                        + $"depending[on/text()='{propName}']/"
                        + $"branch[when/text()='{propValue}']"
                        + "//*[self::integer"
                            + " or self::text"
                            + " or self::decimal"
                            + " or self::options"
                            + " or self::switch"
                            + " or self::complex]/"
                            + $"id/text()"
                    );
            },
            false
        )
        { }
    }
}
