﻿#pragma checksum "..\..\color_selector.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "00D87DFFFCD4CE4B0110D51F26C1201589AC902F1F09CC981E2B18879FD434AD"
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
using Wa3Tuner;


namespace Wa3Tuner {
    
    
    /// <summary>
    /// color_selector
    /// </summary>
    public partial class color_selector : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider RedSlider;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider GreenSlider;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider BlueSlider;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColorCanvas;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\color_selector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;component/color_selector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\color_selector.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.RedSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 28 "..\..\color_selector.xaml"
            this.RedSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.GreenSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 32 "..\..\color_selector.xaml"
            this.GreenSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BlueSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 36 "..\..\color_selector.xaml"
            this.BlueSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.OnColorChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ColorCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\color_selector.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

