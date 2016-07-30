using Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Mobile.ViewModels
{
    /// <summary>
    /// Include BindableBase for future android support
    /// </summary>
    public class LoginViewModel : BindableBase, IApplicationViewModel
    {
		IApiService _apiService;
        public event ViewModelNavigationRequestHandler ViewModelNavigationRequested;

		public LoginViewModel(IApiService apiService)
		{
			_apiService = apiService;	
		}

        public Task Start()
        {
            throw new NotImplementedException();
        }

		public bool Authenticate()
		{
			return _apiService.Authenticate();
		}

    }
}
