using System;

using Cinch;

namespace MVVM.ViewModels
{
    /// <summary>
    /// Holds 2 ICommand(s) that are used to tell
    /// the MainWindowViewModel to open/show new
    /// Views. 
    /// - AddNewCustomer
    /// - SearchCustomers
    /// 
    /// Provides ALL logic for the StartPageView
    /// </summary>
    public class StartPageViewModel : Cinch.ViewModelBase
    {
        #region Data
        private SimpleCommand addCustomerCommand;
        private SimpleCommand searchCustomersCommand;
        #endregion

        #region Ctor
        public StartPageViewModel()
        {
            #region Create Commands

            //Create Add Cusomer command
            addCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ExecuteAddCustomerCommand()
            };

            //Create Search Customers command
            searchCustomersCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ExecuteSearchCusomersCommand()
            };

            #endregion

            this.DisplayName = "Home";
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// AddCustomerCommand : Add customer command
        /// </summary>
        public SimpleCommand AddCustomerCommand
        {
            get { return addCustomerCommand; }
        }

        /// <summary>
        /// SearchCustomersCommand : Search customers command
        /// </summary>
        public SimpleCommand SearchCustomersCommand
        {
            get { return searchCustomersCommand; }
        }
        #endregion

        #region Command Implementation

        #region AddCustomerCommand
        /// <summary>
        /// Executes the AddCustomerCommand : Tells 
        /// MainWindowViewModel to add a new AddEditCustomerViewModel
        /// </summary>
        private void ExecuteAddCustomerCommand()
        {
            AddCustomerCommand.CommandSucceeded = false;

            //Use the Mediator to send a Message to MainWindowViewModel to add a new 
            //Workspace item
            Mediator.NotifyColleagues<Boolean>("AddCustomerMessage", true);

            AddCustomerCommand.CommandSucceeded = true;
        }
        #endregion

        #region SearchCustomersCommand
        /// <summary>
        /// Executes the SearchCustomersCommand : Tells 
        /// MainWindowViewModel to add a new SearchCustomersViewModel
        /// </summary>
        private void ExecuteSearchCusomersCommand()
        {
            SearchCustomersCommand.CommandSucceeded = false;

            //Use the Mediator to send a Message to MainWindowViewModel to add a new 
            //Workspace item
            Mediator.NotifyColleagues<Boolean>("SearchCustomersMessage", true);

            SearchCustomersCommand.CommandSucceeded = true;
        }
        #endregion

        #endregion
    }
}
