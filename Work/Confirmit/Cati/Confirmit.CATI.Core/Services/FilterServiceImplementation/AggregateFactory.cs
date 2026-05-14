using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation;
using Confirmit.CATI.Common.Exceptions;


namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    class AggregateFactory
    {
        private const string aggregate = "COUNT(*) CNT";

        public static BaseQuery CreateAggregateQuery(FilterGenerateMode filterMode, int surveySID)
        {
            BaseQuery result = null;

            switch (filterMode)
            {
                case FilterGenerateMode.ScheduledInterviews:
                case FilterGenerateMode.ScheduledInterviewIds:
                    {
                        result = new ScheduledInterviewIDsQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.CallsAvailableNow:
                    {
                        result = new AvailableNowIDsQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.AllInterviews:
                case FilterGenerateMode.AllInterviewIds:
                    {
                        result = new InterviewIDsQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.AllInterviewStates:
                    {
                        result = new InterviewsStatesQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.SuspendedInterviews:
                case FilterGenerateMode.SuspendedInterviewIds:
                    {
                        result = new SuspendedInterviewIDsQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.HighPriorityInterviews:
                case FilterGenerateMode.HighPriorityInterviewIds:
                    {
                        result = new HighPriorityInterviewIDsQuery(surveySID);
                        break;
                    }
                case FilterGenerateMode.SentToDialerInterviews:
                case FilterGenerateMode.SentToDialerInterviewIds:
                {
                    result = new SentToDialerInterviewIDsQuery(surveySID);
                    break;
                }
                default:
                    {
                        throw new UserMessageException(String.Format(
                           "Count for filter mode {0} is not calculated",
                           filterMode));
                    }
            }

            result.SelectClause = aggregate;

            return result;
        }
    }
}
