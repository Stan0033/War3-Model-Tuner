﻿#pragma checksum "..\..\..\..\Dialogs\TextureBrowser.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "01C7B74CEEA0EDE20390C685173055DB101E4480"
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
    /// TextureBrowser
    /// </summary>
    public partial class TextureBrowser : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl Tabs;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FindSearchBox;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox FindItemListBox;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FavSearchBox;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox FavItemListBox;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ImageHolder;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddTextureButton;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddTextureAndMaterialButton;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAddFavourites;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\Dialogs\TextureBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DelFavouriteButton;
        
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
            System.Uri resourceLocater = new System.Uri("/Wa3Tuner;V1.2.3.5;component/dialogs/texturebrowser.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\TextureBrowser.xaml"
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
            
            #line 9 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            ((Wa3Tuner.TextureBrowser)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 10 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            ((Wa3Tuner.TextureBrowser)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Tabs = ((System.Windows.Controls.TabControl)(target));
            
            #line 37 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.Tabs.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.TabsChange);
            
            #line default
            #line hidden
            return;
            case 3:
            this.FindSearchBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 48 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.FindSearchBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.FindSearchBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.FindItemListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 49 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.FindItemListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DispalyImage);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 53 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.export);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 54 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.exportPNG);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 55 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.copy);
            
            #line default
            #line hidden
            return;
            case 8:
            this.FavSearchBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 71 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.FavSearchBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.FavSearchBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 9:
            this.FavItemListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 72 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.FavItemListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DispalyImage);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ImageHolder = ((System.Windows.Controls.Image)(target));
            return;
            case 11:
            this.AddTextureButton = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.AddTextureButton.Click += new System.Windows.RoutedEventHandler(this.addTexture);
            
            #line default
            #line hidden
            return;
            case 12:
            this.AddTextureAndMaterialButton = ((System.Windows.Controls.Button)(target));
            
            #line 83 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.AddTextureAndMaterialButton.Click += new System.Windows.RoutedEventHandler(this.AddTextureMat);
            
            #line default
            #line hidden
            return;
            case 13:
            this.ButtonAddFavourites = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.ButtonAddFavourites.Click += new System.Windows.RoutedEventHandler(this.AddFavourite);
            
            #line default
            #line hidden
            return;
            case 14:
            this.DelFavouriteButton = ((System.Windows.Controls.Button)(target));
            
            #line 85 "..\..\..\..\Dialogs\TextureBrowser.xaml"
            this.DelFavouriteButton.Click += new System.Windows.RoutedEventHandler(this.DelFavourite);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

