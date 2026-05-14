using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetEngine
{
    public static class SchedulingScriptExecutionReasonConverter
    {
        public static string ConvertToString(SchedulingScriptExecutionReason executionReason)
        {
            string executionReasonString;
            switch (executionReason)
            {
                case SchedulingScriptExecutionReason.Unspecified:
                    executionReasonString = "Unspecified";
                    break;
                case SchedulingScriptExecutionReason.Added:
                    executionReasonString = "Respondent added";
                    break;
                case SchedulingScriptExecutionReason.AddedBySample:
                    executionReasonString = "Respondent uploaded";
                    break;
                case SchedulingScriptExecutionReason.Expired:
                    executionReasonString = "Call expired";
                    break;
                case SchedulingScriptExecutionReason.Inbound:
                    executionReasonString = "Inbound call";
                    break;
                case SchedulingScriptExecutionReason.MovedAndRescheduled:
                    executionReasonString = "Move and reschedule operation";
                    break;
                case SchedulingScriptExecutionReason.NotConnected:
                    executionReasonString = "Call not connected by dialer";
                    break;
                case SchedulingScriptExecutionReason.Processed:
                    executionReasonString = "Interview finished";
                    break;
                case SchedulingScriptExecutionReason.TelephonyError:
                    executionReasonString = "Telephony error";
                    break;
                case SchedulingScriptExecutionReason.Terminated:
                    executionReasonString = "Interview terminated";
                    break;
                default:
                    executionReasonString = "Unrecognised";
                    break;
            }
            return executionReasonString;
        }
    }
}
