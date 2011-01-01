using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Threading;
using MEFedMVVM.ViewModelLocator;
using System.ComponentModel;

namespace Cinch
{


  

    /// <summary>
    /// This class implements the IChildWindowService for SL purposes.
    /// If you have attributed up your views using the PopupNameToViewLookupKeyMetadata
    /// Registration of Views with the IChildWindowService service is automatic.
    /// However you can still register views manually, to do this simply put some lines like this in you App.Xaml.cs
    /// MefLocator.Container.GetExport<IChildWindowService>().Value.Register("ChildWindow1", typeof(ChildWindow1));
    /// </summary>
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///    //ui is an instance of the IChildWindowService which was MEF injected (say) into your ViewModel
    ///    bool? dialogResult = null;
    ///    ui.Show(popName, this, (s, e) =>
    ///    {
    ///        dialogResult = e.Result;
    ///        //you can do what you like with dialogResult
    ///    });
    ///    //NOTE : You should not do anymore here, as the ChildWindow, although it appears
    ///    //modal, it does not block parent code.
    /// ]]>
    /// </example>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportService(ServiceType.Both, typeof(IChildWindowService))]
    public class ChildWindowService : IChildWindowService
    {
        #region Data
        private readonly Dictionary<string, Type> _registeredWindows;
        #endregion

        #region Ctor
        public ChildWindowService()
        {
            _registeredWindows = new Dictionary<string, Type>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Registers a collection of entries
        /// </summary>
        /// <param name="startupData"></param>
        public void Register(Dictionary<string, Type> startupData)
        {
            foreach (var entry in startupData)
                Register(entry.Key, entry.Value);
        }

        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        public void Register(string key, Type winType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (winType == null)
                throw new ArgumentNullException("winType");
            if (!typeof(ChildWindow).IsAssignableFrom(winType))
                throw new ArgumentException("winType must be of type ChildWindow");

            lock (_registeredWindows)
            {
                _registeredWindows.Add(key, winType);
            }
        }

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        public bool Unregister(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            lock (_registeredWindows)
            {
                return _registeredWindows.Remove(key);
            }
        }

        /// <summary>
        /// This method displays ChildWindow associated with the given key
        /// calling code is not blocked, and will not wait on the ChildWindow being
        /// closed. So this should only be used when there is no code dependant on
        /// the ChildWindows DialogResult. If you want to use the result of the ChildWindow
        /// being shown you can should create a callback delegate for the completedProc
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <param name="completedProc">Callback used when UI closes (may be null)</param>
        public void Show(string key, object state, EventHandler<UICompletedEventArgs> completedProc)
        {
            ChildWindow win = CreateChildWindow(key, state, completedProc);
            if (win != null)
            {
                win.Show();
            }
        }

     
 
        #endregion

        #region Private Methods




        /// <summary>
        /// This creates the SL ChildWindow from a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="dataContext">DataContext (state) object</param>
        /// <param name="completedProc">Callback used when UI closes (may be null)</param>
        /// <returns>ChildWindow</returns>
        private ChildWindow CreateChildWindow(string key, object dataContext, 
            EventHandler<UICompletedEventArgs> completedProc)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            Type winType;
            lock (_registeredWindows)
            {
                if (!_registeredWindows.TryGetValue(key, out winType))
                    return null;
            }

            var win = (ChildWindow)Activator.CreateInstance(winType);

            if (dataContext is IViewStatusAwareInjectionAware)
            {
                IViewAwareStatus viewAwareStatus = 
                    ViewModelRepository.Instance.Resolver.Container.GetExport<IViewAwareStatus>().Value;
                viewAwareStatus.InjectContext((FrameworkElement)win);
                ((IViewStatusAwareInjectionAware)dataContext).InitialiseViewAwareService(viewAwareStatus);
            }


            win.DataContext = dataContext;
            ViewModelBase bvm=null;

            EventHandler<CloseRequestEventArgs> handler = ((s, e) =>
                    {
                        try
                        {
                            win.DialogResult = e.Result;
                        }
                        catch (InvalidOperationException)
                        {
                            win.Close();
                        }
                    });


            if (dataContext != null)
            {
                bvm = dataContext as ViewModelBase;
                if (bvm != null)
                {
                    bvm.CloseRequest += handler;

                }
            }


            win.Closed += (s, e) =>
            {
                bvm.CloseRequest -= handler;

                if (completedProc != null)
                {
                    completedProc(this, new UICompletedEventArgs()
                    {
                        State = dataContext,
                        Result = win.DialogResult
                    });

                    GC.Collect();
                }
            };

            return win;
        }

 
        
        
        #endregion
    }
}
