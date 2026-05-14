using System;
using System.Threading;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Connection
{
    public class DialerConnectionStateProvider : IDialerConnectionStateProvider
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialerStateRepository _dialerStateRepository;

        public DialerConnectionStateProvider(IDialerSettings dialerSettings, IDialerStateRepository dialerStateRepository)
        {
            _dialerSettings = dialerSettings;
            _dialerStateRepository = dialerStateRepository;
        }

        public DialerConnectionState GetCurrentConnectionStateForBackgroundHealthCheck(string tenantId, int dialerId,
            IDialerAPI dialerApi)
        {
            // slow pooling for backward notification to make less queries on database 
            return GetCurrentConnectionState(tenantId, dialerId, dialerApi,
                notificationPoolingInitialDelay: TimeSpan.FromSeconds(1), notificationPoolingDelayMultiplier: 2);
        }
        
        public DialerConnectionState GetCurrentConnectionStateWhenActivatingDialer(string tenantId, int dialerId,
            IDialerAPI dialerApi)
        {
            // faster pooling for backward notification to get the result sooner
            return GetCurrentConnectionState(tenantId, dialerId, dialerApi,
                notificationPoolingInitialDelay: TimeSpan.FromMilliseconds(150), notificationPoolingDelayMultiplier: 1.2);
        }
        
        private DialerConnectionState GetCurrentConnectionState(string tenantId, int dialerId, IDialerAPI dialerApi, TimeSpan notificationPoolingInitialDelay, double notificationPoolingDelayMultiplier)
        {
            // Algorithm to check if dialer is currently available:
            // - Call GetState
            // - Check that we get 'Available' as a response to this call - this ensures that CATI -> Dialer communication is alive
            // - Wait for backward dialer state notification (it is asynchronously issued in GetState method in dialer webservice)
            //    Notification may arrive on any CATI server, so we have to pool database and wait for notification timestamp to be updated
            //    - this ensures that Dialer -> CATI communication is alive

            var previousNotificationTimestamp = _dialerStateRepository.GetLastSuccessfulNotificationTimestamp(dialerId);

            var getStateResult = CallGetState(tenantId, dialerId, dialerApi);

            if (!getStateResult.IsAlive)
                return getStateResult;

            var waitNotificationTimeout = TimeSpan.FromMilliseconds(_dialerSettings.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs);
            bool backwardNotificationReceived = WaitBackwardNotification(dialerId, previousNotificationTimestamp,
                waitNotificationTimeout, notificationPoolingInitialDelay, notificationPoolingDelayMultiplier);
            
            if (!backwardNotificationReceived)
            {
                return new DialerConnectionState
                {
                    CurrentState = DialerConnectionState.ConnectionState.EventNotificationsDown,
                    Error =
                        $"'Dialer available' notification not received within {waitNotificationTimeout.TotalSeconds} seconds after successful GetState call"
                };
            }
            
            return new DialerConnectionState
            {
                CurrentState = DialerConnectionState.ConnectionState.Alive,
            };
        }

        private bool WaitBackwardNotification(int dialerId, DateTime previousNotificationTimestamp, TimeSpan retryPeriod, TimeSpan notificationPoolingInitialDelay, double notificationPoolingDelayMultiplier)
        {
            return RetryWithProgressiveDelay(
                () => _dialerStateRepository.GetLastSuccessfulNotificationTimestamp(dialerId) > previousNotificationTimestamp,
                retryPeriod, notificationPoolingInitialDelay, notificationPoolingDelayMultiplier);
        }

        private bool RetryWithProgressiveDelay(Func<bool> func, TimeSpan retryPeriod, TimeSpan initialDelay, double delayMultiplier)
        {
            var startTime = DateTime.UtcNow;
            double sleepPeriodMs = initialDelay.TotalMilliseconds;
            
            while (DateTime.UtcNow < startTime + retryPeriod)
            {
                Thread.Sleep((int) sleepPeriodMs);

                sleepPeriodMs *= delayMultiplier;
                
                if (func())
                    return true;
            }

            return false;
        }

        private static DialerConnectionState CallGetState(string tenantId, int dialerId, IDialerAPI dialerApi)
        {
            try
            {
                var state = dialerApi.GetState(dialerId, tenantId);

                if (state == DialerState.Available)
                {
                    return new DialerConnectionState{CurrentState = DialerConnectionState.ConnectionState.Alive};
                }
                else
                {
                    return new DialerConnectionState
                    {
                        CurrentState = DialerConnectionState.ConnectionState.DialerWebserviceDown,
                        Error = $"Dialer state '{state}' received on GetState call. It may indicate problems with internal dialer components."
                    };
                }
            }
            catch (Exception e)
            {
                return new DialerConnectionState
                {
                    CurrentState = DialerConnectionState.ConnectionState.DialerWebserviceDown,
                    Error = $"GetState failed with error: {e}"
                };
            }
        }
    }
}