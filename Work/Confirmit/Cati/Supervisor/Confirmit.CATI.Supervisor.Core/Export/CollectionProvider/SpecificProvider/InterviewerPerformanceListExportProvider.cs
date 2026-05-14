using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{   
    public class InterviewerPerformanceListExportProvider : CollectionExportProvider
    {
         #region Constructors

        public InterviewerPerformanceListExportProvider(IEnumerable<InterviewerPerformanceInfo> infos)
            : base(infos)
        {
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public override IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (object obj in m_Collection)
            {
                yield return new ObjectExportRecordProvider(obj);
            }
        }

        #endregion
    }
}
