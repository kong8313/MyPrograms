using System;
using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSeveralActionSimultaneously : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SeveralActionSimultaneously_RecallInCurrentShiftWithoutCallChangeITSAndPriority_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int minuteCount = 10;
            const int its = 93;
            const short priority = 123;
            DateTime eventTime = DateTime.Parse("2008-09-29T14:00:00");
            DateTime resultTime = DateTime.Parse("2008-09-29T14:10:00");

            var script = new TestScript(
                    new[] { new Action(Action.Operation.RecallAfterANumberOfMinutes, minuteCount.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.SetNewITS, its.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.SetNewCallPriority, priority.ToString(CultureInfo.InvariantCulture)) },
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = its;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            call.TimeInShift = resultTime;
            call.Priority = priority;
            BackendTools.CheckCall(call);
        }
    }
}