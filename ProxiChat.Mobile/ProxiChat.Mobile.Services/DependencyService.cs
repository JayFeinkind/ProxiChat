using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using ProxiChat.Mobile.Interfaces;
using Prism.Events;

namespace ProxiChat.Mobile.Services
{
    public class DependencyService : IDependencyService
    {
        private static IUnityContainer _dependencyContainer;

        public DependencyService()
        {
            _dependencyContainer = new UnityContainer();
            SetDependencies();
        }

        private void SetDependencies()
        {
            var eventAggregator = new EventAggregator();
         	_dependencyContainer.RegisterInstance<IEventAggregator>(eventAggregator);
			_dependencyContainer.RegisterType<IApiService, ApiService>();
        }

        public T Resolve<T>()
        {
            return _dependencyContainer.Resolve<T>();
        }

        public void RegisterInstance<T>(T instance)
        {
            _dependencyContainer.RegisterInstance<T>(instance);
        }

        public void RegisterType<T, T1>() where T1 : T
        {
            _dependencyContainer.RegisterType<T, T1>();
        }
    }
}
