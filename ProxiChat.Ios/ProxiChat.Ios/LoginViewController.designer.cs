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
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView _backgroundImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView _containerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton _createAccountButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton _forgotPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton _loginButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch _passwordSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField _passwordTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField _userNameTextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_backgroundImage != null) {
                _backgroundImage.Dispose ();
                _backgroundImage = null;
            }

            if (_containerView != null) {
                _containerView.Dispose ();
                _containerView = null;
            }

            if (_createAccountButton != null) {
                _createAccountButton.Dispose ();
                _createAccountButton = null;
            }

            if (_forgotPassword != null) {
                _forgotPassword.Dispose ();
                _forgotPassword = null;
            }

            if (_loginButton != null) {
                _loginButton.Dispose ();
                _loginButton = null;
            }

            if (_passwordSwitch != null) {
                _passwordSwitch.Dispose ();
                _passwordSwitch = null;
            }

            if (_passwordTextField != null) {
                _passwordTextField.Dispose ();
                _passwordTextField = null;
            }

            if (_userNameTextField != null) {
                _userNameTextField.Dispose ();
                _userNameTextField = null;
            }
        }
    }
}