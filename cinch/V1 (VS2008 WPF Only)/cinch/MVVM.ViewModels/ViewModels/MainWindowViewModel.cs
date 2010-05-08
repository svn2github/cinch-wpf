using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using Cinch;
using MVVM.Models;


namespace MVVM.ViewModels
{

    public delegate void DummyDelegate(Boolean dummy);

    /// <summary>
    /// Manages a list of <see cref="MVVM.ViewModels.ViewModelBase">
    /// MVVM.ViewModels.ViewModelBase</see> derived ViewModels.
    /// The MainWindowView binds to this list of  WorkspaceViewModels
    /// and displays them in a TabControl
    /// 
    /// This ViewModel also gets called back using the Mediator from
    /// the <see cref="MVVM.ViewModels.StartPageViewModel">
    /// MVVM.ViewModels.StartPageViewModel</see> and the
    /// <see cref="MVVM.ViewModels.SearchCustomersViewModel">
    /// MVVM.ViewModels.SearchCustomersViewModel</see>
    /// </summary>
    public class MainWindowViewModel : Cinch.ViewModelBase
    {
        #region Data
        private ObservableCollection<ViewModelBase> workspaces;
        private SimpleCommand exitApplicationCommand;
        private SimpleCommand addCustomerCommand;
        private SimpleCommand searchCustomersCommand;

        //services
        IMessageBoxService messageBoxService = null;
        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            Workspaces = new ObservableCollection<ViewModelBase>();
            Workspaces.CollectionChanged += this.OnWorkspacesChanged;
            StartPageViewModel startPageViewModel = new StartPageViewModel();
            startPageViewModel.IsCloseable = false;
            Workspaces.Add(startPageViewModel);

            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
            }
            catch
            {
                Logger.Error( "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion

            #region Create Commands
            //Create exit application command
            exitApplicationCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ExecuteExitApplicationCommand()
            };

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
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks to see if there is already a workspace item
        /// of the same type as T
        /// </summary>
        /// <typeparam name="T">The type to check for</typeparam>
        /// <returns>True if there is no other workspace item
        /// with the same type as T</returns>
        private Boolean IsNewWorkSpaceItemAllowed<T>()
        {
            foreach (var item in workspaces)
            {
                if (item.GetType().Equals(typeof(T)))
                {
                    return false;
                }
            }
            return true;
        }
      

        /// <summary>
        /// Mediator callback from SearchCustomersViewModel
        /// </summary>
        /// <param name="editableCustomer">The Customer to edit</param>
        [MediatorMessageSink("DeleteCustomerMessage")]
        private void DeleteCustomerMessageSink(CustomerModel customerToDelete)
        {
            bool okToDeleteCustomer = true;

            foreach (var item in workspaces)
            {
                if (item.GetType().Equals(typeof(AddEditCustomerViewModel)))
                {
                    AddEditCustomerViewModel vm = item as AddEditCustomerViewModel;

                    if (vm.CurrentCustomer.CustomerId.DataValue == customerToDelete.CustomerId.DataValue)
                    {
                        okToDeleteCustomer = false;
                        break;
                    }
                }
            }
            Mediator.NotifyColleagues<Boolean>("OkToDeleteCustomerMessage", okToDeleteCustomer);
        }

        
        /// <summary>
        /// Mediator callback from StartPageViewModel
        /// </summary>
        /// <param name="dummy">Dummy not needed</param>
        [MediatorMessageSink("AddCustomerMessage")]
        private void AddCustomerMessageSink(Boolean dummy)
        {
            AddCustomerCommand.Execute(null);
        }

        /// <summary>
        /// Mediator callback from StartPageViewModel
        /// </summary>
        /// <param name="dummy">Dummy not needed</param>
        [MediatorMessageSink("SearchCustomersMessage")]
        private void SearchCustomersMessageSink(Boolean dummy)
        {
            SearchCustomersCommand.Execute(null);
        }

        /// <summary>
        /// Mediator callback from SearchCustomersViewModel
        /// </summary>
        /// <param name="editableCustomer">The Customer to edit</param>
        [MediatorMessageSink("EditCustomerMessage")]
        private void EditCustomerMessageSink(CustomerModel editableCustomer)
        {
            if (!IsNewWorkSpaceItemAllowed<AddEditCustomerViewModel>())
            {
                messageBoxService.ShowWarning(
                          "There is already an Add/Edit View open\r\n\r\n" +
                          "This application only allows 1 active Add/Edit view\r\n" +
                          "to be opened at 1 time");
                return;
            }

            AddEditCustomerViewModel addEditCustomerViewModel = 
                new AddEditCustomerViewModel();
            addEditCustomerViewModel.IsCloseable = true;
            addEditCustomerViewModel.CurrentCustomer = editableCustomer;
            addEditCustomerViewModel.CurrentViewMode = ViewMode.EditMode;
            Workspaces.Add(addEditCustomerViewModel);
            this.SetActiveWorkspace(addEditCustomerViewModel);

        }


        /// <summary>
        /// Creates and returns the menu items
        /// </summary>
        private List<WPFMenuItem> CreateMenus()
        {

            var menu = new List<WPFMenuItem>();

            //create the File Menu
            var miFile = new WPFMenuItem("File");

            var miExit = new WPFMenuItem("Exit") 
                { IconUrl = @"..\Images\Exit.png" };
            miExit.Command = ExitApplicationCommand;
            miFile.Children.Add(miExit);
            menu.Add(miFile);

            //create the Actions Menu
            var miActions = new WPFMenuItem("Actions");

            var miAddCustomer = new WPFMenuItem("Add Customer") 
                { IconUrl = @"..\Images\Customers.png" };
            miAddCustomer.Command = AddCustomerCommand;
            miActions.Children.Add(miAddCustomer);

            var miSearchCustomers = new WPFMenuItem("Search Customers") 
                { IconUrl = @"..\Images\Search.png" };
            miSearchCustomers.Command = SearchCustomersCommand;
            miActions.Children.Add(miSearchCustomers);

            menu.Add(miActions);

            
            return menu;

        }
 

        /// <summary>
        /// If we get a request to add a new Workspace, add a new WorkSpace to the 
        /// collection and hook up the CloseWorkSpace event in a weak manner
        /// </summary>
        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (ViewModelBase workspace in e.NewItems)
                    workspace.CloseWorkSpace +=
                         new EventHandler<EventArgs>(OnCloseWorkSpace).
                             MakeWeak(eh => workspace.CloseWorkSpace -= eh);
        }

