using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel.Composition;
using MEFedMVVM.ViewModelLocator;


namespace Cinch
{
    /// <summary>
    /// This class implements the IMessageBoxService for WPF/SL purposes.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportService(ServiceType.Both, typeof(IMessageBoxService))]
    public class SLMessageBoxService : IMessageBoxService
    {
        #region IMessageBoxService Members

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowError(string message)
        {
            ShowMessage(message, "Error");
        }

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowInformation(string message)
        {
            ShowMessage(message, "Information");
        }

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowWarning(string message)
        {
            ShowMessage(message, "Warning");
        }

        /// <summary>
        /// Displays a OK/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message)
        {
            return ShowQuestionWithButton(message, CustomDialogButtons.OKCancel);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Shows a standard System.Windows.MessageBox using the parameters requested
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="heading">The heading to be displayed</param>
        private void ShowMessage(string message, string heading)
        {
            MessageBox.Show(message, heading, MessageBoxButton.OK);
        }


        /// <summary>
        /// Shows a standard System.Windows.MessageBox using the parameters requested
        /// but will return a translated result to enable adhere to the IMessageBoxService
        /// implementation required. 
        /// 
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="button"></param>
        /// <returns>CustomDialogResults results to use</returns>
        private CustomDialogResults ShowQuestionWithButton(string message,
            CustomDialogButtons button)
        {
            MessageBoxResult result = MessageBox.Show(message, "Please confirm...",
                GetButton(button));
            return GetResult(result);
        }




        /// <summary>
        /// Translates a CustomDialogButtons into a standard WPF System.Windows.MessageBox MessageBoxButton.
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="btn">The button type to be displayed.</param>
        /// <returns>A standard WPF System.Windows.MessageBox MessageBoxButton</returns>
        private MessageBoxButton GetButton(CustomDialogButtons btn)
        {
            MessageBoxButton button = MessageBoxButton.OK;

            switch (btn)
            {
                case CustomDialogButtons.OK:
                    button = MessageBoxButton.OK;
                    break;
                case CustomDialogButtons.OKCancel:
                    button = MessageBoxButton.OKCancel;
                    break;
            }
            return button;
        }


        /// <summary>
        /// Translates a standard WPF System.Windows.MessageBox MessageBoxResult into a
        /// CustomDialogIcons.
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="result">The standard WPF System.Windows.MessageBox MessageBoxResult</param>
        /// <returns>CustomDialogResults results to use</returns>
        private CustomDialogResults GetResult(MessageBoxResult result)
        {
            CustomDialogResults customDialogResults = CustomDialogResults.None;

            switch (result)
            {
                case MessageBoxResult.Cancel:
                    customDialogResults = CustomDialogResults.Cancel;
                    break;
                case MessageBoxResult.No:
                    customDialogResults = CustomDialogResults.No;
                    break;
                case MessageBoxResult.None:
                    customDialogResults = CustomDialogResults.None;
                    break;
                case MessageBoxResult.OK:
                    customDialogResults = CustomDialogResults.OK;
                    break;
                case MessageBoxResult.Yes:
                    customDialogResults = CustomDialogResults.Yes;
                    break;
            }
            return customDialogResults;
        }


        #endregion

    }
}
