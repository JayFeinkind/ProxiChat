using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Ios
{
	public partial class ProxyUsersViewController : UIViewController, IUnsubscribeViewController
    {
		ProxyUsersViewModel _viewModel;

		public ProxyUsersViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				_viewModel = value;
			}
		}

        public ProxyUsersViewController (IntPtr handle) : base (handle)
        {
			if (_viewModel == null)
			{
				_viewModel = AppDelegate.DependencyService.Resolve<ProxyUsersViewModel>();
			}
        }

		public override void LoadView()
		{
			base.LoadView();
			_userListTableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

			this.NavigationItem.Title = "Proxi Users";

			_userListTableView.RowHeight = UITableView.AutomaticDimension;
			_userListTableView.EstimatedRowHeight = 70;
			_userListTableView.Source = new MainUserListTableViewSource(_viewModel);
			_userListTableView.ReloadData();

			_viewModel.ViewModelNavigationRequested += OnViewModelNavigationRequested;
		}

		private void OnViewModelNavigationRequested(IApplicationViewModel viewModel)
		{
			if (viewModel is ConversationViewModel)
			{
				var controller = UIStoryboard.FromName("Messages", null)
									.InstantiateViewController("ConversationViewController")
											 as ConversationViewController;

				controller.ViewModel = viewModel as ConversationViewModel;

				this.NavigationController.PushViewController(controller, true);
			}
		}

		public void UnsubscribeFromEvents()
		{
			if (_viewModel != null)
			{
				_viewModel.ViewModelNavigationRequested -= OnViewModelNavigationRequested;
			}
		}
	}

	public class MainUserListTableViewSource : UITableViewSource
	{
		ProxyUsersViewModel _viewModel;

		public MainUserListTableViewSource(ProxyUsersViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(UserListTableViewCell.Key) as UserListTableViewCell;
			cell.UpdateCell(" ", "JohnnyApplesead", "Some text that was sent last we talked", DateTime.Now);
			return cell;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return "Within " + (section + 1) * 5 + " Miles";
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return 3;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var tmp = "https://upload.wikimedia.org/wikipedia/commons/9/98/Christopher_Fabian_profile.jpg";
			_viewModel.NavigateToConversation("JohnnyApplesead", tmp);
		}
    }
}