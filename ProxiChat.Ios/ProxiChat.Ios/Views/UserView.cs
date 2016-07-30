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

		public static UserView Create()
		{
			var nib = NSBundle.MainBundle.LoadNib("UserView", null, null);
			return Runtime.GetNSObject<UserView>(nib.ValueAt(0));
		}

		public override void AwakeFromNib()
		{
			_profileImageView.Layer.MasksToBounds = true;
			_profileImageView.Layer.CornerRadius = 20;
			BackgroundColor = UIColor.Clear;
		}

		public void SetValues(string userName, string profileImageUrl, UIColor textColor)
		{
			_userNameLabel.TextColor = textColor;

			_profileImageView.SetImage(
				url: new NSUrl(profileImageUrl),
				placeholder: ImageUtility.UserProfileImagePlaceHolder.Value
			);

			_userNameLabel.Text = userName;
		}
    }
}