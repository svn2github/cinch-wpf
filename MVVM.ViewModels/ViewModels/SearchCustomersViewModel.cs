using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

using Cinch;
using MVVM.DataAccess;
using MVVM.Models;

namespace MVVM.ViewModels
{
    /// <summary>
    /// Creates a dynamically constructed string that is then
    /// used to send to the <see cref="MVVM.DataAccess.DataService">
    /// DataService.FindMatchingCustomers()</see> method. The
    /// DataService.FindMatchingCustomers() makes use of the Dynamic
    /// linq API.
    /// 
    /// Provides ALL logic for the SearchCustomersView
    /// </summary>
    public class SearchCustomersViewModel : Cinch.ViewModelBase
    {
        #region Data
        private List<String> availableConditions = null;
        private String currentCondition = null;
        private ICollectionView conditionCollectionView = null;

        private List<PropertyInfo> availableProperties = null;
        private PropertyInfo currentProperty = null;
        private ICollectionView propertiesCollectionView = null;

        private Type boundType = null;
        private String currentFilterText = String.Empty;
        private String clauseResult = String.Empty;

        private SimpleCommand doSearchCommand;
        private SimpleCommand deleteCustomerCommand;
        private SimpleCommand editCustomerCommand;

        private List<CustomerModel> matchedCustomers = new List<CustomerModel>();
        private ICollectionView matchedCustomersCollectionView = null;
        private CustomerModel currentCustomer;


        //Services
        private IMessageBoxService messageBoxService = null;

        #endregion

