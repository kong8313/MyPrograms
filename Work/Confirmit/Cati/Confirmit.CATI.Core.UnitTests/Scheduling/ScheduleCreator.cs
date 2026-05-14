using System;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using Confirmit.CATI.Core.UnitTests.Resources;
using System.IO;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    internal static class ScheduleCreator
    {
        /// <summary>
        /// Returns new Schedule object filled with all data.
        /// </summary>
        /// <returns>Schedule object.</returns>
        public static Schedule GetSchedule()
        {
            Schedule result = new Schedule();
            result.Id = 1;

            RuleCollection rules = new RuleCollection();
            Rule rule1 = new Rule();
            rule1.Id = rules.GetNewId();
            rule1.Description = "Rule #1";

            SubRuleCollection subRulesRule1 = new SubRuleCollection();
            SubRule subRule1Rule1 = new SubRule();
            subRule1Rule1.Id = subRulesRule1.GetNewId();
            subRule1Rule1.Description = "Sub-rule #1 of rule #1";
            subRule1Rule1.ShiftTypeId = 1;
            subRule1Rule1.ItsId = 1;

            SubRuleActionCollection actionsSubRule1Rule1 = new SubRuleActionCollection();
            SubRuleAction action1SubRule1Rule1 = new SubRuleAction();
            action1SubRule1Rule1.Id = actionsSubRule1Rule1.GetNewId();
            action1SubRule1Rule1.Description = "Action #1 of sub-rule #1 of rule #1";
            action1SubRule1Rule1.ActionId = 1;

            actionsSubRule1Rule1.Add( action1SubRule1Rule1 );

            SubRuleAction action2SubRule1Rule1 = new SubRuleAction();
            action2SubRule1Rule1.Id = actionsSubRule1Rule1.GetNewId();
            action2SubRule1Rule1.Description = "Action #2 of sub-rule # 1 of rule #1";
            action2SubRule1Rule1.ActionId = 2;
            action2SubRule1Rule1.Parameter.Constant = "1";
            action2SubRule1Rule1.Enabled = false;

            actionsSubRule1Rule1.Add( action2SubRule1Rule1 );

            subRule1Rule1.SubRuleActions = actionsSubRule1Rule1;

            subRulesRule1.Add( subRule1Rule1 );

            SubRule subRule2Rule1 = new SubRule();
            subRule2Rule1.Id = subRulesRule1.GetNewId();
            subRule2Rule1.Description = "Sub-rule #2 of rule #1";
            subRule2Rule1.ItsId = 2;
            subRule2Rule1.ShiftTypeId = 1;
            subRule2Rule1.FilterEnabled = false;

            SubRuleActionCollection actionsSubRule2Rule1 = new SubRuleActionCollection();
            SubRuleAction action1SubRule2Rule1 = new SubRuleAction();
            action1SubRule2Rule1.Id = actionsSubRule2Rule1.GetNewId();
            action1SubRule2Rule1.Description = "Action #1 of sub-rule #2 of rule #1";
            action1SubRule2Rule1.ActionId = 3;
            action1SubRule2Rule1.FilterEnabled = true;
            action1SubRule2Rule1.Filter = "a > 10";
            action1SubRule2Rule1.Parameter.Constant = "2";

            actionsSubRule2Rule1.Add( action1SubRule2Rule1 );

            subRule2Rule1.SubRuleActions = actionsSubRule2Rule1;

            subRulesRule1.Add( subRule2Rule1 );

            rule1.SubRules = subRulesRule1;
            rules.Add( rule1 );

            Rule rule2 = new Rule();
            rule2.Id = rules.GetNewId();
            rule2.Description = "Rule #2";

            SubRuleCollection subRulesRule2 = new SubRuleCollection();
            SubRule subRule1Rule2 = new SubRule();
            subRule1Rule2.Id = subRulesRule2.GetNewId();
            subRule1Rule2.Description = "Sub-rule #1 of rule #2";
            subRule1Rule2.ItsId = 4;
            subRule1Rule2.ShiftTypeId = 3;
            subRule1Rule2.FilterEnabled = true;
            subRule1Rule2.Filter = "count >= 11 && name != 'Ken'";

            SubRuleActionCollection actionsSubRule1Rule2 = new SubRuleActionCollection();
            SubRuleAction action1SubRule1Rule2 = new SubRuleAction();
            action1SubRule1Rule2.Id = actionsSubRule1Rule2.GetNewId();
            action1SubRule1Rule2.Description = "Action #1 of sub-rule #1 of rule #2";
            action1SubRule1Rule2.ActionId = 9;
            action1SubRule1Rule2.FilterEnabled = true;
            action1SubRule1Rule2.Filter = "f == 10";
            action1SubRule1Rule2.Parameter.Constant = "Script1";

            actionsSubRule1Rule2.Add( action1SubRule1Rule2 );

            SubRuleAction action2SubRule1Rule2 = new SubRuleAction();
            action2SubRule1Rule2.Id = actionsSubRule1Rule2.GetNewId();
            action2SubRule1Rule2.Description = "Action #2 of sub-rule #1 of rule #2";
            action2SubRule1Rule2.ActionId = 25;
            action2SubRule1Rule2.Parameter.Constant = rule1.Id.ToString();

            actionsSubRule1Rule2.Add( action2SubRule1Rule2 );

            subRule1Rule2.SubRuleActions = actionsSubRule1Rule2;
            subRulesRule2.Add( subRule1Rule2 );

            SubRule subRule2Rule2 = new SubRule();
            subRule2Rule2.Id = subRulesRule2.GetNewId();
            subRule2Rule2.Description = "Sub-rule #2 of rule #2";
            subRule2Rule2.ItsId = 5;
            subRule2Rule2.ShiftTypeId = 5;

            SubRuleActionCollection actionsSubRule2Rule2 = new SubRuleActionCollection();
            SubRuleAction action1SubRule2Rule2 = new SubRuleAction();
            action1SubRule2Rule2.Id = actionsSubRule1Rule2.GetNewId();
            action1SubRule2Rule2.Description = "Action #1 of sub-rule #2 of rule #2";
            action1SubRule2Rule2.ActionId = 23;
            action1SubRule2Rule2.Parameter.Constant = subRule1Rule2.Id.ToString();

            actionsSubRule2Rule2.Add( action1SubRule2Rule2 );

            SubRuleAction action2SubRule2Rule2 = new SubRuleAction();
            action2SubRule2Rule2.Id = actionsSubRule1Rule2.GetNewId();
            action2SubRule2Rule2.Description = "Action #2 of sub-rule #2 of rule #2";
            action2SubRule2Rule2.ActionId = 26/*Set new Extended Status*/;
            action2SubRule2Rule2.Parameter.ParameterID = 1;

            actionsSubRule2Rule2.Add(action2SubRule2Rule2);

            subRule2Rule2.SubRuleActions = actionsSubRule2Rule2;
            subRulesRule2.Add( subRule2Rule2 );
			
            rule2.SubRules = subRulesRule2;

            rules.Add( rule2 );

            var customParameters = new CustomParameterCollection
                                       {
                                           new CustomParameter()
                                               {
                                                   Description = "used custom parameter",
                                                   Id = 1,
                                                   Name = "ITS",
                                                   Type = SchedulingParameterType.ExtendedStatus,
                                                   Value = 10
                                               },
                                           new CustomParameter()
                                               {
                                                   Description = "unused custom parameter ",
                                                   Id = 2,
                                                   Name = "ITS1",
                                                   Type = SchedulingParameterType.ExtendedStatus,
                                                   Value = 20
                                               },

                                       };

            result.Rules = rules;
            result.CustomParameters = customParameters;

            result.ShiftTypes = GenerateShiftTypes();
            result.Shifts = GenerateShifts();
            result.Exclusions = GenerateExclusions();

            CustomScript customScript = new CustomScript();
            customScript.Id = 1;
            StringBuilder scriptBody = new StringBuilder();
            scriptBody.AppendLine( "function boo()" );
            scriptBody.AppendLine( "{" );
            scriptBody.AppendLine( "\tvar i = 0;" );
            scriptBody.AppendLine( "\tif(i > 0)" );
            scriptBody.AppendLine( "\t{" );
            scriptBody.AppendLine( "\t\ti = 0;" );
            scriptBody.AppendLine( "\t}" );
            scriptBody.AppendLine( "}" );
            customScript.Body = scriptBody.ToString();

            result.CustomScript = customScript;

            return result;
        }

        /// <summary>
        /// Generate some shift types.
        /// </summary>
        /// <returns>Shift type collection.</returns>
        private static ShiftTypeCollection GenerateShiftTypes()
        {
            ShiftTypeCollection shiftTypes = new ShiftTypeCollection();
            ShiftType shiftType1 = new ShiftType();
            shiftType1.Id = 1;
            shiftType1.Name = "Shift type 1";
            shiftType1.Color = Color.Red;
            shiftTypes.Add( shiftType1 );

            ShiftType shiftType2 = new ShiftType();
            shiftType2.Id = 2;
            shiftType2.Name = "Shift type 2";
            shiftType2.Color = Color.Beige;
            shiftTypes.Add( shiftType2 );

            ShiftType shiftType3 = new ShiftType();
            shiftType3.Id = 3;
            shiftType3.Name = "Shift type 3";
            shiftType3.Color = Color.Green;
            shiftTypes.Add( shiftType3 );

            ShiftType shiftType4 = new ShiftType();
            shiftType4.Id = 4;
            shiftType4.Name = "Shift type 4";
            shiftType4.Color = Color.Orange;
            shiftTypes.Add( shiftType4 );

            ShiftType exclusionType = new ShiftType();
            exclusionType.ConvertToExclusionShiftType();
            exclusionType.Name = "Exclusion shift type";
            exclusionType.Color = Color.Blue;
            shiftTypes.Add( exclusionType );

            return shiftTypes;
        }

        /// <summary>
        /// Returns some shifts.
        /// </summary>
        /// <returns>Shift collection.</returns>
        private static ShiftCollection GenerateShifts()
        {
            ShiftCollection shifts = new ShiftCollection();

            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            ShiftData dataShift1 = new ShiftData( DayOfWeek.Monday, new TimeSpan( 10, 0, 13 ),
                                                  DayOfWeek.Monday, new TimeSpan( 18, 4, 23 ) );
            shift1.SetDataForTimezone( Shift.RespondentTimezoneId, dataShift1 );
            ShiftData data2Shift1 = new ShiftData( DayOfWeek.Monday, new TimeSpan( 12, 0, 0 ),
                                                   DayOfWeek.Monday, new TimeSpan( 13, 0, 0 ) );
            shift1.SetDataForTimezone( 1, data2Shift1 );
            shifts.Add( shift1 );

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 2;
            ShiftData dataShift2 = new ShiftData( DayOfWeek.Tuesday, new TimeSpan( 17, 0, 0 ),
                                                  DayOfWeek.Wednesday, new TimeSpan( 6, 0, 0 ) );
            shift2.SetDataForTimezone( 2, dataShift2 );
            shifts.Add( shift2 );

            return shifts;
        }

        /// <summary>
        /// Returns some exclusions.
        /// </summary>
        /// <returns>Exclusion collection.</returns>
        private static ExclusionCollection GenerateExclusions()
        {
            ExclusionCollection exclusions = new ExclusionCollection();
            Exclusion exclusion1 = new Exclusion();
            exclusion1.Id = exclusions.GetNewId();
            ExclusionData dataExclusion1 = new ExclusionData( DateTime.UtcNow, DateTime.UtcNow );
            exclusion1.SetDataForTimezone( Exclusion.RespondentTimezoneId, dataExclusion1 );
            ExclusionData data2Exclusion1 = new ExclusionData( DateTime.UtcNow, DateTime.UtcNow );
            exclusion1.SetDataForTimezone( 2, data2Exclusion1 );
            exclusions.Add( exclusion1 );

            Exclusion exclusion2 = new Exclusion();
            exclusion2.Id = exclusions.GetNewId();
            ExclusionData dataExclusion2 = new ExclusionData( DateTime.UtcNow, DateTime.UtcNow );
            exclusion2.SetDataForTimezone( 1, dataExclusion2 );
            exclusions.Add( exclusion2 );

            return exclusions;
        }

        /// <summary>
        /// Returns XML representation of schedule object.
        /// </summary>
        /// <returns>XML as string.</returns>
        public static string GetXmlSchedule()
        {
            return Strings.Schedule;
        }
    }
}