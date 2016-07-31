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

			_userListTableView.RowHeight = UITableView.AutomaticDimension;
			_userListTableView.EstimatedRowHeight = 70;
			_userListTableView.Source = new MainUserListTableViewSource();
			_userListTableView.ReloadData();
		}
    }

	public class MainUserListTableViewSource : UITableViewSource
	{
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(UserListTableViewCell.Key) as UserListTableViewCell;
			cell.UpdateCell(" ", "JohnApplesead", "Some text that was sent last we talked", DateTime.Now);;
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