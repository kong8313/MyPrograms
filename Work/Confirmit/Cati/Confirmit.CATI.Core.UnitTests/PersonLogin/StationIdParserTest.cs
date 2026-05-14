using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.PersonLogin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.PersonLogin
{
    [TestClass]
    public class StationIdParserTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void ParceStationId_StationIdIsParcedCorrectly()
        {
            CheckParseStationId("Aaa155", 1, "155", false);
            CheckParseStationId("Aaa957L", 1, "957", true);
            CheckParseStationId("Aaa103155", 2, "3155", false);
            CheckParseStationId("Aaa291155L", 3, "91155", true);
            CheckParseStationId(string.Empty, 0, string.Empty, false);
            CheckParseStationId(null, 0, string.Empty, false);
        }

        private void CheckParseStationId(
            string stationId,
            int expectedDialerId,
            string expectedExtensionNumber,
            bool expectedIsLocal)
        {
            var stationInfo = new StationIdParser().Parse(stationId);

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is parsed incorrectly.");
            Assert.AreEqual(expectedExtensionNumber, stationInfo.ExtensionNumber,
                "ExtensionNumber is parsed incorrectly.");
            Assert.AreEqual(expectedIsLocal, stationInfo.IsLocal, "IsLocal is parsed incorrectly.");
            Assert.AreEqual(stationId, stationInfo.StationId, "StationId is not set correctly.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void StationIdIsNotValid_ExceptionIsThrown()
        {
            const string incorrectStationId = "888";

            try
            {
                new StationIdParser().Parse(incorrectStationId);
                Assert.Fail("UserMessageException was expected but is not thrown.");
            }
            catch (UserMessageException ex)
            {
                // The exception is expected
                Assert.AreEqual("Error_StationIdentifierHasIncorrectFormat", ex.MessageKey);

                var expectedExceptionMessage = "Station identifier '" + incorrectStationId + "' has incorrect format.";
                Assert.AreEqual(expectedExceptionMessage, ex.Message);
            }
        }

    }
}
