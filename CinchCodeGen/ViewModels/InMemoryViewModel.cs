using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;

using Cinch;
using System.IO;




namespace CinchCodeGen
{
    /// <summary>
    /// The type for the ViewModel 
    /// </summary>
    public enum ViewModelType { Standard, Validating, ValidatingAndEditable}

    /// <summary>
    /// Save or generate
    /// </summary>
    public enum SaveOrGenerate { Save, Generate}

    /// <summary>
    /// Represents a single ViewModel with a name/namespace/type and properties
    /// </summary>
    public class InMemoryViewModel : ValidatingViewModelBase
    {
        #region Data
        //Commands
        private SimpleCommand saveVMCommand;
        private SimpleCommand generateVMCommand;
  

        //Data
        private String viewModelName = null;
        private String viewmodelNamespace = null;
        private ViewModelType currentViewModelType = ViewModelType.Standard;
        private ObservableCollection<ViewModelBase> workspaces;
        private PropertiesViewModel propertiesViewModel = null;

        //services
        private IMessageBoxService messageBoxService = null;
        private ISaveFileService saveFileService = null;
        #endregion

        #region Ctor
        public InMemoryViewModel()
        {
            #region Obtain Services
            try
            {
                messageBoxService = Resolve<IMessageBoxService>();
                saveFileService = Resolve<ISaveFileService>();
            }
            catch
            {
                Logger.Log(LogType.Error, "Error resolving services");
                throw new ApplicationException("Error resolving services");
            }
            #endregion

            #region Commands

            //Save VM Command
            saveVMCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteSaveVMCommand,
                ExecuteDelegate = x => ExecuteSaveVMCommand()
            };

            //Generate VM Command
            generateVMCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => CanExecuteGenerateVMCommand,
                ExecuteDelegate = x => ExecuteGenerateVMCommand()
            };


            #endregion

            Workspaces = new ObservableCollection<ViewModelBase>();
            Workspaces.CollectionChanged += this.OnWorkspacesChanged;

            #region Create Validation Rules
            this.AddRule(new SimpleRule(viewModelNameChangeArgs.PropertyName,
                         "ViewModelName can't be empty",
                           delegate
                           {
                               return String.IsNullOrEmpty(this.ViewModelName);
                           }));
           
            this.AddRule(new SimpleRule(viewModelNameChangeArgs.PropertyName,
                         "ViewModelName can't contain spaces",
                           delegate
                           {
                               if (String.IsNullOrEmpty(this.ViewModelName))
                               {
                                   return true;
                               }
                               else
                               {
                                   return this.ViewModelName.Contains(" ");
                               }
                           }));

            this.AddRule(new SimpleRule(viewModelNamespaceChangeArgs.PropertyName,
                         "ViewModelNamespace can't be empty",
                           delegate
                           {
                               return String.IsNullOrEmpty(this.ViewModelNamespace);
                           }));
            
