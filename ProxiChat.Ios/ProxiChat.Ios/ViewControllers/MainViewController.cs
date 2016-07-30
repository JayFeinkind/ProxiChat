using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace ProxiChat.Ios
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();
			_userListTableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

			var a = this.NavigationController;

			_userListTableView.RowHeight = UITableView.AutomaticDimension;
			_userListTableView.EstimatedRowHeight = 70;
			_userListTableView.Source = new MainUserListTableViewSource();
			_userListTableView.ReloadData();

			var controller = UIStoryboard.FromName("Messages", null)
			                    .InstantiateViewController("ConversationViewController") 
			                             as ConversationViewController;

			controller.UserName = "JohnnyApplsead";
			controller.ProfileImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/98/Christopher_Fabian_profile.jpg";

			this.NavigationController.PushViewController(controller, true);
		}
    }

	public class MainUserListTableViewSource : UITableViewSource
	{
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(UserListTableViewCell.Key) as UserListTableViewCell;
			cell.UpdateCell(" ", "JohnApplesead", "Some text that was sent last we talked", DateTime.Now);
			return cell;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return "Title " + section;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return 10;
		}
	}
}