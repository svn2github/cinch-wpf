using System;
using System.Linq;
using System.Collections.ObjectModel;

using Cinch;
using MVVM.DataAccess;
using System.Collections.Generic;
using System.ComponentModel;



namespace MVVM.Models
{
    /// <summary>
    /// Respresents a UI Customer Model, which has all the
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
    public class CustomerModel : Cinch.EditableValidatingObject
    {
        #region Data
        //Any data item is declared as a Cinch.DataWrapper, to allow the ViewModel
        //to decide what state the data is in, and the View just renders 
        //the data state accordingly
        private Cinch.DataWrapper<Int32> customerId;
        private Cinch.DataWrapper<String> firstName;
        private Cinch.DataWrapper<String> lastName;
        private Cinch.DataWrapper<String> email;
        private Cinch.DataWrapper<String> homePhoneNumber;
        private Cinch.DataWrapper<String> mobilePhoneNumber;
        private Cinch.DataWrapper<String> address1;
        private Cinch.DataWrapper<String> address2;
        private Cinch.DataWrapper<String> address3;
        private Cinch.DispatcherNotifiedObservableCollection<OrderModel> orders =
            new Cinch.DispatcherNotifiedObservableCollection<OrderModel>();
        private Boolean hasOrders = false;
        #endregion

        #region Ctor

