using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Policy;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace CinchCodeGen
{
    /// <summary>
    /// Loads an assembly into a new AppDomain and obtains all the
    /// namespaces in the loaded Assembly, which are returned as a 
    /// List. The new AppDomain is then Unloaded.
    /// 
    /// This class creates a new instance of a 
    /// <c>AssemblyLoader</c> class
    /// which does the actual ReflectionOnly loading 
    /// of the Assembly into
    /// the new AppDomain.
    /// </summary>
    public class SeperateAppDomainAssemblyLoader
    {
        #region Public Methods
        /// <summary>
        /// Loads an assembly into a new AppDomain and obtains all the
        /// namespaces in the loaded Assembly, which are returned as a 
        /// List. The new AppDomain is then Unloaded
        /// </summary>
        /// <param name="assemblyLocation">The Assembly file 
        /// location</param>
        /// <returns>A list of found namespaces</returns>
        public List<String> LoadAssemblies(List<FileInfo> assemblyLocations)
        {
            List<String> namespaces = new List<String>();

            AppDomain childDomain = BuildChildDomain(
                AppDomain.CurrentDomain);

            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    AssemblyLoader loader =
                        (AssemblyLoader)childDomain.
                            CreateInstanceFrom(
                            loaderType.Assembly.Location,
                            loaderType.FullName).Unwrap();

                    namespaces = loader.LoadAssemblies(
                        assemblyLocations);
                }
                return namespaces;
            }

            finally
            {

                AppDomain.Unload(childDomain);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a new AppDomain based on the parent AppDomains 
        /// Evidence and AppDomainSetup
        /// </summary>
        /// <param name="parentDomain">The parent AppDomain</param>
        /// <returns>A newly created AppDomain</returns>
        private AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            Evidence evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion",
                evidence, setup);
        }
        #endregion


        /// <summary>
        /// Remotable AssemblyLoader, this class 
        /// inherits from <c>MarshalByRefObject</c> 
        /// to allow the CLR to marshall
        /// this object by reference across 
        /// AppDomain boundaries
        /// </summary>
        class AssemblyLoader : MarshalByRefObject
        {
            #region Private/Internal Methods
            /// <summary>
            /// ReflectionOnlyLoad of single Assembly based on 
            /// the assemblyPath parameter
            /// </summary>
            /// <param name="assemblyPath">The path to the Assembly</param>
            [SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic")]
            internal List<String> LoadAssemblies(List<FileInfo> assemblyLocations)
            {
                List<String> namespaces = new List<String>();
                try
                {
                    foreach (FileInfo assemblyLocation in assemblyLocations)
                    {
                        Assembly.ReflectionOnlyLoadFrom(assemblyLocation.FullName);
                    }

                    foreach (Assembly reflectionOnlyAssembly in AppDomain.CurrentDomain.
                            ReflectionOnlyGetAssemblies())
                    {
                        foreach (Type type in reflectionOnlyAssembly.GetTypes())
                        {
                            String ns = String.Format("using {0};", type.Namespace);
                            if (!namespaces.Contains(ns))
                                namespaces.Add(ns);
                        }
                    }
                    return namespaces;
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */
                    return namespaces;
                }
            }
            #endregion
        }
    }
}
