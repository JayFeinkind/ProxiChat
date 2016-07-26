using Foundation;
using System;
using UIKit;

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
		#region Ctor
        public LoginViewController (IntPtr handle) : base (handle)
        {
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

			_createAccountButton.TouchDown += CreateNewAccountPress;
			_passwordSwitch.ValueChanged += SwitchValueChanged;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

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
				var alert = AppUtilities.CreateGenericAlert("Oops", "User Name and Password are required to continue");
				await PresentViewControllerAsync(alert, true);
			}
			else
			{
				//TODO initiate login, most likely should be from a pcl
			}
		}

		private void SwitchValueChanged(object sender, EventArgs e)
		{
			_passwordTextField.SecureTextEntry = !_passwordTextField.SecureTextEntry;
		}
		#endregion

		private void SetLoginButtonStyles(UIButton button)
		{
			// round the corners of the button
			button.Layer.CornerRadius = 5f;

			// property controls how ui is drawn.  If prop = true the ui control will
			//   fill a rectangular space which hides the rounded corners
			button.Layer.MasksToBounds = false;
		}
    }
}