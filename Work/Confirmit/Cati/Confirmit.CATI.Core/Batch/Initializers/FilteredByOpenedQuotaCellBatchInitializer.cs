using System;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class FilteredByOpenedQuotaCellBatchInitializer : AbstractBatchInitializer<FilteredByOpenedQuotaCellBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, FilteredByOpenedQuotaCellBatchParameters parameters)
        {
            if (parameters.CellsIds == null && parameters.CellId != 0)//backward compatibility
            {
                parameters.CellsIds = new System.Collections.Generic.List<int>();
                parameters.CellsIds.Add(parameters.CellId);
            }

            var replicatedTable = ReplicationSchemaService.GetDestinationTableName(parameters.SurveyId);
            
            string query =
                $@"
                SELECT DISTINCT icell.InterviewId id 
                FROM BvInterviewQuotaCell AS icell
                INNER JOIN BvSurveyQuotaCell AS qcell
                    ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 1 
                LEFT JOIN BvSurveyQuotaCell AS qcell2
                    ON icell.SurveyID = qcell2.SurveyID AND icell.QuotaID = qcell2.QuotaID AND icell.CellID = qcell2.CellID AND qcell2.IsOpen = 0
                WHERE 
                qcell.SurveyId = @SurveyId AND 
                qcell.QuotaID = @QuotaId AND 
                EXISTS(SELECT 1 FROM @ids where Value = qcell.CellId) AND
                qcell2.CellID is NULL";

            uploader.UploadFromDatabase(query,
                new SqlParameter("@SurveyId", parameters.SurveyId),
                new SqlParameter("@QuotaId", parameters.QuotaId),
                BvIntArrayTypeAdapter.CreateSqlParameter("@ids", parameters.CellsIds));
            //we dont call BvSpClosedCellHistoryInsert because ClosedCellHistory is unused for now
        }
    }
}