﻿#pragma checksum "..\..\..\Popups\ReferencedAssembliesPopup.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DA463A1F3A2A1005EFCD361F8A3A317B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CinchCodeGen;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CinchCodeGen {
    
    
    /// <summary>
    /// ReferencedAssembliesPopup
    /// </summary>
    public partial class ReferencedAssembliesPopup : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
        internal CinchCodeGen.ReferencedAssembliesPopup win;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
        internal System.Windows.Controls.ListBox lst;
        
        #line default
        #line hidden
        
        
        #line 156 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CinchCodeGen;component/popups/referencedassembliespopup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.win = ((CinchCodeGen.ReferencedAssembliesPopup)(target));
            return;
            case 2:
            
            #line 50 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Rectangle_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lst = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 159 "..\..\..\Popups\ReferencedAssembliesPopup.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
