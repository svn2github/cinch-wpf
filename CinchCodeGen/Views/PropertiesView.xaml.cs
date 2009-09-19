using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinchCodeGen
{
    /// <summary>
    /// Displays a list of properties for the ViewModel, this
    /// is bound to a <c>PropertiesViewModel</c>
    /// </summary>
    public partial class PropertiesView : UserControl
    {
        #region Ctor
        public PropertiesView()
        {
            InitializeComponent();
        }
        #endregion
    }
}
