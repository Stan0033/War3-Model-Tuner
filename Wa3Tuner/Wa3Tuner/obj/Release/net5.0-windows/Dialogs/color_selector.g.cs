﻿#pragma checksum "..\..\..\..\Dialogs\color_selector.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6CDE62CEA27900A020C4D78F2553A29298288BE4"
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
    /// color_selector
    /// </summary>
    public partial class color_selector : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DisplayR;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DisplayG;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DisplayB;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider RedSlider;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider GreenSlider;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider BlueSlider;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColorCanvas;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Dialogs\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2.3.5;component/dialogs/color_selector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\color_selector.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\..\Dialogs\color_selector.xaml"
            ((Wa3Tuner.color_selector)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DisplayR = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.DisplayG = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.DisplayB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.RedSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 31 "..\..\..\..\Dialogs\color_selector.xaml"
            this.RedSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.GreenSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 34 "..\..\..\..\Dialogs\color_selector.xaml"
            this.GreenSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BlueSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 37 "..\..\..\..\Dialogs\color_selector.xaml"
            this.BlueSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ColorCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 9:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\..\Dialogs\color_selector.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

