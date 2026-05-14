using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Confirmit.CATI.Common.WcfTools
{
    public interface IChannelFactoryWrapper<T> where T : class
    {
        /// <summary>
        /// Gets the WCF client proxy. Either cached or newly constructed - depending on caching option in constructor.
        /// </summary>
        /// <returns>WCF client proxy.</returns>
        T GetChannel();

        /// <summary>
        /// Get factory object
        /// </summary>
        /// <returns></returns>
        Uri GetFactoryUri();

        /// <summary>
        /// Releases inner channel factory and closes all active connections of all channels created by this factory.
        /// </summary>
        void Release();

        /// <summary>
        /// Executes the specified action over the channel stored in this class.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="methodName"></param>
        void Execute(Action<T> action, [CallerMemberName]string methodName="");

        /// <summary>
        /// Executes the specified function over the channel stored in this class.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <param name="methodName"></param>
        TResult Execute<TResult>(Func<T, TResult> function, [CallerMemberName]string methodName="");
    }
}