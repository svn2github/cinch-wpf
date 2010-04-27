using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Cinch;

namespace MVVM.ViewModels
{
    /// <summary>
    /// Reflection helper methods used by 
    /// SearchCustomersViewModel
    /// </summary>
    public static class ReflectionHelper
    {
        #region Public Methods
        /// <summary>
        /// Gets generic parameter name for Type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Name of generic parameter type</returns>
        public static string GetGenericsForType(Type t)
        {
            string name = "";
            if (!t.GetType().IsGenericType)
            {
                //see if there is a ' char, which there is for
                //generic types
                int idx = t.Name.IndexOfAny(new char[] { '`', '\'' });
                if (idx >= 0)
                {
                    name = t.Name.Substring(0, idx);
                    //get the generic arguments
                    Type[] genTypes = t.GetGenericArguments();
                    //and build the list of types for the result string
                    if (genTypes.Length == 1)
                    {
                        name += "<" + GetGenericsForType(genTypes[0]) + ">";
                    }
                    else
                    {
                        name += "<";
                        foreach (Type gt in genTypes)
                        {
                            name += GetGenericsForType(gt) + ", ";
                        }
                        if (name.LastIndexOf(",") > 0)
                        {
                            name = name.Substring(0,
                                name.LastIndexOf(","));
                        }
                        name += ">";
                    }
                }
                else
                {
                    name = t.Name;
                }
                return name;
            }
            else
            {
                return t.Name;
            }
        }


        #endregion
    }
}
