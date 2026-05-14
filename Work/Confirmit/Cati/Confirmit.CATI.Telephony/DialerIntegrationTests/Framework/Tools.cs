using System;
using System.Linq.Expressions;
using Confirmit.CATI.Telephony.DialerCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerIntegrationTests.Framework
{
    public static class Tools
    {
        /// <summary>
        /// Parses the lambda expression that contains a call of a method.
        /// </summary>
        /// <param name="expectedMethodLambda">The expected method in a form of lambda expression.</param>
        /// <returns>The expected method name.</returns>
        public static string ParseLambda(Expression<Action<IDialerEventsHandlerService>> expectedMethodLambda)
        {
            Assert.AreEqual(ExpressionType.Lambda, expectedMethodLambda.NodeType);
            Assert.AreEqual(ExpressionType.Call, expectedMethodLambda.Body.NodeType);
            var methodCall = (MethodCallExpression)expectedMethodLambda.Body;
            return methodCall.Method.Name;
        }
    }
}