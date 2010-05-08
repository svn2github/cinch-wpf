using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MVVM.DataAccess;
using MVVM.ViewModels;
using MVVM.Models;
using Cinch;
using System.Threading;



namespace MVVM.Test
{
    /// <summary>
    /// Tests the AddEditCustomerViewModel functionality
    /// </summary>
    [TestFixture]
    public class AddEditCustomerVM_Tests
    {
        #region Data
        private Customer cust= null;
        private Order ord= null;
        #endregion

        #region Setup
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

            DataService.AddOrder(GetOrder(customerId));
            DataService.AddOrder(GetOrder(customerId));
            DataService.AddOrder(GetOrder(customerId));
            DataService.AddOrder(GetOrder(customerId));
            Int32 orderId = DataService.AddOrder(
                GetOrder(customerId));


            if (orderId > 0)
                ord = DataAccess.DataService.
                    FetchAllOrders(customerId).Where(o => o.OrderId == orderId).Single();
        }
        #endregion

        #region Tests
        [Test]
        public void AddOrderFromOrderVMTest()
        {
            AddEditOrderViewModel addEditOrderViewModel =
                new AddEditOrderViewModel();

            addEditOrderViewModel.CurrentViewMode = ViewMode.AddMode;
            addEditOrderViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            addEditOrderViewModel.CurrentCustomerOrder = OrderModel.OrderToOrderModel(ord);
            addEditOrderViewModel.CurrentCustomerOrder.Quantity.DataValue = 1;

            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();
            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);


            //Execute AddEditOrderViewModel.SaveOrderCommand
            addEditOrderViewModel.SaveOrderCommand.Execute(null);

            Int32 currentCustomerOrderCount = 
                addEditCustomerViewModel.CurrentCustomer.Orders.Count;


