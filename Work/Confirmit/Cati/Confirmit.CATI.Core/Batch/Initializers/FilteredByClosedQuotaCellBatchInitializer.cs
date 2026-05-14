using System;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class FilteredByClosedQuotaCellBatchInitializer : AbstractBatchInitializer<FilteredByClosedQuotaCellBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, FilteredByClosedQuotaCellBatchParameters parameters)
        {
            if (parameters.CellsIds == null && parameters.CellId != 0)//backward compatibility
            {
                parameters.CellsIds = new System.Collections.Generic.List<int>();
                parameters.CellsIds.Add(parameters.CellId);
            }

            var replicatedTable = ReplicationSchemaService.GetDestinationTableName(parameters.SurveyId);

            string query;
            string whereCondition;

            if (parameters.QuotaId == 0)
            {
                whereCondition = $@"qcell.SurveyId = @SurveyId";
            }
            else if (parameters.CellsIds.Count == 0)
            {
                whereCondition = $@"qcell.SurveyId = @SurveyId AND
                                    qcell.QuotaId = @QuotaId";
            }
            else
            {
                whereCondition = $@"qcell.SurveyId = @SurveyId AND
                                    qcell.QuotaId = @QuotaId AND
                                    EXISTS(SELECT 1 FROM @ids where Value = qcell.CellId)";
            }

            query =
                $@"SELECT DISTINCT interview.ID id FROM BvInterview AS interview
                INNER JOIN BvSurvey sr 
                    ON sr.SID = @SurveyId
                INNER JOIN BvState state 
                    ON state.StateId = interview.TransientState AND state.StateGroupId = sr.StateGroupId
                INNER JOIN BvInterviewQuotaCell AS icell
                    ON icell.SurveyId = @SurveyId AND icell.InterviewId = interview.ID
                INNER JOIN BvSurveyQuotaCell AS qcell
                    ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID
                WHERE {whereCondition} AND
                interview.SurveySID = @SurveyId AND
                qcell.IsOpen = 0 AND state.FcdAction = 0";

            uploader.UploadFromDatabase(query,
               new SqlParameter("@SurveyId", parameters.SurveyId),
               new SqlParameter("@QuotaId", parameters.QuotaId),
               BvIntArrayTypeAdapter.CreateSqlParameter("@ids", parameters.CellsIds));
        }
    }
}
