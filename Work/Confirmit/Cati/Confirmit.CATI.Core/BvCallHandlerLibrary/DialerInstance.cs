using System;
using System.Diagnostics;
using System.Globalization;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.DialerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Connection;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Core.BvCallHandlerLibrary
{
    public class DialerInstance : IDialerInstance
    {
        public IDialerAPI Api { get; private set; }

        public int DialerId { get; set; }

        public string DialerName { get; set; }

        public DialType DialType { get; set; }

        public bool DialerOperationalState { get; set; }
        public string TenantId { get; set; }

        // TODO: WTF Why we have int and string tenant id in the different classes
        public int TenantIdInt { get; set; }

        public bool IsDialerInitialized { get; set; }

        public DialerFeatures SupportedFeatures { get; set; }

        public string Version { get; private set; }

        private readonly IDialerInitializer _dialerInitializer;
        private readonly IDialerOperationalStateNotificator _dialerOperationalStateNotificator;
        private readonly IDialerEmailNotificationService _dialerEmailNotificationService;
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerStateTools _dialerStateTools;
        private readonly ICompanyInfo _companyInfo;
        private readonly IDialerConnectionStateProvider _dialerConnectionStateProvider;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public DialerInstance(
            IDialerStateTools dialerStateTools,
            IDialerInitializer dialerInitializer,
            IDialerOperationalStateNotificator dialerOperationalStateNotificator,
            IDialerEmailNotificationService dialerEmailNotificationService,
            IDialersRepository dialersRepository,
            ICompanyInfo companyInfo,
            IDialerConnectionStateProvider dialerConnectionStateProvider,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _dialerStateTools = dialerStateTools;
            _dialerInitializer = dialerInitializer;
            _dialerOperationalStateNotificator = dialerOperationalStateNotificator;
            _dialerEmailNotificationService = dialerEmailNotificationService;
            _dialersRepository = dialersRepository;
            _companyInfo = companyInfo;
            _dialerConnectionStateProvider = dialerConnectionStateProvider;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
            Api = _dialerInitializer.CreateInstance();
        }

        public void Create()
        {
            Initialize(false);
        }

        public void Initialize()
        {
            Initialize(true);
        }

        private void Initialize(bool sendInitializeToWebService)
        {
            Api = _dialerInitializer.InitializeDialer(
                DialerId,
                Api,
                sendInitializeToWebService,
                out var tenantId,
                out var name,
                out DialType dialType);

            // TODO: TenantId is obsolete. CompanyId should be used instead.
            TenantIdInt = tenantId;
            TenantId = tenantId.ToString(CultureInfo.InvariantCulture);

            DialerName = name;
            DialType = dialType;

            if (sendInitializeToWebService)
            {
                WaitForDialerWsNotification();
            }
            
            // TODO: save in database when initializing a dialer
            Version = GetDialerVersion();
        }



        private string GetDialerVersion()
        {
            var activityEvent = new DialerActivityEvent(DialerId, nameof(Api.GetDialerVersion));
            try
            {
                var version = Api.GetDialerVersion();
                activityEvent.LogInfo(version);
                return version;
            }
            catch (Exception e)
            {
                activityEvent.LogError(e);
                throw;
            }
        }

        private void WaitForDialerWsNotification()
        {
            var connectionState = _dialerConnectionStateProvider.GetCurrentConnectionStateWhenActivatingDialer(TenantId, DialerId, Api);

            if (!connectionState.IsAlive)
            {
                throw new InternalErrorException(
                    string.Format(
                        "Dialer [id={0}, name={1}] is not available. {2} /// {3}", 
                        DialerId, DialerName,
                        connectionState, _dialerStateTools.BvDialerStateToString(DialerId)
                    ));
            }
        }

        public void Uninitialize(bool releaseDialerWs, bool withReconnection = false)
        {
            var activityEvent = new DialerActivityEvent(DialerId, nameof(Api.Release));
            try
            {
                if (!releaseDialerWs)
                {
                    return;
                }
                
                var result = (DialerErrorCode)Api.Release(DialerId, _companyInfo.CompanyId);

                if (result != DialerErrorCode.Success)
                {
                    activityEvent.LogError(result);
                    Trace.TraceError(
                        "DialerInstance.Uninitialize: Api.Release() returned error code. " +
                        "/// errorCode={0}, dialerId={1}.",
                        result,
                        DialerId);
                }
                else
                {
                    activityEvent.LogInfo(result);
                }
            }
            catch (Exception e)
            {
                activityEvent.LogError(e);
                Trace.TraceError(
                    "DialerInstance.Uninitialize failed with exception. " +
                    "/// dialerId={0}, ex={1}.",
                    DialerId,
                    e);
            }
            finally
            {
                Version = null;
                SupportedFeatures = null;
                IsDialerInitialized = false;
                DialerOperationalState = false;
            }

            Deactivate(withReconnection);
        }

        private void Deactivate(bool withReconnection)
        {
            try
            {
                var entity = _dialersRepository.GetById(DialerId);
                if (entity == null)
                    return;

                entity.IsActive = false;

                if(!withReconnection)
                    entity.ExpectedState = (int)DialerStatus.DisconnectedAndDeactivated;

                BvDialersAdapter.Update(entity);
                _sqlTableUpdatedPublisher.PublishDialersUpdated();
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "DialerInstance.Deactivate failed with exception. " +
                    "/// dialerId={0}, ex={1}.",
                    DialerId,
                    e);
            }
        }
        
        public void OnDialerState(DialerState dialerState)
        {
            // Trace warning for everything except of DialerState.Available
            if (dialerState != DialerState.Available)
            {
                Trace.TraceWarning(
                    "DialerInstance.OnDialerState: Dialer[{0}, {1}], state = {2}",
                    DialerName,
                    DialerId,
                    dialerState);
            }

            switch (dialerState)
            {
                case DialerState.DialerServiceStarted:
                    _dialerEmailNotificationService.SendDialerWsStartedEmailNotification(DialerId);
                    break;

                case DialerState.Available:
                    _dialerStateTools.UpdateDialerStateNotificationTime(DialerId);
                    break;

                case DialerState.DialerLoggerProblem:
                    _dialerEmailNotificationService.SendDialerLoggerProblemEmailNotification(DialerId);
                    break;
            }
        }
    }
}
