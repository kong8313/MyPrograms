using System;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Common.ServiceLocation
{
    public interface IServiceRegistrator
    {
        IServiceRegistrator Register<TFrom, TTo>() where TTo : TFrom;
        IServiceRegistrator Register<TFrom, TTo>(string name) where TTo : TFrom;
        IServiceRegistrator RegisterSingleton<TFrom, TTo>() where TTo : TFrom;
        IServiceRegistrator RegisterSingleton<TFrom>(TFrom instance);
        IServiceRegistrator RegisterSingleton<TFrom, TTo>(string name) where TTo : TFrom;
        IServiceRegistrator RegisterInstance<TFrom>(TFrom instance);
        IServiceRegistrator RegisterInstance<TFrom>(string name, TFrom instance);
        IServiceRegistrator RegisterSingletonPerThread<TFrom, TTo>() where TTo : TFrom;
        IServiceRegistrator RegisterFactory<TFrom>(Func<TFrom> factory);
        IServiceRegistrator Register<TFrom, TTo>(LifetimeManager manager) where TTo : TFrom;
        IServiceRegistrator Register<TFrom, TTo>(string name, LifetimeManager manager) where TTo : TFrom;
        IServiceRegistrator Register<TFrom>();
    }
}