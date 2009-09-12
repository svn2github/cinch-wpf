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
    /// Tests the SearchCustomersViewModel functionality
    /// </summary>
    [TestFixture]
    public class SearchCustomersVM_Tests
    {
        #region Tests
        [Test]
        public void CheckFormValueCreation()
        {
            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            Assert.Greater(searchCustomersVM.AvailableProperties.Count(), 0);
            Assert.Greater(searchCustomersVM.AvailableConditions.Count(), 0);
        }

        [Test]
        public void CheckFetchCustomer()
        {
            //create some Customers in the Database 1st

            String cust1String = Guid.NewGuid().ToString();
            String cust2String = Guid.NewGuid().ToString();

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust1String, "1")), 0);

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust2String, "2")), 0);

            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            FetchCustomer(searchCustomersVM, cust1String + "_1");

            Int32 matchingCustomers = searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count();

            Assert.AreEqual(1, matchingCustomers);

            //set a filter value that should NOT work
            searchCustomersVM.CurrentFilterText = "gibberish";

            searchCustomersVM.DoSearchCommand.Execute(null);

            Assert.AreEqual(0,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());

        }


        [Test]
        [Ignore] //Work in Isolation, need to find out why doesn't work when running all tests
        public void CheckDeleteCustomer_Fails()
        {
            //create some Customers in the Database 1st

            String cust1String = Guid.NewGuid().ToString();

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust1String, "1")), 0);


            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            //should not be able to delete without a current customer
            Assert.AreEqual(false,
                searchCustomersVM.DeleteCustomerCommand.CanExecute(null));

            FetchCustomer(searchCustomersVM, String.Format("{0}_1", cust1String));

            Assert.AreEqual(1,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());

            searchCustomersVM.CurrentCustomer = searchCustomersVM.MatchedCustomers[0];

            //should be able to delete with a current customer, unless
            //it is open for edit within the MainWindowViewModel
            Assert.AreEqual(true,
                searchCustomersVM.DeleteCustomerCommand.CanExecute(null));

            //As we are testing for failure ensure that this Customer up for deletion
            //is currently being edited within the AddEditCustomerViewModel workspace
            //within the MainWindowViewModel.
            //Then try and delete the customer, but can only do it if the customer 
            //is not being edited in MainWindowViewModel, so should fail
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();

            addEditCustomerViewModel.CurrentCustomer =
                searchCustomersVM.MatchedCustomers[0];

            mainWindowViewModel.Workspaces.Add(addEditCustomerViewModel);

            searchCustomersVM.DeleteCustomerCommand.Execute(null);

            Assert.AreEqual(1,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());
        }


        [Test]
        [Ignore] //Work in Isolation, need to find out why doesn't work when running all tests
        public void CheckDeleteCustomer_Passes()
        {
            //create some Customers in the Database 1st

            String cust1String = Guid.NewGuid().ToString();

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust1String, "1")), 0);


            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            //should not be able to delete without a current customer
            Assert.AreEqual(false,
                searchCustomersVM.DeleteCustomerCommand.CanExecute(null));

            FetchCustomer(searchCustomersVM, String.Format("{0}_1", cust1String));

            Assert.AreEqual(1,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());

            searchCustomersVM.CurrentCustomer = searchCustomersVM.MatchedCustomers[0];

            //should be able to delete with a current customer
            Assert.AreEqual(true,
                searchCustomersVM.DeleteCustomerCommand.CanExecute(null));

            //As the customer is not being edited within the mainWindowViewModel, 
            //as there is no active AddEditCustomerViewModel workspacce, should be
            //able to delete it, but we need to make sure the mainWindowViewModel
            //is available to notify us back that it is ok to proceed with the delete
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            searchCustomersVM.DeleteCustomerCommand.Execute(null);
            FetchCustomer(searchCustomersVM, String.Format("{0}_1", cust1String));

            Assert.AreEqual(0,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());
        }


        [Test]
        public void CheckEditCustomer_Fails()
        {
            //create some Customers in the Database 1st

            String cust1String = Guid.NewGuid().ToString();

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust1String, "1")), 0);


            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            //should not be able to delete without a current customer
            Assert.AreEqual(false,
                searchCustomersVM.EditCustomerCommand.CanExecute(null));

            FetchCustomer(searchCustomersVM, String.Format("{0}_1", cust1String));

            Assert.AreEqual(1,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());

            searchCustomersVM.CurrentCustomer = searchCustomersVM.MatchedCustomers[0];

            //should be able to delete with a current customer
            Assert.AreEqual(true,
                searchCustomersVM.EditCustomerCommand.CanExecute(null));

            //As we are testing for failure ensure that this Customer up for editing
            //is currently being edited within the AddEditCustomerViewModel workspace
            //within the MainWindowViewModel.
            //Then try and edit the customer, but can only do it if the customer 
            //is not being edited in MainWindowViewModel, so should fail
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            AddEditCustomerViewModel addEditCustomerViewModel =
                new AddEditCustomerViewModel();

            addEditCustomerViewModel.CurrentCustomer =
                searchCustomersVM.MatchedCustomers[0];

            mainWindowViewModel.Workspaces.Add(addEditCustomerViewModel);

            searchCustomersVM.EditCustomerCommand.Execute(null);

            //should only have the 1 AddEditCustomerViewModel
            Int32 openCustomerWorkSpaces =
                mainWindowViewModel.Workspaces.Where(w => w.GetType() ==
                    typeof(AddEditCustomerViewModel)).Count();

            Assert.AreEqual(1, openCustomerWorkSpaces);
            Assert.IsNull(searchCustomersVM.CurrentCustomer);


        }

        [Test]
        public void CheckEditCustomer_Passes()
        {
            //create some Customers in the Database 1st

            String cust1String = Guid.NewGuid().ToString();

            Assert.Greater(DataService.AddCustomer(
                GetCustomer(cust1String, "1")), 0);


            SearchCustomersViewModel searchCustomersVM =
                new SearchCustomersViewModel();

            //should not be able to delete without a current customer
            Assert.AreEqual(false,
                searchCustomersVM.EditCustomerCommand.CanExecute(null));

            FetchCustomer(searchCustomersVM, String.Format("{0}_1", cust1String));

            Assert.AreEqual(1,
                searchCustomersVM.MatchedCustomers.Where(
                    c => c.FirstName.DataValue ==
                        searchCustomersVM.CurrentFilterText).Count());

            searchCustomersVM.CurrentCustomer = searchCustomersVM.MatchedCustomers[0];

            //should be able to delete with a current customer
            Assert.AreEqual(true,
                searchCustomersVM.EditCustomerCommand.CanExecute(null));

            //As the customer is not being edited within the mainWindowViewModel, 
            //as there is no active AddEditCustomerViewModel workspacce, should be
            //able to edit it, but we need to make sure the mainWindowViewModel
            //is available to notify us back that it is ok to proceed with the delete
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            searchCustomersVM.EditCustomerCommand.Execute(null);

            //should only have the 1 AddEditCustomerViewModel
            Int32 openCustomerWorkSpaces =
                mainWindowViewModel.Workspaces.Where(w => w.GetType() ==
                    typeof(AddEditCustomerViewModel)).Count();

            Assert.AreEqual(1, openCustomerWorkSpaces);
            Assert.IsNull(searchCustomersVM.CurrentCustomer);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Fetches a customer from the Db
        /// </summary>
        /// <param name="searchCustomersVM">The search ViewModel</param>
        /// <param name="customerFilterName">The firstname of the Customer</param>
        private void FetchCustomer(
            SearchCustomersViewModel searchCustomersVM,
            String customerFilterName)
        {
            //get a property
            searchCustomersVM.CurrentProperty =
                searchCustomersVM.AvailableProperties.Where
                    (p => p.Name == "FirstName").SingleOrDefault();

            //get a matching Condition that can be used with this property
            searchCustomersVM.CurrentCondition =
                searchCustomersVM.AvailableConditions.Where
                    (o => o.ToString() == "Equals").SingleOrDefault();

            //set a filter value that should work
            searchCustomersVM.CurrentFilterText = customerFilterName;

            searchCustomersVM.DoSearchCommand.Execute(null);


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
        #endregion
    }

}

