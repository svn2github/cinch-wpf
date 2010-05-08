using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Cinch;


namespace CinchCodeGen
{
    /// <summary>
    /// ViewModel for the <c>MainWindow</c>
    /// </summary>
    public class MainWindowViewModel : ValidatingViewModelBase
    {
        #region Data
        //Commands
        private SimpleCommand newVMCommand;
        private SimpleCommand openVMCommand;
        private InMemoryViewModel currentVM = null;
        private Boolean isValid = false;

        //Data
        private Boolean hasContent = false;


        //services
        private IMessageBoxService messageBoxService = null;
        private IOpenFileService openFileService = null;
        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
                openFileService = Resolve<IOpenFileService>();
            }
            catch
            {
                Logger.Error( "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion

            #region Commands

            //New VM Command
            newVMCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteNewVMCommand,
                ExecuteDelegate = x => ExecuteNewVMCommand()
            };


            //Open VM Command
            openVMCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteOpenVMCommand,
                ExecuteDelegate = x => ExecuteOpenVMCommand()
            };
            #endregion


        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new InMemoryViewModel by reading the persisted XML file from disk
        /// </summary>
        private void HydrateViewModelFromXml()
        {

            //Ask the user where they want to open the file from, and open it
            try
            {
                openFileService.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileService.FileName = String.Empty;
                openFileService.Filter = "Xml files (*.xml)|*.xml";
                bool? result = openFileService.ShowDialog(null);

                if (result.HasValue && result.Value)
                {
                    //open to XML
                    PesistentVM pesistentVM =
                        ViewModelPersistence.HydratePersistedViewModel(openFileService.FileName);
                    //check we got something, and recreate the full weight InMemoryViewModel from
                    //the lighter weight XML read PesistentVM
                    if (pesistentVM != null)
                    {
                        CurrentVM = new InMemoryViewModel();

                        //Start out with PropertiesViewModel shown
                        PropertiesViewModel propertiesViewModel =
                            new PropertiesViewModel();
                        propertiesViewModel.IsCloseable = false;
                        CurrentVM.PropertiesVM = propertiesViewModel;
                        //and now read in other data
                        CurrentVM.ViewModelName = pesistentVM.VMName;
                        CurrentVM.CurrentViewModelType = pesistentVM.VMType;
                        CurrentVM.ViewModelNamespace = pesistentVM.VMNamespace;
                        //and add in the individual properties
                        foreach (var prop in pesistentVM.VMProperties)
                        {
                            CurrentVM.PropertiesVM.PropertyVMs.Add(new
                            SinglePropertyViewModel
                            {
                                PropertyType = prop.PropertyType,
                                PropName = prop.PropName,
                                UseDataWrapper = prop.UseDataWrapper
                            });
                        }

                        HasContent = true;
                    }
                    else
                    {
                        messageBoxService.ShowError(String.Format("Could not open the ViewModel {0}",
                        openFileService.FileName));
                    }

                    }

            }
            catch (Exception ex)
            {
                messageBoxService.ShowError("An error occurred trying to Opening the ViewModel\r\n" +
                    ex.Message);
            }
        }
        #endregion

        #region Mediator Message Sinks
        /// <summary>
        /// Mediator callback from InMemoryViewModel to signal that this ViewModel
        /// should check its valid flag again
        /// </summary>
        /// <param name="dummy">dummy data, not important</param>
        [MediatorMessageSink("CheckInMemoryVMValidPart1")]
        private void CheckInMemoryVMValidPart1Sink(Boolean dummy)
        {
            IsValid = this.CurrentVM.IsValid;
        }

        /// <summary>
        /// Mediator callback from InMemoryViewModel.PropertiesVM to signal that this ViewModel
        /// should check its valid flag again
        /// </summary>
        /// <param name="dummy">dummy data, not important</param>
        [MediatorMessageSink("CheckInMemoryVMValidPart2")]
        private void CheckInMemoryVMValidPart2Sink(Boolean dummy)
        {
            IsValid = this.CurrentVM.IsValid;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// NewVMCommand : New VM Command
        /// </summary>
        public SimpleCommand NewVMCommand
        {
            get { return newVMCommand; }
        }

        /// <summary>
        /// openVMCommand : Open VM Command
        /// </summary>
        public SimpleCommand OpenVMCommand
        {
            get { return openVMCommand; }
        }

        /// <summary>
        /// NewVMCommand : New VM Command
        /// </summary>
        static PropertyChangedEventArgs currentVMChangeArgs =
            ObservableHelper.CreateArgs<MainWindowViewModel>(x => x.CurrentVM);

        public InMemoryViewModel CurrentVM
        {
            get { return currentVM; }
            set
            {
                if (currentVM != value)
                {
                    currentVM = value;
                    HasContent = currentVM != null;
                    NotifyPropertyChanged(currentVMChangeArgs);
                }
            }
        }


        /// <summary>
        /// IsValid : Embedded InMemory ViewModel IsValid
        /// </summary>
        static PropertyChangedEventArgs isValidChangeArgs =
            ObservableHelper.CreateArgs<MainWindowViewModel>(x => x.IsValid);

        public Boolean IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    NotifyPropertyChanged(isValidChangeArgs);
                }
            }
        }


        /// <summary>
        /// HasContent : True if the CurrentVM is not null
        /// </summary>
        static PropertyChangedEventArgs hasContentChangeArgs =
            ObservableHelper.CreateArgs<MainWindowViewModel>(x => x.HasContent);

        public Boolean HasContent
        {
            get { return hasContent; }
            set
            {
                if (hasContent != value)
                {
                    hasContent = value;
                    NotifyPropertyChanged(hasContentChangeArgs);
                }

            }
        }
        #endregion
 
        #region Command Implementations

        #region NewVMCommand
        /// <summary>
        /// Logic to determine if NewVMCommand can execute
        /// </summary>
        private Boolean CanExecuteNewVMCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the NewVMCommand
        /// </summary>
        private void ExecuteNewVMCommand()
        {
            if (CurrentVM != null)
            {
                if (messageBoxService.ShowYesNo("There is an active ViewModel, loose edits?",
                    CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    CurrentVM = new InMemoryViewModel();
                    //Start out with PropertiesViewModel shown
                    PropertiesViewModel propertiesViewModel =
                        new PropertiesViewModel();
                    propertiesViewModel.IsCloseable = false;
                    CurrentVM.PropertiesVM = propertiesViewModel;
            }
            else
            {
                CurrentVM = new InMemoryViewModel();
                //Start out with PropertiesViewModel shown
                PropertiesViewModel propertiesViewModel =
                    new PropertiesViewModel();
                propertiesViewModel.IsCloseable = false;
                CurrentVM.PropertiesVM = propertiesViewModel;
            }


        }
        #endregion

        #region OpenVMCommand
        /// <summary>
        /// Logic to determine if OpenVMCommand can execute
        /// </summary>
        private Boolean CanExecuteOpenVMCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the OpenVMCommand
        /// </summary>
        private void ExecuteOpenVMCommand()
        {
            if (CurrentVM != null)
            {
                if (messageBoxService.ShowYesNo("There is currently a ViewModel open\r\n" +
                        "Do you wish to loose the current ViewModel by opening a saved ViewModel?",
                        CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    HydrateViewModelFromXml();
            }
            else
            {
                HydrateViewModelFromXml();
            }
        }
        #endregion


        #endregion
    }
}
