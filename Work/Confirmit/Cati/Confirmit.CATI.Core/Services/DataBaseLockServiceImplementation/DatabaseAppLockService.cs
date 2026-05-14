using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class DatabaseAppLockService : IDatabaseAppLockService
    {
        private readonly IProcessAndEnvironmentInfo _processAndEnvironmentInfo;

        public DatabaseAppLockService(IProcessAndEnvironmentInfo processAndEnvironmentInfo)
        {
            _processAndEnvironmentInfo = processAndEnvironmentInfo;
        }

        public int GetExclusiveLock(
            string resourceName,
            string lockMode,
            int lockTimeout,
            int waitPeriod,
            string resourceOwner,
            int commandExecutionTimeout)
        {
            int result;

            BvSpGetAppLockAdapter.ExecuteNonQuery(
                resourceName,
                "Exclusive",
                lockTimeout,
                _processAndEnvironmentInfo.MachineName,
                waitPeriod,
                resourceOwner,
                commandExecutionTimeout,
                out result);

            return result;
        }

        /// <summary>
        /// Releases app lock previously taken by GetExclusiveLock()
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="succesfull"></param>
        /// <param name="deleteFromBvAppLock"></param>
        /// <returns></returns>
        public int ReleaseLock(string resourceName, bool succesfull, bool deleteFromBvAppLock)
        {
            int result;
            BvSpReleaseAppLockAdapter.ExecuteNonQuery(resourceName, succesfull, deleteFromBvAppLock, out result);
            return result;
        }

        public BvAppLocksEntity WhoLocked(string resourceName)
        {
            BvAppLocksEntity locksEntity = BvAppLocksAdapter.GetByCondition(
                "ResourceName = @ResourceName\r\n",
                new SqlParameter("@ResourceName", resourceName)).FirstOrDefault();

            return locksEntity;
        }
    }
}