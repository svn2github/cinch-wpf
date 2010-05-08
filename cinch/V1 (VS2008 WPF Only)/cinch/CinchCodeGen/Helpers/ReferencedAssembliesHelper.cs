using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Runtime.Remoting;

using Cinch;
using System.Security.Policy;
using System.Security;


namespace CinchCodeGen
{
    /// <summary>
    /// Provides methods to read/write to the file that is used for the 
    /// globally available list of referenced assemblies. This file is a simple
    /// .txt file
    /// </summary>
    public static class ReferencedAssembliesHelper
    {
        #region Data
        public static String REF_ASS_FILE_NAME = "ReferencedAssemblies.txt";
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads in a collection of available referenced assemblies from the .txt file whos
        /// location is specified by the REF_ASS_FILE_NAME constant
        /// </summary>
        /// <returns>A collection of read in available referenced assemblies</returns>
        public static ObservableCollection<FileInfo> ReadCurrentlyAvailableReferencedAssemblies()
        {
            try
            {
                ObservableCollection<FileInfo> foundRefAssemblies = new ObservableCollection<FileInfo>();

                String appDir = AppDomain.CurrentDomain.BaseDirectory;
                String refAssFileLocation = Path.Combine(appDir, REF_ASS_FILE_NAME);

                if (File.Exists(refAssFileLocation))
                {
                    string line;
                    using (StreamReader file = new StreamReader(refAssFileLocation))
                        while ((line = file.ReadLine()) != null)
                            foundRefAssemblies.Add(new FileInfo(line));
                }

                (App.Current as App).ReferencedAssemblies.Clear();
                foreach (var refAss in foundRefAssemblies)
                {
                    (App.Current as App).ReferencedAssemblies.Add(refAss);
                }
                return foundRefAssemblies;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Write in a collection of available referenced assemblies to the .txt file whos
        /// location is specified by the REF_ASS_FILE_NAME constant
        /// </summary>
        /// <param name="propertyTypes">A collection of read in available referenced assemblies</param>
        /// <returns>True if the write succeeded</returns>
        public static Boolean WriteCurrentlyReferencedAssemblies(
            ObservableCollection<FileInfo> referencedAssemblies)
        {
            try
            {
                String appDir = AppDomain.CurrentDomain.BaseDirectory;
                String refAssFileLocation = Path.Combine(appDir, REF_ASS_FILE_NAME);

                if (File.Exists(refAssFileLocation))
                    File.Delete(refAssFileLocation);

                using (StreamWriter file = new StreamWriter(refAssFileLocation))
                {
                    (App.Current as App).ReferencedAssemblies.Clear();
                    foreach (FileInfo refAss in referencedAssemblies)
                    {
                        file.WriteLine(refAss);
                        (App.Current as App).ReferencedAssemblies.Add(refAss);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Examines the referenced assemblies list the user picked, and loads each
        /// assembly in turn and gets all the required namespaces, and includes these
        /// as valid Using statements in the generated code.
        /// 
        /// NOTE: These referenced assemblies are loaded into a seperate AppDomain
        /// so that they can be unloaded. The use of the AppDomain also means the loaded
        /// assemblies are not loaded into the current AppDomain so do not impact the
        /// memory footpriont of the code generator app.
        /// </summary>
        /// <returns>A list of additional referenced assembly namespace strings to
        /// include in the generated code Using statements</returns>
        public static List<String> GetReferencedAssembliesUsingStatements()
        {
            try
            {
                //references assembly namespaces added
                List<String> namespacesAdded = new List<String>();
                List<FileInfo> assemblies = 
                    ((App)App.Current).ReferencedAssemblies.ToList();
                
                //load them into new AppDomain and get the namespaces
                SeperateAppDomainAssemblyLoader appDomainAssemblyLoader = 
                    new SeperateAppDomainAssemblyLoader();

                namespacesAdded = appDomainAssemblyLoader.LoadAssemblies(assemblies);
                return namespacesAdded;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
