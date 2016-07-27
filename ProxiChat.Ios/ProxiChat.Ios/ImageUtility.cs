using System;
using UIKit;

namespace ProxiChat.Ios
{
	public static class ImageUtility
	{
		public static Lazy<UIImage> UserProfileImagePlaceHolder = new Lazy<UIImage>(() => 
		                                           UIImage.FromBundle("PlaceholderFaceImage"));

		public static Lazy<UIImage> LoginBackgroundImage = new Lazy<UIImage>(() =>
												   UIImage.FromBundle("LoginBackgroundImage"));

		public static UIColor LoginBackgroundImageColor = UIColor.FromPatternImage(LoginBackgroundImage.Value);
	}
}

