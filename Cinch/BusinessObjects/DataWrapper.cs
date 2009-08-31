using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;

namespace Cinch
{
    /// <summary>
    /// Provides a wrapper around a single peice of data
    /// such that the ViewModel can put the data item
    /// into a editable state and the View can bind to
    /// both the DataValue for the actual Value, and to 
    /// the IsEditable to determine if the control which
    /// has the data is allowed to be used for entering data.
    /// 
    /// The Viewmodel is expected to set the state of the
    /// IsEditable property for all DataWrappers in a given Model
    /// </summary>
    /// <typeparam name="T">The type of the Data</typeparam>
    public class DataWrapper<T> : EditableValidatingObject
    {
        #region Data
        private T dataValue = default(T);
        private Boolean isEditable = false;

        private IParentablePropertyExposer parent = null;
        private PropertyChangedEventArgs parentPropertyChangeArgs = null;

        #endregion

        #region Ctors
        public DataWrapper()
        {
        }

        public DataWrapper(IParentablePropertyExposer parent, 
            PropertyChangedEventArgs parentPropertyChangeArgs)
        {
            this.parent = parent;
            this.parentPropertyChangeArgs = parentPropertyChangeArgs;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Notifies all the parent (INPC) objects INotifyPropertyChanged.PropertyChanged subscribed delegates
        /// that an internal DataWrapper property value has changed, which in turn raises the appropriate
        /// INotifyPropertyChanged.PropertyChanged event on the parent (INPC) object
        /// </summary>
        private void NotifyParentPropertyChanged()
        {
            if (parent == null || parentPropertyChangeArgs == null)
                return;

            //notify all delegates listening to DataWrapper<T> parent objects PropertyChanged
            //event
            Delegate[] subscribers = parent.GetINPCSubscribers();
            if (subscribers != null)
            {
                foreach (PropertyChangedEventHandler d in subscribers)
                {
                    d(parent, parentPropertyChangeArgs);
                }
            }
        }


        #endregion

        #region Public Properties
        /// <summary>
        /// The actual data value, the View is
        /// expected to bind to this to display data
        /// </summary>
        static PropertyChangedEventArgs dataValueChangeArgs =
            ObservableHelper.CreateArgs<DataWrapper<T>>(x => x.DataValue);

        public T DataValue
        {
            get { return dataValue; }
            set
            {
                dataValue = value;
                NotifyPropertyChanged(dataValueChangeArgs);
                NotifyParentPropertyChanged();
            }
        }


        /// <summary>
        /// The editable state of the data, the View
        /// is expected to use this to enable/disable
        /// data entry. The ViewModel would set this
        /// property
        /// </summary>
        static PropertyChangedEventArgs isEditableChangeArgs =
            ObservableHelper.CreateArgs<DataWrapper<T>>(x => x.IsEditable);

        public Boolean IsEditable
        {
            get { return isEditable; }
            set
            {
                if (isEditable != value)
                {
                    isEditable = value;
                    NotifyPropertyChanged(isEditableChangeArgs);
                    NotifyParentPropertyChanged();
                }
            }

        }
        #endregion
    }


    /// <summary>
    /// Provides helper methods for dealing with DataWrappers
    /// within the Cinch library. 
    /// </summary>
    public class DataWrapperHelper
    {
        #region Static Methods

