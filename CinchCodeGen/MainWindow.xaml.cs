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
using Cinch;




namespace CinchCodeGen
{
    /// <summary>
    /// Top level container View that hosts all the other
    /// sub Views that make up the UI. This is bound to a 
    /// MainWindowViewModel
    /// </summary>
	public partial class MainWindow
    {
        #region Ctor
        public MainWindow()
		{

            //register known windows
            IUIVisualizerService popupVisualizer = ViewModelBase.ServiceProvider.Resolve<IUIVisualizerService>();
            popupVisualizer.Register("PropertyListPopup", typeof(PropertyListPopup));
            popupVisualizer.Register("ReferencedAssembliesPopup", typeof(ReferencedAssembliesPopup));
            popupVisualizer.Register("StringEntryPopup", typeof(StringEntryPopup));
            this.DataContext = new MainWindowViewModel();
            this.InitializeComponent();
        }
        #endregion

        #region Window Control Buttons

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSize_Click(object sender, RoutedEventArgs e)
        {
            this.Height = this.MinHeight;
            this.Width = this.MinWidth;
            this.WindowState = WindowState.Normal;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion
	}
}