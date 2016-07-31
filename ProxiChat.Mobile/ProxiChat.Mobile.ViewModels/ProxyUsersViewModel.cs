using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Mobile.ViewModels
{
	public class ProxyUsersViewModel : BindableBase, IApplicationViewModel
	{
		IDependencyService _dependencyService;

		public ProxyUsersViewModel(IDependencyService dependencyService)
		{
			_dependencyService = dependencyService;
		}

		public event ViewModelNavigationRequestHandler ViewModelNavigationRequested;

		public Task Start()
		{
			throw new NotImplementedException();
		}

		public void NavigateToConversation(string userName, string profileImageUrl)
		{
			if (ViewModelNavigationRequested != null)
			{
				var viewModel = _dependencyService.Resolve<ConversationViewModel>() as ConversationViewModel;
				viewModel.UserName = userName;
				viewModel.ProfileImageUrl = profileImageUrl;
				ViewModelNavigationRequested(viewModel);
			}
		}
	}
}

