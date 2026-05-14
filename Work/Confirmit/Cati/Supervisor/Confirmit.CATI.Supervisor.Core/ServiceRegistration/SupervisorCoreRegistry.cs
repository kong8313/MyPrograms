using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.ServiceLocationExtention;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.BlackList;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.CatiSupervisorApi;
using Confirmit.CATI.Supervisor.Core.Messaging;

namespace Confirmit.CATI.Supervisor.Core.ServiceRegistration
{
    public class SupervisorCoreRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator

                //providers

                .Register<ISurveyPermissionProvider, SurveyPermissionProvider>()
                .Register<IConfirmitQuestionsProvider, ConfirmitQuestionsProvider>()
                .Register<IFilterVariablesProvider, FilterVariablesProvider>()
                .RegisterSingletonPerHttpContext<ICachedLocalTimezoneManager, CachedLocalTimezoneManager>()
                .RegisterSingletonPerHttpContext<ISupervisorNameProvider, HttpContextSupervisorNameProvider>()
                .RegisterSingleton<ICallManagementViewsProvider, CallManagementViewsProvider>()
                .RegisterSingletonPerHttpContext<ICompanyInfoProvider, SupervisorCompanyInfoProvider>()

                //misc
                
                .Register<IAssignmentWithEventLoggingPerformer, AssignmentWithEventLoggingPerformer>()
                .Register<IIdentityService, IdentityService>()
                .Register<ISupervisorIdentityProviderService, SupervisorIdentityProviderService>()
                .RegisterSingletonPerHttpContext<ISupervisorHttpContextService, SupervisorHttpContextService>()
                .Register<ISupervisorIdentityService, SupervisorIdentityService>()
                .Register<ICatiSupervisorApiService, CatiSupervisorApiService>()

                //managers

                .Register<IPersonGroupManager, PersonGroupManager>()
                .Register<IAssignmentManager, AssignmentManager>()
                .Register<IQuotaNameProvider, QuotaNameProvider>()
                .Register<IBlackListService, BlackListService>()
                .Register<IQuotaSettingsProvider, QuotaSettingsProvider>()
                .Register<IChangeAutomaticSurveyService, ChangeAutomaticSurveyService>()
                .Register<IPriorityGroupsManager, PriorityGroupsManager>()
                .Register<IFilterManager, FilterManager>()

                .Register<IExtraQuotaCounterService, ExtraQuotaCounterService>()
                .Register<IUsedCallsCalculator, UsedCallsCalculator>()

                .Register<ICatiServerNameProvider, CatiServerNameProvider>()

                .Register<ISendMessageManager, SendMessageManager>()
                .RegisterSingleton<ISqlTableUpdatedPublisher, SqlTableUpdatedPublisher>()
                .Register<IMultimodeInstanceName, MultimodeInstanceName>()
                .Register<ISideBySideManager, SideBySideManager>()
                .Register<IConnectionStrings, ConnectionStrings>();
            
        }
    }
}
