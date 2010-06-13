using System;
using System.Threading;


namespace Cinch
{
    /// <summary>
    /// This interface defines a UI controller which can be used to ChildWindows
    /// from a ViewModel
    /// </summary>
    public interface IChildWindowService
    {
        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        void Register(string key, Type winType);

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        bool Unregister(string key);

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
        void Show(string key, object state, EventHandler<UICompletedEventArgs> completedProc);

    
    }
}
