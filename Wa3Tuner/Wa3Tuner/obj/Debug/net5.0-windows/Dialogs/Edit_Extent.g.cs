﻿#pragma checksum "..\..\..\..\Dialogs\Edit_Extent.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CF48B8314913DD4DB4B7DC5BFBA576E4BE749310"
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
    /// Edit_Extent
    /// </summary>
    public partial class Edit_Extent : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox minx_;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox miny_;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox minz_;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox maxx_;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox maxy_;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox maxz_;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\Dialogs\Edit_Extent.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox bounds_;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.1.7;component/dialogs/edit_extent.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\Edit_Extent.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.minx_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.miny_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.minz_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.maxx_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.maxy_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.maxz_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.bounds_ = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            
            #line 50 "..\..\..\..\Dialogs\Edit_Extent.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.reset);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 51 "..\..\..\..\Dialogs\Edit_Extent.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.copy);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 52 "..\..\..\..\Dialogs\Edit_Extent.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.paste);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 53 "..\..\..\..\Dialogs\Edit_Extent.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ok);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

