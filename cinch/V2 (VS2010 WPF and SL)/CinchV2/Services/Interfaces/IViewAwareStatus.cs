using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using MEFedMVVM.Services.Contracts;

namespace Cinch
{
    public interface IViewAwareStatus : IContextAware
    {
        event Action ViewLoaded;
        event Action ViewUnloaded;

#if !SILVERLIGHT

        event Action ViewActivated;
        event Action ViewDeactivated;
#else
        void PerformCleanUp();
#endif

        Dispatcher ViewsDispatcher { get; }
        Object View { get; }
    }
}
