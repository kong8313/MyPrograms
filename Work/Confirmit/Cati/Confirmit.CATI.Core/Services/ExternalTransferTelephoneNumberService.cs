using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class ExternalTransferTelephoneNumberService : IExternalTransferTelephoneNumberService
    {
        private readonly IExternalTransferTelephoneNumberRepository _externalTransferTelephoneNumberRepository;

        public ExternalTransferTelephoneNumberService(IExternalTransferTelephoneNumberRepository externalTransferTelephoneNumberRepository)
        {
            _externalTransferTelephoneNumberRepository = externalTransferTelephoneNumberRepository;
        }

        public int[] GetAssignedSurveyIds(int externalTransferTelephoneNumberId)
        {
            return BvExternalTransferTelephoneNumberAssignmentAdapter.GetByCondition(
                "ExternalTransferTelephoneNumberId = @ExternalTransferTelephoneNumberId",
                new SqlParameter("@ExternalTransferTelephoneNumberId", externalTransferTelephoneNumberId)).Select(x => x.SurveyId).ToArray();
        }

        public void SetAssignedSurveyIds(int externalTransferTelephoneNumberId, int[] surveyIds)
        {
            var currentSurveyIds = GetAssignedSurveyIds(externalTransferTelephoneNumberId);
            var idsToDelete = currentSurveyIds.Except(surveyIds);
            var idsToCreate = surveyIds.Except(currentSurveyIds);

            BvExternalTransferTelephoneNumberAssignmentAdapter.DeleteByCondition(
                "ExternalTransferTelephoneNumberId = @ExternalTransferTelephoneNumberId AND EXISTS( SELECT 1 FROM @ids i where i.Value = SurveyId )",
                new SqlParameter("@ExternalTransferTelephoneNumberId", externalTransferTelephoneNumberId),
                BvIntArrayTypeAdapter.CreateSqlParameter("@ids", idsToDelete));

            foreach (var surveyId in idsToCreate)
            {
                BvExternalTransferTelephoneNumberAssignmentAdapter.Insert(
                    new BvExternalTransferTelephoneNumberAssignmentEntity()
                    {
                        ExternalTransferTelephoneNumberId = externalTransferTelephoneNumberId,
                        SurveyId = surveyId
                    });
            }
        }

        public void InsertNumber(string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds)
        {
            var evt = new CreateExternalTransferNumberEvent(telephoneNumber, description, assignedSurveysIds);
            using (var transaction =
                new DatabaseTransactionScope("Supervisor.InsertExtTransfer", DeadlockPriority.Supervisor))
            {
                int id = _externalTransferTelephoneNumberRepository.Insert(new BvExternalTransferTelephoneNumberEntity()
                {
                    TelephoneNumber = telephoneNumber,
                    Description = description,
                    Hidden = isHidden
                });

                SetAssignedSurveyIds(id, assignedSurveysIds);

                transaction.Commit();

                evt.ObjectId = id;
            }

            evt.Finish();
        }

        public void UpdateNumber(int id, string telephoneNumber, string description, bool isHidden,
            int[] assignedSurveysIds)
        {
            var evt = new UpdateExternalTransferNumberEvent(id, telephoneNumber, description, assignedSurveysIds);
            using (var transaction = new DatabaseTransactionScope("Supervisor.UpdateExtTransfer", DeadlockPriority.Supervisor))
            {
                _externalTransferTelephoneNumberRepository.Update(new BvExternalTransferTelephoneNumberEntity()
                {
                    Id = id,
                    TelephoneNumber = telephoneNumber,
                    Description = description,
                    Hidden = isHidden
                });

                SetAssignedSurveyIds(id, assignedSurveysIds);

                transaction.Commit();
            }

            evt.Finish();
        }

        public void DeleteNumbers(int[] ids)
        {
            var evt = new DeleteExternalTransferNumbersEvent(ids);

            using (var transaction = new DatabaseTransactionScope("Supervisor.DeleteExtTransfer", DeadlockPriority.Supervisor))
            {
                foreach (var id in ids)
                {
                    _externalTransferTelephoneNumberRepository.Delete(id);
                }

                transaction.Commit();
            }
            evt.Finish();
        }
    }
}
