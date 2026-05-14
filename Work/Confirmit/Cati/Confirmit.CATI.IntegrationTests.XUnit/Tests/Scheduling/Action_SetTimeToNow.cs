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
    public class ActionSetTimeToNow : BaseMockedIntegrationTest
    {
        internal enum SetTimeToNOWParam
        {
            Now = 0,
            AnyValid = 1,
        }

        private void Test_Base(DateTime eventTime, SetTimeToNOWParam param, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SetTimeToNOW, ((int)param).ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            if (param == SetTimeToNOWParam.AnyValid)
                call.ShiftID = 0;////Any valid for DefaultTZ
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExistsAndTimeOutOfShift_SetTimeToNOW_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T23:00:00"),
                SetTimeToNOWParam.Now,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExistsAndTimeOutOfShift_SetTimeToAnyValid_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T23:00:00"),
                SetTimeToNOWParam.AnyValid,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExistsAndTimeInShift_SetTimeToNOW_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                SetTimeToNOWParam.Now,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExistsAndTimeInShift_SetTimeToAnyValid_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                SetTimeToNOWParam.AnyValid,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeOutOfShift_SetTimeToNOW_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T23:00:00"),
                SetTimeToNOWParam.Now,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeOutOfShift_SetTimeToAnyValid_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T23:00:00"),
                SetTimeToNOWParam.AnyValid,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_SetTimeToNOW_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                SetTimeToNOWParam.Now,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_SetTimeToAnyValid_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                SetTimeToNOWParam.AnyValid,
                false);
        }
    }
}
