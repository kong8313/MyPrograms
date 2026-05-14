using System;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.LogOn;
using Confirmit.CATI.REST.SDK.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass, Ignore]
    public class EnvironmentTests
    {
        private void TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(LogOnSoapClient client)
        {
            try
            {
                client.LogOnUser("123", "321");

                Assert.Fail("FaultException has not beed thrown");
            }
            catch (FaultException e)
            {
                Assert.IsTrue(e.Message.Contains("Incorrect username or password, please try again"));
            }
        }

        private void TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(IRestClient client)
        {
            try
            {
                var service = new SurveyService(client);
                service.GetAsyncByKey("p1231231").Wait();

            }
            catch (AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerExceptions.First(), typeof(InternalServerErrorException));
            }
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_CatiRestApi_Euro()
        {
            var client = Environments.Euro.CreateCatiRestClient("f07f9f10-0eb9-419a-837c-3b2f04c35a65&247:187:2:113:139:90:232:83:242:135:97:11:196:39:67:145+19648", 1);

            TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_CatiRestApi_Us()
        {
            var client = Environments.Us.CreateCatiRestClient("f07f9f10-0eb9-419a-837c-3b2f04c35a65&247:187:2:113:139:90:232:83:242:135:97:11:196:39:67:145+19648", 1);

            TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_CatiRestApi_Au()
        {
            var client = Environments.Australia.CreateCatiRestClient("f07f9f10-0eb9-419a-837c-3b2f04c35a65&247:187:2:113:139:90:232:83:242:135:97:11:196:39:67:145+19648", 1);

            TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_CatiRestApi_Testlab()
        {
            var client = Environments.Testlab.CreateCatiRestClient("f07f9f10-0eb9-419a-837c-3b2f04c35a65&247:187:2:113:139:90:232:83:242:135:97:11:196:39:67:145+19648", 1);

            TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_CatiRestApi_Nordic()
        {
            var client = Environments.Nordic.CreateCatiRestClient("f07f9f10-0eb9-419a-837c-3b2f04c35a65&247:187:2:113:139:90:232:83:242:135:97:11:196:39:67:145+19648", 1);

            TestLogonCatiRestApiThrowsExceptionWithFailedAuthErrorMessage(client);
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_LogonSoapWs_Euro()
        {
            var client = Environments.Euro.CreateLogOnSoapClient();

            TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_LogonSoapWs_Us()
        {
            var client = Environments.Us.CreateLogOnSoapClient();

            TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_LogonSoapWs_Au()
        {
            var client = Environments.Australia.CreateLogOnSoapClient();

            TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_LogonSoapWs_Testlab()
        {
            var client = Environments.Testlab.CreateLogOnSoapClient();

            TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(client);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void EnvironmentTest_LogonSoapWs_Nordic()
        {
            var client = Environments.Nordic.CreateLogOnSoapClient();

            TestLogonSoapClientThrowsFaultExceptionWithFailedAuthErrorMessage(client);
        }
    }
}
