using System.Collections.Generic;
using System.Threading;

namespace Confirmit.CATI.Core.Services
{
    public class ShiftServiceFactory : IShiftServiceFactory
    {
        readonly Dictionary<int, ShiftService> ScheduleIds2ShiftServices = new Dictionary<int, ShiftService>();
        readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// get shift service object by scheduleId
        /// </summary>
        /// <param name="scheduleId">id of scheduling object</param>
        /// <returns>ShiftService object</returns>
        public IShiftService Get(int scheduleId)
        {
            //
            // Use dooble check approach to avoid useless Write or UpgradeableRead lock
            //
            CacheLock.EnterReadLock();
            try
            {
                // if object was loaded to cache then return it
                ShiftService result;

                if (ScheduleIds2ShiftServices.TryGetValue(scheduleId, out result))
                    return result;
            }
            finally
            {
                CacheLock.ExitReadLock();
            }

            // Object was't load to cahce. try load survey to cahce
            CacheLock.EnterUpgradeableReadLock();
            try
            {
                //
                // Check object on exists, because it can be already loaded to cache by second thread
                //
                ShiftService result;

                if (ScheduleIds2ShiftServices.TryGetValue(scheduleId, out result))
                    return result;

                result = new ShiftService(scheduleId);

                CacheLock.EnterWriteLock();
                try
                {
                    ScheduleIds2ShiftServices.Add(scheduleId, result);
                    return result;
                }
                finally
                {
                    CacheLock.ExitWriteLock();
                }

            }
            finally
            {
                CacheLock.ExitUpgradeableReadLock();
            }
        }

        public void DropScheduleCache()
        {
            CacheLock.EnterWriteLock();

            try
            {
                ScheduleIds2ShiftServices.Clear();
            }
            finally
            {
                CacheLock.ExitWriteLock();
            }
        }
    }
}