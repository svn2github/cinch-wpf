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
    /// Represents a single generated code item ViewModel
    /// which will be used to show the code in a COM object.
    /// 
    /// Its a bit of an overkill considering there is a single
    /// non bindable property here, so you may be thinking
    /// whats the point. Well if you check out the resources
    /// section in the <c>MainWindow</c> XAML file, you will
    /// see that I am actually using this Type as a match against
    /// a DataTemplate in the <c>MainWindow</c> XAML file, so it
    /// is quite useful actually
    /// </summary>
    public class GeneratedCodeViewModel : ViewModelBase
    {
        #region Data
        public String FileName { get; set; }

        public override string DisplayName
        {
            get
            {
                return base.DisplayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }

        #endregion
    }
}
