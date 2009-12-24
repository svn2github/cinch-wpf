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
    /// Holds a number of <c>SinglePropertyViewModels</c> which repesent
    /// the ViewModels properties
    /// </summary>
    public class PropertiesViewModel : ValidatingViewModelBase
    {
        #region Data
        //Commands
        private SimpleCommand addNewPropertyCommand;
        private SimpleCommand editPropertiesCommand;
        private SimpleCommand editReferencedAssembliesCommand;

        //Data
        private ObservableCollection<SinglePropertyViewModel> propertyVMs;
        private PropertyTypesViewModel propertyTypesVM = new PropertyTypesViewModel();
        private Dictionary<SinglePropertyViewModel, String> oldPropertyTypeValues =
            new Dictionary<SinglePropertyViewModel, String>();

        private ReferencedAssembliesViewModel referencedAssembliesVM 
            = new ReferencedAssembliesViewModel();


        //services
        private IMessageBoxService messageBoxService = null;
        private IUIVisualizerService uiVisualizerService = null;
        #endregion

        #region Ctor
        public PropertiesViewModel()
        {

            #region Commands

            //AddNewPropertyCommand
            addNewPropertyCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteAddNewPropertyCommand,
                ExecuteDelegate = x => ExecuteAddNewPropertyCommand()
            };

            //EditPropertiesCommand
            editPropertiesCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditPropertiesCommand,
                ExecuteDelegate = x => ExecuteEditPropertiesCommand()
            };

            //EditReferencedAssembliesCommand
            editReferencedAssembliesCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteEditReferencedAssembliesCommand,
                ExecuteDelegate = x => ExecuteEditReferencedAssembliesCommand()
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

            PropertyVMs = new ObservableCollection<SinglePropertyViewModel>();
            this.DisplayName = "Add Some Properties";

        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns true if all the nested SinglePropertyViewModel are Valid 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                Boolean isValid = true;
                foreach (var item in PropertyVMs)
                {
                    isValid &= item.IsValid;
                }

                return isValid;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Restores old property types from backup dictionary
        /// </summary>
        private void WriteOldPropertyValues()
        {
            //now re-apply the old property values that were selected before if we can
            //to the SinglePropertyViewModel that were shown (if any)
            foreach (SinglePropertyViewModel vm in PropertyVMs)
            {
                String oldPropertyTypeValue = oldPropertyTypeValues[vm];
                if (propertyTypesVM.PropertyTypes.Contains(oldPropertyTypeValue))
                {
                    vm.PropertyType = String.Empty;
                    vm.PropertyType = oldPropertyTypeValue;
                    NotifyPropertyChanged(propertyVMsChangeArgs.PropertyName);

                }
                else
                {
                    vm.PropertyType = String.Empty;
                    NotifyPropertyChanged(propertyVMsChangeArgs.PropertyName);
                }

            }
        }


        #endregion

        #region Mediator Message Sinks
        /// <summary>
        /// Mediator callback from SinglePropertyViewModel to state that a SinglePropertyViewModel
        /// should be deleted from the ObservableCollection<SinglePropertyViewModel>
        /// </summary>
        /// <param name="propertyToRemove">SinglePropertyViewModel to be removed</param>
        [MediatorMessageSink("RemovePropertyMessage", ParameterType = typeof(SinglePropertyViewModel))]
        private void RemovePropertyMessageSink(SinglePropertyViewModel propertyToRemove)
        {
            PropertyVMs.Remove(propertyToRemove);
        }


        /// <summary>
        /// Mediator callback from SinglePropertyViewModel to see if the currently selected
        /// Property Name can be added. Send message back to PropertyTypesViewModel to see if the
        /// currently selected Property Name is not is use anywhere else.
        /// </summary>
        /// <param name="propertyToRemove">Property Type to be removed</param>
        [MediatorMessageSink("DefinePropertyNameMessage", ParameterType = typeof(SinglePropertyViewModel))]
        private void DefinePropertyNameMessageSink(SinglePropertyViewModel currentPropVM)
        {
            if (currentPropVM.PropName == String.Empty)
                return;

            Int32 existingProperties = 
                PropertyVMs.Count(x => x.PropName == currentPropVM.PropName);

            if (existingProperties > 1 && PropertyVMs.Count > 1)
            {
                PropertyVMs.Remove(currentPropVM);
                messageBoxService.ShowError("That Property Name is in use elsewhere, please retry");
            }
             
        }


        /// <summary>
        /// Mediator callback from PropertyTypesViewModel to see if the currently selected
        /// Property Type can be removed. Send message back to PropertyTypesViewModel if the
        /// currently selected Property Type is not is use anywhere else.
        /// </summary>
        /// <param name="propertyToRemove">Property Type to be removed</param>
        [MediatorMessageSink("RemovePropertyTypeMessage", ParameterType = typeof(String))]
        private void RemovePropertyTypeMessageSink(String propertyToRemove)
        {
            Int32 existingProperties =
                oldPropertyTypeValues.Values.AsEnumerable().Count(x => x == propertyToRemove);

            if (existingProperties == 0)
                Mediator.NotifyColleagues<Boolean>("OkRemovePropertyTypeMessage", true);
            else
                Mediator.NotifyColleagues<Boolean>("OkRemovePropertyTypeMessage", false);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// AddNewPropertyCommand : Adds a new SinglePropertyViewModel
        /// </summary>
        public SimpleCommand AddNewPropertyCommand
        {
            get { return addNewPropertyCommand; }
        }

        /// <summary>
        /// EditPropertiesCommand : Shows the PropertyListPopup
        /// </summary>
        public SimpleCommand EditPropertiesCommand
        {
            get { return editPropertiesCommand; }
        }

        /// <summary>
        /// EditReferencedAssembliesCommand : Shows the ReferencedAssembliesPopup
        /// </summary>
        public SimpleCommand EditReferencedAssembliesCommand
        {
            get { return editReferencedAssembliesCommand; }
        }

        /// <summary>
        /// ViewModelName : The View Models Name
        /// </summary>
        static PropertyChangedEventArgs propertyVMsChangeArgs =
            ObservableHelper.CreateArgs<PropertiesViewModel>(x => x.PropertyVMs);

        public ObservableCollection<SinglePropertyViewModel> PropertyVMs
        {
            get { return propertyVMs; }
            set
            {
                if (propertyVMs != value)
                {
                    propertyVMs = value;
                    NotifyPropertyChanged(propertyVMsChangeArgs);
                }

            }
        }

        #endregion

        #region Command Implementations

        #region AddNewPropertyCommand
        /// <summary>
        /// Logic to determine if AddNewPropertyCommand can execute
        /// </summary>
        private Boolean CanExecuteAddNewPropertyCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the AddNewPropertyCommand
        /// </summary>
        private void ExecuteAddNewPropertyCommand()
        {
            PropertyVMs.Add(new SinglePropertyViewModel());
        }
        #endregion

        #region EditPropertiesCommand
        /// <summary>
        /// Logic to determine if EditPropertiesCommand can execute
        /// </summary>
        private Boolean CanExecuteEditPropertiesCommand
        {
            get
            {
                return true;
            }
        }

         /// <summary>
        /// Executes the EditPropertiesCommand
        /// </summary>
        private void ExecuteEditPropertiesCommand()
        {
            try
            {
                //Clear old selected PropertyTypes and create new list to check
                //against when user finishes editing list of available/wanted Property types
                oldPropertyTypeValues.Clear();
                foreach (SinglePropertyViewModel vm in PropertyVMs)
                    oldPropertyTypeValues.Add(vm, vm.PropertyType);

                ////read in the currently available types
                var props = PropertyTypeHelper.ReadCurrentlyAvailablePropertyTypes();
                propertyTypesVM.PropertyTypes.Clear();
                propertyTypesVM.PropertyTypes = props;

                //allow user to edit list of Property Types, and write them to disk
                bool? result = uiVisualizerService.ShowDialog("PropertyListPopup", propertyTypesVM);

                if (result.HasValue && result.Value)
                {
                    PropertyTypeHelper.WriteCurrentlyAvailablePropertyTypes(propertyTypesVM.PropertyTypes);
                }
                WriteOldPropertyValues();
             }
            catch
            {
                messageBoxService.ShowError(
                    "There was a problem obtaining the list of available property types");
            }


        }
        #endregion

        #region EditReferencedAssembliesCommand
        /// <summary>
        /// Logic to determine if EditReferencedAssembliesCommand can execute
        /// </summary>
        private Boolean CanExecuteEditReferencedAssembliesCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the EditReferencedAssembliesCommand
        /// </summary>
        private void ExecuteEditReferencedAssembliesCommand()
        {
            try
            {

                referencedAssembliesVM.ReferencedAssemblies.Clear();
                foreach (var refAss in  (App.Current as App).ReferencedAssemblies)
                {
                    referencedAssembliesVM.ReferencedAssemblies.Add(refAss);
                }

                //allow user to edit list of Referenced Assemblies, and write them to disk
                bool? result = uiVisualizerService.ShowDialog("ReferencedAssembliesPopup",
                    referencedAssembliesVM);

                if (result.HasValue && result.Value)
                {
                    ReferencedAssembliesHelper.WriteCurrentlyReferencedAssemblies(
                        referencedAssembliesVM.ReferencedAssemblies);
                }

            }
            catch
            {
                messageBoxService.ShowError(
                    "There was a problem obtaining the list of referenced assemblies");
            }


        }
        #endregion


        #endregion


    }
}
