using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Cinch;


namespace CinchCodeGen
{


    /// <summary>
    /// Represents a single ViewModel property, and is used in conjunction
    /// with the <c>SinglePropertyView</c>
    /// </summary>
    public class SinglePropertyViewModel : ValidatingViewModelBase
    {
        #region Data
        //Data
        private String propName=String.Empty;
        private String propertyType = String.Empty;
        private Boolean useDataWrapper = false;

        //Commands
        private SimpleCommand removePropertyCommand;
        
        //services
        private IMessageBoxService messageBoxService = null;
        #endregion

        #region Ctor
        public SinglePropertyViewModel()
        {
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

            #region Commands

            //RemovePropertyCommand
            removePropertyCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteRemovePropertyCommand,
                ExecuteDelegate = x => ExecuteRemovePropertyCommand()
            };
            #endregion

            #region Create Validation Rules

            this.AddRule(new SimpleRule(propNameChangeArgs.PropertyName, 
                    "Property Name can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.PropName);
                      }));
            this.AddRule(new SimpleRule(propertyTypeChangeArgs.PropertyName, 
                    "Property Type can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.PropertyType);
                      }));
            #endregion

        }
        #endregion
 
        #region Public Properties

        /// <summary>
        /// RemovePropertyCommand : Removes a new SinglePropertyViewModel
        /// </summary>
        public SimpleCommand RemovePropertyCommand
        {
            get { return removePropertyCommand; }
        }

        /// <summary>
        /// PropName : The Property Name
        /// </summary>
        static PropertyChangedEventArgs propNameChangeArgs =
            ObservableHelper.CreateArgs<SinglePropertyViewModel>(x => x.PropName);

        public String PropName
        {
            get { return propName; }
            set
            {
                if (propName != value)
                {
                    propName = value;
                    NotifyPropertyChanged(propNameChangeArgs);
                    Mediator.NotifyColleagues<SinglePropertyViewModel>("DefinePropertyNameMessage", this);
                    Mediator.NotifyColleagues<Boolean>("CheckInMemoryVMValidPart2", true);
                }

            }
        }

        /// <summary>
        /// PropertyType : The Property Type
        /// </summary>
        static PropertyChangedEventArgs propertyTypeChangeArgs =
            ObservableHelper.CreateArgs<SinglePropertyViewModel>(x => x.PropertyType);

        public String PropertyType
        {
            get { return propertyType; }
            set
            {
                if (propertyType != value)
                {
                    propertyType = value;
                    NotifyPropertyChanged(propertyTypeChangeArgs);
                }
            }
        }


        /// <summary>
        /// UseDataWrapper : True if property should use DataWrapper
        /// </summary>
        static PropertyChangedEventArgs useDataWrapperChangeArgs =
            ObservableHelper.CreateArgs<SinglePropertyViewModel>(x => x.UseDataWrapper);

        public Boolean UseDataWrapper
        {
            get { return useDataWrapper; }
            set
            {
                if (useDataWrapper != value)
                {
                    useDataWrapper = value;
                    NotifyPropertyChanged(useDataWrapperChangeArgs);
                }

            }
        }
        #endregion

        #region Command Implementations

        #region RemovePropertyCommand
        /// <summary>
        /// Logic to determine if RemovePropertyCommand can execute
        /// </summary>
        private Boolean CanExecuteRemovePropertyCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the RemovePropertyCommand
        /// </summary>
        private void ExecuteRemovePropertyCommand()
        {
            Mediator.NotifyColleagues<SinglePropertyViewModel>("RemovePropertyMessage", this);
        }
        #endregion

        #endregion
    }
}
