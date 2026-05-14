using System;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Common.ServiceLocation.Fakes
{
    public class StubIServiceRegistrator : IServiceRegistrator 
    {
        private IServiceRegistrator _inner;

        public StubIServiceRegistrator()
        {
            _inner = null;
        }

        public IServiceRegistrator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>()
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(string name)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom, TTo>()
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom>(TFrom instance)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom, TTo>(string name)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterInstance<TFrom>(TFrom instance)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterInstance<TFrom>(string name, TFrom instance)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingletonPerThread<TFrom, TTo>()
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.RegisterFactory<TFrom>(Func<TFrom> factory)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(LifetimeManager manager)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(string name, LifetimeManager manager)
        {


            return default(IServiceRegistrator);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom>()
        {


            return default(IServiceRegistrator);
        }

    }
}