        #region Ctor
        public SearchCustomersViewModel()
        {
            this.DisplayName = "Search Customers";
            BoundType = typeof(MVVM.DataAccess.Customer);

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

            #region Create Commands
            //Create do search Command
            doSearchCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ExecuteDoSearchCommand()
            };
            //Create delete customer Command
            deleteCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteDeleteCustomerCommand,
                ExecuteDelegate = x => ExecuteDeleteCustomerCommand()
            };
            //Create edit customer Command
            editCustomerCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditCustomerCommand,
                ExecuteDelegate = x => ExecuteEditCustomerCommand()
            };
            #endregion
        }
        #endregion

        #region Public Properties

        public Boolean IsCollectionProperty { get; private set; }

        public Boolean IsOperatorComparable { get; private set; }

        public Boolean IsString { get; private set; }

        public Boolean IsNumeric { get; private set; }

        public Int32 ParameterNumber { get; set; }

        /// <summary>
        /// Bound Type for search
        /// </summary>
        static PropertyChangedEventArgs boundTypeChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.BoundType);

        public Type BoundType
        {
            get { return boundType; }
            set
            {
                boundType = value;
                CreateFormValues();
                NotifyPropertyChanged(boundTypeChangeArgs);
            }
        }

        /// <summary>
        /// Filter text
        /// </summary>
        static PropertyChangedEventArgs currentFilterTextChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.CurrentFilterText);

        public String CurrentFilterText
        {
            get { return currentFilterText; }
            set
            {
                currentFilterText = value;
                ValidateFilterText();
                NotifyPropertyChanged(currentFilterTextChangeArgs);
            }
        }

        /// <summary>
        /// Property text
        /// </summary>
        static PropertyChangedEventArgs currentPropertyChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.CurrentProperty);

        public PropertyInfo CurrentProperty
        {
            get { return currentProperty; }
            set
            {
                currentProperty = value;
                NotifyPropertyChanged(currentPropertyChangeArgs);
                propertiesCollectionView.MoveCurrentTo(currentProperty);
            }
        }

        /// <summary>
        /// Property text
        /// </summary>
        static PropertyChangedEventArgs currentConditionChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.CurrentCondition);

        public String CurrentCondition
        {
            get { return currentCondition; }
            set
            {
                currentCondition = value;
                NotifyPropertyChanged(currentConditionChangeArgs);
                conditionCollectionView.MoveCurrentTo(currentCondition);
            }
        }

        /// <summary>
        /// Matched customers 
        /// </summary>
        static PropertyChangedEventArgs matchedCustomersChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.MatchedCustomers);

        public List<CustomerModel> MatchedCustomers
        {
            get { return matchedCustomers; }
            set
            {
                if (matchedCustomers != null)
                {
                    matchedCustomers = value;
                    NotifyPropertyChanged(matchedCustomersChangeArgs);
                    if (matchedCustomersCollectionView != null)
                        matchedCustomersCollectionView.CurrentChanged -=
                            MatchedCustomersCollectionView_CurrentChanged;

                    matchedCustomersCollectionView =
                        CollectionViewSource.GetDefaultView(matchedCustomers);
                    matchedCustomersCollectionView.CurrentChanged +=
                        MatchedCustomersCollectionView_CurrentChanged;
                    matchedCustomersCollectionView.MoveCurrentToPosition(-1);
                    ShowUserMessageBasedOnResults();
                }
            }
        }

        /// <summary>
        /// AvailableConditions 
        /// </summary>
        static PropertyChangedEventArgs availableConditionsChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.AvailableConditions);

        public List<String> AvailableConditions
        {
            get { return availableConditions; }
            set
            {
                availableConditions = value;
                NotifyPropertyChanged(availableConditionsChangeArgs);
            }
        }

        /// <summary>
        /// AvailableProperties 
        /// </summary>
        static PropertyChangedEventArgs availablePropertiesChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.AvailableProperties);

        public List<PropertyInfo> AvailableProperties
        {
            get { return availableProperties; }
            set
            {
                availableProperties = value;
                NotifyPropertyChanged(availablePropertiesChangeArgs);
            }
        }

        /// <summary>
        /// CurrentCustomer 
        /// </summary>
        static PropertyChangedEventArgs currentCustomerChangeArgs =
            ObservableHelper.CreateArgs<SearchCustomersViewModel>(x => x.CurrentCustomer);

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
        /// Clause
        /// </summary>
        public String ClauseResult
        {
            get
            {

                StringBuilder sb = new StringBuilder(1000);

                if (IsCollectionProperty)
                    sb.Append(String.Format("{0}.Count ", currentProperty.Name));
                else
                {
                    if (IsString)
                        sb.Append(String.Format("{0}.", currentProperty.Name));
                    else
                        sb.Append(String.Format("{0} ", currentProperty.Name));
                }

                if (IsOperatorComparable)
                {
                    sb.Append(String.Format("{0} ", currentCondition));
                    sb.Append(String.Format("{0} ", CurrentFilterText));
                }

                if (IsString)
                {
                    sb.Append(String.Format("{0}(\"{1}\")",
                        currentCondition,
                        CurrentFilterText));
                }


                return sb.ToString();
            }
        }

        /// <summary>
        /// DoSearchCommand : Search for customers command
        /// </summary>
        public SimpleCommand DoSearchCommand
        {
            get { return doSearchCommand; }
        }

        /// <summary>
        /// deleteCustomerCommand : Deletes a Customer
        /// </summary>
        public SimpleCommand DeleteCustomerCommand
        {
            get { return deleteCustomerCommand; }
        }

        /// <summary>
        /// editCustomerCommand : Edits a Customer
        /// </summary>
        public SimpleCommand EditCustomerCommand
        {
            get { return editCustomerCommand; }
        }

        /// <summary>
        /// Returns the bindbable Menu options
        /// </summary>
        public List<WPFMenuItem> SearchMenuOptions
        {
            get
            {
                return CreateMenus();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and returns the menu items
        /// </summary>
        private List<WPFMenuItem> CreateMenus()
        {

            var menu = new List<WPFMenuItem>();


            var miEditCustomer = new WPFMenuItem("Edit Customer");
            miEditCustomer.Command = EditCustomerCommand;
            menu.Add(miEditCustomer);

            var miDeleteCustomer = new WPFMenuItem("Delete Customer");
            miDeleteCustomer.Command = DeleteCustomerCommand;
            menu.Add(miDeleteCustomer);

            return menu;

        }

        /// <summary>
        /// Is raised whenever a new Customer is selected from the list of Customers
        /// </summary>
        private void  MatchedCustomersCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            if (matchedCustomersCollectionView.CurrentItem != null)
            {
                CurrentCustomer = matchedCustomersCollectionView.CurrentItem as CustomerModel;
            }
        }


        /// <summary>
        /// Shows message to user stating matching customer count
        /// </summary>
        private void ShowUserMessageBasedOnResults()
        {
            String msg = MatchedCustomers == null || MatchedCustomers.Count == 0
                ? "There were no results" : 
                String.Format("There are {0} matches", MatchedCustomers.Count);
            
            messageBoxService.ShowInformation(msg);
        }


        /// <summary>
        /// Mediator callback from MainWindowViewModel
        /// </summary>
        /// <param name="okToDeleteCustomer">True if its ok to delete Customer</param>
        [MediatorMessageSink("OkToDeleteCustomerMessage", ParameterType = typeof(Boolean))]
        private void OkToDeleteCustomerMessageSink(Boolean okToDeleteCustomer)
        {
            if (okToDeleteCustomer)
            {
                try
                {
                    if (DataAccess.DataService.DeleteCustomer(CurrentCustomer.CustomerId.DataValue))
                    {
                        //now we need to tell SearchCustomersViewModel to refresh its list of
                        //customers as we deleted the customer that was selected
                        CurrentCustomer = null;
                        matchedCustomersCollectionView.MoveCurrentToPosition(-1);
                        DoSearchCommand.Execute(null);
                        messageBoxService.ShowInformation("Sucessfully deleted the Customer");
                    }
                    else
                        messageBoxService.ShowError("There was a problem deleting the Customer");
                }
                catch
                {
                    messageBoxService.ShowError("There was a problem deleting the Customer");
                }

            }
            else
                messageBoxService.ShowError("Can't delete Customer as it is currently being edited");
        }


        /// <summary>
        /// Validates the text for the filter
        /// </summary>
        private void ValidateFilterText()
        {
            if (IsCollectionProperty || IsNumeric)
            {
                Regex objIntPattern = new Regex("^[0-9]+$");
                if (!objIntPattern.IsMatch(currentFilterText))
                    CurrentFilterText = String.Empty;
            }
        }

        /// <summary>
        /// Updates the condition based on the current property picked
        /// </summary>
        private void propertiesCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            CurrentProperty = propertiesCollectionView.CurrentItem as PropertyInfo;
            if (CurrentProperty != null)
                if (CurrentProperty.PropertyType.AssemblyQualifiedName.Contains("Collections"))
                    IsCollectionProperty = true;
                else
                    IsCollectionProperty = false;


            AvailableConditions = GetPossibleConditionsForProperty();

            conditionCollectionView = CollectionViewSource.GetDefaultView(this.AvailableConditions);
            conditionCollectionView.MoveCurrentTo(this.AvailableConditions[0]);
            
            CurrentCondition = conditionCollectionView.CurrentItem.ToString();
            conditionCollectionView.CurrentChanged += conditionCollectionView_CurrentChanged;
        }

        /// <summary>
        /// Stores current condition
        /// </summary>
        private void conditionCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            CurrentCondition = conditionCollectionView.CurrentItem.ToString();
        }



        /// <summary>
        /// Creates all the form values
        /// </summary>
        private void CreateFormValues()
        {
            //get a list of properties
            AvailableProperties = this.BoundType.GetProperties().ToList();

            AvailableConditions = GetPossibleConditionsForProperty();

            propertiesCollectionView = CollectionViewSource.GetDefaultView(this.AvailableProperties);
            propertiesCollectionView.CurrentChanged += propertiesCollectionView_CurrentChanged;

            conditionCollectionView = CollectionViewSource.GetDefaultView(this.AvailableConditions);
            conditionCollectionView.CurrentChanged += conditionCollectionView_CurrentChanged;
        }



        /// <summary>
        /// Applies the new conditions
        /// </summary>
        private List<string> ApplyNewConditions()
        {
            //default values
            List<String> conds = new List<string>()
                                     {
                                         "=<",
                                         ">=",
                                         "<",
                                         ">",
                                         "==",
                                         "!="
                                     };

            //should we change from default
            if (IsString)
                conds = new List<string>()
                                     {
                                         "Equals",
                                         "StartsWith",
                                         "EndsWith",
                                         "Contains"
                                     };
            return conds;
        }

        /// <summary>
        /// Get list of conditions for a property
        /// </summary>
        private List<string> GetPossibleConditionsForProperty()
        {
            if (CurrentProperty != null)
            {

                if (CurrentProperty.PropertyType.FullName.Contains("Collection"))
                {
                    IsNumeric = true;
                    IsOperatorComparable = true;
                    IsString = false;
                }
                else
                {
                    String name = ReflectionHelper.GetGenericsForType(
                        CurrentProperty.PropertyType);

                    switch (name)
                    {
                        case "Int32":
                            IsNumeric = true;
                            IsOperatorComparable = true;
                            IsString = false;
                            break;
                        case "DateTime":
                            IsNumeric = false;
                            IsOperatorComparable = true;
                            IsString = false;
                            break;
                        case "String":
                            IsNumeric = false;
                            IsOperatorComparable = false;
                            IsString = true;
                            break;
                        default:
                            IsNumeric = false;
                            IsOperatorComparable = false;
                            IsString = true;
                            break;
                    }
                }


            }

            return ApplyNewConditions();
        }
        #endregion

        #region Command Implementation

        #region DoSearchCommand
        /// <summary>
        /// Executes the DoSearchCommand
        /// </summary>
        private void ExecuteDoSearchCommand()
        {
            DoSearchCommand.CommandSucceeded = false;

            try
            {
                List<Customer> customers =
                    DataAccess.DataService.FindMatchingCustomers(ClauseResult);

                //convert to UI type objects
                MatchedCustomers = customers.ConvertAll(
                    new Converter<Customer, CustomerModel>(CustomerModel.CustomerToCustomerModel));
            }
            catch(Exception ex)
            {
                messageBoxService.ShowError("There was a problem fetching the Customers");
            }


            DoSearchCommand.CommandSucceeded = true;

        }
        #endregion

        #region DeleteCustomerCommand
        /// <summary>
        /// Logic to determine if DeleteCustomerCommand can execute
        /// </summary>
        private Boolean CanExecuteDeleteCustomerCommand
        {
            get
            {
                return CurrentCustomer != null;
            }
        }

        /// <summary>
        /// Executes the DeleteCustomerCommand
        /// </summary>
        private void ExecuteDeleteCustomerCommand()
        {

            DeleteCustomerCommand.CommandSucceeded = false;
            //Use the Mediator to send a Message to MainWindowViewModel to delete the
            //Customer if it not being editing already in another workspace
            Mediator.NotifyColleagues<CustomerModel>("DeleteCustomerMessage", CurrentCustomer);
            DeleteCustomerCommand.CommandSucceeded = true;
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
                return CurrentCustomer != null;
            }
        }

        /// <summary>
        /// Executes the EditCustomerCommand
        /// </summary>
        private void ExecuteEditCustomerCommand()
        {

            EditCustomerCommand.CommandSucceeded = false;
            //Use the Mediator to send a Message to MainWindowViewModel to add a new 
            //Workspace item to edit the customer
            Mediator.NotifyColleagues<CustomerModel>("EditCustomerMessage", CurrentCustomer);
            CurrentCustomer = null;
            matchedCustomersCollectionView.MoveCurrentToPosition(-1);
            EditCustomerCommand.CommandSucceeded = true;
        }
        #endregion

        #endregion

    }
}
