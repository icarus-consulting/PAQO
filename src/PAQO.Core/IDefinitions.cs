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
using Yaapii.Atoms;

namespace PAQO.Core.Schema
{
    /// <summary>
    /// Definitions of types and dependant choices in a schema.
    /// </summary>
    public interface IDefinitions
    {
        /// <summary>
        /// Types of a element's props.
        /// </summary>
        IDictionary<string, string> Types();

        /// <summary>
        /// Choices for an option prop at a program.
        /// </summary>
        IList<IKvp> Choices(string propName, IProps props);

        /// <summary>
        /// Convert a prop to string.
        /// </summary>
        /// <param name="propName"></param>
        string AsString(string propName, byte[] propValue);

        /// <summary>
        /// Display names of the prop
        /// </summary>
        IDictionary<string, string> DisplayNames();
    }
}
