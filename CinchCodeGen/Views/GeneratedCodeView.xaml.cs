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


using System.IO;

namespace CinchCodeGen
{
    /// <summary>
    /// Displays the generated code using a lovely Visual Studio like
    /// NATIVE WPF syntax highlighting control. Which sadly you MUST have a license to use
    /// it. I do you more than likely do not, so use the
    /// <c>GeneratedCodeViewNoLicense</c> in the DataTemplate in the <c>MainWindow</c>
    /// XAML resources
    /// 
    /// NOTE : This class has one tiny bit of code behind which could have been achieved
    /// using attached behaviours, but to be honest I did not see the point of it, however
    /// attached behaviours are generally preferable to code behind
    /// </summary>
    public partial class GeneratedCodeView : UserControl
    {
        #region Ctor
        public GeneratedCodeView()
        {

            InitializeComponent();
            this.DataContextChanged += GeneratedCodeView_DataContextChanged;
        }
        #endregion

        //EEEK : Code behind but read the comments at start before you go nuts
        #region Private Methods
        /// <summary>
        /// Nasty code behind, but as we are using Winforms Interop control
        /// this seemed a sensible place to put it rather than in some attached
        /// behaviour that would have done this anyway
        /// </summary>
        private void GeneratedCodeView_DataContextChanged(object sender, 
            DependencyPropertyChangedEventArgs e)
        {
            GeneratedCodeViewModel vm = e.NewValue as GeneratedCodeViewModel;
            using (StreamReader sr = new StreamReader(vm.FileName))
            {
                txtCode.IsReadOnly = false;
                txtCode.Load(vm.FileName);
                txtCode.IsReadOnly = true;
            }
        }
        #endregion
    }
}
