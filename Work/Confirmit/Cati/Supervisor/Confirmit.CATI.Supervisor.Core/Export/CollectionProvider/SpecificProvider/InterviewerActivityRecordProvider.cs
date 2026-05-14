using System;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.TelephonyProblemStates.ProblemState;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represent record provider for certain record in SurveyActivity view
    /// </summary>
    public class InterviewerActivityRecordExportProvider : ObjectExportRecordProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public InterviewerActivityRecordExportProvider(TaskActivityInfo taskActivityInfo)
            : base(taskActivityInfo)
        {
        }

        #endregion

        public override object this[string name]
        {
            get
            {
                TaskActivityInfo info = (TaskActivityInfo)m_Object;

                switch (name)
                {
                    case "StatusLogout":
                        return StringHelper.GetStringFromEnum(info.StatusLogout);
                    case "LoggedInToDialer":
                        return StringHelper.GetStringFromEnum(info.LoggedInToDialer);
                    case "InterviewState":
                        return StringHelper.GetStringFromEnum(info.InterviewState);
                    case "ProblemState":
                        return new CatiProblemStateFactory(new CatiProblemStateInfo(info.StationIdentifier))
                            .GetState(info.ProblemState).Message;
                    case "DiallingMode":
                        return StringHelper.GetStringFromEnum(info.DiallingMode);                
                    case "TimeCallDelivered":
                        return info.TimeCallDelivered.HasValue? info.TimeCallDelivered.Value.TimeOfDay : new TimeSpan(0);
                    case "LastKeepAliveTime":
                        return info.LastKeepAliveTime.HasValue ? info.LastKeepAliveTime.Value.TimeOfDay : new TimeSpan(0);
                    case "Duration":
                        if (info.TimeCallDelivered.HasValue)
                        {
                            return DateTime.UtcNow - info.TimeCallDelivered.Value;                            
                        }
                        break;
                }

                return base[name];                                
            }
        }

        #region IExportRecordProvider Members

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        public override IExportRecordProvider Descendants
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
