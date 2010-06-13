using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace Cinch
{
    public abstract partial class ViewModelBase
    {

        #region Ctor
        public ViewModelBase()
        {

            //This is used for popup control only
            CloseActivePopUpCommand = new SimpleCommand<object, object>(x => true, x => ExecuteCloseActivePopupCommand(x));
            CloseWorkSpaceCommand = new SimpleCommand<object, object>(x => true, x => ExecuteCloseWorkSpaceCommand());

        }
        #endregion

        #region Data
        /// <summary>
        /// This event should be raised to close the view.  Any view tied to this
        /// ViewModel should register a handler on this event and close itself when
        /// this event is raised.  If the view is not bound to the lifetime of the
        /// ViewModel then this event can be ignored.
        /// </summary>
        public event EventHandler<CloseRequestEventArgs> CloseRequest;
        #endregion

        #region Public Methods
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
                    Debug.WriteLine(ex);
                }
            }
        }
        #endregion
    }
}
