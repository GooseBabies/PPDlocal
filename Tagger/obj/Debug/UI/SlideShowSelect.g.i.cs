﻿#pragma checksum "..\..\..\UI\SlideShowSelect.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A87386C0C3EE334A0D9670805D2CF8E7E74DFE86"
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
using System.Windows.Shell;
using Tagger;


namespace Tagger {
    
    
    /// <summary>
    /// SlideShowSelect
    /// </summary>
    public partial class SlideShowSelect : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Ratingbox;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MainSearch;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label RowCount;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Main;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button All;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel TagList;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\UI\SlideShowSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Hyperlink li;
        
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
            System.Uri resourceLocater = new System.Uri("/Tagger;component/ui/slideshowselect.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UI\SlideShowSelect.xaml"
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
            
            #line 8 "..\..\..\UI\SlideShowSelect.xaml"
            ((Tagger.SlideShowSelect)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Ratingbox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 22 "..\..\..\UI\SlideShowSelect.xaml"
            this.Ratingbox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Ratingbox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MainSearch = ((System.Windows.Controls.TextBox)(target));
            
            #line 29 "..\..\..\UI\SlideShowSelect.xaml"
            this.MainSearch.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.MainSearch_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.RowCount = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.Main = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\UI\SlideShowSelect.xaml"
            this.Main.Click += new System.Windows.RoutedEventHandler(this.Main_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.All = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\UI\SlideShowSelect.xaml"
            this.All.Click += new System.Windows.RoutedEventHandler(this.All_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.TagList = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 8:
            this.li = ((System.Windows.Documents.Hyperlink)(target));
            
            #line 37 "..\..\..\UI\SlideShowSelect.xaml"
            this.li.Click += new System.Windows.RoutedEventHandler(this.Hyperlink_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

