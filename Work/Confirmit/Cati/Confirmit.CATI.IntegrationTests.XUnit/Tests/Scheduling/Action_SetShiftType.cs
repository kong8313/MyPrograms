using System.Globalization;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetShiftType : BaseMockedIntegrationTest
    {
        private void Test_Base(int newShiftType, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SetShiftType, newShiftType.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);

            switch(newShiftType)
            {
                case 0:
                    call.ShiftID = 0;////Any valid for DefaultTZ
                    break;
                case -1:
                    call.ShiftID = (int)CallShiftType.None;
                    break;
                default:
                    call.ShiftID = script.GetShiftTypeWorkID(newShiftType);
                    break;
            }
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallNotExists_SetNoneShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( -1/*None*/, false );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExists_SetNoneShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( -1/*None*/, true );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallNotExists_SetAnyValidShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( 0/*AnyValid*/, false );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExists_SetAnyValidShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( 0/*AnyValid*/, true );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallNotExists_SetSpecificShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( 1/*Specific shift*/, false );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallExists_SetSpecificShiftType_ShiftTypeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base( 1/*Specific shift*/, true );
        }
    }
}