        public CustomerModel()
        {
            #region Create DataWrappers

            CustomerId = new DataWrapper<Int32>(this, customerIdChangeArgs);
            FirstName = new DataWrapper<String>(this, firstNameChangeArgs);
            LastName = new DataWrapper<String>(this, lastNameChangeArgs);
            Email = new DataWrapper<String>(this, emailChangeArgs);
            HomePhoneNumber = new DataWrapper<String>(this, homePhoneNumberChangeArgs);
            MobilePhoneNumber = new DataWrapper<String>(this, mobilePhoneNumberChangeArgs);
            Address1 = new DataWrapper<String>(this, address1ChangeArgs);
            Address2 = new DataWrapper<String>(this, address2ChangeArgs);
            Address3 = new DataWrapper<String>(this, address3ChangeArgs);

            #endregion

            #region Create Validation Rules

            firstName.AddRule(new SimpleRule("DataValue", "Firstname can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.FirstName.DataValue);
                      }));
            lastName.AddRule(new SimpleRule("DataValue", "LastName can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.LastName.DataValue);
                      }));
            email.AddRule(new SimpleRule("DataValue", "Email can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.Email.DataValue);
                      }));
            homePhoneNumber.AddRule(new SimpleRule("DataValue", "HomePhoneNumber can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.HomePhoneNumber.DataValue);
                      }));
            address1.AddRule(new SimpleRule("DataValue", "Address1 can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.address1.DataValue);
                      }));
            address2.AddRule(new SimpleRule("DataValue", "Address2 can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.address2.DataValue);
                      }));
            address3.AddRule(new SimpleRule("DataValue", "Address3 can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.address3.DataValue);
                      }));
            #endregion

        }
        #endregion

        #region Public Properties

        public String FullName
        {
            get { return FirstName.DataValue + " " + LastName.DataValue; }
        }

        /// <summary>
        /// CustomerId
        /// </summary>
        static PropertyChangedEventArgs customerIdChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.CustomerId);

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
        /// FirstName
        /// </summary>
        static PropertyChangedEventArgs firstNameChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.FirstName);

        public Cinch.DataWrapper<String> FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                NotifyPropertyChanged(firstNameChangeArgs);
            }
        }

        /// <summary>
        /// LastName
        /// </summary>
        static PropertyChangedEventArgs lastNameChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.LastName);

        public Cinch.DataWrapper<String> LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                NotifyPropertyChanged(lastNameChangeArgs);
            }
        }

        /// <summary>
        /// Email
        /// </summary>
        static PropertyChangedEventArgs emailChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.Email);

        public Cinch.DataWrapper<String> Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyPropertyChanged(emailChangeArgs);
            }
        }

        /// <summary>
        /// HomePhoneNumber
        /// </summary>
        static PropertyChangedEventArgs homePhoneNumberChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.HomePhoneNumber);

        public Cinch.DataWrapper<String> HomePhoneNumber
        {
            get { return homePhoneNumber; }
            set
            {
                homePhoneNumber = value;
                NotifyPropertyChanged(homePhoneNumberChangeArgs);
            }
        }

        /// <summary>
        /// MobilePhoneNumber
        /// </summary>
        static PropertyChangedEventArgs mobilePhoneNumberChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.MobilePhoneNumber);

        public Cinch.DataWrapper<String> MobilePhoneNumber
        {
            get { return mobilePhoneNumber; }
            set
            {
                mobilePhoneNumber = value;
                NotifyPropertyChanged(mobilePhoneNumberChangeArgs);
            }
        }

        /// <summary>
        /// Address1
        /// </summary>
        static PropertyChangedEventArgs address1ChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.Address1);

        public Cinch.DataWrapper<String> Address1
        {
            get { return address1; }
            set
            {
                address1 = value;
                NotifyPropertyChanged(address1ChangeArgs);
            }
        }

        /// <summary>
        /// Address2
        /// </summary>
        static PropertyChangedEventArgs address2ChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.Address2);

        public Cinch.DataWrapper<String> Address2
        {
            get { return address2; }
            set
            {
                address2 = value;
                NotifyPropertyChanged(address2ChangeArgs);
            }
        }

        /// <summary>
        /// Address3
        /// </summary>
        static PropertyChangedEventArgs address3ChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.Address3);

        public Cinch.DataWrapper<String> Address3
        {
            get { return address3; }
            set
            {
                address3 = value;
                NotifyPropertyChanged(address3ChangeArgs);
            }
        }

        /// <summary>
        /// Orders
        /// </summary>
        static PropertyChangedEventArgs ordersChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.Orders);

        public DispatcherNotifiedObservableCollection<OrderModel> Orders
        {
            get { return orders; }
            set
            {
                orders = value;
                NotifyPropertyChanged(ordersChangeArgs);
                HasOrders = orders.Count > 0;
            }
        }

        /// <summary>
        /// HasOrders
        /// </summary>
        static PropertyChangedEventArgs hasOrdersChangeArgs =
            ObservableHelper.CreateArgs<CustomerModel>(x => x.HasOrders);

        public Boolean HasOrders
        {
            get { return hasOrders; }
            set
            {
                hasOrders = value;
                NotifyPropertyChanged(hasOrdersChangeArgs);
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
        public static CustomerModel CustomerToCustomerModel(Customer cust)
        {
            CustomerModel customerModel = new CustomerModel();
            customerModel.CustomerId.DataValue = cust.CustomerId;
            customerModel.FirstName.DataValue = String.IsNullOrEmpty(cust.FirstName) ? String.Empty : cust.FirstName.Trim();
            customerModel.LastName.DataValue = String.IsNullOrEmpty(cust.LastName) ? String.Empty : cust.LastName.Trim();
            customerModel.Email.DataValue = String.IsNullOrEmpty(cust.Email) ? String.Empty : cust.Email.Trim();
            customerModel.HomePhoneNumber.DataValue = String.IsNullOrEmpty(cust.HomePhoneNumber) ? String.Empty : cust.HomePhoneNumber.Trim();
            customerModel.MobilePhoneNumber.DataValue = String.IsNullOrEmpty(cust.MobilePhoneNumber) ? String.Empty : cust.MobilePhoneNumber.Trim();
            customerModel.Address1.DataValue = String.IsNullOrEmpty(cust.Address1) ? String.Empty : cust.Address1.Trim();
            customerModel.Address2.DataValue = String.IsNullOrEmpty(cust.Address2) ? String.Empty : cust.Address2.Trim();
            customerModel.Address3.DataValue = String.IsNullOrEmpty(cust.Address3) ? String.Empty : cust.Address3.Trim();
            //convert to UI type objects
            customerModel.Orders = new Cinch.DispatcherNotifiedObservableCollection<OrderModel>(cust.Orders.ToList().ConvertAll(
                    new Converter<Order, OrderModel>(OrderModel.OrderToOrderModel)));
            return customerModel;
         
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
                    customerId.IsValid &&
                    firstName.IsValid &&
                    lastName.IsValid &&
                    email.IsValid &&
                    homePhoneNumber.IsValid &&
                    mobilePhoneNumber.IsValid &&
                    address1.IsValid &&
                    address2.IsValid &&
                    address3.IsValid;
            }
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
            //Now walk the list of properties in the CustomerModel
            //and call BeginEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetBeginEdit<CustomerModel>(this);
        }

        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects into the EndEdit state
        /// </summary>
        protected override void OnEndEdit()
        {
            base.OnEndEdit();
            //Now walk the list of properties in the CustomerModel
            //and call CancelEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetEndEdit<CustomerModel>(this);
        }

        /// <summary>
        /// Override hook which allows us to also put any child 
        /// EditableValidatingObject objects into the CancelEdit state
        /// </summary>
        protected override void OnCancelEdit()
        {
            base.OnCancelEdit();
            //Now walk the list of properties in the CustomerModel
            //and call CancelEdit() on all Cinch.DataWrapper<T>s.
            //we can use the Cinch.DataWrapperHelper class for this
            DataWrapperHelper.SetCancelEdit<CustomerModel>(this);

        }
        #endregion
    }
}
