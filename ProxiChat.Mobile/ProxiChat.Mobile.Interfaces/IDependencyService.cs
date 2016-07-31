using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxiChat.Mobile.Interfaces
{
    public interface IDependencyService
    {
        T Resolve<T>();
        void RegisterInstance<T>(T instance);
        void RegisterType<T, T1>() where T1 : T;
    }
}
