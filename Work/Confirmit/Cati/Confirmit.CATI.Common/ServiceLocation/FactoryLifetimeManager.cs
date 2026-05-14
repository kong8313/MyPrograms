using System;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Common.ServiceLocation
{
    public class FactoryLifetimeManager<T> : LifetimeManager
    {
        readonly Func<T> _factoryMethod;

        public FactoryLifetimeManager(Func<T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public override object GetValue()
        {
            return  _factoryMethod();
        }

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
        }
    }
}