using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Cinch
{
    /// <summary>
    /// Provides a set of commonly used Dispatcher extension methods
    /// </summary>
    public static class DispatcherExtensions
    {
        #region Dispatcher Extensions
        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        /// <param name="priority">The DispatcherPriority for the invoke.</param>
        public static void InvokeIfRequired(this Dispatcher dispatcher,
            Action action, DispatcherPriority priority)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(priority, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <typeparam name="T">
        /// The return type of the delegate.
        /// </typeparam>
        /// <param name="dispatcher">
        /// The Dispatcher object on which to 
        /// perform the Invoke
        /// </param>
        /// <param name="function">
        /// The delegate to run
        /// </param>
        /// <param name="priority">
        /// The DispatcherPriority for the invoke.
        /// </param>
        /// <returns>
        /// The return value of the invoked function.
        /// </returns>
        public static T InvokeIfRequired<T>(this Dispatcher dispatcher,
            Func<T> function, DispatcherPriority priority)
        {
            if (!dispatcher.CheckAccess())
            {
                return (T)dispatcher.Invoke(priority, function);
            }
            
            return function();
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <typeparam name="T">
        /// The return type of the delegate.
        /// </typeparam>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="function">The delegate to run</param>
        /// <returns>
        /// The return value of the invoked function.
        /// </returns>
        public static T InvokeIfRequired<T>(this Dispatcher dispatcher, Func<T> function)
        {
            if (!dispatcher.CheckAccess())
            {
                return (T)dispatcher.Invoke(DispatcherPriority.Normal, function);
            }

            return function();
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeInBackgroundIfRequired(
            this Dispatcher dispatcher, 
            Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Background, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread asynchronously if it is not currently 
        /// on the correct thread which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        /// <param name="priority">The DispatcherPriority for the invoke.</param>
        public static void InvokeAsynchronouslyIfRequired(
            this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(priority, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread asynchronously if it is not currently 
        /// on the correct thread which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeAsynchronouslyIfRequired(
            this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread asynchronously if it is not currently 
        /// on the correct thread which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeAsynchronouslyInBackground(
            this Dispatcher dispatcher, Action action)
        {
            if (dispatcher != null)
                dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            else
                action();
        }
        #endregion
    }
}
