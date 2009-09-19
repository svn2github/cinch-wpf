using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinchCodeGen
{
    /// <summary>
    /// A simple C# Syntax Checker, which is used in the
    /// <c>SyntaxRichTextBox</c>
    /// </summary>
    public class CSharpSyntaxChecker : ISyntaxChecker
    {
        #region Data
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        #endregion

        #region ctor
        static CSharpSyntaxChecker()
        {
            string[] strs = {
                "bool",
                "class",
                "char",
                "decimal",
                "float",
                "get",
                "int",
                "interface",
                "internal",
                "new",
                "namespace",
                "object",
                "partial",
                "public",
                "private",
                "protected",
                "#region",
                "#endregion",
                "struct",
                "set",
                "static",
                "string",
                "void",
                "using"

            };
            tags = new List<string>(strs);

            char[] chrs = {
                '.',
                ')',
                '(',
                '[',
                ']',
                '>',
                '<',
                ':',
                ';',
                '\n',
                '\t'
            };
            specials = new List<char>(chrs);

         }
        #endregion

        #region ISyntaxChecker implementation
        /// <summary>
        /// Gets special chars
        /// </summary>
        public List<char> GetSpecials
        {
            get { return specials; }
        }

        /// <summary>
        /// Gets known tags list
        /// </summary>
        public List<string> GetTags
        {
            get { return tags; }
        }

        /// <summary>
        /// Tests a string to see if it is a known tag
        /// </summary>
        public bool IsKnownTag(string tag)
        {
            return tags.Contains(tag);
        }
        #endregion
    }
}
