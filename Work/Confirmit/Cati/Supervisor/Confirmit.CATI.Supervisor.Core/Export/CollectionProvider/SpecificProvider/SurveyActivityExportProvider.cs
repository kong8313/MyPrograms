using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represents export provider for Survey Activity view
    /// </summary>
    public class SurveyActivityExportProvider : CollectionExportProvider
    {
         #region Constructors

        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public SurveyActivityExportProvider(IEnumerable<SurveyActivityInfo> surveyInfos)
            : base(surveyInfos)
        {
        }

        public SurveyActivityExportProvider(IEnumerable<SurveyActivityInfo> surveyInfos, IDictionary<string, string> additionalParams)
            : base(surveyInfos, additionalParams)
        {
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public override IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (object obj in m_Collection)
            {
                yield return new SurveyActivityRecordExportProvider((SurveyActivityInfo)obj);
            }
        }

        #endregion
    }
}
