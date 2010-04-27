using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Documents;




namespace CinchCodeGen
{
    /// <summary>
    /// Displays a list of available referenced Assemblies 
    /// which can be added/removed.
    /// This is bound to a <c>ReferencedAssembliesViewModel</c>
    /// </summary>
	public partial class ReferencedAssembliesPopup : Window
    {
        #region Ctor
        public ReferencedAssembliesPopup()
		{
			this.InitializeComponent();
        }
        #endregion

        #region Window Control Buttons

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion
	}
}