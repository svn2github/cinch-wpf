using System;
using System.Windows;


namespace System.Windows
{
    // Summary:
    //     Provides event listening support for classes that expect to receive events
    //     through the WeakEvent pattern and a System.Windows.WeakEventManager.
    public interface IWeakEventListener
    {
        // Summary:
        //     Receives events from the centralized event manager.
        //
        // Parameters:
        //   managerType:
        //     The type of the System.Windows.WeakEventManager calling this method.
        //
        //   sender:
        //     Object that originated the event.
        //
        //   e:
        //     Event data.
        //
        // Returns:
        //     true if the listener handled the event. It is considered an error by the
        //     System.Windows.WeakEventManager handling in WPF to register a listener for
        //     an event that the listener does not handle. Regardless, the method should
        //     return false if it receives an event that it does not recognize or handle.
        bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e);
    }
}
