using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;

using Cinch;




namespace CinchCodeGen
{
    /// <summary>
    /// Holds and manages a list of available property types
    /// and is used in conjunction with the <c>PropertyListPopup</c>
    /// </summary>
    public class PropertyTypesViewModel : ValidatingViewModelBase
    {
        #region Data
        //Data
        private ObservableCollection<String> propertyTypes;
        private ICollectionView propertyTypesCV = null;
        private TextEntryViewModel textEntryViewModel = new TextEntryViewModel();

        //Commands
        private SimpleCommand addNewPropertyTypeCommand;
        private SimpleCommand removePropertyTypeCommand;

        //services
        private IMessageBoxService messageBoxService = null;
        private IUIVisualizerService uiVisualizerService = null;
        #endregion

        #region Ctor
        public PropertyTypesViewModel()
        {
            #region Commands

            //AddNewPropertyTypeCommand
            addNewPropertyTypeCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteAddNewPropertyTypeCommand,
                ExecuteDelegate = x => ExecuteAddNewPropertyTypeCommand()
            };

            //RemovePropertyTypeCommand
            removePropertyTypeCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteRemovePropertyTypeCommand,
                ExecuteDelegate = x => ExecuteRemovePropertyTypeCommand()
            };
            #endregion

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

            propertyTypes = new ObservableCollection<String>();
            propertyTypesCV = CollectionViewSource.GetDefaultView(propertyTypes);
            propertyTypesCV.SortDescriptions.Add(new SortDescription());

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// AddNewPropertyTypeCommand : Adds a new available Property Type
        /// </summary>
        public SimpleCommand AddNewPropertyTypeCommand
        {
            get { return addNewPropertyTypeCommand; }
        }

        /// <summary>
        /// RemovePropertyTypeCommand : Removes an available Property Type
        /// </summary>
        public SimpleCommand RemovePropertyTypeCommand
        {
            get { return removePropertyTypeCommand; }
        }

        /// <summary>
        /// ViewModelName : The View Models Name
        /// </summary>
        static PropertyChangedEventArgs propertyTypesChangeArgs =
            ObservableHelper.CreateArgs<PropertyTypesViewModel>(x => x.PropertyTypes);

        public ObservableCollection<String> PropertyTypes
        {
            get { return propertyTypes; }
            set
            {
                if (propertyTypes != value)
                {
                    propertyTypes = value;
                    propertyTypesCV = CollectionViewSource.GetDefaultView(propertyTypes);
                    propertyTypesCV.SortDescriptions.Add(new SortDescription());
                    NotifyPropertyChanged(propertyTypesChangeArgs);
                }

            }
        }

        /// <summary>
        /// TextEntryVM : TextEntryViewModel to use to enter text
        /// </summary>
        static PropertyChangedEventArgs textEntryViewModelChangeArgs =
            ObservableHelper.CreateArgs<PropertyTypesViewModel>(x => x.TextEntryVM);

        public TextEntryViewModel TextEntryVM
        {
            get { return textEntryViewModel; }
            set
            {
                if (textEntryViewModel != value)
                {
                    textEntryViewModel = value;
                    NotifyPropertyChanged(textEntryViewModelChangeArgs);
                }

            }
        }

        #endregion

        #region Mediator Message Sinks
        /// <summary>
        /// Mediator callback from PropertyTypesViewModel to see if the currently selected
        /// Property Type can be removed. Send message back to PropertyTypesViewModel if the
        /// currently selected Property Type is not is use anywhere else.
        /// </summary>
        /// <param name="propertyToRemove">Property Type to be removed</param>
        [MediatorMessageSink("OkRemovePropertyTypeMessage")]
        private void OkRemovePropertyTypeMessageSink(Boolean OkToRemoveSelectedProperty)
        {
            if (OkToRemoveSelectedProperty)
            {
                propertyTypes.Remove(propertyTypesCV.CurrentItem as String);
                propertyTypesCV.MoveCurrentToPosition(-1);
                propertyTypesCV.MoveCurrentTo(null);
            }
            else
                messageBoxService.ShowError(
                    "Can't delete the current Property Type as it currently being used");
        }
        #endregion

        #region Command Implementations

        #region AddNewPropertyTypeCommand
        /// <summary>
        /// Logic to determine if AddNewPropertyTypeCommand can execute
        /// </summary>
        private Boolean CanExecuteAddNewPropertyTypeCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the AddNewPropertyTypeCommand
        /// </summary>
        private void ExecuteAddNewPropertyTypeCommand()
        {
            TextEntryVM = new TextEntryViewModel();


            bool? result = uiVisualizerService.ShowDialog("StringEntryPopup", TextEntryVM);

            if (result.HasValue && result.Value)
            {
                if (!this.PropertyTypes.Contains(TextEntryVM.CurrentPropertyType))
                {
                    this.PropertyTypes.Add(TextEntryVM.CurrentPropertyType);
                }
                else
                    messageBoxService.ShowError("A property with that type already exists");
            }

        }
        #endregion

        #region RemovePropertyTypeCommand
        /// <summary>
        /// Logic to determine if RemovePropertyTypeCommand can execute
        /// </summary>
        private Boolean CanExecuteRemovePropertyTypeCommand
        {
            get
            {
                return propertyTypesCV.CurrentItem != null;
            }
        }

        /// <summary>
        /// Executes the RemovePropertyTypeCommand
        /// </summary>
        private void ExecuteRemovePropertyTypeCommand()
        {
            Mediator.NotifyColleagues<String>("RemovePropertyTypeMessage", propertyTypesCV.CurrentItem as String);
        }
        #endregion


        #endregion
    }
}