            //Wait for Lazy load of AddEditCustomerViewModel.CurrentCustomer.Orders
            //which is triggered via Mediator, and is run on the AddEditCustomerViewModel
            //BackgroundTaskManager
            manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted += 
                delegate(object sender, EventArgs args)
            {
                // Signal the waiting NUnit thread that we're ready to move on.
                manualEvent.Set();
            };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);
            Assert.AreEqual(currentCustomerOrderCount + 1, 
                addEditCustomerViewModel.CurrentCustomer.Orders.Count);


        }

        [Test]
        public void AddOrderTest()
        {

            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();

            //Test Command can't run without an order
            Assert.AreEqual(addEditCustomerViewModel.AddOrderCommand.CanExecute(null), false);


            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);

            //Test that AddOrderCommand can now Execute
            Assert.AreEqual(addEditCustomerViewModel.AddOrderCommand.CanExecute(null), true);

            //Run the AddOrderCommand
            TestUIVisualizerService testUIVisualizerService =
              (TestUIVisualizerService)
                ViewModelBase.ServiceProvider.Resolve<IUIVisualizerService>();

            //Queue up the response we expect for our given TestUIVisualizerService
            //for a given ICommand/Method call within the test ViewModel
            testUIVisualizerService.ShowDialogResultResponders.Enqueue
             (() =>
                {
                    return true;
                }
             );

            addEditCustomerViewModel.AddOrderCommand.Execute(null);
            Assert.AreEqual(addEditCustomerViewModel.AddOrderCommand.CommandSucceeded, true);

            //Clear the TestUIVisualizerService.ShowDialogResultResponders
            testUIVisualizerService.ShowDialogResultResponders.Clear();
        }

        [Test]
        public void EditOrderTest()
        {

            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();

            //Test Command can't run without an order
            Assert.AreEqual(addEditCustomerViewModel.EditOrderCommand.CanExecute(null), false);


            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);

            addEditCustomerViewModel.CurrentCustomerOrder =
                addEditCustomerViewModel.CurrentCustomer.Orders.First();
            addEditCustomerViewModel.CurrentCustomerOrder.Quantity.DataValue = 1;

            //Test that EditOrderCommand can now Execute
            Assert.AreEqual(addEditCustomerViewModel.EditOrderCommand.CanExecute(null), true);

            //Run the AddOrderCommand
            TestUIVisualizerService testUIVisualizerService =
              (TestUIVisualizerService)
                ViewModelBase.ServiceProvider.Resolve<IUIVisualizerService>();

            //Queue up the response we expect for our given TestUIVisualizerService
            //for a given ICommand/Method call within the test ViewModel
            testUIVisualizerService.ShowDialogResultResponders.Enqueue
             (() =>
             {
                 return true;
             }
             );

            addEditCustomerViewModel.EditOrderCommand.Execute(null);
            Assert.AreEqual(addEditCustomerViewModel.EditOrderCommand.CommandSucceeded, true);

            //Clear the TestUIVisualizerService.ShowDialogResultResponders
            testUIVisualizerService.ShowDialogResultResponders.Clear();
        }

        [Test]
        public void DeleteOrderTest()
        {

            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();

            //Test Command can't run without an order
            Assert.AreEqual(addEditCustomerViewModel.DeleteOrderCommand.CanExecute(null), false);


            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);

            addEditCustomerViewModel.CurrentCustomerOrder =
                addEditCustomerViewModel.CurrentCustomer.Orders.First();
            addEditCustomerViewModel.CurrentCustomerOrder.Quantity.DataValue = 1;

            //Test that DeleteOrderCommand can now Execute
            Assert.AreEqual(addEditCustomerViewModel.DeleteOrderCommand.CanExecute(null), true);

            #region Test NO, delete MessageBox option
            //Run the DeleteOrderCommand, with a NO, Do not delete MessageBox option Queued up
            TestMessageBoxService testMessageBoxService =
              (TestMessageBoxService)
                ViewModelBase.ServiceProvider.Resolve<IMessageBoxService>();

            //Queue up the response we expect for our given TestMessageBoxService
            //for a given ICommand/Method call within the test ViewModel
            testMessageBoxService.ShowYesNoResponders.Enqueue
             (() =>
             {
                 return CustomDialogResults.No;
             }
             );


            Int32 existingCustomerOrdersCount = addEditCustomerViewModel.CurrentCustomer.Orders.Count(); 
            addEditCustomerViewModel.DeleteOrderCommand.Execute(null);
            //Clear the TestMessageBoxService.ShowYesNoResponders
            testMessageBoxService.ShowYesNoResponders.Clear();

            Assert.AreEqual(existingCustomerOrdersCount, addEditCustomerViewModel.CurrentCustomer.Orders.Count());
            #endregion

            #region Test YES, delete MessageBox option
            //Run the DeleteOrderCommand, with a YES, Do delete MessageBox option Queued up
            //Queue up the response we expect for our given TestMessageBoxService
            //for a given ICommand/Method call within the test ViewModel
            testMessageBoxService.ShowYesNoResponders.Enqueue
             (() =>
             {
                 return CustomDialogResults.Yes;
             }
             );


            existingCustomerOrdersCount = addEditCustomerViewModel.CurrentCustomer.Orders.Count();
            addEditCustomerViewModel.DeleteOrderCommand.Execute(null);
            //Clear the TestMessageBoxService.ShowYesNoResponders
            testMessageBoxService.ShowYesNoResponders.Clear();

            manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);


            Assert.AreEqual(existingCustomerOrdersCount-1, addEditCustomerViewModel.CurrentCustomer.Orders.Count());
            #endregion
        }


        [Test]
        public void SaveCustomerCommandTest()
        {
            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();


            //Test Command can't run without an order
            Assert.AreEqual(addEditCustomerViewModel.SaveCustomerCommand.CanExecute(null), false);

            #region AddMode
            addEditCustomerViewModel.CurrentViewMode = ViewMode.AddMode;


            Customer newCust = GetCustomer("blah", "more");
            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(newCust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);




            //test Save Command can run
            Assert.AreEqual(addEditCustomerViewModel.SaveCustomerCommand.CanExecute(null), true);

            //Execute SaveCommand
            addEditCustomerViewModel.SaveCustomerCommand.Execute(null);
            Assert.Greater(addEditCustomerViewModel.CurrentCustomer.CustomerId.DataValue, 0);


            #endregion

            #region EditMode
            addEditCustomerViewModel.CurrentViewMode = ViewMode.EditMode;


            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);


            //test Save Command can run
            Assert.AreEqual(addEditCustomerViewModel.SaveCustomerCommand.CanExecute(null), true);

            //Execute SaveCustomerCommand
            addEditCustomerViewModel.SaveCustomerCommand.Execute(null);
            Assert.AreEqual(addEditCustomerViewModel.SaveCustomerCommand.CommandSucceeded, true);


            #endregion
        }


        [Test]
        public void CancelCustomerCommandTest()
        {
            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();


            //test the edit command first, which allows us to put 
            //order into edit mode
            EditCustomerCommandTest(addEditCustomerViewModel);

            //so now make an edit to the Customer, say change MobilePhoneNumber
            Int32 editPhoneNum = 9999;
            cust.MobilePhoneNumber = editPhoneNum.ToString();

            #region CancelCustomerCommandTests

            //check that CancelCustomerCommand can't run until its in correct mode
            addEditCustomerViewModel.CurrentViewMode = ViewMode.ViewOnlyMode;
            Assert.AreEqual(addEditCustomerViewModel.CancelCustomerCommand.CanExecute(null), false);


            //now allow the CancelCustomerCommand to run by placing it in the correct mode
            addEditCustomerViewModel.CurrentViewMode = ViewMode.EditMode;
            Assert.AreEqual(addEditCustomerViewModel.CancelCustomerCommand.CanExecute(null), true);

            //execute the CancelCustomerCommand
            addEditCustomerViewModel.CancelCustomerCommand.Execute(null);
            Assert.AreNotEqual(addEditCustomerViewModel.CurrentCustomer.HomePhoneNumber.DataValue, editPhoneNum);
            Assert.AreEqual(addEditCustomerViewModel.CancelCustomerCommand.CommandSucceeded, true);



            //Test that CancelCustomerCommand can not execute with out a current Customer
            addEditCustomerViewModel.CurrentCustomer = null;
            Assert.AreEqual(addEditCustomerViewModel.CancelCustomerCommand.CanExecute(null), false);
            #endregion

        }
        #endregion

        #region Private Methods
        private void EditCustomerCommandTest(AddEditCustomerViewModel addEditCustomerViewModel)
        {


            //Test that EditCustomerCommand can not execute with no current order
            addEditCustomerViewModel.CurrentCustomer = null;
            //As setting the AddEditCustomerViewModel.CurrentCustomer causes
            //a background fetch of all CurrentCustomer.Orders, we need to wait
            //until that completes to continue
            ManualResetEvent manualEvent = new ManualResetEvent(false);
            addEditCustomerViewModel.BgWorker.BackgroundTaskCompleted +=
                delegate(object sender, EventArgs args)
                {
                    // Signal the waiting NUnit thread that we're ready to move on.
                    manualEvent.Set();
                };

            //Wait for signal to move on from BackgroundTaskManager.BackgroundTaskCompleted
            manualEvent.WaitOne(5000, false);


            Assert.AreEqual(addEditCustomerViewModel.EditCustomerCommand.CanExecute(null), false);


            //now give it an Customer, and check that EditCustomerCommand can't run until its in correct mode
            addEditCustomerViewModel.CurrentCustomer = CustomerModel.CustomerToCustomerModel(cust);

            addEditCustomerViewModel.CurrentViewMode = ViewMode.EditMode;
            Assert.AreEqual(addEditCustomerViewModel.EditCustomerCommand.CanExecute(null), false);


            //now allow the EditCustomerCommand to run by placing it in the correct mode
            addEditCustomerViewModel.CurrentViewMode = ViewMode.ViewOnlyMode;
            Assert.AreEqual(addEditCustomerViewModel.EditCustomerCommand.CanExecute(null), true);

            //execute the EditCustomerCommand
            addEditCustomerViewModel.EditCustomerCommand.Execute(null);
            Assert.AreEqual(addEditCustomerViewModel.CurrentViewMode, ViewMode.EditMode);
            Assert.AreEqual(addEditCustomerViewModel.EditCustomerCommand.CommandSucceeded, true);
        } 



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

