using Foundation;
using System;
using UIKit;
using System.Linq;

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

			var navController = Storyboard.InstantiateViewController("MainNavigationController") 
			                              									as UINavigationController;

			navController.SetViewControllers(new UIViewController[] {
				Storyboard.InstantiateViewController("MainViewController")
			}, true);

			this.ShowDetailViewController(Storyboard.InstantiateViewController("MainViewController"), this);
		}
    }
}