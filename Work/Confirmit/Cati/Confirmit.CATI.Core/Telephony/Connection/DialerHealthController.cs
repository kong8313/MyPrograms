using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Connection
{
    public class DialerHealthController : IDialerHealthController
    {
        private readonly IDialerStateTools _dialerStateTools;
        private readonly IDialerEmailNotificationService _dialerEmailNotificationService;
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialersRepository _dialersRepository;
        private readonly IAsyncManager _asyncManager;
        private readonly IDialerAvailabilityManager _dialerAvailabilityManager;
        private readonly IDialerCollection _dialerCollection;
        private readonly IDialerConnectionStateProvider _dialerConnectionStateProvider;
        private readonly IDialerCampaignInitializer _dialerCampaignInitializer;

        public DialerHealthController(
            IDialerStateTools dialerStateTools,
            IDialerEmailNotificationService dialerEmailNotificationService,
            IDialerSettings dialerSettings,
            IDialersRepository dialersRepository,
            IAsyncManager asyncManager,
            IDialerCollection dialerCollection,
            IDialerConnectionStateProvider dialerConnectionStateProvider,
            IDialerAvailabilityManager dialerAvailabilityManager,
            IDialerCampaignInitializer dialerCampaignInitializer
            )
        {
            _dialerStateTools = dialerStateTools;
            _dialerEmailNotificationService = dialerEmailNotificationService;
            _dialerSettings = dialerSettings;
            _dialersRepository = dialersRepository;
            _asyncManager = asyncManager;
            _dialerCollection = dialerCollection;
            _dialerConnectionStateProvider = dialerConnectionStateProvider;
            _dialerAvailabilityManager = dialerAvailabilityManager;
            _dialerCampaignInitializer = dialerCampaignInitializer;
        }

        public void CheckDialersHealth(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_dialerSettings.Dialer == DiallerType.NoDialler)
                return;

            var evt = new DialerHealthControlThreadEvent();

            var dialers = _dialersRepository.GetAll();

            foreach (var dialer in dialers)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                try
                {
                    switch (GetDialerStatus(dialer))
                    {
                        case DialerStatus.ConnectedAndActivated:
                        case DialerStatus.ConnectedAndDeactivated:
                            CheckDialerHealth(dialer);
                            break;
                        case DialerStatus.DisconnectedTryingToConnect:
                        case DialerStatus.DisconnectedTryingToConnectAndActivate:
                            ReconnectDialer(dialer);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    TraceHelper.TraceException(e,
                        $"Dialer [id={dialer.Id}, name={dialer.Name}]: health check failed");
                }
            }

            evt.Finish();
        }

        public DialerStatus GetDialerStatus(BvDialersEntity dialer)
        {
            var status = !dialer.DialerOperationalStateNotification
                ? DialerStatus.DisconnectedAndDeactivated
                : dialer.IsActive
                    ? DialerStatus.ConnectedAndActivated
                    : DialerStatus.ConnectedAndDeactivated;

            // Dialer already connected - no need to reconnect
            if (status != DialerStatus.DisconnectedAndDeactivated)
                return status;
            
            // Automatic reconnection disabled for the dialer
            if (dialer.ReconnectionDuration == null)
                return status;

            switch ((DialerStatus)dialer.ExpectedState)
            {
                case DialerStatus.ConnectedAndDeactivated:
                    status = DialerStatus.DisconnectedTryingToConnect;
                    break;
                case DialerStatus.ConnectedAndActivated:
                    status = DialerStatus.DisconnectedTryingToConnectAndActivate;
                    break;
            }

            return status;
        }

        private void CheckDialerHealth(BvDialersEntity dialer)
        {
            var dialerApi = _dialerCollection.GetDialerById(dialer.Id).Api;
            var tenantId = dialer.TenantId.ToString(CultureInfo.InvariantCulture);

            var connectionState = _dialerConnectionStateProvider.GetCurrentConnectionStateForBackgroundHealthCheck(tenantId, dialer.Id, dialerApi);

            _dialerStateTools.UpdateGetStateTime(dialer.Id, connectionState.IsAlive);

            if (connectionState.IsAlive)
            {
                CheckTrunkLineStatesAndAlarms(tenantId, dialer.Id, dialer.Name, dialerApi);
            }
            else
            {
                if (ShouldDisconnect(dialer.Id, out var lastSuccessfulGetState))
                {
                    var unavailableMinutes = (int)TimeSpan
                        .FromMilliseconds(_dialerSettings.HealthControlUnavailableTimeoutInMs).TotalMinutes;

                    Trace.TraceError(
                        $"Dialer [id={dialer.Id}, name={dialer.Name}] connection has been lost and cannot be established within"
                        + $" {unavailableMinutes} minutes. Last successful get state: {lastSuccessfulGetState:u}. Dialer will be disconnected. Last error: {connectionState} ");

                    DisconnectDialer(dialer);
                }
                else
                {
                    Trace.TraceWarning(
                        $"Dialer [id={dialer.Id}, name={dialer.Name}] connection has been lost. Error: {connectionState} ");
                }
            }
        }

        private void ReconnectDialer(BvDialersEntity dialer)
        {
            if (_dialerStateTools.IsReconnectTimeoutElapsed(dialer))
            {
                Trace.TraceWarning($"Dialer [id={dialer.Id}, name={dialer.Name}, expectedState={(DialerStatus)dialer.ExpectedState}, reconnectionDuration={dialer.ReconnectionDuration}, isConnected={dialer.DialerOperationalStateNotification}, isActive={dialer.IsActive}] reconnection timeout elapsed. Dialer will be disabled.");
                StopReconnection(dialer.Id);
                return;
            }

            if (!_dialerAvailabilityManager.IsDialerNotificationStateOperational(dialer.Id) && _dialerAvailabilityManager.ReconnectDialer(dialer.Id))
            {
                _dialerCampaignInitializer.InitializeAllCampaigns();

                _asyncManager.QueueWorkItem(() =>
                {
                    _dialerEmailNotificationService.SendDialerAutoReconnectionEmailNotification(dialer.Id);
                });
            }

            if (dialer.ExpectedState == (int)DialerStatus.ConnectedAndActivated && _dialerAvailabilityManager.IsDialerNotificationStateOperational(dialer.Id))
            {
                _dialerAvailabilityManager.ActivateDialer(dialer.Id);
            };
        }

        private void DisconnectDialer(BvDialersEntity dialer)
        {
            var withReconnection = dialer.ReconnectionDuration != null;

            _asyncManager.QueueWorkItem(() =>
            {
                _dialerEmailNotificationService.SendDialerUnavailableEmailNotification(dialer.Id, withReconnection);
            });

            _asyncManager.QueueWorkItem(() =>
            {
                _dialerAvailabilityManager.DisableDialer(dialer.Id, withReconnection);
            });
        }

        private bool ShouldDisconnect(int dialerId, out DateTime lastSuccessfulGetState)
        {
            return _dialerStateTools.IsGetStateTimeoutElapsed(dialerId, out lastSuccessfulGetState);
        }

        private void StopReconnection(int dialerId)
        {
            _asyncManager.QueueWorkItem(() =>
            {
                _dialerEmailNotificationService.SendDialerStopReconnectingEmailNotification(dialerId);
            });

            _asyncManager.QueueWorkItem(() =>
            {
                _dialerAvailabilityManager.StopReconnectingDialer(dialerId);
            });
        }

        private void CheckTrunkLineStatesAndAlarms(string tenantId, int dialerId, string dialerName, IDialerAPI dialerApi)
        {
            try
            {
                var dialerErrorCode = (DialerErrorCode)dialerApi.GetTrunkLineStatesAndAlarms(tenantId, dialerId, out var statesAndAlarms);

                if (dialerErrorCode != DialerErrorCode.Success)
                {
                    Trace.TraceError(
                        "DialerInstance.CheckTrunkLineStatesAndAlarms: _dialerApi.GetTrunkLineStatesAndAlarms is failed with error code: {0}" +
                        " /// Dialer[id={1}, name={2}], _tenantId={3}",
                        dialerErrorCode, dialerId, dialerName, tenantId);

                    return;
                }

                var message = new StringBuilder();

                foreach (var stateAndAlarms in statesAndAlarms)
                {
                    if (stateAndAlarms.AlarmsList != null)
                    {
                        foreach (var alarm in stateAndAlarms.AlarmsList)
                        {
                            message.AppendLine(
                                $"Trunk Line Name: '{stateAndAlarms.LineName}'. " +
                                $"State is {((alarm.State == TrunkLineState.Up) ? "UP" : "DOWN")}." +
                                $" Time: {alarm.Time} (UTC).");
                        }
                    }
                }

                if (message.Length > 0)
                {
                    _dialerEmailNotificationService.SendDialerTrunkLinesAlarmsEmailNotification(dialerId, message.ToString());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "DialerInstance.CheckTrunkLineStatesAndAlarms: {0}" +
                    " /// Dialer[id={1}, name={2}], _tenantId={3}",
                    e, dialerId, dialerName, tenantId);
            }
        }
    }
}