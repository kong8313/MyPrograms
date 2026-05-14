using System;
using System.Linq.Expressions;
using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.IntegrationTests.Framework.Dialer
{
    public interface ITestDialer : IDialerAPI
    {
        DialType DialType { get; }
        /// <summary>
        /// Checks that there are no expected requests in the expectations queue.
        /// </summary>
        void CheckNoExpectedRequests();

        /// <summary>
        /// Makes the method call to be expected by test dialer.
        /// </summary>
        /// <param name="methodName">The expected method name.</param>
        /// <param name="action">The action to execute during call of the method.</param>
        void AddExpectedRequest(string methodName, Action action = null);

        void SetDefaultRequestBehavior(string methodName, Func<object[], object> action);

        int[] GroupsSentWithLastSetGroups { get; }
    }
}