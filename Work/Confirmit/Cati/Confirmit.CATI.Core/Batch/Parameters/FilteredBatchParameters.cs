using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Batch
{
    [Serializable]
    public class FilteredBatchParameters : BatchParameters
    {
        public int SurveyId { get; set; }
        public int FilterId { get; set; }
        public bool ForceDispose { get; set; } = false;
        public int TimezoneId { get; set; }
        public CallStates Mode { get; set; }
        public SearchParameterCollection SearchParams { get; set; }

        public FilteredBatchParameters()
        {
        }

        public FilteredBatchParameters(int surveyId, int filterId, int timezoneId, CallStates mode, SearchParameterCollection searchParams)
        {
            SurveyId = surveyId;
            FilterId = filterId;
            Mode = mode;
            TimezoneId = timezoneId;
            SearchParams = searchParams;
        }

        public override BatchType Type
        {
            get { return BatchType.Filtered; }
        }
    }
}