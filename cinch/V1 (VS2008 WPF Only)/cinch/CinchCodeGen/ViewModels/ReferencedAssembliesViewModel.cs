using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;

using Cinch;
using System.IO;




namespace CinchCodeGen
{
    /// <summary>
    /// Holds and manages a list of available referenced assemblies
    /// and is used in conjunction with the <c>ReferencedAssembliesPopup</c>
    /// </summary>
    public class ReferencedAssembliesViewModel : ValidatingViewModelBase
    {
        #region Data
        //Data
        private ObservableCollection<FileInfo> referencedAssemblies;
        private ICollectionView referencedAssembliesCV = null;

        //Commands
        private SimpleCommand addNewAssemblyCommand;
        private SimpleCommand removeAssemblyCommand;

        //services
        private IMessageBoxService messageBoxService = null;
        private IOpenFileService openFileService = null;
        private IUIVisualizerService uiVisualizerService = null;
        #endregion

        #region Ctor
        public ReferencedAssembliesViewModel()
        {
            #region Commands

            //AddNewPropertyTypeCommand
            addNewAssemblyCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteAddNewAssemblyCommand,
                ExecuteDelegate = x => ExecuteAddNewAssemblyCommand()
            };

            //RemovePropertyTypeCommand
            removeAssemblyCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteRemoveAssemblyCommand,
                ExecuteDelegate = x => ExecuteRemoveAssemblyCommand()
            };
            #endregion

            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
                uiVisualizerService = Resolve<IUIVisualizerService>();
                openFileService = Resolve<IOpenFileService>();
            }
            catch
            {
                Logger.Error( "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion

            referencedAssemblies = new ObservableCollection<FileInfo>();
            referencedAssembliesCV = CollectionViewSource.GetDefaultView(referencedAssemblies);
            referencedAssembliesCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// AddNewAssemblyCommand : Adds a new referenced assembly
        /// </summary>
        public SimpleCommand AddNewAssemblyCommand
        {
            get { return addNewAssemblyCommand; }
        }

        /// <summary>
        /// RemoveAssemblyCommand : Removes a referenced assembly
        /// </summary>
        public SimpleCommand RemoveAssemblyCommand
        {
            get { return removeAssemblyCommand; }
        }

        /// <summary>
        /// ViewModelName : The View Models Name
        /// </summary>
        static PropertyChangedEventArgs referencedAssembliesChangeArgs =
            ObservableHelper.CreateArgs<ReferencedAssembliesViewModel>(x => x.ReferencedAssemblies);

        public ObservableCollection<FileInfo> ReferencedAssemblies
        {
            get { return referencedAssemblies; }
            set
            {
                if (referencedAssemblies != value)
                {
                    referencedAssemblies = value;
                    referencedAssembliesCV = CollectionViewSource.GetDefaultView(referencedAssemblies);
                    referencedAssembliesCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    NotifyPropertyChanged(referencedAssembliesChangeArgs);
                }

            }
        }
        #endregion

        #region Command Implementations

        #region AddNewAssemblyCommand
        /// <summary>
        /// Logic to determine if AddNewAssemblyCommand can execute
        /// </summary>
        private Boolean CanExecuteAddNewAssemblyCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the AddNewAssemblyCommand
        /// </summary>
        private void ExecuteAddNewAssemblyCommand()
        {
            //Ask the user where they want to open the file from, and open it
            try
            {
                openFileService.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileService.FileName = String.Empty;
                openFileService.Filter = "Dll files (*.dll)|*.dll";
                bool? result = openFileService.ShowDialog(null);

                if (result.HasValue && result.Value)
                {
                    FileInfo file = new FileInfo(openFileService.FileName);
                    if(file.Extension.ToLower().Equals(".dll"))
                    {
                        this.referencedAssemblies.Add(file);
                    }
                    else
                    {
                        messageBoxService.ShowError(String.Format("The file {0} is not a Dll",file.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                messageBoxService.ShowError("An error occurred trying to Open the file\r\n" +
                    ex.Message);
            }

        }
        #endregion

        #region RemoveAssemblyCommand
        /// <summary>
        /// Logic to determine if RemoveAssemblyCommand can execute
        /// </summary>
        private Boolean CanExecuteRemoveAssemblyCommand
        {
            get
            {
                return referencedAssembliesCV.CurrentItem != null;
            }
        }

        /// <summary>
        /// Executes the RemovePropertyTypeCommand
        /// </summary>
        private void ExecuteRemoveAssemblyCommand()
        {
            this.referencedAssemblies.Remove((FileInfo)referencedAssembliesCV.CurrentItem);
        }
        #endregion


        #endregion
    }
}
