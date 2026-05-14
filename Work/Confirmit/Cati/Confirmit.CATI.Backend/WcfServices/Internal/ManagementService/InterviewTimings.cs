using System;
using System.Diagnostics;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.Internal.ManagementService
{
    public class InterviewTimings : IInterviewTimings
    {
        public BvInterviewTimings GetInterviewTimings(BvTasksEntity task, BvSurveyEntity survey)
        {
            var timings = new BvInterviewTimings();

            timings.InterviewDuriationTime = task.TimeCallDelivered.HasValue ? GetTimeDiffInSeconds(task.TimeCallDelivered.Value, task.CurrentUtcTime) : 0;

            if (!task.StartTime.HasValue || !task.TimeCallDelivered.HasValue)
            {
                Trace.TraceError(
                    "GetInterviewTimings: task state is not correct. starttime={0}, timecalldelivered={1}, callid={2}, interviewid={3}, surveyid={4}, survey name={5}, personSid = {6}",
                    task.StartTime, task.TimeCallDelivered, task.CallID, task.InterviewID, task.SurveySID, survey.Name,
                    task.PersonSID);
            }
            else
            {
                timings.WaitingTime = GetTimeDiffInSeconds(task.StartTime.Value, task.TimeCallDelivered.Value); 
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
                timings.OpenEndReviewDurationTime = GetTimeDiffInSeconds(task.OpenEndReviewStartTime.Value, task.CurrentUtcTime);
            }

            timings.CallCenterID = task.CallCenterID;

            return timings;
        }

        private int GetTimeDiffInSeconds(DateTime start, DateTime finish)
        {
            return (int) Math.Round((finish - start).TotalSeconds);
        }
    }
}
