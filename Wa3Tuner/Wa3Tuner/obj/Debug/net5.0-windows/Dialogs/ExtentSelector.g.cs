﻿#pragma checksum "..\..\..\..\Dialogs\ExtentSelector.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "72F166C206819E8C17AF563EF6B08E2C5CE3E3C5"
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
using Wa3Tuner.Dialogs;


namespace Wa3Tuner.Dialogs {
    
    
    /// <summary>
    /// ExtentSelector
    /// </summary>
    public partial class ExtentSelector : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton m1;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton m2;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton m3;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton m4;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton m5;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox list;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Dialogs\ExtentSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox list2;
        
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
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2.5;component/dialogs/extentselector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\ExtentSelector.xaml"
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
            
            #line 8 "..\..\..\..\Dialogs\ExtentSelector.xaml"
            ((Wa3Tuner.Dialogs.ExtentSelector)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.m1 = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 3:
            this.m2 = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 4:
            this.m3 = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 5:
            this.m4 = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 6:
            this.m5 = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 7:
            this.list = ((System.Windows.Controls.ListBox)(target));
            
            #line 30 "..\..\..\..\Dialogs\ExtentSelector.xaml"
            this.list.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.list_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.list2 = ((System.Windows.Controls.ListBox)(target));
            return;
            case 9:
            
            #line 32 "..\..\..\..\Dialogs\ExtentSelector.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

