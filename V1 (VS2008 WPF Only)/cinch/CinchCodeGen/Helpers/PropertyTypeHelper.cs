using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;


namespace CinchCodeGen
{
    /// <summary>
    /// Provides methods to read/write to the file that is used for the 
    /// globally available list of property types. This file is a simple
    /// .txt file
    /// </summary>
    public static class PropertyTypeHelper
    {
        #region Data
        public static String PROPERTY_TYPE_FILE_NAME = "AvailablePropertyTypes.txt";
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads in a collection of available property names from the .txt file whos
        /// location is specified by the PROPERTY_TYPE_FILE_NAME constant
        /// </summary>
        /// <returns>A collection of read in available property names</returns>
        public static ObservableCollection<String> ReadCurrentlyAvailablePropertyTypes()
        {
            try
            {
                ObservableCollection<String> foundPropertyTypes = new ObservableCollection<String>();

                String appDir = AppDomain.CurrentDomain.BaseDirectory;
                String propertyFileLocation = Path.Combine(appDir, PROPERTY_TYPE_FILE_NAME);

                if (File.Exists(propertyFileLocation))
                {
                    string line;
                    using (StreamReader file = new StreamReader(propertyFileLocation))
                        while ((line = file.ReadLine()) != null)
                            foundPropertyTypes.Add(line);
                }

                (App.Current as App).PropertyTypes.Clear();
                foreach (var prop in foundPropertyTypes)
                {
                    (App.Current as App).PropertyTypes.Add(prop);
                }
                return foundPropertyTypes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Write in a collection of available property names to the .txt file whos
        /// location is specified by the PROPERTY_TYPE_FILE_NAME constant
        /// </summary>
        /// <param name="propertyTypes">A collection of read in available property names</param>
        /// <returns>True if the write succeeded</returns>
        public static Boolean WriteCurrentlyAvailablePropertyTypes(
            ObservableCollection<String> propertyTypes)
        {
            try
            {
                String appDir = AppDomain.CurrentDomain.BaseDirectory;
                String propertyFileLocation = Path.Combine(appDir, PROPERTY_TYPE_FILE_NAME);

                if (File.Exists(propertyFileLocation))
                    File.Delete(propertyFileLocation);

                using (StreamWriter file = new StreamWriter(propertyFileLocation))
                {
                    (App.Current as App).PropertyTypes.Clear();
                    foreach (String prop in propertyTypes)
                    {
                        file.WriteLine(prop);
                        (App.Current as App).PropertyTypes.Add(prop);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
