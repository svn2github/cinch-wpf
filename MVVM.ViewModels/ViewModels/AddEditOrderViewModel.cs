using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;

using Cinch;
using MVVM.Models;
using MVVM.DataAccess;


namespace MVVM.ViewModels
{
    /// <summary>
    /// Provides ALL logic for the AddEditOrderView
    /// </summary>
    public class AddEditOrderViewModel : Cinch.ViewModelBase
    {
        #region Data
        private ViewMode currentViewMode = ViewMode.AddMode;

        private OrderModel currentCustomerOrder;
        private SimpleCommand saveOrderCommand;
        private SimpleCommand cancelOrderCommand;
        private SimpleCommand editOrderCommand;


        private CustomerModel currentCustomer;
        private List<ProductModel> products;
        private ICollectionView productsCV;
        private ProductModel currentProduct;

        //services
        IMessageBoxService messageBoxService = null;
        #endregion

        #region Ctor
        public AddEditOrderViewModel()
        {
            this.DisplayName = "Customer Orders";

            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
            }
            catch
            {
                Logger.Log(LogType.Error, "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion



            //Save Order to customer Command
            saveOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteSaveOrderCommand,
                ExecuteDelegate = x => ExecuteSaveOrderCommand()
            };
            //Edit Order
            editOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditOrderCommand,
                ExecuteDelegate = x => ExecuteEditOrderCommand()
            };
            //Cancel Edit
            cancelOrderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteCancelOrderCommand,
                ExecuteDelegate = x => ExecuteCancelOrderCommand()
            };

