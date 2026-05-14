using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.WcfServices.Clients;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerAvailabilityManager : IDialerAvailabilityManager
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerCollection _dialerCollection;
        private readonly IMnTciTools _mnTciTools;
        private readonly IAuthoringService _authoringService;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IDialerOperationalStateNotificator _dialerOperationalStateNotificator;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        
        public DialerAvailabilityManager(
            IDialersRepository dialersRepository,
            IDatabaseLockTimeouts databaseLockTimeouts,
            IDialerCollection dialerCollection,
            IAuthoringService authoringService,
            IMnTciTools mnTciTools,
            IDialerOperationalStateNotificator dialerOperationalStateNotificator,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _dialersRepository = dialersRepository;
            _dialerCollection = dialerCollection;
            _authoringService = authoringService;
            _mnTciTools = mnTciTools;
            _dialerOperationalStateNotificator = dialerOperationalStateNotificator;
            _databaseLockTimeouts = databaseLockTimeouts;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        public bool IsDialerNotificationStateOperational(int dialerId)
        {
            var dialerEntity = _dialersRepository.GetById(dialerId);
            return dialerEntity.DialerOperationalStateNotification;
        }

        private void InitDialer(int dialerId, [CallerMemberName] string callerName = "")
        {
            if (IsDialerInitialized(dialerId))
            {
                Trace.TraceWarning($"DialerAvailabilityManager.{callerName}(), dialer is already available.");
                return;
            }

            if (!(_mnTciTools.DoesCompanyUseTelephony() && _authoringService.IsCompanyTelephonyEnabled(BackendInstance.Current.CompanyId)))
            {
                Trace.TraceError($"DialerAvailabilityManager.{ callerName}(), cannot enable dialer. System configured not to use dialer.");
                return;
            }

            Trace.TraceInformation($"DialerAvailabilityManager.{callerName}(), calling Initialize dialer library.");

            _dialerCollection.InitializeCollection();

            var dialerInstance = _dialerCollection.GetDialerById(dialerId);
            dialerInstance.Initialize();

            var dialer = _dialersRepository.GetById(dialerId);
            dialer.ExpectedState = (int)DialerStatus.ConnectedAndDeactivated;
            BvDialersAdapter.Update(dialer);

            Trace.TraceInformation($"DialerAvailabilityManager.{callerName}(), calling SendDialerOperationalStateNotification.");

            _dialerOperationalStateNotificator.SendDialerOperationalStateNotification(dialerId, true);
        }

        public void EnableDialer(int dialerId)
        {
            var evt = new EnableDialerEvent(dialerId, true);
            string error = null;
            try
            {
                if (!_mnTciTools.IsDialerConfigured())
                {
                    throw new InvalidOperationException(Strings.CantEnableDialerMessage);
                }

                EnableDialer(dialerId, true);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw;
            }
            finally
            {
                evt.Details.IsSuccessful = error == null;
                evt.Details.ErrorMessage = error;
                evt.Finish();
            }
        }

        /// <summary>
        /// Initializes dialer Library and connects to dialer. This method is synchronous so 
        /// we can be sure that dialer is currently available if this method completed successfully
        /// </summary>
        /// <param name="dialerId"> Identifier of the dialer </param>
        /// <param name="needToSendNotification">We should not send the notification in case of we do the
        /// operation as the result of notification :)</param>
        public void EnableDialer(int dialerId, bool needToSendNotification)
        {
            Trace.TraceWarning(
                "DialerAvailabilityManager.EnableDialer(), dialerId = {0}, needToSendNotification={1}, " +
                "IsDialerOperational={2}",
                dialerId,
                needToSendNotification,
                IsDialerInitialized(dialerId));

            using (var dbLock = DatabaseLockService.CreateLock(
                       DatabaseLockTimeoutsAndRecourceNames.DialerStateOperationLockerResourceName,
                       "DialerAvailabilityManager.EnableDialer",
                       _databaseLockTimeouts.DefaultLockTimeoutInMs,
                       true))
            {
                dbLock.EnterLock();
                
                try
                {
                    InitDialer(dialerId);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("DialerAvailabilityManager.EnableDialer(), Failed to initialize dialer, dialerId = {0}, ex: {1}", dialerId, ex);
                    DisableDialer(dialerId);

                    throw;
                }
            }
        }

        public bool ReconnectDialer(int dialerId)
        {
            var evt = new ReconnectDialerEvent(dialerId);
            var result = true;
            try
            {
                if (!_mnTciTools.IsDialerConfigured())
                {
                    throw new InvalidOperationException(Strings.CantEnableDialerMessage);
                }

                TryReconnectDialer(dialerId);
            }
            catch (Exception ex)
            {
                result = false;
                Trace.TraceError("DialerAvailabilityManager.ReconnectDialer(), Failed to reenable dialer, dialerId = {0}, ex: {1}", dialerId, ex);
            }
            finally
            {
                evt.Details.IsSuccessful = result;
                evt.Finish();
            }

            return result;
        }
        /// <summary>
        /// ReInitializes dialer Library and connects to dialer. This method is synchronous so 
        /// we can be sure that dialer is currently available if this method completed successfully
        /// </summary>
        /// <param name="dialerId"> Identifier of the dialer </param>
        private void TryReconnectDialer(int dialerId)
        {
            Trace.TraceWarning(
                "DialerAvailabilityManager.ReconnectDialer(), dialerId = {0}," +
                "IsDialerOperational={1}",
                dialerId,
                IsDialerInitialized(dialerId));

            using (var dbLock = DatabaseLockService.CreateLock(
                       DatabaseLockTimeoutsAndRecourceNames.DialerStateOperationLockerResourceName,
                       "DialerAvailabilityManager.TryReconnectDialer",
                       _databaseLockTimeouts.DefaultLockTimeoutInMs,
                       true))
            {
                dbLock.EnterLock();

                try
                {
                    InitDialer(dialerId);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("DialerAvailabilityManager.ReconnectDialer(), Failed to reinitialize dialer, dialerId = {0}, ex: {1}", dialerId, ex);
                    DisableDialer(dialerId, true);
                    throw new InternalErrorException("Failed to initialize Dialer", ex);
                }
            }
        }
        public bool StopReconnectingDialer(int dialerId)
        {
            var evt = new StopDialerReconnectionEvent(dialerId);
            var result = true;
            try
            {
                DisableDialer(dialerId, false, false);
            }
            catch (Exception ex)
            {
                result = false;
                Trace.TraceError("DialerAvailabilityManager.StopReconnectingDialer(), Failed to disable dialer, dialerId = {0}, ex: {1}", dialerId, ex);
            }
            finally
            {
                evt.Finish();
            }

            return result;
        }
        public bool DisableDialer(int dialerId, bool withReconnection = false)
        {
            var evt = new DisableDialerEvent(dialerId, true);
            var result = true;
            try
            {
                DisableDialer(dialerId, withReconnection, true);
            }
            catch (Exception ex)
            {
                result = false;
                Trace.TraceError("DialerAvailabilityManager.DisableDialer(), Failed to disable dialer, dialerId = {0}, ex: {1}", dialerId, ex);
            }
            finally
            {
                evt.Details.IsSuccessful = result;
                evt.Finish();
            }

            return result;
        }

        /// <summary>
        /// disables dialer
        /// </summary>
        /// <param name="dialerId"> Identifier of the dialer </param>
        /// <param name="withReconnection">If dialer disabled by system and not by user</param>
        /// <param name="needToSendNotification">We should not send the notification in case of we do the
        /// operation as the result of notification :)</param>
        public void DisableDialer(int dialerId, bool withReconnection, bool needToSendNotification)
        {
            var isDialerAvailable = IsDialerInitialized(dialerId);
            Trace.TraceWarning(
                "DialerAvailabilityManager.DisableDialer(), needToSendNotification={0}, " +
                "IsDialerOperational={1}",
                needToSendNotification,
                isDialerAvailable);

            using (var dbLock = DatabaseLockService.CreateLock(
                       DatabaseLockTimeoutsAndRecourceNames.DialerStateOperationLockerResourceName,
                       "DialerAvailabilityManager.DisableDialer",
                       _databaseLockTimeouts.DefaultLockTimeoutInMs,
                       true))
            {
                dbLock.EnterLock();

                if (needToSendNotification)
                {
                    Trace.TraceWarning("DialerAvailabilityManager.DisableDialer(), calling SiteService.SendDialerOperationalStateNotification");
                    _dialerOperationalStateNotificator.SendDialerOperationalStateNotification(dialerId, false);
                }

                Trace.TraceWarning("DialerAvailabilityManager.DisableDialer(), calling UninitializeDialers");

                _dialerCollection.GetDialerById(dialerId).Uninitialize(true, withReconnection);

                Trace.TraceWarning("DialerAvailabilityManager.DisableDialer(), calling BvSpTasks_SetTelephonyProblemForLoggedIn");

                // TODO: Must set it only for persons logged in to the problem dialer
                BvSpTasks_SetTelephonyProblemForLoggedInAdapter.ExecuteNonQuery(dialerId, (int)DialerErrorCode.NotAvailable);
            }
        }

        private bool IsDialerInitialized(int dialerId)
        {
            return _dialerCollection.IsDialerInitialized(dialerId);
        }

        public bool IsDialerInitializedAndAvaialble(int dialerId)
        {
            return IsDialerInitialized(dialerId) && IsDialerNotificationStateOperational(dialerId);
        }

        public bool IsConnectedToDialer(DialType dialType, int dialerId)
        {
            return (dialerId != 0) ?
                IsDialerInitialized(dialerId) && _dialerCollection.GetDialerById(dialerId).DialType == dialType
                : _dialerCollection.InitializedDialerExists(dialType);
        }

        private bool TryToActivateDialer(int dialerId, bool isActive)
        {
            try
            {
                var dialer = _dialersRepository.GetById(dialerId);
                dialer.IsActive = isActive;
                dialer.ExpectedState = isActive?(int)DialerStatus.ConnectedAndActivated: (int)DialerStatus.ConnectedAndDeactivated;
                BvDialersAdapter.Update(dialer);
                _sqlTableUpdatedPublisher.PublishDialersUpdated();
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "DialerAvailabilityManager.ActivateDialer(), Failed to {0} dialer, dialerId = {1}, isActive = {2}, ex: {3}",
                    isActive ? "activate" : "deactivate", dialerId, isActive, ex);
                return false;
            }

            return true;
        }

        public bool ActivateDialer(int dialerId)
        {
            var evt = new ActivateDialerEvent(dialerId, true);

            var result = TryToActivateDialer(dialerId, true);
            evt.Details.IsSuccessful = result;
            evt.Finish();

            return result;
        }

        public bool DeactivateDialer(int dialerId)
        {
            var evt = new DectivateDialerEvent(dialerId, true);
            var result = TryToActivateDialer(dialerId, false);
            evt.Details.IsSuccessful = result;
            evt.Finish();

            return result;
        }
    }
}
