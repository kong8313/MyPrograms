using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Telephony.IVR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class IvrVariablesProviderTest
    {
        [TestMethod]
        public void ConvertToIvrVariables_ResultIsCorrect()
        {
            var provider = new IvrVariablesProvider();
            var expectedInterviewId = 2;
            var expectedPersonSid = 3;

            var variables = MakeTestVariables(expectedInterviewId, expectedPersonSid);

            var ivrVariables = provider.ConvertToIvrVariables(variables);

            Assert.AreEqual(2, ivrVariables.Count);

            Assert.AreEqual("__catiinterviewerid", ivrVariables[0].Name);
            Assert.AreEqual(expectedPersonSid.ToString(), ivrVariables[0].Value);

            Assert.AreEqual("catiinterviewid__", ivrVariables[1].Name);
            Assert.AreEqual(expectedInterviewId.ToString(), ivrVariables[1].Value);
        }

        [TestMethod]
        public void GetObjectsFromVariables_ResultIsCorrect()
        {
            var provider = new IvrVariablesProvider();
            var expectedInterviewId = 2;
            var expectedPersonSid = 3;

            var variables = MakeTestVariables(expectedInterviewId, expectedPersonSid);

            Assert.AreEqual(expectedInterviewId, provider.GetInterviewId(variables));
            Assert.AreEqual(expectedPersonSid, provider.GetPersonSid(variables));
        }

        [TestMethod]
        public void NoExpectedVariables_ResultIsNull()
        {
            var provider = new IvrVariablesProvider();

            var variables = new KeyValuePair<string, string>[0];

            Assert.IsNull(provider.GetInterviewId(variables));
            Assert.IsNull(provider.GetPersonSid(variables));
        }

        private KeyValuePair<string, string>[] MakeTestVariables(
            int expectedInterviewId, int expectedPersonSid)
        {
            var variables = new KeyValuePair<string, string>[2];
            variables[0] = new KeyValuePair<string, string>("__catiinterviewerid", expectedPersonSid.ToString());
            variables[1] = new KeyValuePair<string, string>("catiinterviewid__", expectedInterviewId.ToString());

            return variables;
        }
    }
}
