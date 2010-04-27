using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MVVM.ViewModels;
using Cinch;


namespace MVVM.Test
{

    /// <summary>
    /// Tests the MainWindowViewModel functionality
    /// </summary>
    [TestFixture]
    public class MainWindowVM_Tests
    {
        #region Tests
        [Test]
        public void CheckNoMoreWorkSpacesCanBeAdded()
        {
            MainWindowViewModel mainWindowVM = new MainWindowViewModel();
            mainWindowVM.Workspaces.Add(new AddEditCustomerViewModel());
            mainWindowVM.Workspaces.Add(new SearchCustomersViewModel());

            //MainWindowViewModel.Workspaces starts out 
            //with a StartPageViewModel already present
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);


            //Test AddCustomerCommand : Should not be able 
            //to add a new AddEditCustomerViewModel
            mainWindowVM.AddCustomerCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

            //Test SearchCustomersViewModel : Should not be able 
            //to add a new SearchCustomersViewModel
            mainWindowVM.SearchCustomersCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

        }

        [Test]
        public void AddCustomerCommand_Test()
        {
            MainWindowViewModel mainWindowVM = new MainWindowViewModel();
            mainWindowVM.Workspaces.Add(new AddEditCustomerViewModel());
            mainWindowVM.Workspaces.Add(new SearchCustomersViewModel());

            //MainWindowViewModel.Workspaces starts out 
            //with a StartPageViewModel already present
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

            //Now remove all the current AddEditCustomerViewModel 
            //from the list of Workspaces in MainWindowViewModel
            var addEditCustomerVM =
                mainWindowVM.Workspaces.Where(x => x.GetType() ==
                  typeof(AddEditCustomerViewModel)).FirstOrDefault();

            mainWindowVM.Workspaces.Remove(addEditCustomerVM);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 2);
            //Test AddCustomerCommand : Should be able 
            //to add a new AddEditCustomerViewModel
            mainWindowVM.AddCustomerCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

        }

        [Test]
        public void SearchCustomersCommand_Test()
        {

            MainWindowViewModel mainWindowVM = new MainWindowViewModel();
            mainWindowVM.Workspaces.Add(new AddEditCustomerViewModel());
            mainWindowVM.Workspaces.Add(new SearchCustomersViewModel());

            //MainWindowViewModel.Workspaces starts out 
            //with a StartPageViewModel already present
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

            //Now remove all the current SearchCustomersViewModel 
            //from the list of Workspaces in MainWindowViewModel
            var searchCustomersVM =
                mainWindowVM.Workspaces.Where(x => x.GetType() ==
                  typeof(SearchCustomersViewModel)).FirstOrDefault();

            mainWindowVM.Workspaces.Remove(searchCustomersVM);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 2);
            //Test SearchCustomersCommand : Should be able 
            //to add a new AddEditCustomerViewModel
            mainWindowVM.SearchCustomersCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);
        }
        #endregion
    }

}

