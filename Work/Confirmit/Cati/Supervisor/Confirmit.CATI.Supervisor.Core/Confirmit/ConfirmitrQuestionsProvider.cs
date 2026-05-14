using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    /// <summary>
    /// Class provided questions replicated from Confirmit
    /// ('questions' means exactly questions excluding CallAttemptCount replicated variable)
    /// </summary>
    public class ConfirmitQuestionsProvider : IConfirmitQuestionsProvider
    {
        /// <summary>
        /// Returns a list of questions replicated from Confirmit ordered by name.
        /// </summary>
        /// <param name="surveyId">Fusion survey SID.</param>        
        /// <returns></returns>
        public List<VariableInfo> GetReplicatedQuestionsOrderedByName(int surveyId)
        {
            return GetReplicatedQuestionsFromAuthoring(surveyId).OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Returns list of confirmit questions with their types taken from Confirmit.
        /// (excludes CallAttemptCount variable)
        /// </summary>
        public List<VariableInfo> GetReplicatedQuestionsFromAuthoring(int surveyId)
        {
            IEnumerable<BvReplicationColumnsEntity> replicatedQuestionColumns = GetReplicatedQuestionColumns(surveyId);

            var formNames = from column in replicatedQuestionColumns select column.ColumnName;

            string projectId = SurveyRepository.GetById(surveyId).Name;

            var authoringService = ServiceLocator.Resolve<IAuthoringService>();

            var forms = authoringService.GetFormInfos(projectId, formNames, SchemaSourceType.RuntimeProduction);

            return forms.Select(ConvertToVariableInfo).Where(x => x != null).ToList();
        }


        /// <summary>
        /// Returns list of integer based replicated questions.
        /// (excludes CallAttemptCount variable)
        /// </summary>
        public List<VariableInfo> GetIntegerBasedReplicatedColumns(int surveySid)
        {
            return GetReplicatedQuestionsFromAuthoring(surveySid)
                .Where(x =>
                    x.ConfirmitVariableType == ConfirmitVariableType.Numeric ||
                    x.ConfirmitVariableType == ConfirmitVariableType.Single).ToList();
        }

        public List<VariableInfo> GetSingleTypedReplicatedColumns(int surveySid)
        {
            return GetReplicatedQuestionsFromAuthoring(surveySid)
                .Where(x=>x.ConfirmitVariableType == ConfirmitVariableType.Single).ToList();
        }

        /// <summary>
        /// Returns list of single replicated questions.
        /// </summary>
        public SingleVarWithAnswers GetSingleVariableWithAnswers(int surveySid, string questionName)
        {
            string projectId = SurveyRepository.GetById(surveySid).Name;

            var authoringService = ServiceLocator.Resolve<IAuthoringService>();

            var singleForm = authoringService.GetFormInfosWithText(projectId, new[] { questionName }, SchemaSourceType.RuntimeProduction).First(x => x is SingleForm);

            return ConvertToSingleVarWithAnswers(singleForm);
        }

        public IEnumerable<BvReplicationColumnsEntity> GetReplicatedQuestionColumns(int surveyId)
        {
            List<BvReplicationColumnsEntity> replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(surveyId);

            return replicatedColumns.Where(
                column => !column.ColumnName.Equals("CallAttemptCount", StringComparison.OrdinalIgnoreCase));
        }

        ///// <summary>
        ///// Returns list of confirmit questions replicated to CATI with their types.
        ///// (excludes CallAttemptCount variable).
        ///// </summary>
        //internal static List<VariableInfo> GetReplicatedQuestionsWithType(int surveyId)
        //{
        //    return GetReplicatedQuestionsWithType(surveyId);
        //}

        internal SingleVarWithAnswers ConvertToSingleVarWithAnswers(QuestionnaireNode node)
        {
            if (node is SingleForm singleVar)
            {
                return new SingleVarWithAnswers(singleVar.Name, VariableTypes.String, TableTypes.CFVariables)
                {
                    IsBackground = singleVar.VariableType == VariableDataType.Background,
                    Precodes = singleVar.SingleAnswers.Items.Select(x => x.Precode).ToList(),
                    AnswersList = singleVar.SingleAnswers.Items.Select(x => ((Answer)x).Texts.First().Value).ToList()
                };
            }

            return null;
        }

        internal VariableInfo ConvertToVariableInfo(QuestionnaireNode node)
        {
            if (node is OpenForm open)
            {
                if (open.Numeric)
                {
                    // Numeric.
                    return new VariableInfo(
                        open.Name,
                        VariableTypes.Decimal,
                        TableTypes.CFVariables,
                        ConfirmitVariableType.Numeric)
                        {
                            IsBackground = open.VariableType == VariableDataType.Background
                        };
                }
                else
                {
                    // Open text.
                    return new VariableInfo(
                        open.Name,
                        VariableTypes.String,
                        TableTypes.CFVariables,
                        ConfirmitVariableType.Open)
                        {
                            IsBackground = open.VariableType == VariableDataType.Background
                        };
                }

            }

            if (node is SingleForm single)
            {
                // Single.
                return new VariableInfo(
                    single.Name,
                    VariableTypes.String,
                    TableTypes.CFVariables,
                    ConfirmitVariableType.Single
                    )
                    {
                        IsBackground = single.VariableType == VariableDataType.Background
                    };
            }

            return null;
        }
    }
}