            try
            {
                //fetch all Products
                Products =
                    DataAccess.DataService.FetchAllProducts().ConvertAll(
                    new Converter<Product, ProductModel>(ProductModel.ProductToProductModel));
                productsCV = CollectionViewSource.GetDefaultView(Products);
                productsCV.CurrentChanged += ProductsCV_CurrentChanged;
                productsCV.MoveCurrentToPosition(-1);

            }
            catch
            {
                messageBoxService.ShowError("There was a problem fetching the products");
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The current ViewMode, when changed will loop
        /// through all nested DataWrapper objects and change
        /// their state also
        /// </summary>
        static PropertyChangedEventArgs currentViewModeChangeArgs =
            ObservableHelper.CreateArgs<AddEditOrderViewModel>(x => x.CurrentViewMode);

        public ViewMode CurrentViewMode
        {
            get { return currentViewMode; }
            set
            {
                currentViewMode = value;

                switch (currentViewMode)
                {
                    case ViewMode.AddMode:
                        CurrentCustomerOrder = new OrderModel();
                        this.DisplayName = "Add Order";
                        break;
                    case ViewMode.EditMode:
                        CurrentCustomerOrder.BeginEdit();
                        this.DisplayName = "Edit Order";
                        break;
                    case ViewMode.ViewOnlyMode:
                        this.DisplayName = "View Order";
                        break;
                }

                //Now change all the  CurrentCustomerOrder.CachedListOfDataWrappers
                //Which sets all the Cinch.DataWrapper<T>s to the correct IsEditable
                //state based on the new ViewMode applied to the ViewModel
                //we can use the Cinch.DataWrapperHelper class for this
                DataWrapperHelper.SetMode(
                    CurrentCustomerOrder.CachedListOfDataWrappers,
                    currentViewMode);

                NotifyPropertyChanged(currentViewModeChangeArgs);
            }
        }

        /// <summary>
        /// Current Customer OrderModel
        /// </summary>
        static PropertyChangedEventArgs currentCustomerOrderChangeArgs =
            ObservableHelper.CreateArgs<AddEditOrderViewModel>(x => x.CurrentCustomerOrder);

        public OrderModel CurrentCustomerOrder
        {
            get { return currentCustomerOrder; }
            set
            {
                currentCustomerOrder = value;
                if (currentCustomerOrder != null)
                {
                    if (currentCustomerOrder.ProductId.DataValue > 0)
                    {
                        ProductModel prod = this.Products.Where(p => p.ProductId ==
                            currentCustomerOrder.ProductId.DataValue).Single();
                        productsCV.MoveCurrentTo(prod);
                    }
                }
                NotifyPropertyChanged(currentCustomerOrderChangeArgs);
            }
        }

        /// <summary>
        /// CurrentCustomer that order is assigned to
        /// </summary>
        static PropertyChangedEventArgs currentCustomerChangeArgs =
            ObservableHelper.CreateArgs<AddEditOrderViewModel>(x => x.CurrentCustomer);

        public CustomerModel CurrentCustomer
        {
            get { return currentCustomer; }
            set
            {
                currentCustomer = value;
                NotifyPropertyChanged(currentCustomerChangeArgs);
            }
        }

        /// <summary>
        /// List of Prducts, order must be a valid product
        /// </summary>
        static PropertyChangedEventArgs productsChangeArgs =
            ObservableHelper.CreateArgs<AddEditOrderViewModel>(x => x.Products);

        public List<ProductModel> Products
        {
            get { return products; }
            set
            {
                products = value;
                NotifyPropertyChanged(productsChangeArgs);
            }
        }

        /// <summary>
        /// CurrentProduct that order contains
        /// </summary>
        static PropertyChangedEventArgs currentProductChangeArgs =
            ObservableHelper.CreateArgs<AddEditOrderViewModel>(x => x.CurrentProduct);

        public ProductModel CurrentProduct
        {
            get { return currentProduct; }
            set
            {
                currentProduct = value;
                NotifyPropertyChanged(currentProductChangeArgs);
            }
        }

        /// <summary>
        /// saveOrderCommand : Saves Order against Customer
        /// </summary>
        public SimpleCommand SaveOrderCommand
        {
            get { return saveOrderCommand; }
        }

        /// <summary>
        /// cancelOrderCommand : Cancels edit
        /// </summary>
        public SimpleCommand CancelOrderCommand
        {
            get { return cancelOrderCommand; }
        }

        /// <summary>
        /// editOrderCommand : Edits an Order
        /// </summary>
        public SimpleCommand EditOrderCommand
        {
            get { return editOrderCommand; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Translates a UI OrderModel into a data layer Order
        /// </summary>
        /// <param name="uiLayerOrder">A UI OrderModel</param>
        /// <returns>A data layer Order</returns>
        private Order TranslateUIOrderToDataLayerOrder(OrderModel uiLayerOrder)
        {
            Order dataLayerOrder = new Order();
            dataLayerOrder.OrderId = uiLayerOrder.OrderId.DataValue;
            dataLayerOrder.CustomerId = CurrentCustomer.CustomerId.DataValue;
            dataLayerOrder.ProductId = CurrentProduct.ProductId;
            dataLayerOrder.Quantity = uiLayerOrder.Quantity.DataValue;
            dataLayerOrder.DeliveryDate = uiLayerOrder.DeliveryDate.DataValue;
            return dataLayerOrder;
        }


        /// <summary>
        /// When there is a new Product selected, update the current
        /// Product property
        /// </summary>
        private void ProductsCV_CurrentChanged(object sender, EventArgs e)
        {
            if (productsCV.CurrentItem != null)
                CurrentProduct = productsCV.CurrentItem as ProductModel;
        }
        #endregion

        #region Command Implementation

        #region SaveOrderCommand
        /// <summary>
        /// Logic to determine if SaveOrderCommand can execute
        /// </summary>
        private Boolean CanExecuteSaveOrderCommand
        {
            get
            {
                return CurrentCustomerOrder != null;
            }
        }

        /// <summary>
        /// Executes the SaveOrderCommand
        /// </summary>
        private void ExecuteSaveOrderCommand()
        {
            try
            {
                SaveOrderCommand.CommandSucceeded = false;

                if (!CurrentCustomerOrder.IsValid)
                {
                    messageBoxService.ShowError("There order is invalid");
                    SaveOrderCommand.CommandSucceeded = false;
                    return;
                }

                //Order is valid, so end the edit and try and save/update it
                this.CurrentCustomerOrder.EndEdit();

                switch (currentViewMode)
                {
                    #region AddMode
                    //AddMode
                    case ViewMode.AddMode:
                        Int32 orderId =
                            DataService.AddOrder(
                                TranslateUIOrderToDataLayerOrder(CurrentCustomerOrder));

                        if (orderId > 0)
                        {
                            this.CurrentCustomerOrder.OrderId.DataValue = orderId;
                            messageBoxService.ShowInformation(
                                "Sucessfully saved order");
                            this.CurrentViewMode = ViewMode.ViewOnlyMode;
                        }
                        else
                        {
                            messageBoxService.ShowError(
                                "There was a problem saving the order");
                        }
                        SaveOrderCommand.CommandSucceeded = true;
                        //Use the Mediator to send a Message to AddEditCustomerViewModel to tell it a new
                        //or editable Order needs actioning
                        Mediator.NotifyColleagues<Boolean>("AddedOrderSuccessfullyMessage", true);
                        break;
                    #endregion
                    #region EditMode
                    //EditMode
                    case ViewMode.EditMode:
                        Boolean orderUpdated =
                            DataService.UpdateOrder(
                                TranslateUIOrderToDataLayerOrder(CurrentCustomerOrder));

                        if (orderUpdated)
                        {
                            messageBoxService.ShowInformation(
                                "Sucessfully updated order");
                            this.CurrentViewMode = ViewMode.ViewOnlyMode;
                        }
                        else
                        {
                            messageBoxService.ShowError(
                                "There was a problem updating the order");
                        }
                        SaveOrderCommand.CommandSucceeded = true;
                        //Use the Mediator to send a Message to AddEditCustomerViewModel to tell it a new
                        //or editable Order needs actioning
                        Mediator.NotifyColleagues<Boolean>("AddedOrderSuccessfullyMessage", true);
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, ex);
                messageBoxService.ShowError(
                    "There was a problem saving the order");
            }
        }

        #endregion

        #region CancelOrderCommand
        /// <summary>
        /// Logic to determine if CancelOrderCommand can execute
        /// </summary>
        private Boolean CanExecuteCancelOrderCommand
        {
            get
            {
                return CurrentCustomerOrder != null &&
                    CurrentCustomerOrder.OrderId.DataValue != 0 &&
                    CurrentViewMode == ViewMode.EditMode;
            }
        }

        /// <summary>
        /// Executes the CancelOrderCommand
        /// </summary>
        private void ExecuteCancelOrderCommand()
        {
            CancelOrderCommand.CommandSucceeded = false;
            switch (CurrentViewMode)
            {
                case ViewMode.EditMode:
                    this.CurrentCustomerOrder.CancelEdit();
                    CloseActivePopUpCommand.Execute(false);
                    CancelOrderCommand.CommandSucceeded = true;
                    break;
                default:
                    this.CurrentCustomerOrder.CancelEdit();
                    CancelOrderCommand.CommandSucceeded = true;
                    break;
            }
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
                        CurrentCustomerOrder.OrderId.DataValue != 0 &&
                        currentViewMode == ViewMode.ViewOnlyMode;
            }
        }

        /// <summary>
        /// Executes the EditOrderCommand
        /// </summary>
        private void ExecuteEditOrderCommand()
        {

            this.DisplayName = "Edit Order";
            EditOrderCommand.CommandSucceeded = false;
            CurrentViewMode = ViewMode.EditMode;
            this.CurrentCustomerOrder.BeginEdit();
            EditOrderCommand.CommandSucceeded = true;
        }
        #endregion

        #endregion
    }
}
