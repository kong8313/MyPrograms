using System;
using System.Diagnostics;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewTimings : IInterviewTimings
    {
        public BvInterviewTimings GetInterviewTimings(BvTasksEntity task, BvSurveyEntity survey)
        {
            var timings = new BvInterviewTimings
            {
                InterviewDurationTime = task.TimeCallDelivered.HasValue
                    ? TimeDiff.Seconds(task.TimeCallDelivered.Value, task.CurrentUtcTime.Value)
                    : 0
            };


            if (!task.StartTime.HasValue || !task.TimeCallDelivered.HasValue)
            {
                Trace.TraceError(
                    "GetInterviewTimings: task state is not correct. starttime={0}, timecalldelivered={1}, callid={2}, interviewid={3}, surveyid={4}, survey name={5}, personSid = {6}",
                    task.StartTime, task.TimeCallDelivered, task.CallID, task.InterviewID, task.SurveySID, survey.Name,
                    task.PersonSID);
            }
            else
            {
                timings.WaitingTime = TimeDiff.Seconds(task.StartTime.Value, task.TimeCallDelivered.Value); 
            }

            if (timings.WaitingTime < 0)
            {
                // Negative WaitingTime is possible in some cases for predictive surveys, see Cr 47039.
                // In this case WaitingTime must be considered to be 0.
                timings.WaitingTime = 0;

                Trace.TraceWarning(
                    "GetInterviewTimings: waiting time is negative: [{0}] secs." +
                    " /// personSID='{1}' survey='{2} ({3})', dialMode = {4}, interviewId='{5}'" +
                    "Note: it can be legitimate if the survey is in predictive dial mode and value is not very large.",
                    timings.WaitingTime,
                    task.PersonSID,
                    survey.SID,
                    survey.Name,
                    survey.DialMode,
                    task.InterviewID);
            }

            if (task.OpenEndReviewStartTime != null)
            {
                timings.OpenEndReviewDurationTime = TimeDiff.Seconds(task.OpenEndReviewStartTime.Value, task.CurrentUtcTime.Value);
            }

            timings.CallCenterID = task.CallCenterID;

            return timings;
        }
    }
}
