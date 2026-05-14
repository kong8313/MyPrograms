using System;
using System.Web;
using Confirmit.CATI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests
{
    [TestClass]
    public class InterviewUrlBuilderTest
    {
        private const string StartUrlPrefixWithoutPort = "http://localhost/wix/cati_";
        private const string StartUrlPrefixWithPort = "http://localhost:9999/wix/cati_";        
        private const string ProjectId = "p123456";
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterviewUrlBuilder_ProjectIdIsNull_ExceptionIsThrown()
        {
            new InterviewUrlBuilder(StartUrlPrefixWithoutPort, null, true);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterviewUrlBuilder_ProjectIdIsEmpty_ExceptionIsThrown()
        {
            new InterviewUrlBuilder(StartUrlPrefixWithoutPort, string.Empty, true);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void InterviewUrlBuilder_ProjectIdIsGiven_ProjectIdIsAddedToInterviewUrl()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);

            var url = new Uri(builder.Url);
            Assert.AreEqual("/wix/cati_" + ProjectId + ".aspx", url.AbsolutePath);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void InterviewUrlBuilder_DoNotEnforceHttps_HttpSchemeIsUsed()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);

            var url = new Uri(builder.Url);
            Assert.AreEqual("http", url.Scheme);            
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void InterviewUrlBuilder_EnforceHttps_HttpsSchemeIsUsed()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, true);
            
            var url = new Uri(builder.Url);
            Assert.AreEqual("https", url.Scheme);
            Assert.IsTrue(url.IsDefaultPort);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AddParameterWintUrlEncode_SingleParameter_ParameterIsAdded()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);
            var name = "param";
            var value = "val";
            builder.AddParameterWithUrlEncode(name, value);

            var url = new Uri(builder.Url);
            string[] parameters = AssertParametersCount(url, 1);
            string[] tokens = AssertParameter(parameters[0]);

            Assert.AreEqual(name, tokens[0], "Incorrect parameter name");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AddParameterWintUrlEncode_SingleParameter_ParameterValueIsCorrect()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);
            var name = "param";
            var value = "val";
            builder.AddParameterWithUrlEncode(name, value);

            var url = new Uri(builder.Url);
            string[] parameters = AssertParametersCount(url, 1);
            string[] tokens = AssertParameter(parameters[0]);

            Assert.AreEqual(value, tokens[1]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AddParameterWithUrlEncode_SingleParameter_ParamNameIsEncoded()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);
            var name = "<param name>";
            var value = "val";
            builder.AddParameterWithUrlEncode(name, value);

            var expectedName = HttpUtility.UrlEncode(name);
            var url = new Uri(builder.Url);
            string[] parameters = AssertParametersCount(url, 1);
            string[] tokens = AssertParameter(parameters[0]);

            Assert.AreEqual(expectedName, tokens[0]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AddParameterWithUrlEncode_SingleParameter_ParamValueIsEncoded()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);
            var name = "param";
            var value = "<value name='fff' id=3> & tt";
            builder.AddParameterWithUrlEncode(name, value);

            var expectedValue = HttpUtility.UrlEncode(value);
            var url = new Uri(builder.Url);
            string[] parameters = AssertParametersCount(url, 1);
            string[] tokens = AssertParameter(parameters[0]);

            Assert.AreEqual(expectedValue, tokens[1]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AddParameterWithUrlEncode_ThreeParameters_QueryContainsThreeParameters()
        {
            var parameters = new[]
                                 {
                                     new {Name = "fds", Value = (object) 10},
                                     new {Name = "<ggggm>,", Value = (object) "&vdsds"},
                                     new {Name = "ee'", Value = (object) DateTime.Now}
                                 };

            var builder = new InterviewUrlBuilder(StartUrlPrefixWithoutPort, ProjectId, false);
            Array.ForEach(parameters, param => builder.AddParameterWithUrlEncode(param.Name, param.Value));
            
            var url = new Uri(builder.Url);
            var queryParameters = AssertParametersCount(url, parameters.Length);
            for(int i = 0; i<parameters.Length;i++)
            {
                var tokens = AssertParameter(queryParameters[i]);

                Assert.AreEqual(HttpUtility.UrlEncode(parameters[i].Name), tokens[0], "Incorrect parameter name");
                Assert.AreEqual(HttpUtility.UrlEncode(parameters[i].Value.ToString()), tokens[1], "Incorrect parameter value");
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void InterviewUrlBuilder_EnforceHttpsStartUrlContainsPort_HttpsSchemeIsUsedCorrectPortIsPresent()
        {
            var builder = new InterviewUrlBuilder(StartUrlPrefixWithPort, ProjectId, true);
                       
            var url = new Uri(builder.Url);
            Assert.AreEqual("https", url.Scheme);
            Assert.AreEqual(9999, url.Port);            
        }

        private string[] AssertParametersCount(Uri url, int paramCount)
        {
            var parameters = url.Query.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(paramCount, parameters.Length, "Wrong number of parameters in url query");
            return parameters;
        }

        private string[] AssertParameter(string parameter)
        {
            var tokens = parameter.Split('=');
            Assert.AreEqual(2, tokens.Length, "Incorrect parameter token");
            return tokens;
        }
    }
}
