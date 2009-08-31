using System;

using Cinch;
using MVVM.DataAccess;
using System.ComponentModel;

namespace MVVM.Models
{
    /// <summary>
    /// Respresents a UI Order Model, which has all the
    /// good stuff like Validation/INotifyPropertyChanged/IEditableObject
    /// which are all ready to use within the base class.
    /// 
    /// This class also makes use of <see cref="Cinch.DataWrapper">
    /// Cinch.DataWrapper</see>s. Where the idea is that the ViewModel
    /// is able to control the mode for the data, and as such the View
    /// simply binds to a instance of a <see cref="Cinch.DataWrapper">
    /// Cinch.DataWrapper</see> for both its data and its editable state.
    /// Where the View can disable a control based on the 
    /// <see cref="Cinch.DataWrapper">Cinch.DataWrapper</see> editable state.
    /// </summary>
    public class OrderModel : Cinch.EditableValidatingObject
    {
        #region Data
        //Any data item is declared as a Cinch.DataWrapper, to allow the ViewModel
        //to decide what state the data is in, and the View just renders 
        //the data state accordingly
        private Cinch.DataWrapper<Int32> orderId;
        private Cinch.DataWrapper<Int32> customerId;
        private Cinch.DataWrapper<Int32> productId;
        private Cinch.DataWrapper<Int32> quantity;
        private Cinch.DataWrapper<DateTime> deliveryDate;
        #endregion

        #region Ctor
        public OrderModel()
        {
            #region Create DataWrappers

            OrderId = new DataWrapper<Int32>(this, orderIdChangeArgs);
            CustomerId = new DataWrapper<Int32>(this, customerIdChangeArgs);
            ProductId = new DataWrapper<Int32>(this, productIdChangeArgs);
            Quantity = new DataWrapper<Int32>(this, quantityChangeArgs);
            DeliveryDate = new DataWrapper<DateTime>(this, deliveryDateChangeArgs);

            #endregion

            #region Create Validation Rules

            quantity.AddRule(new SimpleRule("DataValue", "Quantity can not be empty",
                      delegate
                      {
                          return this.Quantity.DataValue <= 0;
                      }));

            #endregion


            //I could not be bothered to write a full DateTime picker in
            //WPF, so for the purpose of this demo, DeliveryDate is
            //fixed to DateTime.Now
            DeliveryDate.DataValue = DateTime.Now;

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// OrderId
        /// </summary>
        static PropertyChangedEventArgs orderIdChangeArgs =
            ObservableHelper.CreateArgs<OrderModel>(x => x.OrderId);

        public Cinch.DataWrapper<Int32> OrderId
        {
            get { return orderId; }
            set
            {
                orderId = value;
                NotifyPropertyChanged(orderIdChangeArgs);
            }
        }

        /// <summary>
        /// CustomerId
        /// </summary>
        static PropertyChangedEventArgs customerIdChangeArgs =
            ObservableHelper.CreateArgs<OrderModel>(x => x.CustomerId);

        public Cinch.DataWrapper<Int32> CustomerId
        {
            get { return customerId; }
            set
            {
                customerId = value;
                NotifyPropertyChanged(customerIdChangeArgs);
            }
        }

        /// <summary>
        /// ProductId
        /// </summary>
        static PropertyChangedEventArgs productIdChangeArgs =
            ObservableHelper.CreateArgs<OrderModel>(x => x.ProductId);

        public Cinch.DataWrapper<Int32> ProductId
        {
            get { return productId; }
            set
            {
                productId = value;
                NotifyPropertyChanged(productIdChangeArgs);
            }
        }

        /// <summary>
        /// Quantity
        /// </summary>
        static PropertyChangedEventArgs quantityChangeArgs =
            ObservableHelper.CreateArgs<OrderModel>(x => x.Quantity);

        public Cinch.DataWrapper<Int32> Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                NotifyPropertyChanged(quantityChangeArgs);
            }
        }

        /// <summary>
        /// DeliveryDate
        /// </summary>
        static PropertyChangedEventArgs deliveryDateChangeArgs =
            ObservableHelper.CreateArgs<OrderModel>(x => x.DeliveryDate);

        public Cinch.DataWrapper<DateTime> DeliveryDate
        {
            get { return deliveryDate; }
            set
            {
                deliveryDate = value;
                NotifyPropertyChanged(deliveryDateChangeArgs);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects IsValid state into
        /// a combined IsValid state for the whole Model
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return
                    base.IsValid &&
                    //AND The DataWrapper IsValid rules
                    //If you and not using the DataWrapper<T> objects
                    //you will not need these extra IsValid properties to be ANDED
                    orderId.IsValid &&
                    customerId.IsValid &&
                    productId.IsValid &&
                    quantity.IsValid &&
                    deliveryDate.IsValid;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Allows Service layer objects to be translated into
        /// UI objects
        /// </summary>
        /// <param name="cust">Service layer object</param>
        /// <returns>UI layer object</returns>
        public static OrderModel OrderToOrderModel(Order order)
        {
            OrderModel orderModel = new OrderModel();
            orderModel.OrderId.DataValue = order.OrderId;
            orderModel.CustomerId.DataValue = order.CustomerId;
            orderModel.Quantity.DataValue = order.Quantity;
            orderModel.ProductId.DataValue = order.ProductId;
            orderModel.DeliveryDate.DataValue = order.DeliveryDate;
            return orderModel;

        }
        #endregion

        #region EditableValidatingObject overrides

        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects into the BeginEdit state
        /// </summary>
        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();
            //Now walk the list of properties in the OrderModel
            //and call BeginEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetBeginEdit<OrderModel>(this);
        }

        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects into the EndEdit state
        /// </summary>
        protected override void OnEndEdit()
        {
            base.OnEndEdit();
            //Now walk the list of properties in the OrderModel
            //and call CancelEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetEndEdit<OrderModel>(this);
        }

        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects into the CancelEdit state
        /// </summary>
        protected override void OnCancelEdit()
        {
            base.OnCancelEdit();
            //Now walk the list of properties in the OrderModel
            //and call CancelEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetCancelEdit<OrderModel>(this);
        }
        #endregion
    }
}
