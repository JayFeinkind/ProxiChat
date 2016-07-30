using System;
using System.Threading.Tasks;

namespace ProxiChat.Mobile.Interfaces
{
	public interface IApplicationViewModel
	{
		Task Start();
		event ViewModelNavigationRequestHandler ViewModelNavigationRequested;
	}

	public delegate void ViewModelNavigationRequestHandler(IApplicationViewModel viewModel);
}

