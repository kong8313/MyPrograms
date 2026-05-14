using System;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    /// <summary>
    /// All filter are based on template.
    /// this class presents this templates - default queries for all filter modes
    /// </summary>
    internal class DefaultQueries
    {
        private readonly int m_SurveySid;
        private readonly ICallCenterService _callCenterService;

        /// <summary>
        /// returns default query for filter mode
        /// </summary>
        /// <param name="mode">filter mode</param>
        /// <returns>default Query for this mode</returns>
        public BaseQuery this[FilterGenerateMode mode]
        {
            get
            {
                string replicationTable = ReplicationSchemaService.GetDestinationTableName(m_SurveySid);

                BaseQuery result;

                switch (mode)
                {
                    case FilterGenerateMode.ScheduledInterviews:
                        {
                            result = new ScheduledCallsQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    case FilterGenerateMode.SuspendedInterviews:
                        {
                            result = new SuspendedCallsQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    case FilterGenerateMode.AllInterviews:
                        {
                            result = new AllInterviewsQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    case FilterGenerateMode.SuspendedInterviewIds:
                        {
                            result = new SuspendedInterviewIDsQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.ScheduledInterviewIds:
                        {
                            result = new ScheduledInterviewIDsQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.AllInterviewIds:
                        {
                            result = new InterviewIDsQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.AllInterviewStates:
                        {
                            result = new InterviewsStatesQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.HighPriorityInterviews:
                        {
                            result = new HighPriorityCallsQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    case FilterGenerateMode.HighPriorityInterviewIds:
                        {
                            result = new HighPriorityInterviewIDsQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.SentToDialerInterviews:
                        {
                            result = new SentToDialerCallsQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    case FilterGenerateMode.SentToDialerInterviewIds:
                        {
                            result = new SentToDialerInterviewIDsQuery(m_SurveySid);
                            break;
                        }
                    case FilterGenerateMode.CallsAvailableNow:
                        {
                            result = new CallsAvailableNowQuery(m_SurveySid, replicationTable);
                            break;
                        }
                    default:
                        {
                            throw new IndexOutOfRangeException($"Filter mode {mode} is not processed");
                        }
                }

                return result;
            }
        }

        public DefaultQueries(int surveySID, ICallCenterService callCenterService)
        {
            m_SurveySid = surveySID;
            _callCenterService = callCenterService;
        }
    }
}