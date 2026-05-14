using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories
{
    public class DialerStateRepository : IDialerStateRepository
    {
        [NotNull]
        public void CreateOrUpdateWithNotificationExpirationDateTime(string serverName, int dialerId, DateTime notificationExpirationTime)
        {
            var entities = BvDialerStateAdapter.GetByCondition(
                "[DialerId] = @DialerId AND\r\n" +
                "[ServerName] = @ServerName\r\n",
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@ServerName", serverName));

            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                entity = new BvDialerStateEntity
                {
                    ServerName = serverName,
                    DialerId = dialerId,
                    LatestDialerNotificationDateTime = new DateTime(1900, 1, 1),
                    // We have to set the default 01.01.1900 values explicitely below, because DAL seems not to to get default values from SQL
                    // and set wrong default values (DateTime.MinValue) in entity constructor 
                    LatestGetStateRequestDateTime = new DateTime(1900, 1, 1),
                    LatestSuccessfulGetStateDateTime = new DateTime(1900, 1, 1),
                    DialerNotificationExpirationTime = notificationExpirationTime
                };

                BvDialerStateAdapter.Insert(entity);
            }
            else
            {
                entity.DialerNotificationExpirationTime = notificationExpirationTime;
                BvDialerStateAdapter.Update(entity);
            }
        }

        [NotNull]
        public BvDialerStateEntity GetByDialerIdAndServerName(string serverName, int dialerId)
        {
            var entities = BvDialerStateAdapter.GetByCondition(
                "[DialerId] = @DialerId AND\r\n" +
                "[ServerName] = @ServerName\r\n",
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@ServerName", serverName));

            var entity = entities.FirstOrDefault();

            if (entity != null)
            {
                return entity;
            }

            entity = new BvDialerStateEntity
            {
                ServerName = serverName,
                DialerId = dialerId,
                // We have to set the default 01.01.1900 values explicitely below, because DAL seems not to to get default values from SQL
                // and set wrong default values (DateTime.MinValue) in entity constructor 
                LatestDialerNotificationDateTime = new DateTime(1900, 1, 1),
                LatestGetStateRequestDateTime = new DateTime(1900, 1, 1),
                LatestSuccessfulGetStateDateTime = new DateTime(1900, 1, 1),
                DialerNotificationExpirationTime = new DateTime(1900, 1, 1)
            };

            BvDialerStateAdapter.Insert(entity);

            Trace.TraceWarning(
                "BvDialerStateRepository.UpdateDialerStateNotificationTime: A record with DialerId=[{0}] and ServerName=[{1}] is not found in BvDialerState table. " +
                "A new record is created.",
                dialerId,
                serverName);

            return entity;
        }

        [NotNull]
        public List<BvDialerStateEntity> GetByDialerId(int dialerId)
        {
            return BvDialerStateAdapter.GetByCondition(
                "[DialerId] = @DialerId",
                new SqlParameter("@DialerId", dialerId));
        }

        public DateTime GetDialerNotificationExpirationTime(int dialerId)
        {
            // No exceptions are expected here. The DialerNotificationExpirationTime is "NOT NULL DEFAULT ('01/01/1900')" in the DB.
            // But if it occurs we must catch and log it
            try
            {
                return GetByDialerId(dialerId).Max(x => x.DialerNotificationExpirationTime);
            }
            catch (Exception)
            {
                // TODO: WHY do we catch at all??? 
                Trace.TraceWarning(
                    "BvDialerStateRepository.GetDialerNotificationExpirationTime: There is no record in BvDialerState for dialer [{0}]. " +
                    "The default notification expiration time (1970, 1, 1) is returned.",
                    dialerId);

                return new DateTime(1970, 1, 1);
            }
        }

        public DateTime GetLastSuccessfulGetStateTimestamp(int dialerId)
        {
            return GetByDialerId(dialerId).Select(x => x.LatestSuccessfulGetStateDateTime).DefaultIfEmpty().Max();
        }
        
        public DateTime GetLastSuccessfulNotificationTimestamp(int dialerId)
        {
            return GetByDialerId(dialerId).Select(x => x.LatestDialerNotificationDateTime).DefaultIfEmpty().Max();
        }
    }
}
