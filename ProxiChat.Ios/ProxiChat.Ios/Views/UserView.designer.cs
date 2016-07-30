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
    [Register ("UserView")]
    partial class UserView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView _profileImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _userNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_profileImageView != null) {
                _profileImageView.Dispose ();
                _profileImageView = null;
            }

            if (_userNameLabel != null) {
                _userNameLabel.Dispose ();
                _userNameLabel = null;
            }
        }
    }
}