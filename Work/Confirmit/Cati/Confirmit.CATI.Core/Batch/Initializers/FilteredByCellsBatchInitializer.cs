using System;
using System.Linq;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class FilteredByCellsBatchInitializer : AbstractBatchInitializer<FilteredByCellsBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, FilteredByCellsBatchParameters parameters)
        {
            var query = String.Join(" UNION ALL ", parameters.Cells.Select(x => QuotaService.GetIterviewIdQueryForCell(parameters.SurveyId, parameters.Fields, x)).ToArray());
            query = String.Format("SELECT DISTINCT * FROM( {0} ) t", query);

            uploader.UploadFromDatabase(query);
        }
    }
}