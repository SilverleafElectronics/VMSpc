﻿#pragma checksum "..\..\..\DlgWindows\CommDlg.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F55BC9F2FD5567E2AB90D884125FAFB793C613F6F8BBA445B92E24EBF74CC2E1"
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
using System.Windows.Shell;
using VMSpc;
using VMSpc.Communication;
using VMSpc.DlgWindows;


namespace VMSpc.DlgWindows {
    
    
    /// <summary>
    /// CommDlg
    /// </summary>
    public partial class CommDlg : VMSpc.DlgWindows.VMSDialog, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CommSelection;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PortSelection;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ByteCount;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label GoodPacketCount;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label BadPacketCount;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ParsingBehavior;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LogPlayerFileName;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\DlgWindows\CommDlg.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeLogPlayerFile;
        
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
            System.Uri resourceLocater = new System.Uri("/VMSpc;component/dlgwindows/commdlg.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\DlgWindows\CommDlg.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.CommSelection = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.PortSelection = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.ByteCount = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.GoodPacketCount = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.BadPacketCount = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.ParsingBehavior = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.LogPlayerFileName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.ChangeLogPlayerFile = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\..\DlgWindows\CommDlg.xaml"
            this.ChangeLogPlayerFile.Click += new System.Windows.RoutedEventHandler(this.ChangeLogPlayerFile_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

