using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.Filters;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class FilteredBatchInitializer : AbstractBatchInitializer<FilteredBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, FilteredBatchParameters parameters)
        {
            using (var filterHelper = new FilterHelper(parameters.FilterId, parameters.TimezoneId, parameters.SearchParams, parameters.ForceDispose))
            {
                FilterGenerateMode mode = GetFilterGenerationMode(parameters.Mode);

                var filter = ServiceLocator.Resolve<ISqlFilterProvider>().TryToGetFilter(filterHelper.FilterID, parameters.SurveyId);

                string filterSql = ServiceLocator.Resolve<IFilterService>().GenerateSqlWithSelect(filter, parameters.SurveyId, mode);

                uploader.UploadFromDatabase(filterSql);
            }
        }

        private FilterGenerateMode GetFilterGenerationMode(CallStates callState)
        {
            switch (callState)
            {
                case CallStates.Scheduled:
                    return FilterGenerateMode.ScheduledInterviewIds;
                case CallStates.Suspended:
                    return FilterGenerateMode.SuspendedInterviewIds;
                case CallStates.All:
                    return FilterGenerateMode.AllInterviewIds;
                case CallStates.HighPriority:
                    return FilterGenerateMode.HighPriorityInterviewIds;
                case CallStates.SentToDialer:
                    return FilterGenerateMode.SentToDialerInterviewIds;
            }

            throw new ArgumentException("Unknown callState");
        }
    }
}
