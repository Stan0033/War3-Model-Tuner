﻿#pragma checksum "..\..\..\..\Dialogs\SCALETOFIT.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C6C44A2865024F146DBBDD11ECDABEF9F5FB4BCD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
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
using System.Windows.Shell;
using Wa3Tuner;


namespace Wa3Tuner {
    
    
    /// <summary>
    /// SCALETOFIT
    /// </summary>
    public partial class SCALETOFIT : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MinXInput;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MinYInput;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MinZInput;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MaxXInput;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MaxYInput;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MaxZInput;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton SelectedGeosetsEach;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton SelectedGeosetsTogether;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2.7;component/dialogs/scaletofit.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
            ((Wa3Tuner.SCALETOFIT)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MinXInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.MinYInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.MinZInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.MaxXInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.MaxYInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.MaxZInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.SelectedGeosetsEach = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 9:
            this.SelectedGeosetsTogether = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 10:
            
            #line 48 "..\..\..\..\Dialogs\SCALETOFIT.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

