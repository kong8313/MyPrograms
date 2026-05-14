using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Common.ConsoleService;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;

namespace Confirmit.CATI.Backend.UnitTests.WcfServices
{
    /// <summary>
    /// Summary description for ExternalWcfServicesAuthorizationTest
    /// </summary>
    [TestClass]
    public class ExternalWcfServicesAuthorizationTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            ServiceLocator.Register<IConsoleVersionValidator, StubIConsoleVersionValidator>();
            ServiceLocator.Register<IInterviewerApiClient, StubIInterviewerApiClient>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            BackendInstance.Current = null;

            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        private void CallTestingMethods<I>(object instance, IEnumerable<MethodInfo> methods2Call)
        {
            var type = typeof(I);

            foreach (var method2Call in methods2Call)
            {
                try
                {
                    var targetMethod = type.GetMethod(method2Call.Name, BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, method2Call.GetParameters().Select(x => x.ParameterType).ToArray(), null);

                    var parameters = (from parameterInfo in targetMethod.GetParameters()
                        select CreateParameterInstance(
                            parameterInfo.ParameterType)).ToArray();

                    targetMethod.Invoke(
                        instance,
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        parameters,
                        null);
                }
                catch (Exception e)
                {
                    ProcessException(method2Call.Name, e);
                }
            }

        }

        private object CreateParameterInstance(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private void ProcessException(string methodName, Exception e)
        {
            var invocationException = e as TargetInvocationException;

            if (invocationException == null)
            {
                throw new Exception(
                    string.Format(
                        "Failed to test authorization for the method {0}", methodName),
                    e);
            }

            var targetException = invocationException.InnerException as TestAuthorizationException;

            if (targetException == null)
            {
                throw new Exception(
                    string.Format(
                        "Failed to test authorization for the method {0}", methodName),
                    e);
            }

        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void MakeSureAllConsoleStateWebServiceMethodsPerformsAuthorization()
        {
            var authorizerStub = new StubIConsoleStateWsRequestsAuthoriser
            {
                AuthoriseRequestBvPersonEntityOutBvTasksEntityOut =
                    (out BvPersonEntity interviewer, out BvTasksEntity task) =>
                    {
                        throw new TestAuthorizationException();
                    }
            };

            ServiceLocator.RegisterInstance<IConsoleStateWsRequestsAuthoriser>(authorizerStub);
            ServiceLocator.RegisterInstance<IConnectionStrings>(new StubIConnectionStrings());
            ServiceLocator.RegisterInstance<IServiceDiscoveryClientProxy>(new StubIServiceDiscoveryClientProxy());

            var consoleStateService = new ConsoleStateService();
            CallTestingMethods<IConsoleStateService>(consoleStateService, typeof(IConsoleStateService).GetMethods());
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void MakeSureAllConsoleWebServiceMethodsPerformsAuthorization()
        {
            var authorizerStub = new StubIConsoleWsRequestsAuthoriser();

            authorizerStub.AuthoriseRequest = () =>
            {
                throw new TestAuthorizationException();
            };

            authorizerStub.AuthoriseRequestBvPersonEntityOut = (out BvPersonEntity interviewer) => 
            {
                throw new TestAuthorizationException();
            };

            authorizerStub.AuthoriseRequestBvPersonEntityOutBvTasksEntityOut = (out BvPersonEntity interviewer, out BvTasksEntity task) => 
            {
                throw new TestAuthorizationException();
            };

            authorizerStub.AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBoolean = (out BvPersonEntity interviewer, out BvTasksEntity task, bool exist) =>
            {
                throw new TestAuthorizationException();
            };

            ServiceLocator.RegisterInstance<IConsoleWsRequestsAuthoriser>(authorizerStub);
            ServiceLocator.RegisterInstance<IConnectionStrings>(new StubIConnectionStrings());
            ServiceLocator.RegisterInstance<IServiceDiscoveryClientProxy>(new StubIServiceDiscoveryClientProxy());

            var consoleService = new ConsoleService();
            var consoleServiceMethods = typeof (IConsoleService).GetMethods();
            CallTestingMethods<IConsoleService>(consoleService, consoleServiceMethods);
        }
    }
}