using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinch
{
    /// <summary>
    /// This is an interface that allows different IOC Container providers.
    /// Providers can implement this interface in order to provide the services
    /// from the container.
    /// 
    /// Cinch uses a UnityProvider as default, but you can override this
    /// by supplying a new IIOCProvider variant to the constructor
    /// of your custom Cinch based ViewModels. If no constructor parameter
    /// is supplied to the ViewModelBase class then the default UnityProvider
    /// will be used by Cinch
    /// </summary>
    public interface IIOCProvider
    {
        /// <summary>
        /// Method that <see cref="ViewModelBase">ViewModelBase</see>
        /// can call to tell container to set its self up. You are expected
        /// to register the following Types in this method
        /// <c>ILogger</c>
        /// <c>IUIVisualizerService</c>
        /// <c>IMessageBoxService</c>
        /// <c>IOpenFileService</c>
        /// <c>ISaveFileService</c>
        /// </summary>
        void SetupContainer();

        /// <summary>
        /// Get service from container
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>The service instance</returns>
        T GetTypeFromContainer<T>();
    }
}
