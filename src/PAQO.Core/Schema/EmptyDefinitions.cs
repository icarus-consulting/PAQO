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
using Yaapii.Atoms;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Map;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Definitions that do not define anything.
    /// </summary>
    public sealed class EmptyDefinitions : IDefinitions
    {
        private readonly IList<IKvp> choices;
        private readonly EmptyMap types;

        /// <summary>
        /// Definitions that do not define anything.
        /// </summary>
        public EmptyDefinitions()
        {
            this.choices = new ListOf<IKvp>();
            this.types = new EmptyMap();
        }

        public string AsString(string propName, byte[] propValue)
        {
            throw new ArgumentException($"Schema is emtpy.");
        }

        public IList<IKvp> Choices(string propName, IProps props)
        {
            return this.choices;
        }

        public string DisplayName(string propName)
        {
            throw new ArgumentException($"Cannot get name for {propName} of an empty schema");
        }

        public IDictionary<string, string> DisplayNames()
        {
            return this.DisplayNames();
        }

        public IDictionary<string, string> Types()
        {
            return this.types;
        }
    }
}
