using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinchCodeGen
{
    /// <summary>
    /// A syntax highlighted interface that can be used by possible
    /// SyntaxChecker which may then be used within the <c>SyntaxRichCheckBox</c>
    /// </summary>
    public interface ISyntaxChecker
    {
        #region Properties
        /// <summary>
        /// Gets special chars
        /// </summary>
        List<Char> GetSpecials { get; }

        /// <summary>
        /// Gets known tags list
        /// </summary>
        List<String> GetTags { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Tests a string to see if it is a known tag
        /// </summary>
        Boolean IsKnownTag(string tag);
        #endregion
    }
}
