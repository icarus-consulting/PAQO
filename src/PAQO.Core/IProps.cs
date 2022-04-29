using System.Collections.Generic;

namespace PAQO.Core
{
    /// <summary>
    /// props of an element.
    /// </summary>
    public interface IProps
    {
        /// <summary>
        /// All props.
        /// </summary>
        IList<IProp> All();

        /// <summary>
        /// Test if given prop with value is contained.
        /// </summary>
        bool Contains(IProp prop);

        /// <summary>
        /// Test if given prop is contained.
        /// </summary>
        bool Contains(string propName);

        /// <summary>
        /// Specific prop.
        /// </summary>
        IProp Prop(string propName);

        /// <summary>
        /// Content of a prop.
        /// </summary>
        byte[] Content(string propName);
    }
}
