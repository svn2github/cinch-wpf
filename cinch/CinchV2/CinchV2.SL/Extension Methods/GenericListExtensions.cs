using System;
using System.Collections.Generic;

namespace Cinch
{
    /// <summary>
    /// Defines extension methods on the <see cref="List{T}"/> class.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Removes the all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the List's items.</typeparam>
        /// <param name="list"><see langword="this"/>.</param>
        /// <param name="filter">The delegate that defines the conditions of the elements to remove.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void RemoveAll<T>(this List<T> list, Func<T, bool> filter)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (filter(list[i]))
                {
                    list.Remove(list[i]);
                }
            }
        }
    }

}