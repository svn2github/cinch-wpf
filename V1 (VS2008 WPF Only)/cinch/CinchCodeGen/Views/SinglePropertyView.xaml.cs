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
    /// Displays the UI for a single ViewModel property such as
    /// Property name/Property type/use DataWrapper. This is bound
    /// to a <c>SinglePropertyViewModel</c>
    /// </summary>
    public partial class SinglePropertyView : UserControl
    {
        #region Ctor
        public SinglePropertyView()
        {
            InitializeComponent();
        }
        #endregion
    }
}
