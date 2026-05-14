using Confirmit.TelephonyProblemStates.ProblemState;
using Confirmit.TelephonyProblemStates.Resources;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Common.UnitTests
{
    class TestCatiProblemStateInfo : ICatiProblemStateInfo
    {
        public string StationId
        {
            get { return string.Empty; }
        }
    }

    [TestClass]
    public class CatiProblemStateFactoryTest
    {
        private CatiProblemStateFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new CatiProblemStateFactory(new TestCatiProblemStateInfo());
        }
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetState_SupportedErrorCodeProvided_ProperProblemStateIsReturned()
        {
            var expectedState = DialerErrorCode.NoMoreLicences;
            string expectedMessage = Strings.CatiProblem_TelephonyNoMoreLicences;

            var state = _factory.GetState(expectedState);

            Assert.IsInstanceOfType(state, typeof(NoMoreLicencesState));
            Assert.AreEqual((int)expectedState, state.State);
            Assert.AreEqual(expectedMessage, state.Message);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetState_UnsupportedErrorCodeProvided_UnknownProblemStateIsReturned()
        {
            var expectedState = 199302;
            string expectedMessage = string.Format(Strings.CatiProblem_UnknownErrorCode, expectedState);

            var state = _factory.GetState(expectedState);

            Assert.IsInstanceOfType(state, typeof(UnknownErrorState));
            Assert.AreEqual((int)expectedState, state.State);
            Assert.AreEqual(expectedMessage, state.Message);
        }
    }
}