        /// <summary>
        /// If we get a request to close a new Workspace, remove the WorkSpace from the 
        /// collection
        /// </summary>
        private void OnCloseWorkSpace(object sender, EventArgs e)
        {
            ViewModelBase workspace = sender as ViewModelBase;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }


        /// <summary>
        /// Sets a ViewModel to be active, which for the View equates
        /// to selected Tab
        /// </summary>
        /// <param name="workspace">workspace to activate</param>
        private void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = 
                CollectionViewSource.GetDefaultView(this.Workspaces);

            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The active workspace ViewModels
        /// </summary>
        static PropertyChangedEventArgs workspacesChangeArgs =
            ObservableHelper.CreateArgs<MainWindowViewModel>(x => x.Workspaces);

        public ObservableCollection<ViewModelBase> Workspaces
        {
            get { return workspaces; }
            set
            {
                if (workspaces == null)
                {
                    workspaces = value;
                    NotifyPropertyChanged(workspacesChangeArgs);
                }
            }
        }

        /// <summary>
        /// Returns the bindbable Menu options
        /// </summary>
        public List<WPFMenuItem> MenuOptions
        {
            get
            {
                return CreateMenus();
            }
        }

        /// <summary>
        /// ExitApplicationCommand : Exit the application command
        /// </summary>
        public SimpleCommand ExitApplicationCommand
        {
            get { return exitApplicationCommand; }
        }

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

        #region Command Implementations

        #region ExitApplicationCommand
        /// <summary>
        /// Executes the ExitApplicationCommand
        /// </summary>
        private void ExecuteExitApplicationCommand()
        {
            ExitApplicationCommand.CommandSucceeded = false;
            if (messageBoxService.ShowYesNo(
                "Would you like to exit application",
                CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                ExitApplicationCommand.CommandSucceeded = true;
                Application.Current.Shutdown(0);
            }
        }
        #endregion

        #region AddCustomerCommand
        /// <summary>
        /// Executes the AddCustomerCommand
        /// </summary>
        private void ExecuteAddCustomerCommand()
        {
            AddCustomerCommand.CommandSucceeded = false;

            if (!IsNewWorkSpaceItemAllowed<AddEditCustomerViewModel>())
            {
                messageBoxService.ShowWarning(
                          "There is already an Add/Edit View open\r\n\r\n" +
                          "This application only allows 1 active Add/Edit view\r\n" +
                          "to be opened at 1 time");
                return;
            }

            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();
            addEditCustomerViewModel.IsCloseable = true;
            addEditCustomerViewModel.CurrentViewMode = ViewMode.AddMode;
            Workspaces.Add(addEditCustomerViewModel);
            this.SetActiveWorkspace(addEditCustomerViewModel);
            AddCustomerCommand.CommandSucceeded = true;
        }
        #endregion

        #region SearchCustomersCommand
        /// <summary>
        /// Executes the SearchCustomersCommand
        /// </summary>
        private void ExecuteSearchCusomersCommand()
        {
            if (!IsNewWorkSpaceItemAllowed<SearchCustomersViewModel>())
            {
                messageBoxService.ShowWarning(
                          "There is already an Search open\r\n\r\n" +
                          "This application only allows 1 active Search view\r\n" +
                          "to be opened at 1 time");
                return;
            }


            SearchCustomersCommand.CommandSucceeded = false;
            SearchCustomersViewModel searchCustomersViewModel =
                new SearchCustomersViewModel();
            searchCustomersViewModel.IsCloseable = true;
            Workspaces.Add(searchCustomersViewModel);

            this.SetActiveWorkspace(searchCustomersViewModel);
            SearchCustomersCommand.CommandSucceeded = true;
        }
        #endregion

        #endregion
    }
}
