using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetNewITS : BaseMockedIntegrationTest
    {
        private void Test_Base(int initITS, int resultITS, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SetNewITS, resultITS.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = initITS;
            BackendTools.CreateInterview(interview);
            if (withCall)
                BackendTools.CreateCall(BackendTools.NewCall(interview));

            BackendTools.FireEvent(interview);

            interview.TransientState = resultITS;
            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(interview.SurveySID, interview.ID));
        }
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Its1WithCall_SetIts6_ItsSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(1, 6, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Its1WithoutCall_SetIts6_ItsSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(1, 6, false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Its6WithCall_SetIts6_ItsNotChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 6, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Its6WithoutCall_SetIts6_ItsNotChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 6, false);
        }
    }
}
