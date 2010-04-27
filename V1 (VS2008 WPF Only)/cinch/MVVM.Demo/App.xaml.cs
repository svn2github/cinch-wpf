using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace MVVM.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// This class also contains logic to handle an unhandled
    /// Exceptions
    /// </summary>
    public partial class App : Application
    {
        #region Override
        /// <summary>
        /// On activate hook up a handler for unhandled Exceptions
        /// </summary>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// On TextBox_GotFocus select all the Text
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Dispatcher.BeginInvoke((Action)delegate
            {
                txt.SelectAll();
                txt.ReleaseMouseCapture();
            },
            DispatcherPriority.Normal);
        }


        /// <summary>
        /// Occurs when an un handled Exception occurs for the Dispatcher
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, 
            DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            MessageBox.Show("A fatal error occurred " + ex.Message);
            e.Handled = true;
            Environment.Exit(-1);
        }
        #endregion

    }
}
