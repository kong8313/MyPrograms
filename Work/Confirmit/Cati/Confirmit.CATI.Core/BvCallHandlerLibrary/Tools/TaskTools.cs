using System;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace BvCallHandlerLibrary.Tools
{
    public class TaskLocker : IDisposable
    {
        private readonly int _lockPersonSid;
        private ExclusiveDatabaseLock _dbLock;

        public TaskLocker(int personSid)
        {
            try
            {
                var bvTask = ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personSid);
                if (bvTask != null)
                {
                    _lockPersonSid = personSid;

                    LockDB();
                }
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }

        private void LockDB()
        {
            string resourceName = DatabaseLockTimeoutsAndRecourceNames.GetTaskLockerResourceName(_lockPersonSid);

            _dbLock = ExclusiveDatabaseLock.CreateLock(
                resourceName,
                "Telephony.TaskLocker",
                ServiceLocator.Resolve<IDatabaseLockTimeouts>().TaskLockTimeoutInMs);

            if (!_dbLock.TryEnterLock())
                throw new TimeoutException(
                    String.Format("Can't get lock. wait timeout expired for resource {0}", resourceName));
        }

        public bool TaskExists
        {
            get
            {
                return _lockPersonSid != 0;
            }
        }

        internal static TaskLocker TryLock(int personSid)
        {
            var locker = new TaskLocker(personSid);

            if (locker.TaskExists)
                return locker;

            locker.Dispose();

            return null;
        }

        internal static TaskLocker TryLock(int personId, out BvTasksEntity task)
        {
            task = null;

            var locker = new TaskLocker(personId);

            if (locker.TaskExists)
            {
                task = TaskRepository.GetByPerson(personId);
                return locker;
            }

            locker.Dispose();
            
            return null;
        }

        public static TaskLocker Lock(BvPersonEntity person, out BvTasksEntity task)
        {
            return Lock(person.SID, out task);
        }

        public static TaskLocker Lock(int personId, out BvTasksEntity task)
        {
            var result = TryLock(personId);
            if (result == null)
                throw new Exception("Person dosn't exist");

            task = TaskRepository.GetByPerson(personId);

            return result;
        }

        public void Dispose()
        {
            if (_dbLock != null)
            {
                _dbLock.Dispose();
            }
        }
    }
}
