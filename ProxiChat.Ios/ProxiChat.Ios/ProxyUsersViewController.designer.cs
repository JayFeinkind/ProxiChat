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
    [Register ("ProxyUsersViewController")]
    partial class ProxyUsersViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _userListTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_userListTableView != null) {
                _userListTableView.Dispose ();
                _userListTableView = null;
            }
        }
    }
}