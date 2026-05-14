using System;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace BvCallHandlerLibrary
{
    public class DialerStateTools : IDialerStateTools
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly IProcessAndEnvironmentInfo _processAndEnvironmentInfo;
        private readonly IDialerStateRepository _dialerStateRepository;

        public DialerStateTools(
            IDialerStateRepository dialerStateRepository,
            IDialerSettings dialerSettings,
            IProcessAndEnvironmentInfo processAndEnvironmentInfo)
        {
            _dialerSettings = dialerSettings;
            _processAndEnvironmentInfo = processAndEnvironmentInfo;
            _dialerStateRepository = dialerStateRepository;
        }

        public bool IsGetStateTimeoutElapsed(int dialerId, out DateTime lastSuccessfulGetState)
        {
            lastSuccessfulGetState = _dialerStateRepository.GetLastSuccessfulGetStateTimestamp(dialerId);

            return DateTime.UtcNow - lastSuccessfulGetState > 
                   TimeSpan.FromMilliseconds(_dialerSettings.HealthControlUnavailableTimeoutInMs);
        }
        public bool IsReconnectTimeoutElapsed(BvDialersEntity dialer)
        {
            var timestamp = _dialerStateRepository.GetLastSuccessfulGetStateTimestamp(dialer.Id);

            return dialer.ReconnectionDuration == null || DateTime.UtcNow - timestamp > TimeSpan.FromMilliseconds(dialer.ReconnectionDuration.Value);
        }

        public void SetDialerNotificationExpirationTime(int dialerId, DateTime expirationTime)
        {
            _dialerStateRepository.CreateOrUpdateWithNotificationExpirationDateTime(
                _processAndEnvironmentInfo.MachineName, dialerId, expirationTime);
        }

        public void UpdateDialerStateNotificationTime(int dialerId)
        {
            var utcNow = DateTime.UtcNow;
            DateTime? expirationTimeToSet = utcNow.AddMilliseconds(_dialerSettings.HealthControlUnavailableTimeoutInMs);

            var entity = _dialerStateRepository.GetByDialerIdAndServerName(_processAndEnvironmentInfo.MachineName, dialerId);

            entity.LatestDialerNotificationDateTime = utcNow;

            // Update DialerNotificationExpirationTime only if it currently contains time that is earlier than 'Now + DialerUnavailableDetectionTimeout'.
            // This allows not to take into account the dialer notifications until all CATI services are started.
            // See also: usings of SetDialerNotificationExpirationTime method of this class.
            if (entity.DialerNotificationExpirationTime.CompareTo(expirationTimeToSet) > 0)
            {
                expirationTimeToSet = null;
            }

            BvSpDialerState_UpdateNotificationAndExpirationTimeAdapter.ExecuteNonQuery(
                _processAndEnvironmentInfo.MachineName, dialerId, DateTime.UtcNow, expirationTimeToSet);
        }

        public void UpdateGetStateTime(int dialerId, bool isGetStateSuccessful)
        {
            try
            {
                var latestGetStateRequestDateTime = DateTime.UtcNow;

                BvSpDialerState_InsertUpdateGetStateTimeAdapter.ExecuteNonQuery(
                    ServiceLocator.Resolve<IProcessAndEnvironmentInfo>().MachineName, // It's CATI server name
                    dialerId,
                    latestGetStateRequestDateTime,
                    isGetStateSuccessful);
            }
            catch (Exception ex)
            {
                // WTF we again catch here???
                TraceHelper.TraceException(ex, "DialerStateTools.UpdateGetStateTime is failed.");
            }
        }

        public string BvDialerStateToString(int dialerId)
        {
            try
            {
                var bvDialerStateEntities = _dialerStateRepository.GetByDialerId(dialerId);

                var resultString = bvDialerStateEntities.Select(BvDialerStateEntityToString).
                    Aggregate(new StringBuilder("( "), (result, entityString) => result.Append(entityString).Append(' '));

                resultString.Append(')');

                return resultString.ToString();
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, "DialerStateTools.BvDialerStateToString is failed.");
                return "Dialer state info is not available because of [" + ex.Message + "] exception";
            }
        }

        private string BvDialerStateEntityToString(BvDialerStateEntity entity)
        {
            return string.Format(
                "[ServerName: {0}, LatestDialerNotificationDateTime: {1}, LatestGetStateRequestDateTime: {2}, LatestSuccessfulGetStateDateTime: {3}, DialerNotificationExpirationTime: {4}]",
                entity.ServerName,
                entity.LatestDialerNotificationDateTime,
                entity.LatestGetStateRequestDateTime,
                entity.LatestSuccessfulGetStateDateTime,
                entity.DialerNotificationExpirationTime);
        }
    }
}
