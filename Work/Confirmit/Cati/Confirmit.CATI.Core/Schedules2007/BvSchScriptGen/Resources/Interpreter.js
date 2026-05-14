// JScript source code
import System;
import System.Diagnostics;
import Interpreter;
import System.Text.RegularExpressions;
import System.Collections.Generic;
import System.Diagnostics;
import System.Collections;
import BvDotNetScript.Interfaces;
import BvDotNetScript.ScriptObjects;
import Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Services;
import Confirmit.CATI.Core.Repositories;
import Confirmit.CATI.Core.Services;
import Confirmit.CATI.Core.Services.Interfaces;
import Confirmit.CATI.Core.Services.Survey;
import Confirmit.CATI.Core.Timezones;
import Confirmit.CATI.Core.DAL.Generated.Entity.Table;
import Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
import Confirmit.CATI.Core.ManagementService;
import Confirmit.CATI.Common.Exceptions;
import Confirmit.CATI.Common;
import ConfirmitDialerInterface;
import Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
import Confirmit.CATI.Core.DAL.Generated.Entity.Table;
import Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
import Confirmit.CATI.Common.ServiceLocation;
package Interpreter
{
    class RulesInterpreter
    {
       private var bvEvent : IEventSchedule;
       private var schedule : Interpreter.Schedule;
       private var lastCallShiftTypeID : long;
       private var extendedAPI: BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI;
       private var helper: BvDotNetScript.ScriptObjects.SchedulingScriptHelper;
       private var maxActionsToExecute: int;

       private var lastRuleId: int;
       private var lastSubRuleId: int;
       private var lastActionId: int;
       
       var surveySID : int;
       var interviewID : int;
       
       private var customScripWrapper : CustomCode.CustomScriptWrapper;
       
       static private class Point
       {
          public var rule : int;
          public var subRule : int;
          
          public function Point(r : int, sr : int)
          {
             rule = r;
             subRule = sr;
          }
       }
       
       //return number of rule and subrule which start with
       private function GetPoint(subRuleID : String) : RulesInterpreter.Point
       {
           var rules : Schedule.Rule[];
           
           rules = schedule.GetRules(bvEvent.ProcessSampleMode == ProcessSampleMode.Update);
          
           for(var i : int = 0; i < rules.Length; ++i)
          {             
             var subRules : Schedule.Rule.SubRule[] = rules[i].GetSubRules();
             
             for(var j : int = 0; j < rules[i].Length; ++j)
             {
                if(((System.String)(subRules[j].ID)).ToUpper() == ((System.String)(subRuleID)).ToUpper())
                {
                   return new RulesInterpreter.Point(i, j);
                }
             }
          }
       }
       
       private function GetRule(ruleID : String) : RulesInterpreter.Point
       {
           var rules : Schedule.Rule[];
           
           rules = schedule.GetRules(bvEvent.ProcessSampleMode == ProcessSampleMode.Update);
          
           for(var i : int = 0; i < rules.Length; ++i)
          {
             if(((System.String)(rules[i].ID)).ToUpper() == ((System.String)(ruleID)).ToUpper())
             {
                return new RulesInterpreter.Point(i, 0);
             }
          }
          return new RulesInterpreter.Point(0, 0);
       }
       
       public function Init(bevent : IEventSchedule, sch : Schedule, wrapper : CustomCode.CustomScriptWrapper)
       {
          bvEvent = bevent;
          schedule = sch;
          customScripWrapper = wrapper;
          extendedAPI = new BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI();
          helper = new BvDotNetScript.ScriptObjects.SchedulingScriptHelper();
          extendedAPI.Init(bvEvent);
          helper.Init(bvEvent);
          maxActionsToExecute = extendedAPI.SystemSettings.MaxActionsCount;
          //TODO: probably remove Init in other classes
          wrapper.Init(extendedAPI);
          
          lastCallShiftTypeID = 0;

           var shift: ShiftService.MatchingShift = bvEvent.Shifts.GetExactShift(extendedAPI.LastCallTime, extendedAPI.TimezoneID );
          
          if( shift != null )
            lastCallShiftTypeID = shift.ShiftTypeID;
       }
       
    public function Execute(bevent: IEventSchedule, sch: Schedule,
        wrapper: CustomCode.CustomScriptWrapper) {
        try {
            Init(bevent, sch, wrapper);
            extendedAPI.LogMessage(String.Format("Starting scheduling script execution. Triggered by: \"{0}\", Extended Status: \"{1}\"", SchedulingScriptExecutionReasonConverter.ConvertToString(bevent.ExecutionReason), bevent.ExtendedStatus))
            var startPoint: RulesInterpreter.Point;

            //if call was created
            if (bvEvent.LastCall != null) {
                var str: System.String = bvEvent.LastCall.RuleNumber.ToString();
                startPoint = GetRule(str.Trim((new String("{}")).ToCharArray()));
            }
            else {
                startPoint = new RulesInterpreter.Point(0, 0);
            }

            var actionCounter: int = 0;

            //create call if necessary and fill it
            ProcessRules(startPoint, actionCounter);

            if (bvEvent.NewCall != null && bvEvent.NewCall.CallState != 2 && bvEvent.NewCall.CallState != 3) {
                bvEvent.NewCall.CallState = 2;
            }
        }
        catch (ex: Exception) {
            extendedAPI.LogMessage("Error: " + ex.Message);
            throw new SchedulingExecutionException(ex.Message, ex, lastRuleId, lastSubRuleId, lastActionId);
        }
        finally {
            extendedAPI.LogMessage("Finishing scheduling script execution.");
            extendedAPI.LogChangesMade();

            var logEntity = new BvSchedulingScriptLogEntity();
            logEntity.SurveySid = bevent.Survey.SID;
            logEntity.InterviewId = bvEvent.Interview.ID;
            logEntity.ScheduleID = extendedAPI.Scheduling.Shifts.ScheduleID;
            logEntity.Timestamp = DateTime.UtcNow;
            logEntity.LogMessages = extendedAPI.MessagesLog.ToString();

            BvSchedulingScriptLogAdapter.Insert(logEntity);

            extendedAPI.Dispose();
        }
    }
       
    private function ProcessAction(action: Interpreter.Schedule.Rule.SubRule.Action): System.String {
        var newSubRule: System.String = "";
        var i: int;
        var options: System.Object = undefined;
        var match: Match;
        var found: Boolean;
        var shiftCount: int;
        var timeToCall: System.DateTime = new DateTime();
        var ids: int[];
        var resourceId: int;
        var shiftTypeID: int;
        var shift: ShiftService.MatchingShift = null;
        var services: BvDotNetScript.ScriptObjects.SchedulingScriptServices = extendedAPI.Services;

        switch (action.ActionID) {
            //Suspend the interview
            case 1:
                {
                    extendedAPI.LogMessage('Executing action: "Suspend the interview ' + action.GetParamValue(extendedAPI) + '"');

                    //we needn't create call
                    bvEvent.NewCall = null;
                    break;
                }
            //Recall after number of minutes
            case 2:
                {
                    extendedAPI.LogMessage('Executing action: "Recall after number of minutes ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.RecallAfterNumberOfMinutes, action.GetParamValue(extendedAPI));
                    break;
                }
            //Recall after number of shifts
            case 3:
                {
                    extendedAPI.LogMessage('Executing action: "Recall after number of shifts ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.RecallAfterNumberOfShifts, action.GetParamValue(extendedAPI));
                    break;
                }
            //Recall after number of shifts but choose random time within shift
            case 4:
                {
                    extendedAPI.LogMessage('Executing action: "Recall after number of shifts but choose random time within shift ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    shift = bvEvent.Shifts.GetShiftAfterNumberOfShifts(extendedAPI.LastCallTime, extendedAPI.TimezoneID, Int32.Parse(action.GetParamValue(extendedAPI)));

                    bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                    bvEvent.NewCall.TimeInShift = shift.RandomDate;

                    break;
                }
            //Fulfill the specified appointment
            case 5:
                {
                    extendedAPI.LogMessage('Executing action: "Fulfill the specified appointment ' + action.GetParamValue(extendedAPI) + '"');

                    var Appt: BvAppointmentEntity = null;
                    var ApptID: int = 0;
                    var ApptTimeZone: int = bvEvent.Interview.TimezoneID;
                    var Diff: int = int.MaxValue;
                    var expired: Boolean = false;

                    var timeToExpire: System.DateTime = undefined;
                    found = false;


                    //Workaround of "for( Appt in SurveyService.GetAppointments(bvEvent.Survey.SID, bvEvent.Interview.ID, bvEvent.BatchID ) )"
                    var ApptArr: System.Array;
                    ApptArr = SurveyService.GetAppointments(bvEvent.Survey.SID, bvEvent.Interview.ID, bvEvent.BatchID);

                    var runStart: int;
                    var runEnd: int;
                    runStart = ApptArr.GetLowerBound(0);
                    runEnd = ApptArr.GetUpperBound(0);
                    //iterate though appointments to find the nearest one
                    for (var curI: int = runStart; curI <= runEnd; ++curI) {
                        Appt = (BvAppointmentEntity)(ApptArr.GetValue(curI));

                        var expTimeToCompare: System.DateTime;
                        var expTimeToSave: System.DateTime;

                        if (!Appt.ExpTime.HasValue || Appt.ExpTime == DateTime.FromOADate(0)) {
                            expTimeToCompare = bvEvent.Time;
                            expTimeToSave = null;
                        }
                        else {
                            expTimeToCompare = Appt.ExpTime.Value;
                            expTimeToSave = Appt.ExpTime.Value;
                        }

                        //proceed only not expired appointments
                        if ((expTimeToCompare >= bvEvent.Time) || (expTimeToCompare == 0)) {
                            var d: int;
                            d = (Appt.Time - bvEvent.Time).Minutes;

                            if (d < Diff/*this appointment is nearer than preceding ones*/) {
                                timeToCall = Appt.Time;
                                timeToExpire = expTimeToSave;
                                ApptID = Appt.ID;
                                ApptTimeZone = Appt.TZID;

                                Diff = d;
                                found = true;

                                if (d < 0)
                                    break;
                            }
                        }
                        else {
                            expired = true;
                        }
                    }

                    if (found) {
                        //create new call
                        extendedAPI.CallShouldBeCreated();

                        bvEvent.NewCall.TimeInShift = timeToCall.AddMinutes(-Int32.Parse(action.GetParamValue(extendedAPI)));

                        //any valid
                        bvEvent.NewCall.ShiftID = CallShiftType.None;

                        if (timeToExpire == null)
                            bvEvent.NewCall.TimeToExpire = null; //else type will be VT_DATE
                        else
                            bvEvent.NewCall.TimeToExpire = timeToExpire;

                        bvEvent.NewCall.ApptID = ApptID;
                        helper.UpdateInterviewTimezoneByAppointment(ApptTimeZone);
                    } else {
                        var activatedAppointmentsCount = SurveyService.GetNotActiveAppointments(bvEvent.Survey.SID, bvEvent.Interview.ID).Length;
                        if (activatedAppointmentsCount > 0) {
                            throw new UserMessageException("Action \"Fulfill specified appointment\": No appointment is set. The appointment created in a previous call attempt has already been fulfilled.");
                        } else if (expired) {
                            throw new UserMessageException("Action \"Fulfill specified appointment\": No appointment is set. The appointment has expired.");
                        } else {
                            throw new UserMessageException("Action \"Fulfill specified appointment\": No appointment is set.");
                        }
                    }

                    break;
                }
            //Terminate the interview
            case 6:
                {
                    extendedAPI.LogMessage('Executing action: "Terminate the interview ' + action.GetParamValue(extendedAPI) + '"');

                    bvEvent.Interview.TransientState = 6;
                    bvEvent.NewCall = null;
                    break;
                }
            //Recall on next shift of specified type
            case 7:
                {
                    extendedAPI.LogMessage('Executing action: "Recall on next shift of specified type ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    try {
                        shift = bvEvent.Shifts.GetNextShiftOfSpecifiedType(extendedAPI.LastCallTime, extendedAPI.TimezoneID, Int32.Parse(action.GetParamValue(extendedAPI)));
                    }
                    catch (ex: UserMessageException) {
                        throw new UserMessageException(String.Format("Action \"Recall on next shift of specified type\": {0}", ex.Message), ex);
                    }

                    if (shift != null) {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                        bvEvent.NewCall.TimeInShift = shift.StartDate;
                    }
                    else {
                        throw new UserMessageException(String.Format("Action \"Recall on next shift of specified type\": No shifts found with type ID {0}", Int32.Parse(action.GetParamValue(extendedAPI))));
                    }

                    break;
                }
            //Set time to NOW
            case 8:
                {
                    extendedAPI.LogMessage('Executing action: "Set time to NOW ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    switch (Int32.Parse(action.GetParamValue(extendedAPI))) {
                        case 0: //NOW
                            {
                                bvEvent.NewCall.ShiftID = CallShiftType.None;
                                break;
                            }
                        case 1: //Any Valid
                            {
                                bvEvent.NewCall.ShiftID = CallShiftType.AnyValid;

                                break;
                            }
                    }

                    bvEvent.NewCall.TimeInShift = undefined;

                    break;
                }
            //Run custom script
            case 9:
                {
                    extendedAPI.LogMessage('Executing action: "Run custom script ' + action.GetParamValue(extendedAPI) + '"');

                    action.ExecuteScript(customScripWrapper, extendedAPI);

                    break;
                }
            //Increment quantity variable
            case 10:
                {
                    extendedAPI.LogMessage('Executing action: "Increment quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.f(action.GetParamValue(extendedAPI)).increment();

                    break;
                }
            //Special increment quantity variable
            case 11:
                {
                    extendedAPI.LogMessage('Executing action: "Special increment quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    if (extendedAPI.IsITSNotChanged())
                        extendedAPI.f(action.GetParamValue(extendedAPI)).increment();

                    break;
                }
            //Decrement quantity variable
            case 12:
                {
                    extendedAPI.LogMessage('Executing action: "Decrement quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.f(action.GetParamValue(extendedAPI)).decrement();

                    break;
                }
            //Special decrement quantity variable
            case 13:
                {
                    extendedAPI.LogMessage('Executing action: "Special decrement quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    if (extendedAPI.IsITSNotChanged())
                        extendedAPI.f(action.GetParamValue(extendedAPI)).decrement();

                    break;
                }
            //Reset quantity variable
            case 14:
                {
                    extendedAPI.LogMessage('Executing action: "Reset quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.f(action.GetParamValue(extendedAPI)).reset();

                    break;
                }
            //Special reset quantity variable
            case 15:
                {
                    extendedAPI.LogMessage('Executing action: "Special reset quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    if (extendedAPI.IsITSNotChanged())
                        extendedAPI.f(action.GetParamValue(extendedAPI)).reset();

                    break;
                }
            //Assign value to quantity variable
            case 16:
                {
                    extendedAPI.LogMessage('Executing action: "Assign value to quantity variable ' + action.GetParamValue(extendedAPI) + '"');

                    match = Regex.Match((System.String)(action.GetParamValue(extendedAPI)), "^(.*)\=(.*)$");

                    extendedAPI.f(match.Groups[1].Value.Trim()).setValue(match.Groups[2].Value.Trim());

                    break;
                }
            //Recall after number of shifts specified by variable
            case 17:
                {
                    extendedAPI.LogMessage('Executing action: "Recall after number of shifts specified by variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    shiftCount = extendedAPI.f(action.GetParamValue(extendedAPI)).toInt();
                    //shiftCount must be >0
                    if (shiftCount <= 0) {
                        throw new UserMessageException(
                            String.Format("The variable value is zero or less in the action 'Recall after number of shifts specified by variable'. The value must be greater than zero.")
                        );
                    }

                    shift = bvEvent.Shifts.GetShiftAfterNumberOfShifts(extendedAPI.LastCallTime, extendedAPI.TimezoneID, shiftCount);

                    bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                    bvEvent.NewCall.TimeInShift = shift.StartDate;

                    break;
                }
            //Assign function call result to variable
            case 18:
                {
                    extendedAPI.LogMessage('Executing action: "Assign function call result to variable ' + action.GetParamValue(extendedAPI) + '"');

                    action.ExecuteFunction(customScripWrapper, extendedAPI);

                    break;
                }
            //Place call history bookmark
            case 19:
                {
                    extendedAPI.LogMessage('Executing action: "Place call history bookmark ' + action.GetParamValue(extendedAPI) + '"');
                    extendedAPI.SetBookmark(Int32.Parse(action.GetParamValue(extendedAPI)));

                    break;
                }
            //Recall on next shift of the type specified by variable
            case 20:
                {
                    extendedAPI.LogMessage('Executing action: "Recall on next shift of the type specified by variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    shiftTypeID = extendedAPI.f(action.GetParamValue(extendedAPI)).toInt();

                    try {
                        shift = bvEvent.Shifts.GetNextShiftOfSpecifiedType(extendedAPI.LastCallTime, extendedAPI.TimezoneID, shiftTypeID);
                    }
                    catch (ex: UserMessageException) {
                        throw new UserMessageException(String.Format("The action 'Recall on next shift of the type specified by variable' cannot be executed due to: {0}", ex.Message), ex);
                    }

                    if (shift != null) {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                        bvEvent.NewCall.TimeInShift = shift.StartDate;
                    }
                    else {
                        throw new UserMessageException(String.Format("No shift found with ID '{0}' in the action 'Recall on next shift of the type specified by variable'", shiftTypeID));
                    }

                    break;
                }
            //Place call history bookmark to NOW
            case 21:
                {
                    extendedAPI.LogMessage('Executing action: "Place call history bookmark to NOW ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.SetBookmarkToNow();

                    break;
                }
            //Stop execution
            case 22:
                {
                    extendedAPI.LogMessage('Executing action: "Stop execution ' + action.GetParamValue(extendedAPI) + '"');

                    return null;
                }
            //Go To
            case 23:
                {
                   

                    extendedAPI.LogMessage(String.Format("Executing action: Go To rule {0}", GetSubRuleNumber(action.GetParamValue(extendedAPI))));

                    return action.GetParamValue(extendedAPI);
                }
            //Assign virtual extension
            case 24:
                {
                    extendedAPI.LogMessage('Executing action: "Assign virtual extension ' + action.GetParamValue(extendedAPI) + '"');

                    //we don't need this action. 
                    break;
                }
            //Set Next Rule
            case 25:
                {
                    extendedAPI.LogMessage(String.Format("Executing action: Set Next Rule {0}", GetRuleNumber(action.GetParamValue(extendedAPI))));

                    extendedAPI.CallShouldBeCreated();

                    bvEvent.NewCall.RuleNumber = new Guid(action.GetParamValue(extendedAPI));

                    break;
                }
            //Set New ITS
            case 26:
                {
                    extendedAPI.LogMessage('Executing action: "Set new Extended Status ' + action.GetParamValue(extendedAPI) + '"');

                    bvEvent.Interview.TransientState = Int32.Parse(action.GetParamValue(extendedAPI));
                    break;
                }
            //Set New Call Priority
            case 27:
                {
                    extendedAPI.LogMessage('Executing action: "Set New Call Priority ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    bvEvent.NewCall.Priority = Int32.Parse(action.GetParamValue(extendedAPI));

                    break;
                }
            //Increment Priority
            case 28:
                {
                    extendedAPI.LogMessage('Executing action: "Increment Priority ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();
                    try {
                        bvEvent.NewCall.Priority = bvEvent.NewCall.Priority + Int32.Parse(action.GetParamValue(extendedAPI));
                    }
                    catch (OverflowException) {
                        //we need not trace this exception
                        bvEvent.NewCall.Priority = Int32.MaxValue;
                    }

                    break;
                }
            //Decrement Priority
            case 29:
                {
                    extendedAPI.LogMessage('Executing action: "Decrement Priority ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    try {
                        bvEvent.NewCall.Priority = bvEvent.NewCall.Priority - Int32.Parse(action.GetParamValue(extendedAPI));
                    }
                    catch (OverflowException) {
                        //we need not trace this exception
                        bvEvent.NewCall.Priority = 1;
                    }
                    if (bvEvent.NewCall.Priority <= 0)
                        bvEvent.NewCall.Priority = 1;

                    break;
                }
            //Assign Resource
            case 30:
                {
                    extendedAPI.LogMessage('Executing action: "Assign user/group(s) ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    switch (action.GetParamValue(extendedAPI).Trim()) {
                        //[Unchanged]
                        case "-1":
                            {
                                if (bvEvent.LastCall != null &&
                                    bvEvent.LastCall != undefined) {
                                    bvEvent.NewCall.Resource = bvEvent.LastCall.Resource;
                                }
                                else {
                                    bvEvent.NewCall.Resource = 0;
                                }
                                break;
                            }
                        //[Last Person]
                        case "-2":
                            {
                                var intResource: int = 0;

                                if (bvEvent.Interview.LastCallPersonSID.HasValue) {
                                    intResource = bvEvent.Interview.LastCallPersonSID.Value;
                                }

                                bvEvent.NewCall.Resource = intResource;
                                break;
                            }
                        //[Survey Interviewers]
                        case "-3":
                            {
                                bvEvent.NewCall.Resource = 0;
                                break;
                            }
                        default:
                            {
                                ids = services.Parse.StringToIntArray(action.GetParamValue(extendedAPI), ",");
                                
                                if(ids.Length == 1) 
                                {
                                    var person = PersonRepository.TryGetById(ids[0]);
                                    var personGroup = PersonGroupRepository.TryGetById(ids[0]);
                                    if (person == null && personGroup == null)
                                        throw new UserMessageException("The specified resource (\"" + ids[0] + "\") was not found. A resource must be a valid interviewer or group ID.\n");
                                    
                                    
                                    if(personGroup != null && personGroup.IsAdministrative)
                                        throw new UserMessageException("The group with ID \"" + ids[0] + "\" is administrative. Administrative groups cannot be assigned to calls.\n");
                                    
                                }else
                                {
                                    if (!services.PersonGroupService.IsExistsAndNotAdministrative(ids))
                                        throw new UserMessageException("One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.\n");
                                }
                                

                                resourceId = services.Assignment.GetAssignmentResourceId(ids);
                                bvEvent.NewCall.Resource = resourceId;
                            }
                    }

                    break;
                }
            //Set Call expiration timeout
            case 31:
                {
                    extendedAPI.LogMessage('Executing action: "Set Call expiration timeout ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    if (!bvEvent.NewCall.TimeInShift.HasValue) {
                        throw new UserMessageException("Action \"Set Call expiration timeout\": Time to call is not set");
                    }
                    else {
                        bvEvent.NewCall.TimeToExpire = bvEvent.NewCall.TimeInShift.Value.AddMinutes(Double.Parse(action.GetParamValue(extendedAPI)));
                    }

                    break;
                }

            //Set Time to Call (explicit)
            case 33:
                {
                    extendedAPI.LogMessage('Executing action: "Recall on specific time ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    timeToCall = DateTime.SpecifyKind(DateTime.Parse(action.GetParamValue(extendedAPI)).ToUniversalTime(), DateTimeKind.Unspecified);

                    timeToCall = TimezoneService.ConvertTimeToUtc(extendedAPI.TimezoneID, timeToCall);

                    //if expiration time less then time to call
                    if (bvEvent.NewCall.TimeToExpire != null &&
                        bvEvent.NewCall.TimeToExpire < timeToCall) {
                        throw new UserMessageException("Action \"Set Time to Call\": Expiration time must not be earlier than the time to call");
                    }

                    shift = bvEvent.Shifts.GetExactShift(timeToCall, extendedAPI.TimezoneID);

                    if (shift == null) {
                        //NONE Shift
                        bvEvent.NewCall.ShiftID = CallShiftType.None;
                        bvEvent.NewCall.TimeInShift = timeToCall;
                    }
                    else {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                        bvEvent.NewCall.TimeInShift = timeToCall;
                    }

                    break;
                }
            //Set Call expiration time (explicit)
            case 34:
                {
                    extendedAPI.LogMessage('Executing action: "Set Call expiration time (explicit) ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    timeToCall = DateTime.SpecifyKind(DateTime.Parse(action.GetParamValue(extendedAPI)).ToUniversalTime(), DateTimeKind.Unspecified);
                    timeToCall = TimezoneService.ConvertTimeToUtc(extendedAPI.TimezoneID, timeToCall);

                    //We should throw exception only if TimeInShift was set and bvEvent.NewCall.TimeInShift > timeToCall
                    if (bvEvent.NewCall.TimeInShift != null &&
                        bvEvent.NewCall.TimeInShift != undefined &&
                        bvEvent.NewCall.TimeInShift > timeToCall) {
                        throw new UserMessageException("Action \"Set Call expiration time\": Expiration time must not be earlier than the time to call");
                    }

                    bvEvent.NewCall.TimeToExpire = timeToCall;

                    break;
                }
            //Recall on the specific shift
            case 35:
                {
                    extendedAPI.LogMessage('Executing action: "Recall on the specific shift ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    shift = bvEvent.Shifts.GetNextShiftByID(extendedAPI.LastCallTime, extendedAPI.TimezoneID, Int32.Parse(action.GetParamValue(extendedAPI)));

                    if (shift != null) {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                        bvEvent.NewCall.TimeInShift = shift.StartDate;
                    }
                    else {
                        throw new UserMessageException(String.Format("Action \"Recall on the specific shift\": No shift found with ID {0}", action.GetParamValue(extendedAPI)));
                    }

                    break;
                }
            //Recall on the shift specified by variable
            case 36:
                {
                    extendedAPI.LogMessage('Executing action: "Recall on the shift specified by variable ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    shiftTypeID = extendedAPI.f(action.GetParamValue(extendedAPI)).toInt();

                    shift = bvEvent.Shifts.GetNextShiftByID(extendedAPI.LastCallTime, extendedAPI.TimezoneID, shiftTypeID);

                    if (shift != null) {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                        bvEvent.NewCall.TimeInShift = shift.StartDate;
                    }
                    else {
                        throw new UserMessageException(String.Format("No shift found with ID '{0}' in the action 'Recall on the shift specified by variable'", shiftTypeID));
                    }

                    break;
                }
            //Set Shift Type
            case 37:
                {
                    extendedAPI.LogMessage('Executing action: "Set Shift Type ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    var scpecifiedShiftType: int;
                    scpecifiedShiftType = System.Int32.Parse(action.GetParamValue(extendedAPI));

                    switch (scpecifiedShiftType) {
                        case 0: //Any Valid
                            {
                                bvEvent.NewCall.ShiftID = CallShiftType.AnyValid;

                                break;
                            }
                        case -1: //NONE
                            {
                                bvEvent.NewCall.ShiftID = CallShiftType.None;
                                break;
                            }
                    }

                    if (scpecifiedShiftType == 0 ||
                        scpecifiedShiftType == -1) {
                        break;
                    }

                    try {
                        shift = bvEvent.Shifts.GetNextShiftOfSpecifiedType(extendedAPI.LastCallTime, extendedAPI.TimezoneID, Int32.Parse(action.GetParamValue(extendedAPI)));
                    }
                    catch (ex: UserMessageException) {
                        throw new UserMessageException(String.Format("Action \"Set shift type\": {0}", ex.Message));
                    }

                    if (shift != null) {
                        bvEvent.NewCall.ShiftID = shift.ShiftTypeID;
                    }
                    else {
                        throw new UserMessageException(String.Format("Action \"Set Shift Type\": No shifts found with type ID {0}", Int32.Parse(action.GetParamValue(extendedAPI))));
                    }

                    break;
                }
            //Set dialing mode
            case 38:
                {
                    extendedAPI.LogMessage('Executing action: "Set dialing mode ' + action.GetParamValue(extendedAPI) + '"');

                    var dialingMode: DialingMode = SurveyService.GetDialingMode(bvEvent.Survey.SID);
                    if (dialingMode != DialingMode.Automatic &&
                        dialingMode != DialingMode.Predictive) {
                        Trace.TraceError("Set dialing mode isn't supported for survey with dialing mode = {0}", dialingMode);
                    }
                    else {
                        bvEvent.Interview.DialingMode = Int32.Parse(action.GetParamValue(extendedAPI));
                    }
                    break;
                }
            //Enable call
            case 39:
                {
                    extendedAPI.LogMessage('Executing action: "Enable call ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    bvEvent.NewCall.CallState = 2;
                    break;
                }

            //Disable call
            case 40:
                {
                    extendedAPI.LogMessage('Executing action: "Disable call ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.CallShouldBeCreated();

                    bvEvent.NewCall.CallState = 3;
                    break;
                }

            //Add additional assignment on group
            case 41:
                {
                    extendedAPI.LogMessage('Executing action: "Add a group to a multiple assignment ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.AssignMultipleGroups, action.GetParamValue(extendedAPI));
                    break;
                }

            //Remove specific assignment on group
            case 42:
                {
                    extendedAPI.LogMessage('Executing action: "Remove a group from a multiple assignment ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.DeassignMultipleGroups, action.GetParamValue(extendedAPI));
                    break;
                }
            //Restore previous call attributes
            case 43:
                {
                    extendedAPI.LogMessage('Executing action: "Restore previous call attributes ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.RestorePreviousCallState);
                    break;
                }

            //Accept inbound call
            case 44:
                {
                    extendedAPI.LogMessage('Executing action: "Accept inbound call ' + action.GetParamValue(extendedAPI) + '"');

                    extendedAPI.ExecuteAction(extendedAPI.Actions.AcceptInboundCall);
                    break;
                }
            default:
                {
                    extendedAPI.LogMessage('Unknown action with ID ${action.ActionID}');

                    throw new UserMessageException(String.Format("Action with actionid {0} does not exist", action.ActionID));
                }
        }

        return newSubRule;
    }

    private function GetSubRuleNumber(Id: System.String)
    {
        var point = GetPoint(Id);
        var rules: Schedule.Rule[];

        rules = schedule.GetRules(bvEvent.ProcessSampleMode == ProcessSampleMode.Update);

        var ruleId = rules[point.rule].RuleNumber + 1;
        var subRuleId = point.subRule + 1;

        return String.Format("{0}.{1}", ruleId, subRuleId);
    }

    private function GetRuleNumber(Id: System.String)
    {
        var rules: Schedule.Rule[];

        rules = schedule.GetRules(bvEvent.ProcessSampleMode == ProcessSampleMode.Update);

        for (var i: int = 0; i < rules.Length; ++i) {

            if (((System.String)(rules[i].ID)).ToUpper() == ((System.String)(Id)).ToUpper())
                return rules[i].RuleNumber+1;
        }
    }

    private function ProcessRules(begin: RulesInterpreter.Point, actionCounter: int)
    {
        var rules: Schedule.Rule[];

        rules = schedule.GetRules(bvEvent.ProcessSampleMode == ProcessSampleMode.Update);

        for (var i: int = begin.rule; i < rules.Length; ++i) {
            lastRuleId = rules[i].RuleNumber;
            begin.rule = 0;

            var subRules: Schedule.Rule.SubRule[] = rules[i].GetSubRules();

            for (var j: int = begin.subRule; j < rules[i].Length; ++j) {
                lastSubRuleId = j;
                begin.subRule = 0;

                var options: System.Object = undefined;
                //if subRule has not got actions, find other subRule
                if (subRules[j].Length == 0 ||
                    ((subRules[j].ITS != 0) && (subRules[j].ITS != bvEvent.Interview.TransientState))) {
                    continue;
                }

                if ((subRules[j].ShiftTypeId != 0)) {
                    var workShiftTypeID: int;
                    workShiftTypeID = bvEvent.Shifts.GetShiftTypeWorkID(subRules[j].ShiftTypeId);
                    if (workShiftTypeID != lastCallShiftTypeID) {
                        continue;
                    }
                }

                if (subRules[j].FilterEnabled == false || subRules[j].IsFilterComplete(customScripWrapper, extendedAPI)) {

                    extendedAPI.LogMessage(String.Format("Executing rule {0}.{1}", lastRuleId + 1, lastSubRuleId + 1));

                    var actions: Schedule.Rule.SubRule.Action[] = subRules[j].GetActions();
                    for (var k: int = 0; k < subRules[j].Length; ++k) {
                        lastActionId = k;
                        if (maxActionsToExecute < actionCounter) {
                            throw new UserMessageException(String.Format("The maximum allowed number of executed actions in scheduling script ({0}) has been exceeded. Most likely it is caused by an infinite loop with a \"Go To\" statements that branches to a point earlier in the script.", maxActionsToExecute));
                        }
                        actionCounter++;
                        if (actions[k].Enabled == true &&
                            (actions[k].FilterEnabled == false || actions[k].IsFilterComplete(customScripWrapper, extendedAPI))) {
                            var nextSubRule: System.String = ProcessAction(actions[k]);
                            if (nextSubRule != "") {
                                //stop execution
                                if (nextSubRule == null) {
                                    return;
                                }

                                ProcessRules(GetPoint(nextSubRule), actionCounter);
                                return;
                            }
                        }
                    }

                    //if sub rule is complete then exit from interpreter
                    return;
                }
            }
        }
    }
}
}

package Interpreter
{
    interface IFilter
    {
        function IsFilterComplete(customScriptWrapper : CustomCode.CustomScriptWrapper,  extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI ) : Boolean;
    }

    interface ICustomScript
    {
       function ExecuteScript(customScriptWrapper : CustomCode.CustomScriptWrapper, extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI);
    }
    
    interface ICustomFunction
    {
       function ExecuteFunction(customScriptWrapper : CustomCode.CustomScriptWrapper, extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI) : System.Object;
    }

    class Schedule
    {
      static class Rule
      {
         static class SubRule implements IFilter
         {
            static class Action implements IFilter, ICustomScript, ICustomFunction
            {
                public function IsFilterComplete(customScriptWrapper : CustomCode.CustomScriptWrapper,  extendedAPI :  BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI) : Boolean
               {
                  var filterInvoker : CustomCode.FilterInvoker = filterFactory.GetInstanceFilterInvoker();
                  customScriptWrapper.Init(extendedAPI);
                  filterInvoker.Init(extendedAPI);
                  return filterInvoker.Invoke(customScriptWrapper);
               }
               
               public function ExecuteScript(customScriptWrapper : CustomCode.CustomScriptWrapper, extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI)
               {
                  var invoker = customScriptInvokerFactory.CreateInvoker();
                  invoker.Init(customScriptWrapper, extendedAPI);
                  invoker.Invoke();
               }
               
               public function ExecuteFunction(customScriptWrapper : CustomCode.CustomScriptWrapper, extendedAPI :  BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI) : System.Object
               {
                  customFunction.Init(customScriptWrapper, extendedAPI);
                  customFunction.Invoke();
               }
               
               private var id : int;
               private var actionID : int;
               private var filterEnabled : Boolean;
               private var enabled : Boolean;
               private var isConstant : Boolean;
               private var parameterValue : System.String;
               private var customParameterID : int;
              
               public var filterFactory : CustomCode.IFilterFactory;
               public var customScriptInvokerFactory : CustomCode.ICustomScriptInvokerFactory;
               public var customFunction : CustomCode.FunctionWrapper;
               
               public function get ID() : int
               {
                  return id;
               }
             
               public function get ActionID() : int
               {
                  return actionID;
               }
             
               public function get FilterEnabled() : Boolean
               {
                  return filterEnabled;
               } 
             
               public function get Enabled() : Boolean
               {
                  return enabled;
               } 

               public function get IsConstant() : Boolean
               {
                  return isConstant;
               }
             
               public function GetParamValue(extendedAPI :  BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI) : System.String
               {
                  if( IsConstant )
                  {
                    return parameterValue;
                  }
                  else
                  {
                    return extendedAPI.GetParamValue( Int32.Parse(customParameterID) )
                  }
               } 
             
               public function Action(idValue : int, 
                    actionIDValue : int, 
                    enabledValue : Boolean,
                    filterEnabledValue : Boolean, 
                    isConstantValue : Boolean,
                    parameter : System.String)
               {
                  id = idValue;
                  actionID = actionIDValue;
                  enabled = enabledValue;
                  filterEnabled = filterEnabledValue;
                  isConstant = isConstantValue;
                  parameterValue = parameter;
                  
                  if( isConstant )
                  {
                     customParameterID = 0;
                  }
                  else
                  {
                     customParameterID = Int32.Parse(parameter);
                  }
               }
            }
            
            public var filterFactory : CustomCode.IFilterFactory;
                    
            private var id : String;
            private var its : int;
            private var shiftTypeId : int;
            private var filterEnabled : Boolean;
            
            private var actions : Schedule.Rule.SubRule.Action[];
            private var count : int;
            
            public function get Length() : int
            {
               return count;
            }
            
            public function IsFilterComplete(customScriptWrapper : CustomCode.CustomScriptWrapper, extendedAPI :  BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI) : Boolean
            {
               var filterInvoker : CustomCode.FilterInvoker = filterFactory.GetInstanceFilterInvoker();
               filterInvoker.Init(extendedAPI);
               return filterInvoker.Invoke(customScriptWrapper);
            }
            
            
            public function get ID() : System.String
            {
               return id;
            }
            
            public function set ID(idValue : System.String)
            {
               id = idValue;
            }
            
            public function get ITS() : int
            {
               return its;
            }
            
            public function set ITS(itsValue : int)
            {
               its = itsValue;
            }
            
            public function get ShiftTypeId() : int
            {
               return shiftTypeId;
            }
            
            public function set ShiftTypeId(shiftTypeIdValue : int)
            {
               shiftTypeId = shiftTypeIdValue;
            }
            
            public function get FilterEnabled() : Boolean
            {
               return filterEnabled;
            }
            
            public function set FilterEnabled(filterEnabledValue : Boolean)
            {
               filterEnabled = filterEnabledValue;
            }
            
            public function SubRule(idValue : System.String, itsValue : int, 
                shiftTypeIdValue : int, filterEnabledValue : Boolean, 
                cnt : int )
            {
               ID = idValue;
               ITS = itsValue;
               ShiftTypeId = shiftTypeIdValue;
               FilterEnabled = filterEnabledValue;
               actions = new Schedule.Rule.SubRule.Action[cnt];
               count = 0;
            }
            
            public function AddAction(action : Schedule.Rule.SubRule.Action)
            {
               actions[count++] = action;
            }
            
            public function GetActions( ) : Schedule.Rule.SubRule.Action[]
            {
               return actions;
            }
         }
         
         private var subRules : Schedule.Rule.SubRule[];
         private var count : int;
         private var id: System.String;
         private var ruleNumber: int;
         private var sampleUpdate: System.Boolean;

         public function get Length() : int
         {
            return count;
         }
         public function get RuleNumber() : int
         {
             return ruleNumber;
         }

         public function set RuleNumber(ruleNumberValue : int)
         {
             ruleNumber = ruleNumberValue;
         }

         public function get ID() : System.String
         {
            return id;
         }
         
         public function set ID(idValue : System.String)
         {
            id = idValue;
         }

         public function get SampleUpdate(): System.Boolean
         {
            return sampleUpdate;
         }

         public function Rule(idValue : System.String, cntSubRules : int, sampleUpdate : System.Boolean)
         {
            count = 0;
            this.ID = idValue;
            this.sampleUpdate = sampleUpdate;
            subRules = new Schedule.Rule.SubRule[cntSubRules];
         }
         
         public function AddSubRule(subRule : Schedule.Rule.SubRule)
         {
            subRules[count++] = subRule;
         }
         
         public function GetSubRules( ) : Schedule.Rule.SubRule[]
         {
            return subRules;
         }
      }
      
      private var rules : Schedule.Rule[];
      private var count : int;
      
      public function get Length() : int
      {
         return count;
      }
      
      public function Schedule(cntRules : int)
      {
         rules = new Schedule.Rule[cntRules];
         count = 0;
      }
         
    public function AddRule(rule: Schedule.Rule) {
        rule.RuleNumber = count;
        rules[count] = rule;
        count++;
    }
      
      public function FindSampleUpdateRules(include : Boolean): Schedule.Rule[]
      {
          var rulesOut : ArrayList = new ArrayList();
          
          for(var i : int = 0; i < rules.Length; ++i)
          {
              if(rules[i].SampleUpdate == true)
              {
                  if (include == true)
                  {
                      rulesOut.Add(rules[i]);
                  }
              }
              else
              {
                  if (include == false)
                  {
                      rulesOut.Add(rules[i]);
                  }
              }
           }

           return rulesOut.ToArray();
      }

      public function GetRules(sampleUpdateMode : Boolean) : Schedule.Rule[]
      {
         if (sampleUpdateMode == true)
         {
             // If we're running sample update then return just sample update rule
             return FindSampleUpdateRules(true);
         }

         // Otherwise return all rules but SampleUpdate
         return FindSampleUpdateRules(false);
      }
    }
}

import System.Collection;
import System.Collection.Generic;

package CustomCode
{
   interface IFilterFactory
   {
      function GetInstanceFilterInvoker() : FilterInvoker;
   }

   abstract class FilterInvoker extends BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI
   {
      public function Init(extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI)
      {
         super.Init(extendedAPI);
      }

      public function Invoke(customScriptWrapper : CustomScriptWrapper) : Boolean
      {
         return true;
      }
   }
   
   abstract class CustomScriptWrapper extends BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI
   {
      public function Init(extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI)
      {
         super.Init(extendedAPI);
      }
   }
   
   interface ICustomScriptInvokerFactory
   {
       function CreateInvoker() : ICustomScriptInvoker;
   }

   interface ICustomScriptInvoker
   {
      function Invoke();
      function Init(wrapper : CustomScriptWrapper, extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI);
   }
   
   abstract class FunctionWrapper extends BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI
   {
      public var CustomScript : CustomScriptWrapper;
      public function Init(customScript : CustomScriptWrapper, extendedAPI : BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI)
      {
         super.Init(extendedAPI);
         CustomScript = customScript;
      }
      
      public abstract function Invoke();
   }
}
