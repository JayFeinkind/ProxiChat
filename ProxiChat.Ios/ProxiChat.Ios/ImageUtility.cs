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

		// RGB value of the left most part of the the login gradient
		public static UIColor DefaultNavigationColor = UIColor.FromRGB(52, 172, 73);

		public static UIColor DefaultNavigationTextColr = UIColor.Black;
	}
}

