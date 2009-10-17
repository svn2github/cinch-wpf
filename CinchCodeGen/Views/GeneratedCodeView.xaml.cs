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
using System.Windows.Threading;
using System.Xml;
using Microsoft.Win32;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;



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
        #region Data
        private FoldingManager foldingManager;
        private AbstractFoldingStrategy foldingStrategy;
        #endregion


        #region Ctor
        public GeneratedCodeView()
        {

            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(GeneratedCodeView).Assembly.GetManifestResourceStream("CinchCodeGen.UserControls.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);



            InitializeComponent();

            txtCode.SyntaxHighlighting = customHighlighting;

            if (txtCode.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                foldingStrategy = new BraceFoldingStrategy();
            }

            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(txtCode.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, txtCode.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();


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
            txtCode.Load(vm.FileName);
        }


        private void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, txtCode.Document);
            }
        }

        #endregion
    }
}
