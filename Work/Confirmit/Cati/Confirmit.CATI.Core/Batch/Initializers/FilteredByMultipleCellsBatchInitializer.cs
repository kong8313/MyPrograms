using System;
using System.Linq;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class FilteredByMultipleCellsBatchInitializer : AbstractBatchInitializer<FilteredByMultipleCellsBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, FilteredByMultipleCellsBatchParameters parameters)
        {
            var query = String.Join(" UNION ALL ",
                parameters.QuotaParameters
                    .SelectMany((x, i) => x.Cells.Select(c => QuotaService.GetIterviewIdQueryForCell(parameters.SurveyId, parameters.QuotaParameters[i].Fields, c)))
            );
            query = String.Format("SELECT DISTINCT * FROM( {0} ) t", query);

            uploader.UploadFromDatabase(query);
        }
    }
}
