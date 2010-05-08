using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace CinchCodeGen
{
    [ValueConversion(typeof(Boolean),typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Public Properties
        public Boolean Inverted { get; set; }
        public Boolean Not { get; set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Converts Visibility To Bool
        /// </summary>
        /// <param name="value">Visibility</param>
        /// <returns>Bool</returns>
        private object VisibilityToBool(object value)
        {
            if (!(value is Visibility))

                return DependencyProperty.UnsetValue;
            return (((Visibility)value) == Visibility.Visible) ^ Not;

        }


        /// <summary>
        /// Converts Bool To Visibility
        /// </summary>
        /// <param name="value">Bool</param>
        /// <returns>Visibility</returns>
        private object BoolToVisibility(object value)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            return ((bool)value ^ Not) ? Visibility.Visible : Visibility.Collapsed;

        }
        #endregion

        #region IValueConverter
        /// <summary>
        /// Converts BoolToVisibility or VisibilityToBool dependent on
        /// Inverted flag
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? BoolToVisibility(value) : VisibilityToBool(value);
        }


        /// <summary>
        /// Converts VisibilityToBool or BoolToVisibility dependent on
        /// Inverted flag
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? VisibilityToBool(value) : BoolToVisibility(value);

        }
        #endregion
    }
}
