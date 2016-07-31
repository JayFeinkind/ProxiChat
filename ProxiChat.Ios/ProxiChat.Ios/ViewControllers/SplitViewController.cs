using Foundation;
using System;
using UIKit;

namespace ProxiChat.Ios
{
    public partial class SplitViewController : UISplitViewController
    {
        public SplitViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();

			var detail = Storyboard.InstantiateViewController("MainNavigationController");
			this.ShowDetailViewController(detail, this);
		}
    }
}