            this.AddRule(new SimpleRule(viewModelNamespaceChangeArgs.PropertyName,
                         "ViewModelNamespace can't contain spaces",
                           delegate
                           {
                               if (String.IsNullOrEmpty(this.ViewModelNamespace))
                               {
                                   return true;
                               }
                               else
                               {
                                   return this.ViewModelNamespace.Contains(" ");
                               }
                           }));
            #endregion
        }
        #endregion

        #region Overrides
        public override bool IsValid
        {
            get
            {
                Boolean propertiesViewModelIsValid = propertiesViewModel == null ?
                    false : propertiesViewModel.IsValid;

                return base.IsValid && propertiesViewModelIsValid;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// If we get a request to add a new Workspace, add a new WorkSpace to the 
        /// collection and hook up the CloseWorkSpace event in a weak manner
        /// </summary>
        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (ViewModelBase workspace in e.NewItems)
                    workspace.CloseWorkSpace +=
                         new EventHandler<EventArgs>(OnCloseWorkSpace).
                             MakeWeak(eh => workspace.CloseWorkSpace -= eh);
        }

        /// <summary>
        /// If we get a request to close a new Workspace, remove the WorkSpace from the 
        /// collection
        /// </summary>
        private void OnCloseWorkSpace(object sender, EventArgs e)
        {
            ViewModelBase workspace = sender as ViewModelBase;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        /// <summary>
        /// Sets a ViewModel to be active, which for the View equates
        /// to selected Tab
        /// </summary>
        /// <param name="workspace">workspace to activate</param>
        private void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView =
                CollectionViewSource.GetDefaultView(this.Workspaces);

            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        /// <summary>
        /// Shows a Save/Generate messagebox
        /// </summary>
        /// <param name="success">true if operation was successful</param>
        /// <param name="filename">the filename</param>
        /// <param name="operation">the operation type</param>
        private void ShowSaveOrGenMessage(Boolean success,String filename, SaveOrGenerate operation)
        {

            if (!success)
            {
                messageBoxService.ShowError(
                    String.Format("There was a problem {0} the\r\nViewModel file : {1}",
                    operation == SaveOrGenerate.Save ? "Saving" : "Generating",
                    filename));
            }
            else
            {
                messageBoxService.ShowInformation(
                    String.Format("Successfully {0} the\r\nViewModel file : {1}",
                    operation == SaveOrGenerate.Save ? "Saved" : "Generated",
                    filename));
            }
        }

        /// <summary>
        /// Saves the current ViewModel as an XML file or generates C# code for the
        /// current ViewModel
        /// </summary>
        /// <param name="filter">The save file filter to use</param>
        /// <param name="operation">The SaveOrGenerate operation to use</param>
        private Tuple<Boolean,String> SaveOrGenerateOperation(String filter, SaveOrGenerate operation)
        {
            if (!IsValid)
            {
                messageBoxService.ShowError("The current ViewModel is InValid\r\nPlease fix it then retry");
                return null;
            }
            else
            {
                //Ask the user where they want to save the file, and save it
                try
                {
                    saveFileService.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    saveFileService.FileName = String.Empty;
                    saveFileService.Filter = filter;
                    saveFileService.OverwritePrompt = true;
                    bool? result = saveFileService.ShowDialog(null);

                    if (result.HasValue && result.Value)
                    {
                        //Create a Persistence ViewModel based on the current In Memory ViewModel
                        PesistentVM pesistentVM = new PesistentVM();
                        pesistentVM.VMName = ViewModelName;
                        pesistentVM.VMType = CurrentViewModelType;
                        pesistentVM.VMNamespace = ViewModelNamespace;

                        foreach (SinglePropertyViewModel propVM in propertiesViewModel.PropertyVMs)
                        {
                            pesistentVM.VMProperties.Add(new
                                PesistentVMSingleProperty
                            {
                                PropertyType = propVM.PropertyType,
                                PropName = propVM.PropName,
                                UseDataWrapper = propVM.UseDataWrapper,
                                ParentViewModelName = ViewModelName
                            });
                        }


                        bool success = false;

                        FileInfo file = new FileInfo(saveFileService.FileName);

                        //decide what file needs saving/generating
                        switch (operation)
                        {
                            case SaveOrGenerate.Save:
                                //save to XML
                                success = ViewModelPersistence.PersistViewModel(
                                    saveFileService.FileName,pesistentVM);
                                ShowSaveOrGenMessage(success, file.Name, SaveOrGenerate.Save);
                                return TupleHelper.New(success, saveFileService.FileName);
                            case SaveOrGenerate.Generate:
                                //generate code
                                success = ViewModelPersistence.CreateViewModelCode(
                                    saveFileService.FileName, pesistentVM);
                                ShowSaveOrGenMessage(success, file.Name, SaveOrGenerate.Generate);
                                return TupleHelper.New(success, saveFileService.FileName);
                        }
                    }

                }
                catch (Exception ex)
                {
                    messageBoxService.ShowError(
                        String.Format("An error occurred trying to {0} the ViewModel\r\n{1}",
                        operation.ToString(), ex.Message));
                    return null;
                }
                return null;
            }
        }

        /// <summary>
        /// Clears all code generator workspaces
        /// </summary>
        private void ClearCodeWorkSpaces()
        {
            if (Workspaces.Count == 3)
            {
                if (Workspaces[2].GetType() == typeof(GeneratedCodeViewModel))
                    Workspaces.Remove(Workspaces[2]); 
                if (Workspaces[1].GetType() == typeof(GeneratedCodeViewModel))
                    Workspaces.Remove(Workspaces[1]);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// saveVMCommand : Save VM Command
        /// </summary>
        public SimpleCommand SaveVMCommand
        {
            get { return saveVMCommand; }
        }

        /// <summary>
        /// generateVMCommand : Generate VM Command
        /// </summary>
        public SimpleCommand GenerateVMCommand
        {
            get { return generateVMCommand; }
        }


        /// <summary>
        /// PropertiesVM : The ineternally used PropertiesViewModel
        /// </summary>
        static PropertyChangedEventArgs propertiesVMChangeArgs =
            ObservableHelper.CreateArgs<InMemoryViewModel>(x => x.PropertiesVM);

        public PropertiesViewModel PropertiesVM
        {
            get { return propertiesViewModel; }
            set
            {
                if (propertiesViewModel != value)
                {
                    propertiesViewModel = value;
                    this.Workspaces.Clear();
                    this.Workspaces.Add(propertiesViewModel);
                    this.SetActiveWorkspace(propertiesViewModel);
                    NotifyPropertyChanged(propertiesVMChangeArgs);
                }
            }
        }     

        /// <summary>
        /// ViewModelName : The View Models Name
        /// </summary>
        static PropertyChangedEventArgs viewModelNameChangeArgs =
            ObservableHelper.CreateArgs<InMemoryViewModel>(x => x.ViewModelName);

        public String ViewModelName
        {
            get { return viewModelName; }
            set
            {
                if (viewModelName != value)
                {
                    viewModelName = value;
                    NotifyPropertyChanged(viewModelNameChangeArgs);
                    Mediator.NotifyColleagues<Boolean>("CheckInMemoryVMValidPart1", true);
                }

            }
        }


        /// <summary>
        /// ViewModelNamespace : The View Models Namespace
        /// </summary>
        static PropertyChangedEventArgs viewModelNamespaceChangeArgs =
            ObservableHelper.CreateArgs<InMemoryViewModel>(x => x.ViewModelNamespace);

        public String ViewModelNamespace
        {
            get { return viewmodelNamespace; }
            set
            {
                if (viewmodelNamespace != value)
                {
                    viewmodelNamespace = value;
                    NotifyPropertyChanged(viewModelNamespaceChangeArgs);
                    Mediator.NotifyColleagues<Boolean>("CheckInMemoryVMValidPart1", true);
                }

            }
        }
        

        /// <summary>
        /// CurrentViewModelType : The View Models Type
        /// </summary>
        static PropertyChangedEventArgs currentViewModelTypeChangeArgs =
            ObservableHelper.CreateArgs<InMemoryViewModel>(x => x.CurrentViewModelType);

        public ViewModelType CurrentViewModelType
        {
            get { return currentViewModelType; }
            set
            {
                if (currentViewModelType != value)
                {
                    currentViewModelType = value;
                    NotifyPropertyChanged(currentViewModelTypeChangeArgs);
                }

            }
        }

        /// <summary>
        /// The active workspace ViewModels
        /// </summary>
        static PropertyChangedEventArgs workspacesChangeArgs =
            ObservableHelper.CreateArgs<InMemoryViewModel>(x => x.Workspaces);

        public ObservableCollection<ViewModelBase> Workspaces
        {
            get { return workspaces; }
            set
            {
                if (workspaces == null)
                {
                    workspaces = value;
                    NotifyPropertyChanged(workspacesChangeArgs);
                }
            }
        }
        #endregion

        #region Command Implementations

        #region SaveVMCommand
        /// <summary>
        /// Logic to determine if SaveVMCommand can execute
        /// </summary>
        private Boolean CanExecuteSaveVMCommand
        {
            get
            {
                return true;
            }
        }

/// <summary>
/// Executes the SaveVMCommand
/// </summary>
private void ExecuteSaveVMCommand()
{
    ClearCodeWorkSpaces();
    SaveOrGenerateOperation("Xml files (*.xml)|*.xml", 
        SaveOrGenerate.Save);
}
        #endregion

        #region GenerateVMCommand
        /// <summary>
        /// Logic to determine if GenerateVMCommand can execute
        /// </summary>
        private Boolean CanExecuteGenerateVMCommand
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the GenerateVMCommand
        /// </summary>
        private void ExecuteGenerateVMCommand()
        {
            ClearCodeWorkSpaces();
            Tuple<Boolean,String> info = SaveOrGenerateOperation("C# files (*.cs)|*.cs", 
                SaveOrGenerate.Generate);
            
            if (info != null && info.First)
            {

                Workspaces.Insert(1, new GeneratedCodeViewModel
                {
                    IsCloseable = false,
                    DisplayName = ".cs",
                    FileName = ViewModelPersistence.StripFileName(info.Second) + ".cs"
                });
                Workspaces.Insert(2, new GeneratedCodeViewModel
                {
                    IsCloseable = false,
                    DisplayName = ".g.cs",
                    FileName = ViewModelPersistence.StripFileName(info.Second) + ".g.cs"
                });
            }
        }
        #endregion
        #endregion
    }
}
