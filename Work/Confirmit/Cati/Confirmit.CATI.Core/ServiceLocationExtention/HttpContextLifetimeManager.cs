using System;
using System.Web;

using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Core.ServiceLocationExtention
{
    public class HttpContextLifetimeManager<T> : LifetimeManager, IDisposable
    {
        private readonly string _itemName = typeof(T).AssemblyQualifiedName;

        public override object GetValue()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }

            return HttpContext.Current.Items[_itemName];
        }

        public override void RemoveValue()
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var disposable = GetValue() as IDisposable;
            HttpContext.Current.Items.Remove(_itemName);

            if (disposable != null)
                disposable.Dispose();
        }

        public override void SetValue(object newValue)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpContext.Current.Items[_itemName] = newValue;
        }

        public void Dispose()
        {
            RemoveValue();
        }
    }
}
