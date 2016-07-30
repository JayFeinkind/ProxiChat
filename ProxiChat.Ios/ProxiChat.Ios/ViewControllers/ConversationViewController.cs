using Foundation;
using System;
using UIKit;

namespace ProxiChat.Ios
{
    public partial class ConversationViewController : UIViewController
    {
        public ConversationViewController (IntPtr handle) : base (handle)
        {
        }


		public override void LoadView()
		{
			base.LoadView();

			var navView = UserView.Create();
			navView.SetValues(UserName, ProfileImageUrl, ImageUtility.DefaultNavigationTextColr);

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, null);
			this.NavigationItem.TitleView = navView;

		}

		public string UserName { get; set; }
		public string ProfileImageUrl { get; set; }
	}
}