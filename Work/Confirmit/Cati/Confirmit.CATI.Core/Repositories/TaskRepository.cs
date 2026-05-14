using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;

namespace Confirmit.CATI.Core.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IPersonSessionHistoryRepository _personSessionHistoryRepository;
        private readonly IDatabaseConnectionProviderFactory _databaseConnectionProviderFactory;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;

        public TaskRepository(IPersonSessionHistoryRepository personSessionHistoryRepository, IDatabaseConnectionProviderFactory databaseConnectionProviderFactory, IConnectionStrings connectionStrings, ICompanyInfo companyInfo)
        {
            _personSessionHistoryRepository = personSessionHistoryRepository;
            _databaseConnectionProviderFactory = databaseConnectionProviderFactory;
            _connectionStrings = connectionStrings;
            _companyInfo = companyInfo;
        }

        private const int TerminateTaskLockTimeoutInMilliseconds = 2 * 60 * 1000;

        public BvTasksEntity GetById(int surveySid, int interviewId)
        {
            var entities = BvTasksAdapter.GetByCondition(
                "[SurveySid] = @SurveySid AND" +
                "[interviewId] = @InterviewId",
                new SqlParameter("@SurveySid", surveySid),
                new SqlParameter("@InterviewId", interviewId));

            return entities.FirstOrDefault();
        }

        public BvTasksEntity GetByIdWithCheck(int surveySid, int interviewId)
        {
            var task = GetById(surveySid, interviewId);

            if (task == null)
            {
                throw new InternalErrorException(
                    String.Format("Task for survey with SID '{0}' and interview '{1}' does not exist.",
                    surveySid,
                    interviewId));
            }

            return task;
        }

        BvTasksEntity ITaskRepository.GetByPerson(
            int personSid)
        {
            var entities = BvTasksAdapter.GetByCondition(
                "[PersonSid] = @PersonSid",
                new SqlParameter("@PersonSid", personSid));

            return entities.FirstOrDefault();
        }

        public BvTasksEntity GetByPersonWithCheck(
            int personSid)
        {
            var entities = BvTasksAdapter.GetByCondition(
                "[PersonSid] = @PersonSid",
                new SqlParameter("@PersonSid", personSid));

            var task = entities.FirstOrDefault();

            if (task == null)
            {
                throw new InternalErrorException(
                    String.Format("Task for person with SID '{0}' does not exist.",
                    personSid));
            }

            return task;
        }

        [CanBeNull]
        public BvTasksEntity GetByPersonNotLocked(int personSid)
        {
            using (var connection = new SqlConnection(_connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId)))
            {
                connection.Open();

                var command = new SqlCommand($@"
                    select * from BvTasks
                        left join BvAppLocks al on al.ResourceName = concat('{DatabaseLockTimeoutsAndRecourceNames.TaskLockerResourceNamePrefix}', PersonSID)
                    where PersonSid = @personSid and (al.IsLockHeld is null or al.IsLockHeld = 0)
                ", connection);
                command.Parameters.AddWithValue("@personSid", personSid);
                
                var reader = command.ExecuteReader();
                return BvTasksAdapter.Read(reader);
            }
        }

        public static IEnumerable<BvTasksEntity> GetBySurvey(int surveySid)
        {
            return BvTasksAdapter.GetByCondition(
                "[SurveySid] = @SurveySid",
                new SqlParameter("@SurveySid", surveySid));
        }

        public IEnumerable<BvTasksEntity> GetBySurveyNotLocked(int surveySid)
        {
            using (var connection = new SqlConnection(_connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId)))
            {
                connection.Open();

                var command = new SqlCommand($@"
                    select * from BvTasks
                        left join BvAppLocks al on al.ResourceName = concat('{DatabaseLockTimeoutsAndRecourceNames.TaskLockerResourceNamePrefix}', PersonSID)
                    where SurveySID = @surveySid and (al.IsLockHeld is null or al.IsLockHeld = 0)
                ", connection);
                command.Parameters.AddWithValue("@surveySid", surveySid);
                
                var reader = command.ExecuteReader();
                return BvTasksAdapter.ReadList(reader);
            }
        }
        
        public static IEnumerable<BvTasksEntity> GetByDialerId(
            int dialerId)
        {
            return BvTasksAdapter.GetByCondition(
                "[DialerId] = @DialerId",
                new SqlParameter("@DialerId", dialerId));
        }

        public void Insert(BvTasksEntity task)
        {
            var transactionOptions = new DatabaseTransactionOptions("Task.Insert");
            using (var transaction = new DatabaseTransactionScope(transactionOptions))
            {
                // create another direct connection to remote ConfirmlogDatabase with independent transaction
                // to prevent using linked server and distributed transaction and control it manually
                using (var remoteConnectionProvider = _databaseConnectionProviderFactory.CreateConnectionProviderForConfirmlogDatabase())
                using (var remoteConnectionTransaction = remoteConnectionProvider.BeginTransaction(transactionOptions.Name))
                {
                    task.SessionId = _personSessionHistoryRepository.InsertStartSessionEvent(remoteConnectionProvider, task.CallCenterID, task.PersonSID);

                    EventDetailsScope.Current.AddTiming("TaskRepository.InsertStartSessionEvent");

                    BvTasksAdapter.Insert(task);

                    EventDetailsScope.Current.AddTiming("TaskRepository:BvTasksAdapter.Insert");

                    transaction.Commit();

                    EventDetailsScope.Current.AddTiming("TaskRepository:transaction.Commit");

                    remoteConnectionTransaction.Commit();
                }
            }
        }

        void ITaskRepository.Update(BvTasksEntity task)
        {
            if (task.PersonSID == 0)
            {
                throw ExceptionManager.NewArgumentException("PersonSID");
            }

            BvTasksAdapter.Update(task);
        }

        BvTasksEntity ITaskRepository.DeleteByPerson(int personSid)
        {
            if (personSid == 0)
            {
                ExceptionManager.NewArgumentException("personSid");
            }

            var transactionOptions = new DatabaseTransactionOptions("Task.DeleteByPerson");
            using (var transaction = new DatabaseTransactionScope(transactionOptions))
            {
                // create another direct connection to remote ConfirmlogDatabase with independent transaction
                // to prevent using linked server and distributed transaction and control it manually
                using (var remoteConnectionProvider = _databaseConnectionProviderFactory.CreateConnectionProviderForConfirmlogDatabase())
                using (var remoteConnectionTransaction = remoteConnectionProvider.BeginTransaction(transactionOptions.Name))
                {
                    var deletedTasks = BvTasksAdapter.DeleteByConditionAndOutput(
                        "[PersonSID] = @PersonSid",
                        new SqlParameter("@PersonSid", personSid));

                    var task = deletedTasks.FirstOrDefault();

                    if (task != null)
                    {
                        _personSessionHistoryRepository.InsertStopSessionEvent(remoteConnectionProvider, task.SessionId);
                    }

                    transaction.Commit();

                    remoteConnectionTransaction.Commit();

                    return task;
                }
            }
        }

        /// <summary>
        /// Adds new or updating existing task.
        /// </summary>
        /// <param name="task">The task entity to insert / update.</param>
        public void Merge([NotNull] BvTasksEntity task)
        {
            // TODO: use MERGE statement.
            var exists = GetByPerson(task.PersonSID);

            EventDetailsScope.Current.AddTiming("TaskRepository:Merge:GetByPerson");

            if (exists == null)
            {
                Insert(task);

                EventDetailsScope.Current.AddTiming("TaskRepository:Merge:Insert");
            }
            else
            {
                task.SessionId = exists.SessionId;
                Update(task);

                EventDetailsScope.Current.AddTiming("TaskRepository:Merge:Update");
            }
        }

        public IEnumerable<int> GetPersonIdsFromBBCC()
        {
            return BvTasksAdapter.GetByCondition(
                "[IsWebConsole] = 1").Select(t => t.PersonSID);
        }

        public static BvTasksEntity DeleteByPerson(int personSid)
        {
            return ServiceLocator.Resolve<ITaskRepository>().DeleteByPerson(personSid);
        }

        public static BvTasksEntity GetByPerson(int personSid)
        {
            return ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personSid);
        }

        public static void Update(BvTasksEntity task)
        {
            ServiceLocator.Resolve<ITaskRepository>().Update(task);
        }

        public async Task UpdateActiveQuestion(string projectId, int catiInterviewerId, string questionId, DateTime showTime)
        {
            using (var connection =
                new SqlConnection(_connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId)))
            {
                connection.Open();
                var command = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "BvSpTask_UpdateActiveQuestion",
                    Connection = connection
                };
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@catiInterviewerId", catiInterviewerId);
                command.Parameters.AddWithValue("@qID", questionId);
                command.Parameters.AddWithValue("@showTime", showTime);
                await command.ExecuteNonQueryAsync(CancellationToken.None);
            }
        }
    }
}
