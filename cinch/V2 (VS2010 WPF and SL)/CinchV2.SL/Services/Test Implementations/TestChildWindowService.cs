using System;
using System.Collections.Generic;


namespace Cinch
{
    /// <summary>
    /// This class implements the IChildWindowService for Unit testing purposes.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///    //Queue up the response we expect for our given TestChildWindowService
    ///    //for a given ICommand/Method call within the test ViewModel
    ///    testChildWindowService.ShowResultResponders.Enqueue
    ///     (() =>
    ///        {
    ///            return new UICompletedEventArgs()
    ///                        {
    ///                            State = WHATEVER STATE YOU LIKE,
    ///                            Result = true
    ///                        } ;
    ///        }
    ///     );
    /// ]]>
    /// </example>
    public class TestChildWindowService : IChildWindowService
    {
        #region Data

        /// <summary>
        /// Queue of callback delegates for the Show methods expected
        /// for the item under test
        /// </summary>
        public Queue<Func<UICompletedEventArgs>> ShowResultResponders { get; set; }

 
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public TestChildWindowService()
        {
            ShowResultResponders = new Queue<Func<UICompletedEventArgs>>();
        }
        #endregion

        #region IUIVisualizerService Members

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        public void Register(string key, Type winType)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }


        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        public bool Unregister(string key)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs, simple return true
            return true;
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
            if (ShowResultResponders.Count == 0)
                throw new Exception(
                    "TestUIVisualizerService Show method expects a Func<UICompletedEventArgs> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<UICompletedEventArgs> responder = ShowResultResponders.Dequeue();
                completedProc(null, responder());
            }
        }

 
        #endregion
    }
}
