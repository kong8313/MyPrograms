using System.Linq;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	/// <summary>
	/// Enum for thresholds types defined in BE.
	/// </summary>
	public enum BvThresholdType
	{
		LastSubmissionAlert = 1,
		InterviewersLoggedCountAlert = 2,
        [AllowNegative]
		NextAppointmentTimeAlert = 3,
		ScheduledCallsCountAlert = 6,
		SuspendedCallsCountAlert = 7,
		MinutesSpentWorkingOnSurveyAlert = 8,
		AssignedInterviewersCountAlert = 9,
		StrikeRateAlert = 10,
		CountCallsAlert = 11,
        AppointmentListAlert = 15,
        LastKeepAliveTimeAlert = 16,
        QuickAnswerSubmissionAlert = 17,
        NoActivityAlert = 18,
	    InterviewDurationAlert = 19,
	    BreakDurationAlert = 20
    }

    public static class ThresholdTypeExtensions
    {
        public static bool IsNegativeAllowed(this BvThresholdType threshold)
        {
            var fieldInfo = typeof (BvThresholdType).GetField(threshold.ToString());
            return fieldInfo.GetCustomAttributes(typeof(AllowNegativeAttribute), false).Any();
        }
    }
}