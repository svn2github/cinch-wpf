using System;
using System.Collections.Generic;


namespace Cinch
{
    /// <summary>
    /// This class implements the IMessageBoxService for Unit testing purposes.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///        //Queue up the response we expect for our given TestMessageBoxService
    ///        //for a given ICommand/Method call within the test ViewModel
    ///        testMessageBoxService.ShowYesNoResponders.Enqueue
    ///            (() =>
    ///                {
    ///
    ///                    return CustomDialogResults.Yes;
    ///                }
    ///            );
    /// ]]>
    /// </example>
    public class TestMessageBoxService : IMessageBoxService
    {
        #region Data

         /// <summary>
        /// Queue of callback delegates for the ShowOkCancel methods expected
        /// for the item under test
        /// </summary>
        public Queue<Func<CustomDialogResults>> ShowOkCancelResponders { get; set; }


        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public TestMessageBoxService()
        {
            ShowOkCancelResponders = new Queue<Func<CustomDialogResults>>();
        }
        #endregion

        #region IMessageBoxService Members

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowError(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowInformation(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowWarning(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs        
        }

        /// <summary>
        /// Returns the next Dequeue ShowOkCancel response expected. See the tests for 
        /// the Func callback expected values
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message)
        {
            if (ShowOkCancelResponders.Count == 0)
                throw new Exception(
                    "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowOkCancelResponders.Dequeue();
                return responder();
            }
        }

        #endregion
    }
}
