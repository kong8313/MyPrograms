using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionIncrementPriority : BaseMockedIntegrationTest
    {
        private void Test_Base(int its, short priority, short param, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.IncrementPriority, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = its;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
            {
                call.Priority = priority;
                BackendTools.CreateCall(call);
            }

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //

            BackendTools.CheckInterview(interview);
            
            call.Priority += param;
            if (call.Priority < 0 )
                call.Priority = 0x7FFF;

            BackendTools.CheckCall(call);
        }

        // ������:
        // ��������� ���������
        // ��������
        // �������� ���������

        /// <summary>
        /// ������� ������:  Interview,Call c Priority = 1. ���������� �� 10. 
        /// ��������: �������� �� ������ ����������, ��������� ����� Call � ID ������� Call-� � ������������ Priority  = 11.
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithPriorityOne_IncrementPriorityOn10_PriorityIncremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 1, 10, true);
        }

        /// <summary>
        /// ������� ������:  Interview ��� Call-�,  PriorityByITS = 1. ���������� �� 1. 
        /// ��������: �������� �� ������ ����������, ��������� ����� Call � ID ������� Call-� � ������������ Priority  = 11.
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithoutCall_IncrementPriorityOn10_PriorityIncremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 0/*default priority for new call*/, 10, false);
        }

        /// <summary>
        /// ������� ������:  Interview,Call � Priority = 10, ���������� �� 10.
        /// ��������: �������� �� ������ ���������� ��������� ����� Call � ID ������� Call-� � ������������ Priority  = 11.
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithPriority10_IncrementPriorityOn20_PriorityIncremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 10, 20, true);
        }

        /// <summary>
        /// ������� ������:  Interview, ��� Call. PriorityByITS = 1, ���������� �� 20.
        /// ��������: �������� �� ������ ���������� ��������� ����� Call � ID ������� Call-� � ������������ Priority  = 11.
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithoutCall_IncrementPriorityOn20_PriorityIncremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 0/*default priority for new call*/, 20, false);
        }

        /// <summary>
        /// ������� ������:  Interview,Call � Priority = max(2^32), ���������� �� 1.
        /// ��������: �������� �� ������ ����������, ��������� ����� Call � ID ������� Call-� � ������������ Priority  = max(2^32).
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void MaximumPriorityIncrementOnOneWithCallll_PriorityNotChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 0x7FFF, 1, true);
        }
    }
}
