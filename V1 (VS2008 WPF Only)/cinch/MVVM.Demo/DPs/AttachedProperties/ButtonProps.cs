using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MVVM.Demo
{
    /// <summary>
    /// Holds attached properties that can be used for Button types
    /// </summary>
    public class ButtonProps
    {
        #region ImageUrl

        /// <summary>
        /// ImageUrl Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.RegisterAttached("ImageUrl", 
                typeof(String), typeof(ButtonProps),
                    new FrameworkPropertyMetadata((String)String.Empty));

        /// <summary>
        /// Gets the ImageUrl property.  
        /// </summary>
        public static String GetImageUrl(DependencyObject d)
        {
            return (String)d.GetValue(ImageUrlProperty);
        }

        /// <summary>
        /// Sets the ImageUrl property.  
        /// </summary>
        public static void SetImageUrl(DependencyObject d, String value)
        {
            d.SetValue(ImageUrlProperty, value);
        }

        #endregion
    }
}
