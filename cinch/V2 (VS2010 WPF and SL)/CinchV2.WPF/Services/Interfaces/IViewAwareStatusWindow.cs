using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using MEFedMVVM.Services.Contracts;
using System.ComponentModel;

namespace Cinch
{
    public interface IViewAwareStatusWindow : IContextAware
    {
        event Action ViewLoaded;
        event Action ViewUnloaded;
        event Action ViewActivated;
        event Action ViewDeactivated;

        event Action ViewWindowClosed;
        event Action ViewWindowContentRendered;
        event Action ViewWindowLocationChanged;
        event Action ViewWindowStateChanged;
        event EventHandler<CancelEventArgs> ViewWindowClosing;


        Dispatcher ViewsDispatcher { get; }
        Object View { get; }
    }
}
