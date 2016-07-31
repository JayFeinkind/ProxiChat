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
    [Register ("ConversationViewController")]
    partial class ConversationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _conversationTableview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView _messageBoxTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton _sendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint MessageBoxHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint MessageBoxToBottom { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_conversationTableview != null) {
                _conversationTableview.Dispose ();
                _conversationTableview = null;
            }

            if (_messageBoxTextView != null) {
                _messageBoxTextView.Dispose ();
                _messageBoxTextView = null;
            }

            if (_sendButton != null) {
                _sendButton.Dispose ();
                _sendButton = null;
            }

            if (MessageBoxHeight != null) {
                MessageBoxHeight.Dispose ();
                MessageBoxHeight = null;
            }

            if (MessageBoxToBottom != null) {
                MessageBoxToBottom.Dispose ();
                MessageBoxToBottom = null;
            }
        }
    }
}