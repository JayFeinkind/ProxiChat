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
    [Register ("ReceiverMessageTableViewCell")]
    partial class ReceiverMessageTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView _messageBodyView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView _profileImageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_messageBodyView != null) {
                _messageBodyView.Dispose ();
                _messageBodyView = null;
            }

            if (_profileImageView != null) {
                _profileImageView.Dispose ();
                _profileImageView = null;
            }
        }
    }
}