using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Ios
{
	public partial class MenuViewController : UIViewController
	{
		MenuViewModel _viewModel;

		public MenuViewController(IntPtr handle) : base(handle)
		{
			_viewModel = AppDelegate.DependencyService.Resolve<MenuViewModel>();
		}

		public async override void LoadView()
		{
			base.LoadView();

			await _viewModel.Start();

			_viewModel.ViewModelNavigationRequested += OnViewModelNavigationRequested;

			_menuActionsTableView.Source = new MenuTableViewSource(_viewModel);
			_menuActionsTableView.ReloadData();
		}

		private void OnViewModelNavigationRequested(IApplicationViewModel viewModel)
		{
			if (viewModel is ProxyUsersViewModel)
			{
				var navController = Storyboard.InstantiateViewController("MainNavigationController") as MainNavigationController;
				var viewController = navController.ViewControllers[0] as ProxyUsersViewController;
				viewController.ViewModel = viewModel as ProxyUsersViewModel;
				this.SplitViewController.ShowDetailViewController(navController, this);
			}
		}
	}

	public class MenuTableViewSource : UITableViewSource
	{
		MenuViewModel _viewModel;

		string defaultCellKey = "DefualtCell";

		public MenuTableViewSource(MenuViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(defaultCellKey) ??
								new UITableViewCell(UITableViewCellStyle.Default, defaultCellKey);

			cell.TextLabel.Text = _viewModel.MenuActions[indexPath.Row].Key;
			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _viewModel.MenuActions.Count;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			_viewModel.MenuActions[indexPath.Row].Value();
		}
	}
}