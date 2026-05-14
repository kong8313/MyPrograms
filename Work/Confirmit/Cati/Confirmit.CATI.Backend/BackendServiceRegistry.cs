using System.Web.Http.Dispatcher;
using BvDotNetEngine;
using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers;
using Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Authorization;
using Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Backend
{
    public class BackendServiceRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            // Classes used ONLY inside backend process

            serviceRegistrator

            // WebAPI
            .Register<IExceptionLogger, ExceptionLogger>()
            .Register<IRequestInfo, RequestInfo>()
            .Register<IAuthorizer, Authorizer>()
            .Register<IDatabaseContextFactory, DatabaseContextFactory>()
            .Register<IHttpControllerActivator, HttpControllerActivator>()
            .Register<IAssignmentManager, AssignmentManager>()
            .Register<ICallCenterProvider, CallCenterProvider>()
            .Register<ICallHistoryDataProvider, CallHistoryDataProvider>()
            .Register<IQueryableRestService, QueryableRestService>()
            .Register<IRestApiMonitorLogger, RestApiMonitorLogger>()
            .Register<IRestApiMonitorHandler, RestApiMonitorHandler>()
            .Register<IAuthorizer, Authorizer>()
            .Register<IAuthorizationKeyProvider, AuthorizationKeyProvider>()
            .Register<IRestApiMonitorInfoKeeper, RestApiMonitorInfoKeeper>()

            // Threads
            .RegisterSingleton<AutoLogoutThread, AutoLogoutThread>()
            .RegisterSingleton<AutoLogoutWebConsoleThread, AutoLogoutWebConsoleThread>()
            .RegisterSingleton<BulkCopyThread, BulkCopyThread>()
            .RegisterSingleton<EmailReportsThread, EmailReportsThread>()
            .RegisterSingleton<DialerHealthControlThread, DialerHealthControlThread>()
            .RegisterSingleton<ExpiredCallsThread, ExpiredCallsThread>()
            .RegisterSingleton<InstanceMonitoringThread, InstanceMonitoringThread>()
            .RegisterSingleton<ScheduleErrorsNotificationThread, ScheduleErrorsNotificationThread>()
            .RegisterSingleton<ReplicationThread, ReplicationThread>()
            .RegisterSingleton<RoutineMaintenanceThread, RoutineMaintenanceThread>()
            .RegisterSingleton<ScheduleThread, ScheduleThread>()
            .RegisterSingleton<IIvrThread, IvrThread>()

            // WS related
            .Register<IConsoleStateWsRequestsAuthoriser, ConsoleStateWsRequestsAuthoriser>()
            .Register<IConsoleWsRequestsAuthoriser, ConsoleWsRequestsAuthoriser>()
            .Register<IAuthorizationMessageHeaderReader, AuthorizationMessageHeaderReader>()
            .Register<IMessageHeaderAccessor, MessageHeaderAccessor>()

            // Misc
            .Register<IConsoleVersionValidator, ConsoleVersionValidator>()
            .Register<IInterviewHistoryAndDataProcessor, InterviewHistoryAndDataProcessor>()
            .Register<IInterviewTimings, InterviewTimings>()
            .Register<ISystemSettingRepository, SystemSettingRepository>()

            .Register<ICatiSymmetricEncryptor, CatiSymmetricEncryptor>()
            .Register<IPersonImportService, PersonImportService>()
            .Register<IConsoleServiceHelper, ConsoleServiceHelper>()
            .Register<IScriptAssembly, ScriptAssembly>()

            .Register<IAudioRecordsManager, AudioRecordsManager>()
            .Register<DatabaseCreator>();
        }
    }
}