using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ServiceLocationExtention;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Classes.Quotas;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;
using Confirmit.CATI.Supervisor.Core.ConfigurationsApi;
using Confirmit.CATI.Supervisor.Core.News;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.UsersApi;
using Confirmit.CATI.Supervisor.Reports;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Supervisor.Surveys;
using Confirmit.ProjectAuthorization;
using DialerCommon;

namespace Confirmit.CATI.Supervisor.ServiceRegistration
{
    public class SupervisorRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<ISystemSettings, SystemSettings>()
                .Register<IProjectPermissionChecker, ProjectAuthorizationManager>()
                .Register<IScheduledEmailReportsRepository, ScheduledEmailReportsRepository>()
                .Register<IActivityManager, ActivityManager>()
                .RegisterSingletonPerHttpContext<ISystemSettingCache, SystemSettingCache>()
                .Register<IQuotaCounterPercentageCssSelector, SimpleQuotaCounterPercentageCssSelector>()
                .Register<IAddHasAudioColumnToCallList, AddHasAudioColumnToCallList>()
                .Register<ICallOperationsProvider, CallOperationsProvider>()
                .Register<ISetDialType, SetDialType>()
                .Register<IDiallingModeNameProvider, DialingModeNameProvider>()
                .Register<ISupervisorSettingsRepository, SupervisorSettingsRepository>()
                .Register<IIpFilterCache, IpFilterCache>()
                .Register<ISupervisorServiceClient, SupervisorServiceClient>()
                .Register<INewsApiService, NewsApiService>()
                .Register<IUsersApiService, UsersApiService>()
                .Register<IConfigurationApiService, ConfigurationApiService>()
                .Register<IAccessTokenService, AccessTokenService>()
                .Register<IInboundHandlerOperationsProvider, InboundHandlerOperationsProvider>()
                .Register<IFileToBrowserSender, FileToBrowserSender>()
                .Register<IScheduleManager, ScheduleManager>()
                .Register<ILocalTimeProvider, LocalTimeProvider>()
                .Register<IDialerStatusProvider, DialerStatusProvider>()
                .Register<IDialerAuthorizationKeyEncryptor, DialerAuthorizationKeyEncryptor>()
                

                .RegisterSingleton<ICustomizableColumnsService, SurveyActivityViewCustomizableColumnsService>(CustomizableViews.SurveyActivityView);
        }
    }
}