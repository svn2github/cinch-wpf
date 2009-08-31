using System;
using System.Collections.Generic;
using System.Text;
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
        private string _regex;
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        public RegexRule(string propertyName, string description, string regex)
            : base(propertyName, description)
        {
            _regex = regex;
        }
        #endregion

        #region Overrides
        public override bool ValidateRule(Object domainObject)
        {
            PropertyInfo pi = domainObject.GetType().GetProperty(this.PropertyName);
            Match m = Regex.Match(pi.GetValue(domainObject, null).ToString(), _regex);
            if (m.Success)
            {
                return true;
            }
            else return false;
        }
        #endregion
    }
}
