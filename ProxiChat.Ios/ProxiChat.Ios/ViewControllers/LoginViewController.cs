using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Ios
{
	/*
	 * Note: Currently if user selects plain text for password -> types in some letters ->
	 *  then switches to secure -> the next character will whipe the text box.  This is not
	 *   really the intended behavior but it is the apple default.  Currently there is no
	 *   easy way to solve this issue.  Will re-address later on if it becomes an issue
	*/
    public partial class LoginViewController : UIViewController
    {
		#region Fields
		LoginViewModel _viewModel;
		#endregion

		#region Properties
		public LoginViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				_viewModel = value;
			}
		}
		#endregion

		#region Ctor
        public LoginViewController (IntPtr handle) : base (handle)
        {
			_viewModel = AppDelegate.DependencyService.Resolve<LoginViewModel>();
        }
		#endregion

		#region Overrides
		public override void LoadView()
		{
			base.LoadView();

			SetLoginButtonStyles(_loginButton);
			SetLoginButtonStyles(_createAccountButton);
			SetLoginButtonStyles(_forgotPassword);

			// stop the password field from clearing text when switching from secure text entry to unsecure
			_passwordTextField.ClearsOnInsertion = false;
			_passwordTextField.ClearsOnBeginEditing = false;

			// set password text field defaults
			_passwordSwitch.On = true;
			_passwordTextField.SecureTextEntry = true;
		}


		/*
		 * All events subscribed to in ViewDidAppear will unsubscribe in
		 *   ViewWillDisappear
		 */

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			_loginButton.TouchDown += LoginButtonPress;
			_createAccountButton.TouchDown += CreateNewAccountPress;
			_passwordSwitch.ValueChanged += SwitchValueChanged;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			_loginButton.TouchDown -= LoginButtonPress;
			_createAccountButton.TouchDown -= CreateNewAccountPress;
			_passwordSwitch.ValueChanged -= SwitchValueChanged;
		}
		#endregion

		#region Events
		private void CreateNewAccountPress(object sender, EventArgs e)
		{
			UIApplication.SharedApplication.OpenUrl(
					new NSUrl("http://www.proxichat.com/proxichatservice/CreateAccount/CreateAccount"));
		}

		private async void LoginButtonPress(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_passwordTextField.Text) || 
				string.IsNullOrWhiteSpace(_userNameTextField.Text))
			{
				var alert = AppUtilities.CreateGenericAlert("Sorry", "User Name and Password are required to continue");
				await PresentViewControllerAsync(alert, true);
				return;
			}

			if (_viewModel.Authenticate())
			{
				//TODO actually authenticate
				var controller = Storyboard.InstantiateViewController("SplitViewController");
				this.ShowViewController(controller, this);
			}
			else
			{
				var alert = AppUtilities.CreateGenericAlert("Sorry", "User Name and Password do not match our records");
				await PresentViewControllerAsync(alert, true);
			}

		}

		private void SwitchValueChanged(object sender, EventArgs e)
		{ 
			_passwordTextField.SecureTextEntry = !_passwordTextField.SecureTextEntry;

			// This shouldn't be necessary but it solves two annoying problems.  
			// 	1: changing SecureEntry from false to true will cause the curser to be drawn farther to the right
			//		then it should.
			//	2: changing SecureEntry from false to true will cause the font size to shrink. Even if you set the 
			//		normal Text property it will be ignored./
			_passwordTextField.AttributedText = new NSAttributedString(_passwordTextField.Text, new UIStringAttributes { 
				Font = UIFont.SystemFontOfSize(14)
			});
		}
		#endregion

		#region UI styles
		private void SetLoginButtonStyles(UIButton button)
		{
			// round the corners of the button
			button.Layer.CornerRadius = 5f;

			// property controls how ui is drawn.  If prop = true the ui control will
			//   fill a rectangular space which hides the rounded corners
			button.Layer.MasksToBounds = false;
		}
		#endregion
    }
}