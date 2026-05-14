using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represent record provider for certain record in SurveyActivity view
    /// </summary>
    public class SurveyActivityRecordExportProvider : ObjectExportRecordProvider
    {
        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public SurveyActivityRecordExportProvider(SurveyActivityInfo surveyInfo)
            : base(surveyInfo)
        {
        }

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        public override IExportRecordProvider Descendants
        {
            get
            {
                var activityManager = ServiceLocator.Resolve<IActivityManager>();
                return new StatusInfoExportProvider(
                    activityManager.GetStatusBreakdown(((SurveyActivityInfo)m_Object).SID));
            }
        }
    }
}
