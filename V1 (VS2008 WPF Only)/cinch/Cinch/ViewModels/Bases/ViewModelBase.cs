using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


using System.Linq.Expressions;


namespace Cinch
{
    /// <summary>
    /// Provides a base class for ViewModels to inherit from. This 
    /// base class provides the following
    /// <list type="Bullet">
    /// <item>Mediator pattern implementation</item>
    /// <item>Service resolution</item>
    /// <item>Window lifetime virtual method hooks</item>
    /// <item>INotifyPropertyChanged</item>
    /// <item>Workspace support</item>
    /// </list>
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable, IParentablePropertyExposer
    {
        #region Data
        private SimpleCommand closeActivePopUpCommand;
        private SimpleCommand activatedCommand;
        private SimpleCommand deactivatedCommand;
        private SimpleCommand loadedCommand;
        private SimpleCommand unloadedCommand;
        private SimpleCommand closeCommand;
        private Boolean isUnloaded=false;
        private Boolean isLoaded=false;
        private Boolean isDeactivated=false;
        private Boolean isActivated = false;
        private IIOCProvider iocProvider = null;
        private static ILogger logger;
        private static Boolean isInitialised = false;
        private static Action<IUIVisualizerService> setupVisualizer = null;


        //workspace data
        private SimpleCommand closeWorkSpaceCommand;
        private Boolean isCloseable = true;

        /// <summary>
        /// Service resolver for view models.  Allows derived types to add/remove
        /// services from mapping.
        /// </summary>
        public static readonly ServiceProvider ServiceProvider = new ServiceProvider();

        /// <summary>
        /// This event should be raised to close the view.  Any view tied to this
        /// ViewModel should register a handler on this event and close itself when
        /// this event is raised.  If the view is not bound to the lifetime of the
        /// ViewModel then this event can be ignored.
        /// </summary>
        public event EventHandler<CloseRequestEventArgs> CloseRequest;

        /// <summary>
        /// This event should be raised to activate the UI.  Any view tied to this
        /// ViewModel should register a handler on this event and close itself when
        /// this event is raised.  If the view is not bound to the lifetime of the
        /// ViewModel then this event can be ignored.
        /// </summary>
        public event EventHandler<EventArgs> ActivateRequest;



        #endregion

        #region Ctor
        /// <summary>
        /// Constructs a new ViewModelBase and wires up all the Window based Lifetime
        /// commands such as activatedCommand/deactivatedCommand/loadedCommand/closeCommand
        /// </summary>
        public ViewModelBase() : this(new UnityProvider())
        {
           	
        }

        public ViewModelBase(IIOCProvider iocProvider)
        {

            if (iocProvider == null)
                throw new InvalidOperationException(
                    String.Format(
                        "ViewModelBase constructor requires a IIOCProvider instance in order to work"));

            this.iocProvider = iocProvider;

            if (!ViewModelBase.isInitialised)
            {
                iocProvider.SetupContainer();
                FetchCoreServiceTypes();
            }

            //Register all decorated methods to the Mediator
            //Register all decorated methods to the Mediator
            Mediator.Instance.Register(this);

            #region Wire up Window/UserControl based Lifetime commands
            activatedCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnWindowActivated()
            };

            deactivatedCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnWindowDeactivated()
            };

            loadedCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnWindowLoaded()
            };

            unloadedCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnWindowUnloaded()
            };

            closeCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnWindowClose()
            };
            #endregion

            #region Wire up Workspace Command

            //This is used for popup control only
            closeWorkSpaceCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ExecuteCloseWorkSpaceCommand()
            };

            #endregion

            //This is used for popup control only
            closeActivePopUpCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => OnCloseActivePopUp(x)
            };
        }

        #endregion

        #region Public/Protected Methods/Events
        /// <summary>
        /// This resolves a service type and returns the implementation.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        protected T Resolve<T>()
        {
            return ServiceProvider.Resolve<T>();
        }

        /// <summary>
        /// This raises the CloseRequest event to close the UI.
        /// </summary>
        public virtual void RaiseCloseRequest()
        {
            EventHandler<CloseRequestEventArgs> handlers = CloseRequest;

            // Invoke the event handlers
            if (handlers != null)
            {
                try
                {
                    handlers(this, new CloseRequestEventArgs(null));
                }
                catch (Exception ex)
                {
                    LogExceptionIfLoggerAvailable(ex);
                }
            }
        }

        /// <summary>
        /// This raises the CloseRequest event to close the UI.
        /// </summary>
        public virtual void RaiseCloseRequest(bool? dialogResult)
        {
            EventHandler<CloseRequestEventArgs> handlers = CloseRequest;

            // Invoke the event handlers
            if (handlers != null)
            {
                try
                {
                    handlers(this, new CloseRequestEventArgs(dialogResult));
                }
                catch (Exception ex)
                {
                    LogExceptionIfLoggerAvailable(ex);
                }
            }
        }

        /// <summary>
        /// This raises the ActivateRequest event to activate the UI.
        /// </summary>
        public virtual void RaiseActivateRequest()
        {
            EventHandler<EventArgs> handlers = ActivateRequest;

            // Invoke the event handlers
            if (handlers != null)
            {
                try
                {
                    handlers(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    LogExceptionIfLoggerAvailable(ex);
                }
            }
        }

        /// <summary>
        /// Allows Window.Activated hook
        /// Can be overriden if required in inheritors
        /// </summary>
        protected virtual void OnWindowActivated()
        {
            //Will only work as long as people, call base.OnWindowUnloaded()
            //when overriding
            IsActivated = true; 
        }

        /// <summary>
        /// Allows Window.Deactivated hook
        /// Can be overriden if required in inheritors
        /// </summary>
        protected virtual void OnWindowDeactivated()
        {
            //Will only work as long as people, call base.OnWindowUnloaded()
            //when overriding
            IsDeactivated = true; 
        }

        /// <summary>
        /// Allows Window.Loaded/UserControl.Loaded hook
        /// Can be overriden if required in inheritors
        /// </summary>
        protected virtual void OnWindowLoaded()
        {
            //Will only work as long as people, call base.OnWindowUnloaded()
            //when overriding
            IsLoaded = true; 
        }

        /// <summary>
        /// Allows Window.Unloaded/UserControl.Unloaded hook
        /// Can be overriden if required in inheritors
        /// </summary>
        protected virtual void OnWindowUnloaded()
        {
            //Will only work as long as people, call base.OnWindowUnloaded()
            //when overriding
            IsUnloaded = true; 
            
        }

        /// <summary>
        /// Allows Window.Close hook
        /// </summary>
        protected virtual void OnWindowClose()
        {
            //Should be overriden if required in inheritors
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Delegate that is called when the services are injected
        /// </summary>
        public static Action<IUIVisualizerService> SetupVisualizer
        {
            get { return setupVisualizer; }
            set { setupVisualizer=value; }
        }
 
        /// <summary>
        /// Mediator : Mediator = Messaging pattern
        /// </summary>
        public static Mediator Mediator
        {
            get { return Mediator.Instance; }
        }

        /// <summary>
        /// Logger : The ILogger implementation in use
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// ActivatedCommand : Window Lifetime command
        /// </summary>
        public SimpleCommand ActivatedCommand
        {
            get { return activatedCommand; }
        }

        /// <summary>
        /// DeactivatedCommand : Window Lifetime command
        /// </summary>
        public SimpleCommand DeactivatedCommand
        {
            get { return deactivatedCommand; }
        }

        /// <summary>
        /// LoadedCommand : Window/UserControl Lifetime command
        /// </summary>
        public SimpleCommand LoadedCommand
        {
            get { return loadedCommand; }
        }

        /// <summary>
        /// UnloadedCommand : Window/UserControl Lifetime command
        /// </summary>
        public SimpleCommand UnloadedCommand
        {
            get { return unloadedCommand; }
        }

        /// <summary>
        /// CloseCommand : Window Lifetime command
        /// </summary>
        public SimpleCommand CloseCommand
        {
            get { return closeCommand; }
        }

        /// <summary>
        /// CloseCommand : Close popup command
        /// </summary>
        public SimpleCommand CloseActivePopUpCommand
        {
            get { return closeActivePopUpCommand; }
        }

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public SimpleCommand CloseWorkSpaceCommand
        {
            get
            {
                return closeWorkSpaceCommand;
            }
        }


        /// <summary>
        /// Is the ViewModel closeable 
        /// </summary>
        static PropertyChangedEventArgs isCloseableChangeArgs =
            ObservableHelper.CreateArgs<ViewModelBase>(x => x.IsCloseable);

        public Boolean IsCloseable
        {
            get { return isCloseable; }
            set
            {
                isCloseable = value;
                NotifyPropertyChanged(isCloseableChangeArgs);
            }
        }

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; set; }


        /// <summary>
        /// View Is Activated
        /// </summary>
        static PropertyChangedEventArgs isActivatedChangeArgs =
            ObservableHelper.CreateArgs<ViewModelBase>(x => x.IsActivated);

        /// <summary>
        /// Will only be reliable if users call base.OnWindowUnloaded()
        /// when overriding virtual ViewModelBase.OnWindowUnloaded()
        /// </summary>
        public Boolean IsActivated
        {
            get { return isActivated; }
            private set
            {
                isActivated = value;
                NotifyPropertyChanged(isActivatedChangeArgs);
            }
        }


        /// <summary>
        /// View Is Deactivated
        /// </summary>
        static PropertyChangedEventArgs isDeactivatedChangeArgs =
            ObservableHelper.CreateArgs<ViewModelBase>(x => x.IsDeactivated);

        /// <summary>
        /// Will only be reliable if users call base.OnWindowDeactivated()
        /// when overriding virtual ViewModelBase.OnWindowDeactivated()
        /// </summary>
        public Boolean IsDeactivated
        {
            get { return isDeactivated; }
            private set
            {
                isDeactivated = value;
                NotifyPropertyChanged(isDeactivatedChangeArgs);
            }
        }

        /// <summary>
        /// View Is Loaded
        /// </summary>
        static PropertyChangedEventArgs isLoadedChangeArgs =
            ObservableHelper.CreateArgs<ViewModelBase>(x => x.IsLoaded);

        /// <summary>
        /// Will only be reliable if users call base.OnWindowLoaded()
        /// when overriding virtual ViewModelBase.OnWindowLoaded()
        /// </summary>
        public Boolean IsLoaded
        {
            get { return isLoaded; }
            private set
            {
                isLoaded = value;
                NotifyPropertyChanged(isLoadedChangeArgs);
            }
        }

        /// <summary>
        /// View Is Unloaded
        /// </summary>
        static PropertyChangedEventArgs isUnloadedChangeArgs =
            ObservableHelper.CreateArgs<ViewModelBase>(x => x.IsUnloaded);

        /// <summary>
        /// Will only be reliable if users call base.OnWindowUnloaded()
        /// when overriding virtual ViewModelBase.OnWindowUnloaded()
        /// </summary>
        public Boolean IsUnloaded
        {
            get { return isUnloaded; }
            private set
            {
                isUnloaded = value;
                NotifyPropertyChanged(isUnloadedChangeArgs);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the CloseWorkSpace Command
        /// </summary>
        private void ExecuteCloseWorkSpaceCommand()
        {
            CloseWorkSpaceCommand.CommandSucceeded = false;

            EventHandler<EventArgs> handlers = CloseWorkSpace;

            // Invoke the event handlers
            if (handlers != null)
            {
                try
                {
                    handlers(this, EventArgs.Empty);
                    CloseWorkSpaceCommand.CommandSucceeded = true;
                }
                catch
                {
                    String err = "Error firing CloseWorkSpace event";
#if debug
                    Debug.WriteLine(err);
#endif
                    Console.WriteLine(err);
                }
            }

        }




        /// <summary>
        /// This method registers services with the service provider.
        /// </summary>
        private void FetchCoreServiceTypes()
        {
            try
            {
                ViewModelBase.isInitialised = false;

                //ILogger : Allows MessageBoxs to be shown 
                logger = (ILogger)this.iocProvider.GetTypeFromContainer<ILogger>();

                ServiceProvider.Add(typeof(ILogger), logger);

                //IMessageBoxService : Allows MessageBoxs to be shown 
                IMessageBoxService messageBoxService = 
                    (IMessageBoxService)UnitySingleton.Instance.Container.Resolve(
                        typeof(IMessageBoxService));

                ServiceProvider.Add(typeof(IMessageBoxService), messageBoxService);

                //IOpenFileService : Allows Opening of files 
                IOpenFileService openFileService = 
                    (IOpenFileService)UnitySingleton.Instance.Container.Resolve(
                        typeof(IOpenFileService));
                ServiceProvider.Add(typeof(IOpenFileService), openFileService);

                //ISaveFileService : Allows Saving of files 
                ISaveFileService saveFileService =
                    (ISaveFileService)UnitySingleton.Instance.Container.Resolve(
                        typeof(ISaveFileService));
                ServiceProvider.Add(typeof(ISaveFileService), saveFileService);

                //IUIVisualizerService : Allows popup management
                IUIVisualizerService uiVisualizerService =
                    (IUIVisualizerService)UnitySingleton.Instance.Container.Resolve(
                        typeof(IUIVisualizerService));
                ServiceProvider.Add(typeof(IUIVisualizerService), uiVisualizerService);

                //call the callback delegate to setup IUIVisualizerService managed
                //windows
                if (SetupVisualizer != null)
                    SetupVisualizer(uiVisualizerService);

                ViewModelBase.isInitialised = true;

            }
            catch (Exception ex)
            {
                LogExceptionIfLoggerAvailable(ex);
            }
        }

        /// <summary>
        /// Raises RaiseCloseRequest event, passing back correct DialogResult
        /// </summary>
        private void OnCloseActivePopUp(Object param)
        {
            if (param is Boolean)
            {
                // Close the dialog using DialogResult requested
                RaiseCloseRequest((bool)param);
                return;
            }

            //param is not a bool so try and parse it to a Bool
            Boolean popupAction=true;
            Boolean result = Boolean.TryParse(param.ToString(), out popupAction);
            if (result)
            {
                // Close the dialog using DialogResult requested
                RaiseCloseRequest(popupAction);
            }
            else
            {
                // Close the dialog passing back true
                RaiseCloseRequest(true);
            }
        }

        /// <summary>
        /// Logs a message if there is a ILoggerService available. And then throws
        /// new ApplicationException which should be caught somewhere external
        /// to this class
        /// </summary>
        /// <param name="ex">Exception to log</param>
        private static void LogExceptionIfLoggerAvailable(Exception ex)
        {
            if (logger != null)
                logger.Error("An error occurred", ex);

            throw new ApplicationException(ex.Message);
        }
        #endregion

        #region Event(s)
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler<EventArgs> CloseWorkSpace;
        #endregion 

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify using pre-made PropertyChangedEventArgs
        /// </summary>
        /// <param name="args"></param>
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IParentablePropertyExposer
        /// <summary>
        /// Returns the list of delegates that are currently subscribed for the
        /// <see cref="System.ComponentModel.INotifyPropertyChanged">INotifyPropertyChanged</see>
        /// PropertyChanged event
        /// </summary>
        public Delegate[] GetINPCSubscribers()
        {
            return PropertyChanged == null ? null : PropertyChanged.GetInvocationList();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// This method currently unregisters this object from the
        /// Mediator.Instance
        /// </summary>
        protected virtual void OnDispose()
        {
            //Register all decorated methods to the Mediator
            Mediator.Instance.Unregister(this);
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", 
                this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members

    }
}
