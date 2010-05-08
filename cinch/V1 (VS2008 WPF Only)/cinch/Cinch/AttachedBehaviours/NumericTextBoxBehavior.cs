using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Text.RegularExpressions;


/// <summary>
/// This forces a TextBoxBase control to be numeric-entry only
/// By using an attached behaviour
/// </summary>
namespace Cinch
{
    /// <summary>
    /// This forces a TextBoxBase control to be numeric-entry only
    /// </summary>
    /// <example>
    /// <![CDATA[  <TextBox Cinch:NumericTextBoxBehavior.IsEnabled="True" />  ]]>
    /// </example>
    public static class NumericTextBoxBehavior
    {
        #region IsEnabled DP
        /// <summary>
        /// Dependency Property for turning on numeric behavior in a TextBox.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled",
                typeof(bool), typeof(NumericTextBoxBehavior),
                    new UIPropertyMetadata(false, OnEnabledStateChanged));

        /// <summary>
        /// Attached Property getter for the IsEnabled property.
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <returns>Current property value</returns>
        public static bool GetIsEnabled(DependencyObject source)
        {
            return (bool)source.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Attached Property setter for the IsEnabled property.
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="value">Value to set on the object</param>
        public static void SetIsEnabled(DependencyObject source, bool value)
        {
            source.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// This is the property changed handler for the IsEnabled property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnEnabledStateChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            tb.PreviewTextInput -= tbb_PreviewTextInput;
            DataObject.RemovePastingHandler(tb, OnClipboardPaste);

            bool b = ((e.NewValue != null && e.NewValue.GetType() == typeof(bool))) ?
                (bool)e.NewValue : false;
            if (b)
            {
                tb.PreviewTextInput += tbb_PreviewTextInput;
                DataObject.AddPastingHandler(tb, OnClipboardPaste);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method handles paste and drag/drop events onto the TextBox.  It restricts the character
        /// set to numerics and ensures we have consistent behavior.
        /// </summary>
        /// <param name="sender">TextBox sender</param>
        /// <param name="e">EventArgs</param>
        private static void OnClipboardPaste(object sender, DataObjectPastingEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = e.SourceDataObject.GetData(e.FormatToApply) as string;

            if (tb != null && !string.IsNullOrEmpty(text) && !Validate(tb, text))
                e.CancelCommand();
        }

        /// <summary>
        /// This checks if the resulting string will match the regex expression
        /// </summary>
        static void tbb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null && !Validate(tb, e.Text))
                e.Handled = true;
        }

        #endregion

        private static bool Validate(TextBox tb, string newContent)
        {
            string testString = string.Empty;
            // replace selection with new text.
            if (!string.IsNullOrEmpty(tb.SelectedText))
            {
                string pre = tb.Text.Substring(0, tb.SelectionStart);
                string after = tb.Text.Substring(tb.SelectionStart + tb.SelectionLength, tb.Text.Length - (tb.SelectionStart + tb.SelectionLength));
                testString = pre + newContent + after;
            }
            else
            {
                string pre = tb.Text.Substring(0, tb.CaretIndex);
                string after = tb.Text.Substring(tb.CaretIndex, tb.Text.Length - tb.CaretIndex);
                testString = pre + newContent + after;
            }

            Regex regExpr = new Regex(@"^([-+]?)(\d*)([,.]?)(\d*)$");
            if (regExpr.IsMatch(testString))
                return true;

            return false;
        }
    }
}