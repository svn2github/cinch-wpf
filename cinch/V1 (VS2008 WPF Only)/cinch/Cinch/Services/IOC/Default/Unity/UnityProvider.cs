using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace Cinch
{
    /// <summary>
    /// Provides a Unity IOC Container resolver
    /// which Cinch uses as default, which registers defaults
    /// for the following types, but will also allow these defaults
    /// to be overriden if a Unity config section scecifies different
    /// implementations
    /// <c>ILogger</c>
    /// <c>IUIVisualizerService</c>
    /// <c>IMessageBoxService</c>
    /// <c>IOpenFileService</c>
    /// <c>ISaveFileService</c>
    /// </summary>
    public class UnityProvider : IIOCProvider
    {
        #region Ctor
        public UnityProvider()
        {

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method registers default services with the service provider. 
        /// These can be overriden by providing a new service implementation 
        /// and a new Unity config section in the project where the new service 
        /// implementation is defined 
        /// </summary>
        private static void RegisterDefaultServices()
        {

            //try add other default services, users can override this 
            //using specific Unity App.Config section entry
            try
            {

                //ILogger : Register a default WPFSLFLogger
                UnitySingleton.Instance.Container.RegisterInstance(
                    typeof(ILogger), new WPFSLFLogger());

                //IUIVisualizerService : Register a default WPFUIVisualizerService
                UnitySingleton.Instance.Container.RegisterInstance(
                    typeof(IUIVisualizerService), new WPFUIVisualizerService());

                //IMessageBoxService : Register a default WPFMessageBoxService
                UnitySingleton.Instance.Container.RegisterInstance(
                    typeof(IMessageBoxService), new WPFMessageBoxService());

                //IOpenFileService : Register a default WPFOpenFileService
                UnitySingleton.Instance.Container.RegisterInstance(
                    typeof(IOpenFileService), new WPFOpenFileService());

                //ISaveFileService : Register a default WPFSaveFileService
                UnitySingleton.Instance.Container.RegisterInstance(
                    typeof(ISaveFileService), new WPFSaveFileService());

            }
            catch (ResolutionFailedException rex)
            {
                String err = String.Format(
                    "An exception has occurred in unityProvider.RegisterDefaultServices()\r\n{0}",
                    rex.StackTrace.ToString());
#if debug
                Debug.WriteLine(err);
#endif
                Console.WriteLine(err);
                throw rex;
            }
            catch (Exception ex)
            {
                String err = String.Format(
                    "An exception has occurred in unityProvider.RegisterDefaultServices()\r\n{0}",
                    ex.StackTrace.ToString());
#if debug
                Debug.WriteLine(err);
#endif
                Console.WriteLine(err);
                throw ex;
            }
        }
        #endregion

        #region IIOCProvider Members
        /// <summary>
        /// Register defaults and sets up Unity container
        /// Method that <see cref="ViewModelBase">ViewModelBase</see>
        /// can call to tell container to set its self up
        /// </summary>
        public void SetupContainer()
        {
            try
            {
                //regiser defaults
                RegisterDefaultServices();

                //configure Unity (there could be some different Service implementations
                //in the config that override the defaults just setup
                UnityConfigurationSection section = (UnityConfigurationSection)
                               ConfigurationManager.GetSection("unity");
                if (section != null && section.Containers.Count > 0)
                {
                    section.Containers.Default.Configure(UnitySingleton.Instance.Container);
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was a problem configuring the Unity container\r\n" + ex.Message);
            }
        }


        /// <summary>
        /// Get service from container
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>The service instance</returns>
        public T GetTypeFromContainer<T>()
        {
            return (T)UnitySingleton.Instance.Container.Resolve(typeof(T));
        }
        #endregion
    }
}
