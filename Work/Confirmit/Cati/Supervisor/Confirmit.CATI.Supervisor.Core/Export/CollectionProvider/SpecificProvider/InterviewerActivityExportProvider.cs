using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represents export provider for Interviewer Activity view
    /// </summary> 
    public class InterviewerActivityExportProvider : CollectionExportProvider
    {
         #region Constructors

        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public InterviewerActivityExportProvider(IEnumerable<TaskActivityInfo> taskInfos)
            : base(taskInfos)
        {
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public override IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (object obj in m_Collection)
            {
                yield return new InterviewerActivityRecordExportProvider((TaskActivityInfo)obj);
            }
        }

        #endregion
    }
}
