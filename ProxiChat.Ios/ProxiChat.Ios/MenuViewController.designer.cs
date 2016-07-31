// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ProxiChat.Ios
{
    [Register ("MenuViewController")]
    partial class MenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _menuActionsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_menuActionsTableView != null) {
                _menuActionsTableView.Dispose ();
                _menuActionsTableView = null;
            }
        }
    }
}