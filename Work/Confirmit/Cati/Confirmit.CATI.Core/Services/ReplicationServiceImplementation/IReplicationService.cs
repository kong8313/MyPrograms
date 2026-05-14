using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public interface IReplicationService
    {
        /// <summary>
        /// Runs the replication. Does not return control till the end of replication
        /// process. It controls the frequency of method calls so some method calls may not
        /// result in replication process if it is called to early after the previous
        /// replication. If there are some other replication
        /// process currently executing - it will immediately return the control.
        /// </summary>
        void RunPeriodicalReplication(CancellationToken cancellationToken);

        /// <summary>
        /// Runs the replication. Does not return control till the end of replication
        /// process. It does not control the frequency of method calls so all method calls
        /// will result in replication process. If there are some other replication
        /// process currently executing - it will wait till the end of it before start.
        /// </summary>
        void RunForceReplication();
        void RunForceReplication(int surveyId, CancellationToken cancellationToken);
        void UploadSampleDataToReplicatedTable(int surveyId, int batchId, CancellationToken cancellationToken);
        void RereadSurveyReplicatedData(int surveyId, string reason, CancellationToken cancellationToken);

        int GetNumberOfReplicationRecords(string projectId, int respid);
        void ReplicateInterviewData(BvSurveyEntity survey, int respondentId);
    }
}