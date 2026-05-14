using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Supervisor.Resources;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    /// <summary>
    /// Helps with getting user-friendly strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Converts InterviewState enumeration value to user-friendly string.
        /// </summary>        
        /// <returns>user-friendly value</returns>
        public static string GetStringFromEnum(this InterviewState enumValue)
        {
            switch(enumValue)
            {
                case InterviewState.DIALLING:
                    return GetResString("Dialling");
                case InterviewState.INTERVIEW_WRAP_UP:
                    return GetResString("InterviewWrapUp");
                case InterviewState.INTERVIEWING:
                     return GetResString("Interviewing");
                case InterviewState.NO_CALLS:
                    return GetResString("NoCalls");
                case InterviewState.OPENEND_REVIEW:
                    return GetResString("OpenendReview");
                case InterviewState.SELECTING:
                    return GetResString("Selecting");
                case InterviewState.WAITING:
                    return GetResString("Waiting");
                case InterviewState.REDIALLING:
                    return GetResString("Redialling");
                case InterviewState.INTERVIEWING_INBOUND:
                    return GetResString("InterviewingInbound");
                case InterviewState.OUTGOING_TRANSFER:
                    return GetResString("OutgoingTransfer");
                case InterviewState.INCOMING_TRANSFER:
                    return GetResString("IncomingTransfer");
                default:
                    return enumValue.ToString();
            }
        }      

        public static string GetAccordingToInterviewState(this CallConnectionState enumValue, InterviewState interviewState)
        {
            switch(interviewState)
            {
                case InterviewState.DIALLING:
                case InterviewState.REDIALLING:
                case InterviewState.OPENEND_REVIEW:
                    return interviewState.GetStringFromEnum();
                default:
                    return enumValue == CallConnectionState.Disconnected
                        ? GetResString("Disconnected")
                        : interviewState.GetStringFromEnum();
            }
        }  

        /// <summary>
        /// Converts DiallingMode enumeration value to user-friendly string.
        /// </summary>
        /// <remarks>
        /// It is used to show localiezed user-friendly message in process control.
        /// </remarks>
        public static string GetStringFromEnum(DialingMode  enumValue)
        {
            switch (enumValue)
            {
                case DialingMode.Manual:
                    return GetResString("Manual");
                case DialingMode.Preview:
                    return GetResString("Preview");
                case DialingMode.Predictive:
                    return GetResString("Predictive");
                case DialingMode.Automatic:
                    return GetResString("Automatic");
                case DialingMode.SpecialDial:
                    return GetResString("SpecialDialDialingMode");
                default:
                    return enumValue.ToString();
            }
        }

        /// <summary>
        /// Converts LoginState enumeration value to user-friendly string.
        /// </summary>        
        /// <returns>user-friendly value</returns>
        public static string GetStringFromEnum(LoginState  enumValue)
        {
            switch (enumValue)
            {
               case LoginState.LOGGED_IN:
                    return GetResString("Yes");                    
                case LoginState.NOT_LOGGED_IN:
                    return GetResString("No");                    
                case LoginState.PENDING_LOGOUT:
                    return  GetResString("Pending");                
                case LoginState.LOGGING_IN:
                    return  GetResString("Logging");
                case LoginState.LOGGING_OUT:
                    return GetResString("LoggingOut");
                case LoginState.PENDING_BREAK:
                    return GetResString("Pending break");
                case LoginState.BREAK:
                    return GetResString("On break");
                default:
                    return enumValue.ToString();
            }            
        }

        public static string GetDialerStateInfo(LoginState loginState, int dialerId)
        {
            var dialerStateInfo = GetStringFromEnum(loginState);

            if ((loginState == LoginState.NOT_LOGGED_IN) || (dialerId < 1))
            {
                return dialerStateInfo;
            }

            return dialerStateInfo + "(" + dialerId + ")";
        }


        /// <summary>
        /// Converts CATIProblemState enumeration value to user-friendly string.
        /// </summary>        
        /// <returns>user-friendly value</returns>
        public static string GetStringFromEnum(AgentTaskChoiceMode enumValue)
        {
            switch (enumValue)
            {
                case AgentTaskChoiceMode.Automatic:
                    return Strings.TaskChoiceAutomatic;
                case AgentTaskChoiceMode.Manual:
                    return Strings.TaskChoiceManualSelection;
                case AgentTaskChoiceMode.CampaignAssignment:
                    return Strings.TaskChoiceSurveySelection;
                case AgentTaskChoiceMode.Choice:
                    return Strings.TaskChoiceChoice;
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Converts ConfirmitVariableType enumeration value into user-friendly string.
        /// </summary>
        /// <param name="enumValue">Enumeration value.</param>
        /// <returns>Localized string.</returns>
        public static string GetStringFromEnum(ConfirmitVariableType enumValue)
        {
            string result;

            switch(enumValue)
            {
                case ConfirmitVariableType.Grid:
                    result = Strings.VariableTypeGrid;
                    break;
                case ConfirmitVariableType.Loop:
                    result = Strings.VariableTypeLoop;
                    break;
                case ConfirmitVariableType.Multi:
                    result = Strings.VariableTypeMulti;
                    break;
                case ConfirmitVariableType.NotSet:
                    result = Strings.VariableTypeNotSet;
                    break;
                case ConfirmitVariableType.Open:
                    result = Strings.VariableTypeOpen;
                    break;
                case ConfirmitVariableType.Numeric:
                    result = Strings.VariableTypeNumeric;
                    break;
                case ConfirmitVariableType.Single:
                    result = Strings.VariableTypeSingle;
                    break;
                default:
                    result = enumValue.ToString();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts SchedulingParameterType enumeration value to user-friendly string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetStringForEnum(SchedulingParameterType type)
        {
            switch (type)
            {
                case SchedulingParameterType.Integer:
                    return Strings.Numeric;
                case SchedulingParameterType.ShiftType:
                    return Strings.ShiftType;
                case SchedulingParameterType.Shift:
                    return Strings.Shift;
                case SchedulingParameterType.ExtendedStatus:
                    return Strings.ExtendedStatus;
                case SchedulingParameterType.Resource:
                    return Strings.AssignmentResource;
                default:
                    return type.ToString();
            }
        }

        /// <summary>
        /// Converts <see cref="DateTimeRange"/> enumeration value into user-friendly string.
        /// </summary>
        /// <param name="enumValue">Enumeration value.</param>
        /// <returns>user-friendly value</returns>
        public static string GetStringFromEnum(DateTimeRange enumValue)
        {
            switch (enumValue)
            {
                case DateTimeRange.Range:
                    return Strings.Range;
                case DateTimeRange.Today:
                    return Strings.Today;
                case DateTimeRange.Last2Hrs:
                    return Strings.Last2hrs;
                case DateTimeRange.Last4Hrs:
                    return Strings.Last4hrs;
                case DateTimeRange.Last2Days:
                    return Strings.Last2Days;
                case DateTimeRange.TodayMinus1:
                    return Strings.Yesterday;
                case DateTimeRange.TodayMinus2:
                    return "2 " + Strings.DaysAgo;
                case DateTimeRange.TodayMinus3:
                    return "3 " + Strings.DaysAgo;
                case DateTimeRange.TodayMinus4:
                    return "4 " + Strings.DaysAgo;
                case DateTimeRange.TodayMinus5:
                    return "5 " + Strings.DaysAgo;
                case DateTimeRange.TodayMinus6:
                    return "6 " + Strings.DaysAgo;
                case DateTimeRange.TodayMinus7:
                    return "7 " + Strings.DaysAgo;
                case DateTimeRange.ThisWeek:
                    return Strings.ThisWeek;
                case DateTimeRange.ThisMonth:
                    return Strings.ThisMonth;
                case DateTimeRange.ThisYear:
                    return Strings.ThisYear;
                case DateTimeRange.All:
                    return Strings.All;
                default:
                    return enumValue.ToString();
            }
        }

        /// <summary>
        /// Converts <see cref="DialerStatus"/> enumeration value into user-friendly string.
        /// </summary>
        /// <param name="enumValue">Enumeration value.</param>
        /// <returns>user-friendly value</returns>
        public static string GetStringFromEnum(DialerStatus enumValue)
        {
            switch (enumValue)
            {
                case DialerStatus.ConnectedAndActivated:
                    return Strings.DialerConnectedAndActivated;
                case DialerStatus.ConnectedAndDeactivated:
                    return Strings.DialerConnectedAndDeactivated;
                case DialerStatus.DisconnectedAndDeactivated:
                    return Strings.DialerDisconnectedAndDeactivated;
                case DialerStatus.DisconnectedTryingToConnect:
                    return Strings.DisconnectedTryingToConnect;
                case DialerStatus.DisconnectedTryingToConnectAndActivate:
                    return Strings.DisconnectedTryingToConnectAndActivate;

                default:
                    return enumValue.ToString();
            }
        }

        public static string GetStringFromEnum(SchedulingScriptState enumValue)
        {
            switch (enumValue)
            {
                case SchedulingScriptState.NotLaunched:
                    return Strings.NotLaunched;
                case SchedulingScriptState.PendingSynchronization:
                    return Strings.PendingSynchronization;
                case SchedulingScriptState.Synchronized:
                    return Strings.Synchronized;
                default:
                    return enumValue.ToString();
            }
        }

        public static string GetStringFromEnum(this InterviewerSubmissionAlert enumValue)
        {
            switch (enumValue)
            {
                case InterviewerSubmissionAlert.All:
                    return Strings.All;
                case InterviewerSubmissionAlert.LastSubmission:
                    return Strings.LastSubmission;
                case InterviewerSubmissionAlert.QuickAnswer:
                    return Strings.QuickAnswer;
                default:
                    return enumValue.ToString();
            }
        }

        public static string GetStringFromEnum(this AsyncOperationState enumValue)
        {
            switch (enumValue)
            {
                case AsyncOperationState.Queued:
                    return Strings.AsyncOperationState_Queued;
                case AsyncOperationState.Executing:
                    return Strings.AsyncOperationState_Executing;
                case AsyncOperationState.Completed:
                    return Strings.AsyncOperationState_Completed;
                case AsyncOperationState.PartiallyCompleted:
                    return Strings.AsyncOperationState_PartiallyCompleted;
                case AsyncOperationState.Failed:
                    return Strings.AsyncOperationState_Failed;
                case AsyncOperationState.Aborted:
                    return Strings.AsyncOperationState_Aborted;
                case AsyncOperationState.Cancelling:
                    return Strings.AsyncOperationState_Cancelling;
                default:
                    return enumValue.ToString();
            }
        }

        public static string GetStringFromEnum(this OperationTypes enumValue)
        {
            switch (enumValue)
            {
                case OperationTypes.ActivateCalls:
                    return Strings.OperationTypes_ActivateCalls;
                case OperationTypes.EditCalls:
                    return Strings.OperationTypes_EditCalls;
                case OperationTypes.EnableCalls:
                    return Strings.OperationTypes_EnableCalls;
                case OperationTypes.MoveCalls:
                    return Strings.OperationTypes_MoveCalls;
                case OperationTypes.RestoreSurvey:
                    return Strings.OperationTypes_RestoreSurvey;
                case OperationTypes.ChangePriorityOfCalls:
                    return Strings.OperationTypes_ChangePriorityOfCalls;
                case OperationTypes.ChangeDialModeOfInterviews:
                    return Strings.OperationTypes_ChangeDialModeOfInterviews;
                case OperationTypes.ChangeShiftTypeOfCalls:
                    return Strings.OperationTypes_ChangeShiftTypeOfCalls;
                case OperationTypes.MoveAndRescheduleCalls:
                    return Strings.OperationTypes_MoveAndRescheduleCalls;
                case OperationTypes.AssignCalls:
                    return Strings.OperationTypes_AssignCalls;
                case OperationTypes.DeactivateCalls:
                    return Strings.OperationTypes_DeactivateCalls;
                case OperationTypes.LaunchSurvey:
                    return Strings.OperationTypes_LaunchSurvey;
                case OperationTypes.DeleteSurvey:
                    return Strings.OperationTypes_DeleteSurvey;
                case OperationTypes.ConfigureClusteredQuota:
                    return Strings.OperationTypes_ConfigureClusteredQuota;
                case OperationTypes.DeleteRespondents:
                    return Strings.OperationTypes_DeleteRespondents;
                case OperationTypes.UpdateFcdQuota:
                    return Strings.OperationTypes_UpdateFcdQuota;
                case OperationTypes.ExecuteRoutineMaintenance:
                    return Strings.OperationTypes_ExecuteRoutineMaintenance;
                case OperationTypes.DeleteCallsByBlacklist:
                    return Strings.OperationTypes_DeleteCallsByBlacklist;
                case OperationTypes.InitializeDeleteCallsByBlacklist:
                    return Strings.OperationTypes_InitializeDeleteCallsByBlacklist;
                case OperationTypes.SynchronizeRespondents:
                    return Strings.OperationTypes_SynchronizeRespondents;
                case OperationTypes.RereadSurveyReplicatedData:
                    return Strings.OperationTypes_RereadSurveyReplicatedData;
                default:
                    return enumValue.ToString();
            }
        }  

        /// <summary>
        /// Returns localized string
        /// </summary>        
        private static string GetResString(string key)
        {
            return ResourceWrapper.Instance.GetString(key);
        }
    }
}
