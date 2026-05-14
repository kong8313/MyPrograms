using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation.Fakes
{
    public class StubIRespondentBatchObtainer : IRespondentBatchObtainer 
    {
        private IRespondentBatchObtainer _inner;

        public StubIRespondentBatchObtainer()
        {
            _inner = null;
        }

        public IRespondentBatchObtainer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate RespondentRecord[] GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32BooleanDelegate(BvSurveyEntity survey, int batchId, int startRangeOfInterviewId, int partitionSize, bool isSampleUpdate);
        public GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32BooleanDelegate GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32Boolean;

        RespondentRecord[] IRespondentBatchObtainer.GetRespondentBatchPartition(BvSurveyEntity survey, int batchId, int startRangeOfInterviewId, int partitionSize, bool isSampleUpdate)
        {


            if (GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32Boolean != null)
            {
                return GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32Boolean(survey, batchId, startRangeOfInterviewId, partitionSize, isSampleUpdate);
            } else if (_inner != null)
            {
                return ((IRespondentBatchObtainer)_inner).GetRespondentBatchPartition(survey, batchId, startRangeOfInterviewId, partitionSize, isSampleUpdate);
            }

            return default(RespondentRecord[]);
        }

        public delegate RespondentRecord[] GetRespondentsForSynchronizationBvSurveyEntityInt32Delegate(BvSurveyEntity survey, int partitionSize);
        public GetRespondentsForSynchronizationBvSurveyEntityInt32Delegate GetRespondentsForSynchronizationBvSurveyEntityInt32;

        RespondentRecord[] IRespondentBatchObtainer.GetRespondentsForSynchronization(BvSurveyEntity survey, int partitionSize)
        {


            if (GetRespondentsForSynchronizationBvSurveyEntityInt32 != null)
            {
                return GetRespondentsForSynchronizationBvSurveyEntityInt32(survey, partitionSize);
            } else if (_inner != null)
            {
                return ((IRespondentBatchObtainer)_inner).GetRespondentsForSynchronization(survey, partitionSize);
            }

            return default(RespondentRecord[]);
        }

    }
}