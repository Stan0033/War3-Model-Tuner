﻿#pragma checksum "..\..\..\..\Node Dialogs\edit_eventobject.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FE89BAEEEDE5A6411CB9CE746AA4AB69DCADB2F7"
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
    /// edit_eventobject
    /// </summary>
    public partial class edit_eventobject : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Searcher;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox box;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox tracks;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox SEquenceSelector;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox input;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox inputID;
        
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
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2;component/node%20dialogs/edit_eventobject.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
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
            
            #line 8 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            ((Wa3Tuner.edit_eventobject)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Searcher = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            this.Searcher.KeyDown += new System.Windows.Input.KeyEventHandler(this.Search);
            
            #line default
            #line hidden
            return;
            case 3:
            this.box = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.tracks = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.SEquenceSelector = ((System.Windows.Controls.ComboBox)(target));
            
            #line 31 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            this.SEquenceSelector.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.SetSequence);
            
            #line default
            #line hidden
            return;
            case 6:
            this.input = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            
            #line 33 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.add);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 34 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.remove);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 35 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.removeall);
            
            #line default
            #line hidden
            return;
            case 10:
            this.inputID = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            
            #line 42 "..\..\..\..\Node Dialogs\edit_eventobject.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

