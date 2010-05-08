using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MVVM.DataAccess;
using MVVM.ViewModels;
using MVVM.Models;
using Cinch;


namespace MVVM.Test
{
    /// <summary>
    /// Tests the AddEditOrderViewModel functionality
    /// </summary>
    [TestFixture]
    public class AddEditOrderVM_Tests
    {
        #region Data
        private Customer cust= null;
        private Order ord= null;
        #endregion

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            //create a customer, and fetch it, so we can add an order to it
            String cust1String = Guid.NewGuid().ToString();

            Int32 customerId = DataService.AddCustomer(
                GetCustomer(cust1String, "1"));

            if (customerId > 0)
                cust = DataAccess.DataService.
                    FetchAllCustomers().Where(c => c.CustomerId == customerId).Single();


            //create a Order against the customer we just created
            Int32 orderId = DataService.AddOrder(
                GetOrder(customerId));


            if (orderId > 0)
                ord = DataAccess.DataService.
                    FetchAllOrders(customerId).Where(o => o.OrderId == orderId).Single();
        }
        #endregion

        #region Tests
        [Test]
        public void SaveOrderCommandTest()
        {
            AddEditOrderViewModel addEditOrderViewModel =
                new AddEditOrderViewModel();
            
            //Test Command can't run without an order
            Assert.AreEqual(addEditOrderViewModel.SaveOrderCommand.CanExecute(null), false);

            #region AddMode
            addEditOrderViewModel.CurrentViewMode = ViewMode.AddMode;


            addEditOrderViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            addEditOrderViewModel.CurrentCustomerOrder = OrderModel.OrderToOrderModel(ord);

            //test Save Command can run
            Assert.AreEqual(addEditOrderViewModel.SaveOrderCommand.CanExecute(null), true);

            //Execute SaveCommand
            addEditOrderViewModel.SaveOrderCommand.Execute(null);
            Assert.Greater(addEditOrderViewModel.CurrentCustomerOrder.OrderId.DataValue, 0);


            #endregion

            #region EditMode
            addEditOrderViewModel.CurrentViewMode = ViewMode.EditMode;


            addEditOrderViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);

            Order o = DataAccess.DataService.FetchAllOrders(cust.CustomerId).First();

            addEditOrderViewModel.CurrentCustomerOrder = OrderModel.OrderToOrderModel(o);

            //test Save Command can run
            Assert.AreEqual(addEditOrderViewModel.SaveOrderCommand.CanExecute(null), true);

            //Execute SaveCommand
            addEditOrderViewModel.SaveOrderCommand.Execute(null);
            Assert.AreEqual(addEditOrderViewModel.SaveOrderCommand.CommandSucceeded, true);


            #endregion


        }

        [Test]
        public void CancelOrderCommandTest()
        {
            AddEditOrderViewModel addEditOrderViewModel =
                new AddEditOrderViewModel();


            //test the edit command first, which allows us to put 
            //order into edit mode
            EditOrderCommandTest(addEditOrderViewModel);

            //so now make an edit to the order, say change Quantity
            Int32 editQty = 9999;
            ord.Quantity = editQty;

            #region CancelOrderCommandTests

            //check that CancelOrderCommand can't run until its in correct mode
            addEditOrderViewModel.CurrentViewMode = ViewMode.ViewOnlyMode;
            Assert.AreEqual(addEditOrderViewModel.CancelOrderCommand.CanExecute(null), false);


            //now allow the CancelOrderCommand to run by placing it in the correct mode
            addEditOrderViewModel.CurrentViewMode = ViewMode.EditMode;
            Assert.AreEqual(addEditOrderViewModel.CancelOrderCommand.CanExecute(null), true);

            //execute the CancelOrderCommand
            addEditOrderViewModel.CancelOrderCommand.Execute(null);
            Assert.AreNotEqual(addEditOrderViewModel.CurrentCustomerOrder.Quantity.DataValue, editQty);
            Assert.AreEqual(addEditOrderViewModel.CancelOrderCommand.CommandSucceeded, true);



            //Test that CancelOrderCommand can not execute with out a current order
            addEditOrderViewModel.CurrentCustomerOrder = null;
            Assert.AreEqual(addEditOrderViewModel.CancelOrderCommand.CanExecute(null), false);
            #endregion

        }


        public void EditOrderCommandTest(AddEditOrderViewModel addEditOrderViewModel)
        {


            //Test that EditOrderCommand can not execute with no current order
            addEditOrderViewModel.CurrentCustomerOrder = null;
            Assert.AreEqual(addEditOrderViewModel.EditOrderCommand.CanExecute(null), false);


            //now give it an order, and check that EditOrderCommand can't run until its in correct mode
            addEditOrderViewModel.CurrentCustomerOrder = OrderModel.OrderToOrderModel(ord);

            addEditOrderViewModel.CurrentViewMode = ViewMode.EditMode;
            Assert.AreEqual(addEditOrderViewModel.EditOrderCommand.CanExecute(null), false);


            //now allow the EditOrderCommand to run by placing it in the correct mode
            addEditOrderViewModel.CurrentViewMode = ViewMode.ViewOnlyMode;
            Assert.AreEqual(addEditOrderViewModel.EditOrderCommand.CanExecute(null), true);

            //execute the EditOrderCommand
            addEditOrderViewModel.EditOrderCommand.Execute(null);
            Assert.AreEqual(addEditOrderViewModel.CurrentViewMode, ViewMode.EditMode);
            Assert.AreEqual(addEditOrderViewModel.EditOrderCommand.CommandSucceeded, true);



            

        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a dummy Customer to insert into the database
        /// </summary>
        private Customer GetCustomer(String prefix, String suffix)
        {
            Customer c = new Customer();
            c.FirstName = String.Format("{0}_{1}", prefix, suffix);
            c.LastName = String.Format("{0}_{1}", prefix, suffix);
            c.HomePhoneNumber = String.Format("{0}_{1}", prefix, suffix);
            c.MobilePhoneNumber = String.Format("{0}_{1}", prefix, suffix);
            c.Address1 = String.Format("{0}_{1}", prefix, suffix);
            c.Email = String.Format("{0}_{1}", prefix, suffix);
            c.Address2 = String.Format("{0}_{1}", prefix, suffix);
            c.Address3 = String.Format("{0}_{1}", prefix, suffix);
            return c;
        }


        /// <summary>
        /// Creates a dummy Order to insert into the database
        /// </summary>
        private Order GetOrder(Int32 customerId)
        {
            Order o = new Order();
            o.CustomerId = customerId;
            o.Quantity=2;
            o.ProductId =1;
            o.DeliveryDate = DateTime.Now;
            return o;
        }
        #endregion
    }

}

