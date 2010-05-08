using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;

using Cinch;
using MVVM.Models;
using MVVM.DataAccess;


namespace MVVM.ViewModels
{
    /// <summary>
    /// Provides ALL logic for the AddEditCustomerView
    /// </summary>
    public class AddEditCustomerViewModel : Cinch.ViewModelBase
    {
        #region Data
        private ViewMode currentViewMode = ViewMode.AddMode;
        private CustomerModel currentCustomer;
        private Boolean hasOrders = false;

        private SimpleCommand saveCustomerCommand;
        private SimpleCommand editCustomerCommand;
        private SimpleCommand cancelCustomerCommand;

        private SimpleCommand addOrderCommand;
        private SimpleCommand deleteOrderCommand;
        private SimpleCommand editOrderCommand;

        private ICollectionView customerOrdersView;
        private OrderModel currentCustomerOrder;
        private AddEditOrderViewModel addEditOrderVM;

        //services
        private IMessageBoxService messageBoxService = null;
        private IUIVisualizerService uiVisualizerService = null;

        //background workers
        private BackgroundTaskManager<DispatcherNotifiedObservableCollection<OrderModel>>
            bgWorker = null;

        #endregion

        #region Ctor
        public AddEditCustomerViewModel()
        {
            this.DisplayName = "Add Customer";

            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
                uiVisualizerService = Resolve<IUIVisualizerService>();
            }
            catch
            {
                Logger.Error( "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion

            #region Create Commands
            //Create save customer Command
            saveCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteSaveCustomerCommand,
                ExecuteDelegate = x => ExecuteSaveCustomerCommand()
            };
            //Create edit customer Command
            editCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditCustomerCommand,
                ExecuteDelegate = x => ExecuteEditCustomerCommand()
            };
            //Create cancel customer Command
            cancelCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteCancelCustomerCommand,
                ExecuteDelegate = x => ExecuteCancelCustomerCommand()
            };
            //Add Order to customer Command
            addOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteAddOrderCommand,
                ExecuteDelegate = x => ExecuteAddOrderCommand()
            };
            //Edit Order to customer Command
            editOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditOrderCommand,
                ExecuteDelegate = x => ExecuteEditOrderCommand()
            };
            //Delete Order Command
            deleteOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteDeleteOrderCommand,
                ExecuteDelegate = x => ExecuteDeleteOrderCommand()
            };

            addEditOrderVM = new AddEditOrderViewModel();

            //setup background worker
            SetUpBackgroundWorker();

            #endregion
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Current CustomerModel
        /// </summary>
        static PropertyChangedEventArgs currentCustomerChangeArgs =
            ObservableHelper.CreateArgs<AddEditCustomerViewModel>(x => x.CurrentCustomer);

        public CustomerModel CurrentCustomer
        {
            get { return currentCustomer; }
            set
            {
                currentCustomer = value;
                NotifyPropertyChanged(currentCustomerChangeArgs);
                LazyFetchOrdersForCustomer();
            }
        }

        /// <summary>
        /// Current Customer OrderModel
        /// </summary>
        static PropertyChangedEventArgs currentCustomerOrderChangeArgs =
            ObservableHelper.CreateArgs<AddEditCustomerViewModel>(x => x.CurrentCustomerOrder);

        public OrderModel CurrentCustomerOrder
        {
            get { return currentCustomerOrder; }
            set
            {
                currentCustomerOrder = value;
                addEditOrderVM.CurrentCustomerOrder = currentCustomerOrder;
                NotifyPropertyChanged(currentCustomerOrderChangeArgs);
            }
        }

        /// <summary>
        /// The current ViewMode, when changed will loop
        /// through all nested DataWrapper objects and change
        /// their state also
        /// </summary>
        static PropertyChangedEventArgs currentViewModeChangeArgs =
            ObservableHelper.CreateArgs<AddEditCustomerViewModel>(x => x.CurrentViewMode);

        public ViewMode CurrentViewMode
        {
            get { return currentViewMode; }
            set
            {
                currentViewMode = value;

                switch (currentViewMode)
                {
                    case ViewMode.AddMode:
                        CurrentCustomer = new CustomerModel();
                        this.DisplayName = "Add Customer";
                        break;
                    case ViewMode.EditMode:
                        CurrentCustomer.BeginEdit();
                        this.DisplayName = "Edit Customer";
                        break;
                    case ViewMode.ViewOnlyMode:
                        this.DisplayName = "View Customer";
                        break;
                }

                //Now change all the CurrentCustomer.CachedListOfDataWrappers
                //Which sets all the Cinch.DataWrapper<T>s to the correct IsEditable
                //state based on the new ViewMode applied to the ViewModel
                //we can use the Cinch.DataWrapperHelper class for this
                DataWrapperHelper.SetMode(
                    CurrentCustomer.CachedListOfDataWrappers,
                    currentViewMode);

                NotifyPropertyChanged(currentViewModeChangeArgs);
            }
        }

        /// <summary>
        /// Returns true if the Customer had orders
        /// </summary>
        static PropertyChangedEventArgs hasOrdersChangeArgs =
            ObservableHelper.CreateArgs<AddEditCustomerViewModel>(x => x.HasOrders);

        public Boolean HasOrders
        {
            get { return hasOrders; }
            set
            {
                hasOrders = value;
                NotifyPropertyChanged(hasOrdersChangeArgs);
            }
        }

        /// <summary>
        /// Background worker which lazy fetches 
        /// Customer Orders
        /// </summary>
        static PropertyChangedEventArgs bgWorkerChangeArgs =
            ObservableHelper.CreateArgs<AddEditCustomerViewModel>(x => x.BgWorker);

        public BackgroundTaskManager<DispatcherNotifiedObservableCollection<OrderModel>> BgWorker
        {
            get { return bgWorker; }
            set
            {
                bgWorker = value;
                NotifyPropertyChanged(bgWorkerChangeArgs);
            }
        }

        /// <summary>
        /// SaveCustomerCommand : Saves a Customer if in AddMode
        /// or updates exiting customer in EditMode
        /// </summary>
        public SimpleCommand SaveCustomerCommand
        {
            get { return saveCustomerCommand; }
        }

        /// <summary>
        /// EditCustomerCommand : Puts views data into edit mode
        /// </summary>
        public SimpleCommand EditCustomerCommand
        {
            get { return editCustomerCommand; }
        }

        /// <summary>
        /// cancelCustomerCommand : Cancels the save or the edit
        /// </summary>
        public SimpleCommand CancelCustomerCommand
        {
            get { return cancelCustomerCommand; }
        }

        /// <summary>
        /// addOrderCommand : Adds an Order to a Customer
        /// </summary>
        public SimpleCommand AddOrderCommand
        {
            get { return addOrderCommand; }
        }

        /// <summary>
        /// editOrderCommand : Edit an Order for a Customer
        /// </summary>
        public SimpleCommand EditOrderCommand
        {
            get { return editOrderCommand; }
        }

        /// <summary>
        /// deleteOrderCommand : Deletes an Order
        /// </summary>
        public SimpleCommand DeleteOrderCommand
        {
            get { return deleteOrderCommand; }
        }

        /// <summary>
        /// Returns the bindbable Menu options
        /// </summary>
        public List<WPFMenuItem> OrderMenuOptions
        {
            get
            {
                return CreateMenus();
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Setup backgrounder worker Task/Completion action
        /// to fetch Orders for Customers
        /// </summary>
        private void SetUpBackgroundWorker()
        {
            bgWorker = new BackgroundTaskManager<DispatcherNotifiedObservableCollection<OrderModel>>(
                () =>
                {
                    return new DispatcherNotifiedObservableCollection<OrderModel>(
                        DataAccess.DataService.FetchAllOrders(
                            CurrentCustomer.CustomerId.DataValue).ConvertAll(
                                new Converter<Order, OrderModel>(
                                      OrderModel.OrderToOrderModel)));
                },
                (result) =>
                {

                    CurrentCustomer.Orders = result;
                    if (customerOrdersView != null)
                        customerOrdersView.CurrentChanged -=
                            CustomerOrdersView_CurrentChanged;

                    customerOrdersView =
                        CollectionViewSource.GetDefaultView(CurrentCustomer.Orders);
                    customerOrdersView.CurrentChanged +=
                        CustomerOrdersView_CurrentChanged;
                    customerOrdersView.MoveCurrentToPosition(-1);

                    HasOrders = CurrentCustomer.Orders.Count > 0;
                });
        }


        /// <summary>
        /// Fetches all Orders for customer
        /// </summary>
        private void LazyFetchOrdersForCustomer()
        {
            if (CurrentCustomer != null &&
                CurrentCustomer.CustomerId.DataValue > 0)
            {
                bgWorker.RunBackgroundTask();
            }
        }


        /// <summary>
        /// Is raised whenever a new Customer is selected from the list of Customers
        /// </summary>
        private void CustomerOrdersView_CurrentChanged(object sender, EventArgs e)
        {
            if (customerOrdersView.CurrentItem != null)
            {
                CurrentCustomerOrder = customerOrdersView.CurrentItem as OrderModel;
                addEditOrderVM.CurrentCustomerOrder = CurrentCustomerOrder;
            }
        }

        /// <summary>
        /// Translates a UI CustomerModel into a data layer Customer
        /// </summary>
        /// <param name="uiLayerCustomer">A UI CustomerModel</param>
        /// <returns>A data layer Customer</returns>
        private Customer TranslateUICustomerToDataLayerCustomer(CustomerModel uiLayerCustomer)
        {
            Customer dataLayerCustomer = new Customer();
            dataLayerCustomer.CustomerId = uiLayerCustomer.CustomerId.DataValue;
            dataLayerCustomer.FirstName = uiLayerCustomer.FirstName.DataValue;
            dataLayerCustomer.LastName = uiLayerCustomer.LastName.DataValue;
            dataLayerCustomer.Email = uiLayerCustomer.Email.DataValue;
            dataLayerCustomer.HomePhoneNumber = uiLayerCustomer.HomePhoneNumber.DataValue;
            dataLayerCustomer.MobilePhoneNumber = uiLayerCustomer.MobilePhoneNumber.DataValue;
            dataLayerCustomer.Address1 = uiLayerCustomer.Address1.DataValue;
            dataLayerCustomer.Address2 = uiLayerCustomer.Address2.DataValue;
            dataLayerCustomer.Address3 = uiLayerCustomer.Address3.DataValue;
            return dataLayerCustomer;
        }

        /// <summary>
        /// Creates and returns the menu items
        /// </summary>
        private List<WPFMenuItem> CreateMenus()
        {

            var menu = new List<WPFMenuItem>();

            var miAddOrder = new WPFMenuItem("Add Order");
            miAddOrder.Command = AddOrderCommand;
            menu.Add(miAddOrder);

            var miEditOrder = new WPFMenuItem("Edit Order");
            miEditOrder.Command = EditOrderCommand;
            menu.Add(miEditOrder);

            var miDeleteOrder = new WPFMenuItem("Delete Order");
            miDeleteOrder.Command = DeleteOrderCommand;
            menu.Add(miDeleteOrder);

            return menu;

        }

        /// <summary>
        /// Mediator callback from AddEditOrderViewModel to state that
        /// the user wants to abandon the changes made to the current Order
        /// </summary>
        /// <param name="dummy">Noy used, we simply need to know about the message</param>
        [MediatorMessageSink("EditedOrderSuccessfullyMessage")]
        private void EditedOrderSuccessfullyMessageSink(Boolean dummy)
        {
            CurrentCustomerOrder.EndEdit();
            messageBoxService.ShowInformation("Need to do this, and same for edit");

        }

        /// <summary>
        /// Mediator callback from AddEditOrderViewModel to state that
        /// the user wants to add a new Order
        /// </summary>
        /// <param name="dummy">Noy used, we simply need to know about the message</param>
        [MediatorMessageSink("AddedOrderSuccessfullyMessage")]
        private void AddedOrderSuccessfullyMessageSink(Boolean dummy)
        {
            addEditOrderVM.CloseActivePopUpCommand.Execute(true);
            LazyFetchOrdersForCustomer();
        }
        #endregion

        #region Command Implementation

        #region SaveCustomerCommand
        /// <summary>
        /// Logic to determine if SaveCustomerCommand can execute
        /// </summary>
        private Boolean CanExecuteSaveCustomerCommand
        {
            get
            {
                return CurrentCustomer != null;
            }
        }

        /// <summary>
        /// Executes the SaveCustomerCommand
        /// </summary>
        private void ExecuteSaveCustomerCommand()
        {

            try
            {
                SaveCustomerCommand.CommandSucceeded = false;

                if (!CurrentCustomer.IsValid)
                {
                    messageBoxService.ShowError("There customer is invalid");
                    SaveCustomerCommand.CommandSucceeded = false;
                    return;
                }

                //Customer is valid, so end the edit and try and save/update it
                this.CurrentCustomer.EndEdit();

                switch (currentViewMode)
                {
                    #region AddMode
                    //AddMode
                    case ViewMode.AddMode:
                        Int32 custId =
                            DataService.AddCustomer(
                                TranslateUICustomerToDataLayerCustomer(currentCustomer));

                        if (custId > 0)
                        {
                            this.CurrentCustomer.CustomerId.DataValue = custId;
                            messageBoxService.ShowInformation(
                                "Sucessfully saved customer");
                            this.CurrentViewMode = ViewMode.ViewOnlyMode;
                        }
                        else
                        {
                            messageBoxService.ShowError(
                                "There was a problem saving the customer");
                        }
                        SaveCustomerCommand.CommandSucceeded = true;
                        break;
                    #endregion
                    #region EditMode
                    //EditMode
                    case ViewMode.EditMode:
                        Boolean custUpdated =
                            DataService.UpdateCustomer(
                                TranslateUICustomerToDataLayerCustomer(currentCustomer));

                        if (custUpdated)
                        {
                            messageBoxService.ShowInformation(
                                "Sucessfully updated customer");
                            this.CurrentViewMode = ViewMode.ViewOnlyMode;
                        }
                        else
                        {
                            messageBoxService.ShowError(
                                "There was a problem updating the customer");
                        }
                        SaveCustomerCommand.CommandSucceeded = true;
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.Error( ex);
                messageBoxService.ShowError(
                    "There was a problem saving the customer");
            }
        }
        #endregion

        #region EditCustomerCommand
        /// <summary>
        /// Logic to determine if EditCustomerCommand can execute
        /// </summary>
        private Boolean CanExecuteEditCustomerCommand
        {
            get
            {
                return CurrentCustomer != null &&
                    CurrentCustomer.CustomerId.DataValue != 0 &&
                    currentViewMode == ViewMode.ViewOnlyMode;
            }
        }

        /// <summary>
        /// Executes the EditCustomerCommand
        /// </summary>
        private void ExecuteEditCustomerCommand()
        {
            this.DisplayName = "Edit Customer";
            EditCustomerCommand.CommandSucceeded = false;
            CurrentViewMode = ViewMode.EditMode;
            this.CurrentCustomer.BeginEdit();
            EditCustomerCommand.CommandSucceeded = true;
        }
        #endregion

        #region CancelCustomerCommand
        /// <summary>
        /// Logic to determine if CancelCustomerCommand can execute
        /// </summary>
        private Boolean CanExecuteCancelCustomerCommand
        {
            get
            {
                return CurrentCustomer != null &&
                    CurrentCustomer.CustomerId.DataValue != 0 &&
                    CurrentViewMode == ViewMode.EditMode;
            }
        }

        /// <summary>
        /// Executes the CancelCustomerCommand
        /// </summary>
        private void ExecuteCancelCustomerCommand()
        {
            CancelCustomerCommand.CommandSucceeded = false;
            switch (CurrentViewMode)
            {
                case ViewMode.EditMode:
                    this.CurrentCustomer.CancelEdit();
                    CloseWorkSpaceCommand.Execute(null);
                    CancelCustomerCommand.CommandSucceeded = true;
                    break;
                default:
                    this.CurrentCustomer.CancelEdit();
                    CancelCustomerCommand.CommandSucceeded = true;
                    break;

            }
        }
        #endregion

        #region AddOrderCommand
        /// <summary>
        /// Logic to determine if AddOrderCommand can execute
        /// </summary>
        private Boolean CanExecuteAddOrderCommand
        {
            get
            {
                return CurrentCustomer != null &&
                    CurrentCustomer.CustomerId.DataValue != 0;
            }
        }

        /// <summary>
        /// Executes the AddOrderCommand
        /// </summary>
        private void ExecuteAddOrderCommand()
        {
            AddOrderCommand.CommandSucceeded = false;
            addEditOrderVM.CurrentViewMode = ViewMode.AddMode;
            addEditOrderVM.CurrentCustomer = CurrentCustomer;
            bool? result = uiVisualizerService.ShowDialog("AddEditOrderPopup", addEditOrderVM);

            if (result.HasValue && result.Value)
            {
                CloseActivePopUpCommand.Execute(true);
            }
            AddOrderCommand.CommandSucceeded = true;
        }
        #endregion

        #region EditOrderCommand
        /// <summary>
        /// Logic to determine if EditOrderCommand can execute
        /// </summary>
        private Boolean CanExecuteEditOrderCommand
        {
            get
            {
                return CurrentCustomerOrder != null &&
                    CurrentCustomerOrder.OrderId.DataValue != 0;
            }
        }

        /// <summary>
        /// Executes the EditOrderCommand
        /// </summary>
        private void ExecuteEditOrderCommand()
        {
            EditOrderCommand.CommandSucceeded = false;
            addEditOrderVM.CurrentViewMode = ViewMode.EditMode;
            CurrentCustomerOrder.BeginEdit();
            addEditOrderVM.CurrentCustomer = CurrentCustomer;
            bool? result = uiVisualizerService.ShowDialog("AddEditOrderPopup", addEditOrderVM);

            if (result.HasValue && result.Value)
            {
                CloseActivePopUpCommand.Execute(true);
            }
            EditOrderCommand.CommandSucceeded = true;
        }
        #endregion

        #region DeleteOrderCommand
        /// <summary>
        /// Logic to determine if DeleteOrderCommand can execute
        /// </summary>
        private Boolean CanExecuteDeleteOrderCommand
        {
            get
            {
                return CurrentCustomerOrder != null &&
                    CurrentCustomerOrder.OrderId.DataValue != 0;
            }
        }

        /// <summary>
        /// Executes the DeleteOrderCommand
        /// </summary>
        private void ExecuteDeleteOrderCommand()
        {
            DeleteOrderCommand.CommandSucceeded = false;
            if (messageBoxService.ShowYesNo("Are you sure you want to remove this Order",
                CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                try
                {
                    if (DataAccess.DataService.DeleteOrder(
                        CurrentCustomerOrder.OrderId.DataValue))
                    {
                        CurrentCustomerOrder = null;
                        LazyFetchOrdersForCustomer();
                    }
                }
                catch
                {
                    messageBoxService.ShowError("There was a problem revoming the Order");
                }
            }
            DeleteOrderCommand.CommandSucceeded = true;
        }
        #endregion

        #endregion
    }
}
