﻿#pragma checksum "..\..\..\..\Dialogs\ImportGeoset.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A335F2BCADB2433E1558D56CBE6CD53910C9E252"
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
    /// ImportGeosetDialog
    /// </summary>
    public partial class ImportGeosetDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\Dialogs\ImportGeoset.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboAttachTo;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\Dialogs\ImportGeoset.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Check_NewBone;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\Dialogs\ImportGeoset.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboMaterial;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2;component/dialogs/importgeoset.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\ImportGeoset.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\..\Dialogs\ImportGeoset.xaml"
            ((Wa3Tuner.Dialogs.ImportGeosetDialog)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ComboAttachTo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.Check_NewBone = ((System.Windows.Controls.CheckBox)(target));
            
            #line 22 "..\..\..\..\Dialogs\ImportGeoset.xaml"
            this.Check_NewBone.Checked += new System.Windows.RoutedEventHandler(this.CheckedNB);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\..\Dialogs\ImportGeoset.xaml"
            this.Check_NewBone.Unchecked += new System.Windows.RoutedEventHandler(this.CheckedNB);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ComboMaterial = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            
            #line 28 "..\..\..\..\Dialogs\ImportGeoset.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

