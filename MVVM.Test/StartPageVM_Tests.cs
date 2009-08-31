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
    /// Tests the StartPageViewModel functionality
    /// </summary>
    [TestFixture]
    public class StartPageVM_Tests
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

            //Create a new StartPageViewModel and test its command
            //to ensure that they do no effect the number of workspace
            //items in the MainWindowViewModel
            StartPageViewModel startPageVM = new StartPageViewModel();

            //Test AddCustomerCommand : Should not be able 
            //to add a new AddEditCustomerViewModel
            startPageVM.AddCustomerCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);

            //Test SearchCustomersViewModel : Should not be able 
            //to add a new SearchCustomersViewModel
            startPageVM.SearchCustomersCommand.Execute(null);
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

            //Create a new StartPageViewModel and test its AddCustomerCommand
            //is able to add a new Workspace item to the MainWindowViewModel
            StartPageViewModel startPageVM = new StartPageViewModel();

            //Test AddCustomerCommand : Should be able 
            //to add a new AddEditCustomerViewModel
            startPageVM.AddCustomerCommand.Execute(null);
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

            //Create a new StartPageViewModel and test its SearchCustomersCommand
            //is able to add a new Workspace item to the MainWindowViewModel
            StartPageViewModel startPageVM = new StartPageViewModel();

            //Test SearchCustomersCommand : Should be able 
            //to add a new AddEditCustomerViewModel
            startPageVM.SearchCustomersCommand.Execute(null);
            Assert.AreEqual(mainWindowVM.Workspaces.Count(), 3);
        }
        #endregion
    }

}

