using System;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.Fakes
{
    public class StubIConfirmitQuestionsProvider : IConfirmitQuestionsProvider 
    {
        private IConfirmitQuestionsProvider _inner;

        public StubIConfirmitQuestionsProvider()
        {
            _inner = null;
        }

        public IConfirmitQuestionsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<VariableInfo> GetReplicatedQuestionsOrderedByNameInt32Delegate(int surveyId);
        public GetReplicatedQuestionsOrderedByNameInt32Delegate GetReplicatedQuestionsOrderedByNameInt32;

        List<VariableInfo> IConfirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(int surveyId)
        {


            if (GetReplicatedQuestionsOrderedByNameInt32 != null)
            {
                return GetReplicatedQuestionsOrderedByNameInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetReplicatedQuestionsOrderedByName(surveyId);
            }

            return default(List<VariableInfo>);
        }

        public delegate List<VariableInfo> GetReplicatedQuestionsFromAuthoringInt32Delegate(int surveyId);
        public GetReplicatedQuestionsFromAuthoringInt32Delegate GetReplicatedQuestionsFromAuthoringInt32;

        List<VariableInfo> IConfirmitQuestionsProvider.GetReplicatedQuestionsFromAuthoring(int surveyId)
        {


            if (GetReplicatedQuestionsFromAuthoringInt32 != null)
            {
                return GetReplicatedQuestionsFromAuthoringInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetReplicatedQuestionsFromAuthoring(surveyId);
            }

            return default(List<VariableInfo>);
        }

        public delegate List<VariableInfo> GetIntegerBasedReplicatedColumnsInt32Delegate(int surveySid);
        public GetIntegerBasedReplicatedColumnsInt32Delegate GetIntegerBasedReplicatedColumnsInt32;

        List<VariableInfo> IConfirmitQuestionsProvider.GetIntegerBasedReplicatedColumns(int surveySid)
        {


            if (GetIntegerBasedReplicatedColumnsInt32 != null)
            {
                return GetIntegerBasedReplicatedColumnsInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetIntegerBasedReplicatedColumns(surveySid);
            }

            return default(List<VariableInfo>);
        }

        public delegate List<VariableInfo> GetSingleTypedReplicatedColumnsInt32Delegate(int surveySid);
        public GetSingleTypedReplicatedColumnsInt32Delegate GetSingleTypedReplicatedColumnsInt32;

        List<VariableInfo> IConfirmitQuestionsProvider.GetSingleTypedReplicatedColumns(int surveySid)
        {


            if (GetSingleTypedReplicatedColumnsInt32 != null)
            {
                return GetSingleTypedReplicatedColumnsInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetSingleTypedReplicatedColumns(surveySid);
            }

            return default(List<VariableInfo>);
        }

        public delegate SingleVarWithAnswers GetSingleVariableWithAnswersInt32StringDelegate(int surveySid, string questionName);
        public GetSingleVariableWithAnswersInt32StringDelegate GetSingleVariableWithAnswersInt32String;

        SingleVarWithAnswers IConfirmitQuestionsProvider.GetSingleVariableWithAnswers(int surveySid, string questionName)
        {


            if (GetSingleVariableWithAnswersInt32String != null)
            {
                return GetSingleVariableWithAnswersInt32String(surveySid, questionName);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetSingleVariableWithAnswers(surveySid, questionName);
            }

            return default(SingleVarWithAnswers);
        }

        public delegate IEnumerable<BvReplicationColumnsEntity> GetReplicatedQuestionColumnsInt32Delegate(int surveyId);
        public GetReplicatedQuestionColumnsInt32Delegate GetReplicatedQuestionColumnsInt32;

        IEnumerable<BvReplicationColumnsEntity> IConfirmitQuestionsProvider.GetReplicatedQuestionColumns(int surveyId)
        {


            if (GetReplicatedQuestionColumnsInt32 != null)
            {
                return GetReplicatedQuestionColumnsInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IConfirmitQuestionsProvider)_inner).GetReplicatedQuestionColumns(surveyId);
            }

            return default(IEnumerable<BvReplicationColumnsEntity>);
        }

    }
}