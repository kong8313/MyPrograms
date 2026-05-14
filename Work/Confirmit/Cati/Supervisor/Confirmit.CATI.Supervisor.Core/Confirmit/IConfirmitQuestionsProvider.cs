using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    public interface IConfirmitQuestionsProvider
    {
        /// <summary>
        /// Returns a list of questions replicated from Confirmit ordered by name.
        /// </summary>
        /// <param name="surveyId">Fusion survey SID.</param>        
        /// <returns></returns>
        List<VariableInfo> GetReplicatedQuestionsOrderedByName(int surveyId);

         /// <summary>
        /// Returns list of confirmit questions with their types taken from Confirmit.
        /// (excludes CallAttemptCount variable)
        /// </summary>
        List<VariableInfo> GetReplicatedQuestionsFromAuthoring(int surveyId);

        /// <summary>
        /// Returns list of integer based replicated questions.
        /// (excludes CallAttemptCount variable)
        /// </summary>
        List<VariableInfo> GetIntegerBasedReplicatedColumns(int surveySid);

        List<VariableInfo> GetSingleTypedReplicatedColumns(int surveySid);

        /// <summary>
        /// Returns list of single replicated questions.
        /// </summary>
        SingleVarWithAnswers GetSingleVariableWithAnswers(int surveySid, string questionName);

        IEnumerable<BvReplicationColumnsEntity> GetReplicatedQuestionColumns(int surveyId);
    }
}