        #region Public Methods
        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to set all Cinch.DataWrapper fields to have the correct Cinch.DataWrapper.IsEditable 
        /// to the correct state based on the current ViewMode 
        /// </summary>
        /// <typeparam name="T">The type which has the DataWrappers on it</typeparam>
        /// <param name="objectToSetModeOn">The source object to alter the 
        /// Cinch.DataWrapper on</param>
        /// <param name="currentViewMode">The current ViewMode</param
        public static void SetModeForObject<T>(T objectToSetModeOn, ViewMode currentViewMode)
        {
            if (objectToSetModeOn == null)
                return;

            bool isEditable = currentViewMode ==
                    ViewMode.EditMode || currentViewMode == ViewMode.AddMode;

            foreach (var item in objectToSetModeOn.GetType().GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    var fieldType = item.GetValue(objectToSetModeOn).GetType();
                    if (fieldType.GetGenericTypeDefinition() == typeof(Cinch.DataWrapper<>))
                    {
                        fieldType.GetProperty("IsEditable").SetValue(
                            item.GetValue(objectToSetModeOn), isEditable, null);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem setting the currentViewMode");
                }
            }
        }


        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the BeginEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <typeparam name="T">The type which has the DataWrappers on it</typeparam>
        /// <param name="objectToSetModeOn">The source object to alter the 
        /// Cinch.DataWrapper on</param>
        public static void SetBeginEdit<T>(T objectToSetModeOn)
        {
            if (objectToSetModeOn == null)
                return;

            SetBeginEndOrCancel<T>(objectToSetModeOn, "BeginEdit");
        }

        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the CancelEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <typeparam name="T">The type which has the DataWrappers on it</typeparam>
        /// <param name="objectToSetModeOn">The source object to alter the 
        /// Cinch.DataWrapper on</param>
        public static void SetCancelEdit<T>(T objectToSetModeOn)
        {
            if (objectToSetModeOn == null)
                return;

            SetBeginEndOrCancel<T>(objectToSetModeOn, "CancelEdit");
        }

        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the EditEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <typeparam name="T">The type which has the DataWrappers on it</typeparam>
        /// <param name="objectToSetModeOn">The source object to alter the 
        /// Cinch.DataWrapper on</param>
        public static void SetEndEdit<T>(T objectToSetModeOn)
        {
            if (objectToSetModeOn == null)
                return;

            SetBeginEndOrCancel<T>(objectToSetModeOn, "EndEdit");
        }


        #endregion

        #region Private Methods
        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the BeginEdit or CancelEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <typeparam name="T">The type which has the DataWrappers on it</typeparam>
        /// <param name="objectToSetModeOn">The source object to alter the 
        /// Cinch.DataWrapper on</param>
        /// <param name="editMethodNameString">The name of the method to call, should be
        /// either "BeginEdit" or "EndEdit" or "CancelEdit" ONLY</param>
        private static void SetBeginEndOrCancel<T>(T objectToSetModeOn, String editMethodNameString)
        {
            foreach (var propItem in objectToSetModeOn.GetType().GetProperties(
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    if (propItem.CanRead)
                    {
                        var fieldType = propItem.GetValue(objectToSetModeOn, null).GetType();

                        if (fieldType.GetGenericTypeDefinition() == typeof(Cinch.DataWrapper<>))
                        {
                            MethodInfo miEditMethod = fieldType.GetGenericTypeDefinition()
                                .GetMethod(editMethodNameString);
                            miEditMethod.Invoke(propItem.GetValue(objectToSetModeOn,null), null);

                            //now call the INotifyPropertyChanged in the parent object
                            //so it can notify the Bindings that are bound to a nested
                            //DataWrapper<T> object, that may have changed due to a 
                            //BeginEdit/CancelEdit method call on the DataWrapper<T>
                            MethodInfo miOnPropertyChanged =
                                objectToSetModeOn.GetType().GetMethod("OnPropertyChanged");

                            miOnPropertyChanged.Invoke(objectToSetModeOn, new Object[] { propItem.Name });
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem calling the Edit method for the current DataWrapper");
                }
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// This interface is implemented by both the 
    /// <see cref="ValidatingObject">ValidatingObject</see> and the
    /// <see cref="ViewModelBase">ViewModelBase</see> classes, and is used
    /// to expose the list of delegates that are currently listening to the
    /// <see cref="System.ComponentModel.INotifyPropertyChanged">INotifyPropertyChanged</see>
    /// PropertyChanged event. This is done so that the internal 
    /// <see cref="DataWrapper">DataWrapper</see> classes can notify their parent object
    /// when an internal <see cref="DataWrapper">DataWrapper</see> property changes
    /// </summary>
    public interface IParentablePropertyExposer
    {
        Delegate[] GetINPCSubscribers();
    }

}
