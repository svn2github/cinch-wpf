using System;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// A simple RegEx rule may be used to validate objects
/// that inherit from <see cref="Cinch.ValidatingObject">
/// Cinch.ValidatingObject</see>
/// </summary>
namespace Cinch
{
    /// <summary>
    /// A class to define a RegEx rule, using a delegate for validation.
    /// </summary>
    public class RegexRule : Rule
    {
        #region Data
        private string regex;
        private RegexOptions regexOptions;
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        public RegexRule(string propertyName, string description, string regex, RegexOptions regexOptions)
            : base(propertyName, description)
        {
            this.regex = regex;
            this.regexOptions = regexOptions;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RegexRule(string propertyName, string description, string regex)
          : this(propertyName, description, regex, RegexOptions.None)
        {
        }
        #endregion

        #region Overrides
        public override bool ValidateRule(Object domainObject)
        {
            PropertyInfo pi = domainObject.GetType().GetProperty(this.PropertyName);
            string value = pi.GetValue(domainObject, null) as string;
            if (!string.IsNullOrEmpty(value))
            {
                Match m = Regex.Match(value, this.regex, this.regexOptions);
                return !m.Success;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
