using System;
using System.Threading;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIReplicationService : IReplicationService 
    {
        private IReplicationService _inner;

        public StubIReplicationService()
        {
            _inner = null;
        }

        public IReplicationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void RunPeriodicalReplicationCancellationTokenDelegate(CancellationToken cancellationToken);
        public RunPeriodicalReplicationCancellationTokenDelegate RunPeriodicalReplicationCancellationToken;

        void IReplicationService.RunPeriodicalReplication(CancellationToken cancellationToken)
        {

            if (RunPeriodicalReplicationCancellationToken != null)
            {
                RunPeriodicalReplicationCancellationToken(cancellationToken);
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).RunPeriodicalReplication(cancellationToken);
            }
        }

        public delegate void RunForceReplicationDelegate();
        public RunForceReplicationDelegate RunForceReplication;

        void IReplicationService.RunForceReplication()
        {

            if (RunForceReplication != null)
            {
                RunForceReplication();
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).RunForceReplication();
            }
        }

        public delegate void RunForceReplicationInt32CancellationTokenDelegate(int surveyId, CancellationToken cancellationToken);
        public RunForceReplicationInt32CancellationTokenDelegate RunForceReplicationInt32CancellationToken;

        void IReplicationService.RunForceReplication(int surveyId, CancellationToken cancellationToken)
        {

            if (RunForceReplicationInt32CancellationToken != null)
            {
                RunForceReplicationInt32CancellationToken(surveyId, cancellationToken);
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).RunForceReplication(surveyId, cancellationToken);
            }
        }

        public delegate void UploadSampleDataToReplicatedTableInt32Int32CancellationTokenDelegate(int surveyId, int batchId, CancellationToken cancellationToken);
        public UploadSampleDataToReplicatedTableInt32Int32CancellationTokenDelegate UploadSampleDataToReplicatedTableInt32Int32CancellationToken;

        void IReplicationService.UploadSampleDataToReplicatedTable(int surveyId, int batchId, CancellationToken cancellationToken)
        {

            if (UploadSampleDataToReplicatedTableInt32Int32CancellationToken != null)
            {
                UploadSampleDataToReplicatedTableInt32Int32CancellationToken(surveyId, batchId, cancellationToken);
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).UploadSampleDataToReplicatedTable(surveyId, batchId, cancellationToken);
            }
        }

        public delegate void RereadSurveyReplicatedDataInt32StringCancellationTokenDelegate(int surveyId, string reason, CancellationToken cancellationToken);
        public RereadSurveyReplicatedDataInt32StringCancellationTokenDelegate RereadSurveyReplicatedDataInt32StringCancellationToken;

        void IReplicationService.RereadSurveyReplicatedData(int surveyId, string reason, CancellationToken cancellationToken)
        {

            if (RereadSurveyReplicatedDataInt32StringCancellationToken != null)
            {
                RereadSurveyReplicatedDataInt32StringCancellationToken(surveyId, reason, cancellationToken);
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).RereadSurveyReplicatedData(surveyId, reason, cancellationToken);
            }
        }

        public delegate int GetNumberOfReplicationRecordsStringInt32Delegate(string projectId, int respid);
        public GetNumberOfReplicationRecordsStringInt32Delegate GetNumberOfReplicationRecordsStringInt32;

        int IReplicationService.GetNumberOfReplicationRecords(string projectId, int respid)
        {


            if (GetNumberOfReplicationRecordsStringInt32 != null)
            {
                return GetNumberOfReplicationRecordsStringInt32(projectId, respid);
            } else if (_inner != null)
            {
                return ((IReplicationService)_inner).GetNumberOfReplicationRecords(projectId, respid);
            }

            return default(int);
        }

        public delegate void ReplicateInterviewDataBvSurveyEntityInt32Delegate(BvSurveyEntity survey, int respondentId);
        public ReplicateInterviewDataBvSurveyEntityInt32Delegate ReplicateInterviewDataBvSurveyEntityInt32;

        void IReplicationService.ReplicateInterviewData(BvSurveyEntity survey, int respondentId)
        {

            if (ReplicateInterviewDataBvSurveyEntityInt32 != null)
            {
                ReplicateInterviewDataBvSurveyEntityInt32(survey, respondentId);
            } else if (_inner != null)
            {
                ((IReplicationService)_inner).ReplicateInterviewData(survey, respondentId);
            }
        }

    }
}