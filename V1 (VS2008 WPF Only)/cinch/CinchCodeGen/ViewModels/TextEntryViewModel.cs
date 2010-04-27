using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;

using Cinch;




namespace CinchCodeGen
{


    /// <summary>
    /// Represents a single text item. This ViewModel is used in conjunction with
    /// the <c>StringEntryPopup</c>
    /// </summary>
    public class TextEntryViewModel : ValidatingViewModelBase
    {
        #region Data
        //Data
        private String currentPropertyType = String.Empty;
        #endregion

        #region Ctor
        public TextEntryViewModel()
        {
   
            #region Create Validation Rules

            this.AddRule(new SimpleRule(currentPropertyTypeChangeArgs.PropertyName, 
                    "CurrentPropertyType can not be empty",
                      delegate
                      {
                          return String.IsNullOrEmpty(this.CurrentPropertyType);
                      }));
            #endregion

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// CurrentPropertyType : The property type
        /// </summary>
        static PropertyChangedEventArgs currentPropertyTypeChangeArgs =
            ObservableHelper.CreateArgs<TextEntryViewModel>(x => x.CurrentPropertyType);

        public String CurrentPropertyType
        {
            get { return currentPropertyType; }
            set
            {
                if (currentPropertyType != value)
                {
                    currentPropertyType = value;
                    NotifyPropertyChanged(currentPropertyTypeChangeArgs);
                }

            }
        }

        #endregion
    }
}
