﻿#pragma checksum "..\..\..\..\Dialogs\model_starter.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "27465FA48ABB0E5E75108551A61181191EAFC291"
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
    /// model_starter
    /// </summary>
    public partial class model_starter : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 31 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputName;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListType;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListSequences;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListAttachments;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckAlternate;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckMedLar;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckSwim;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckUpgrades;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboUpgradeNumber;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboVar;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\..\Dialogs\model_starter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputSequenceLen;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.3.1;component/dialogs/model_starter.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\model_starter.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\..\Dialogs\model_starter.xaml"
            ((Wa3Tuner.Dialogs.model_starter)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.InputName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ListType = ((System.Windows.Controls.ListBox)(target));
            
            #line 34 "..\..\..\..\Dialogs\model_starter.xaml"
            this.ListType.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 55 "..\..\..\..\Dialogs\model_starter.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.sff);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ListSequences = ((System.Windows.Controls.ListBox)(target));
            return;
            case 6:
            this.ListAttachments = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.CheckAlternate = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 8:
            this.CheckMedLar = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this.CheckSwim = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.CheckUpgrades = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.ComboUpgradeNumber = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 12:
            this.ComboVar = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 13:
            this.InputSequenceLen = ((System.Windows.Controls.TextBox)(target));
            return;
            case 14:
            
            #line 102 "..\..\..\..\Dialogs\model_starter.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

