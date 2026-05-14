using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Transactions.Filters
{
    public class MoveSurveySpecificFiltersToSurveyTransaction : ITransaction
    {
        private readonly int _sourceSurveyId;
        private readonly int _targetSurveyId;

        public MoveSurveySpecificFiltersToSurveyTransaction(int sourceSurveyId, int targetSurveyId)
        {
            _sourceSurveyId = sourceSurveyId;
            _targetSurveyId = targetSurveyId;
        }

        public void Execute()
        {
            using (var transaction = new DatabaseTransactionScope("MoveFiltersToSurvey", DeadlockPriority.Supervisor))
            {
                var targetProject = SurveyRepository.GetById(_targetSurveyId);
                var evt = new MoveSurveySpecificFiltersToSurveyEvent(_sourceSurveyId, _targetSurveyId, targetProject.ProjectId);

                new FilterCopyingService().MoveSurveySpecificFiltersToSurvey(_sourceSurveyId, _targetSurveyId);

                evt.Finish();
                transaction.Commit();
            }
        }
    }
}