using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Mvvm;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Mobile.ViewModels
{
	public class MenuViewModel : BindableBase, IApplicationViewModel
	{
		List<KeyValuePair<string, Action>> _menuActions;
		IDependencyService _dependencyService;

		public List<KeyValuePair<string, Action>> MenuActions
		{
			get
			{
				return _menuActions;
			}
		}

		public MenuViewModel(IDependencyService dependencyService)
		{
			_dependencyService = dependencyService;

			_menuActions = new List<KeyValuePair<string, Action>>();
		}

		public event ViewModelNavigationRequestHandler ViewModelNavigationRequested;

		public Task Start()
		{
			return Task.Run(() => SetUpMenuActions());
		}

		public void SetUpMenuActions()
		{
			_menuActions.Add(new KeyValuePair<string, Action>("People in Proximity", NavigateToProxyUsers));
		}

		private void NavigateToProxyUsers()
		{
			if (ViewModelNavigationRequested != null)
			{
				ProxyUsersViewModel viewModel = _dependencyService.Resolve<ProxyUsersViewModel>();
				ViewModelNavigationRequested(viewModel);
			}
		}
	}
}

