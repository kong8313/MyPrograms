using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Common.ServiceLocation
{
    public class ServiceLocator : IServiceRegistrator, IServiceResolver, IServiceInitializer
    {
        // TODO: This should be the only static field/object in whole app, rest singleton must be registered in the container.
        private static IUnityContainer _unityContainer;

        public static void StaticInitialize()
        {
            if (_unityContainer != null)
            {
                throw new ServiceLocatorException("ServiceLocator already initialized.");
            }

            _unityContainer = new UnityContainer();

            // Register ourself for the IServiceRegistrator/IServiceResolver
            // So, other types can use interface and not just a static methods.
            _unityContainer.RegisterType<IServiceRegistrator, ServiceLocator>();
            _unityContainer.RegisterType<IServiceResolver, ServiceLocator>();
            _unityContainer.RegisterType<IServiceInitializer, ServiceLocator>();
        }

        public static void StaticCleanup()
        {
            if (_unityContainer != null)
            {
                _unityContainer.Dispose();
            }

            _unityContainer = null;
        }

        public void Initialize()
        {
            StaticInitialize();
        }


        public void Cleanup()
        {
            StaticCleanup();
        }

        public static IUnityContainer CreateChildContainer()
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.CreateChildContainer();
        }

        public static IUnityContainer GlobalContainer
        {
            get { return _unityContainer; }

        }

        private static void ValidateIsServiceLocatorInitialized()
        {
            if (_unityContainer == null)
            {
                throw new ServiceLocatorException("ServiceLocator is not initialized");
            }
        }

        /// <summary>
        /// Resolves dependency.
        /// Static shortcut method. Try to avoid using it.
        /// Instead of explicitly calling static Resolve inject IServiceResolver interfaces throught constructor.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Resolved type</returns>
        public static T Resolve<T>([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            var timer = Stopwatch.StartNew();
            
            ValidateIsServiceLocatorInitialized();

            var resolve = _unityContainer.Resolve<T>();
            
            if (timer.Elapsed.TotalMilliseconds > 100)
            {
                var elapsed = Math.Round(timer.Elapsed.TotalMilliseconds, 2);
                var location = sourceFilePath.Split('\\').Last().Split('.').First() + "." + memberName;
                Trace.TraceInformation($"{location}: Resolving {typeof(T).Name} {elapsed}ms");
            }
            
            return resolve;
        }

        public static object Resolve(Type t)
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.Resolve(t);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.ResolveAll<T>();
        }

        public static IEnumerable<object> ResolveAll(Type t)
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.ResolveAll(t);
        }

        public static T ResolveByName<T>(string name)
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.Resolve<T>(name);
        }

        /// <summary>
        /// Resolves dependency.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Resolved type</returns>
        T IServiceResolver.Resolve<T>()
        {
            ValidateIsServiceLocatorInitialized();

            return _unityContainer.Resolve<T>();
        }

        public static void Register<TFrom, TTo>() where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>();
        }
        
        public static void Register<TFrom>()
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom>();
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>()
        {
            Register<TFrom, TTo>();
            return this;
        }
        
        IServiceRegistrator IServiceRegistrator.Register<TFrom>()
        {
            Register<TFrom>();
            return this;
        }

        public static void Register<TFrom, TTo>(string name) where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(name);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(string name)
        {
            Register<TFrom, TTo>(name);
            return this;
        }

        public static void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom, TTo>()
        {
            RegisterSingleton<TFrom, TTo>();
            return this;
        }

        public static void RegisterSingleton<TFrom>(TFrom instance)
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterInstance(instance, new ContainerControlledLifetimeManager());
        }


        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom>(TFrom instance)
        {
            RegisterSingleton(instance);
            return this;
        }

        public static void RegisterSingleton<TFrom, TTo>(string name) where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager());
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingleton<TFrom, TTo>(string name)
        {
            RegisterSingleton<TFrom, TTo>(name);
            return this;
        }

        public static void RegisterInstance<TFrom>(TFrom instance)
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterInstance(instance, new ContainerControlledLifetimeManager());
        }

        IServiceRegistrator IServiceRegistrator.RegisterInstance<TFrom>(TFrom instance)
        {
            RegisterInstance(instance);
            return this;
        }

        public static void RegisterInstance<TFrom>(string name, TFrom instance)
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterInstance(name, instance, new ContainerControlledLifetimeManager());
        }

        IServiceRegistrator IServiceRegistrator.RegisterInstance<TFrom>(string name, TFrom instance)
        {
            RegisterInstance(name, instance);
            return this;
        }

        public static void RegisterSingletonPerThread<TFrom, TTo>() where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(new PerThreadLifetimeManager());
        }

        IServiceRegistrator IServiceRegistrator.RegisterSingletonPerThread<TFrom, TTo>()
        {
            RegisterSingletonPerThread<TFrom, TTo>();
            return this;
        }

        public static void RegisterFactory<TFrom>(Func<TFrom> factory)
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom>(new FactoryLifetimeManager<TFrom>(factory));
        }

        IServiceRegistrator IServiceRegistrator.RegisterFactory<TFrom>(Func<TFrom> factory)
        {
            RegisterFactory(factory);
            return this;
        }

        public static void Register<TFrom, TTo>(LifetimeManager manager) where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(manager);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(LifetimeManager manager)
        {
            Register<TFrom, TTo>(manager);
            return this;
        }

        public static void Register<TFrom, TTo>(string name, LifetimeManager manager) where TTo : TFrom
        {
            ValidateIsServiceLocatorInitialized();

            _unityContainer.RegisterType<TFrom, TTo>(name, manager);
        }

        IServiceRegistrator IServiceRegistrator.Register<TFrom, TTo>(string name, LifetimeManager manager)
        {
            Register<TFrom, TTo>(name, manager);
            return this;
        }
    }
}