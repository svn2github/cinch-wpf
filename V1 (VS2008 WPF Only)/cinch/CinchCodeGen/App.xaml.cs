using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

using Cinch;

namespace CinchCodeGen
{
    /// <summary>
    /// Provides a globally available list of available property types
    /// and supplies logic for dealing with unhandled Dispatcher excpetions
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        #region Data
        private ObservableCollection<String> propertyTypes = new ObservableCollection<String>();
        private ObservableCollection<FileInfo> referencedAssemblies = new ObservableCollection<FileInfo>();
        #endregion

        #region Overrides
        /// <summary>
        /// On activate hook up a handler for unhandled Exceptions
        /// </summary>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //Read Available Properties
            PropertyTypeHelper.ReadCurrentlyAvailablePropertyTypes();
            ICollectionView propTypeView = CollectionViewSource.GetDefaultView(propertyTypes);
            propTypeView.SortDescriptions.Add(new SortDescription());

            //Read Referenced Assemblies
            ReferencedAssembliesHelper.ReadCurrentlyAvailableReferencedAssemblies();
            ICollectionView refAssView = CollectionViewSource.GetDefaultView(referencedAssemblies);
            refAssView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
 
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }



        #endregion

        #region Private Methods

        /// <summary>
        /// On TextBox_GotFocus select all the Text
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Dispatcher.BeginInvoke((Action)delegate
            {
                txt.SelectAll();
                txt.ReleaseMouseCapture();
            },
            DispatcherPriority.Normal);
        }


        /// <summary>
        /// Occurs when an un handled Exception occurs for the Dispatcher
        /// </summary>
        private void App_DispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            MessageBox.Show("A fatal error occurred " + ex.Message);
            e.Handled = true;
            Environment.Exit(-1);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// PropertyTypes : The list of global Property Types
        /// </summary>
        static PropertyChangedEventArgs propertyTypesChangeArgs =
            ObservableHelper.CreateArgs<App>(x => x.PropertyTypes);

        public ObservableCollection<String> PropertyTypes
        {
            get { return propertyTypes; }
            set
            {
                if (propertyTypes != value)
                {
                    propertyTypes = value;
                   NotifyPropertyChanged(propertyTypesChangeArgs);
                }
            }
        }

        /// <summary>
        /// ReferencedAssemblies : The list of global Referenced Assemblies
        /// </summary>
        static PropertyChangedEventArgs referencedAssembliesChangeArgs =
            ObservableHelper.CreateArgs<App>(x => x.ReferencedAssemblies);

        public ObservableCollection<FileInfo> ReferencedAssemblies
        {
            get { return referencedAssemblies; }
            set
            {
                if (referencedAssemblies != value)
                {
                    referencedAssemblies = value;
                    NotifyPropertyChanged(referencedAssembliesChangeArgs);
                }
            }
        }
        #endregion

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify using pre-made PropertyChangedEventArgs
        /// </summary>
        /// <param name="args"></param>
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
