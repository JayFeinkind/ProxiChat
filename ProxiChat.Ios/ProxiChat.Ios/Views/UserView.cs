using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using ObjCRuntime;
using SDWebImage;

namespace ProxiChat.Ios
{
    public partial class UserView : UIView
    {
        public UserView (IntPtr handle) : base (handle)
        {
        }

		/// <summary>
		/// Must be progromatically called.  Will start to instantiate view elements
		/// </summary>
		public static UserView Create()
		{
			var nib = NSBundle.MainBundle.LoadNib("UserView", null, null);
			return Runtime.GetNSObject<UserView>(nib.ValueAt(0));
		}

		/// <summary>
		/// replaces usual ViewDidLoad() method for xib's
		/// </summary>
		/// <returns>None</returns>
		public override void AwakeFromNib()
		{
			_profileImageView.Layer.MasksToBounds = true;

			BackgroundColor = UIColor.Clear;
		}

		/// <summary>
		/// Sets the UI values. Must be called programmatically to set up UI
		/// </summary>
		/// <returns>None</returns>
		/// <param name="userName">User name.</param>
		/// <param name="profileImageUrl">Profile image URL used to set image.</param>
		public void SetValues(string userName, string profileImageUrl, nfloat navBarHeight)
		{
			_userNameLabel.TextColor = ImageUtility.DefaultNavigationTextColr;

			_profileImageView.SetImage(
				url: new NSUrl(profileImageUrl),
				placeholder: ImageUtility.UserProfileImagePlaceHolder.Value
			);

			_userNameLabel.Text = userName;

			_profileImageView.Layer.CornerRadius = navBarHeight / 2.0f;
		}
    }
}