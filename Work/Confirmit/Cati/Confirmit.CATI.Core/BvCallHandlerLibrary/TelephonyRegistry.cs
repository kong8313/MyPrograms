using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.DeleteCalls;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Connection;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.BvCallHandlerLibrary
{
    public class TelephonyRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator

                .RegisterSingleton<IDialerCollection, DialerCollection>()

                .Register<IDialerType, DialerType>()
                .Register<IDialerAvailabilityManager, DialerAvailabilityManager>()
                .Register<IDialerInitializer, DialerInitializer>()
                .Register<IDialerInstanceFactory, DialerInstanceFactory>()
                .Register<IDialerInstance, DialerInstance>()
                .Register<IDialerLoginLogoutManager, DialerLoginLogoutManager>()
                .Register<IDialerEmailNotificationService, DialerEmailNotificationService>()
                .Register<IDialerOperation, DialerOperation>()
                .Register<IDialerRecordingWrapper, DialerRecordingWrapper>()
                .Register<IDialerStateRepository, DialerStateRepository>()
                .Register<IDialerSurveyParametersManager, DialerSurveyParametersManager>()
                .Register<IDialerStateTools, DialerStateTools>()
                .Register<IDialerHealthController, DialerHealthController>()
                .Register<IDialerEventsHandler, DialerEventsHandler>()
                .Register<IDialerOperationalStateNotificator, DialerOperationalStateNotificator>()
                .Register<IDialerCampaignInitializer, DialerCampaignInitializer>()
                .Register<IInboundAudioMessages, InboundAudioMessages>()
                .Register<IDialerFacilities, DialerFacilities>()
                .Register<IDialerService, DialerService>()
                .Register<IDialerConnectionStateProvider, DialerConnectionStateProvider>()

                .Register<IMnTciTools, MnTciTools>()
                .Register<IProblemStateSetter, ProblemStateSetter>()
                .Register<IBvCallHandlerRoot, BvCallHandlerRoot>()

                .Register<ITelephony, TelephonyProvider>()

                .Register<IConsoleLoginProcessor, ConsoleLoginProcessor>()
                .Register<IConsoleLoginToDialerProcessor, ConsoleLoginToDialerProcessor>()
                .Register<IConsoleStartInterviewProcessor, ConsoleStartInterviewProcessor>()
                .Register<IConsoleWrapUpProcessor, ConsoleWrapUpProcessor>()
                .Register<IConsoleTransferStartProcessor, ConsoleTransferStartProcessor>()
                .Register<IConsoleTransferCancelProcessor, ConsoleTransferCancelProcessor>()
                .Register<IConsoleTransferCompleteProcessor, ConsoleTransferCompleteProcessor>()
                .Register<IConsoleTransferSetConnectionStateProcessor, ConsoleTransferSetConnectionStateProcessor>()
                .Register<IConsoleStateProvider, ConsoleStateProvider>()
                .Register<IConsoleTransferProcessProcessor, ConsoleTransferProcessProcessor>()
                .Register<IConsoleDialProcessor, ConsoleDialProcessor>()

                //services
                .Register<ITransferService, TransferService>()

                // TODO: Move these classes to some other registries
                .Register<IPersonService, PersonService>()
                .Register<ISupervisorNotificationService, SupervisorNotificationService>()
                .Register<IQuotaBalancingConfigurationValidator, QuotaBalancingConfigurationValidator>()
                .Register<IEmailNotificationService, EmailNotificationService>()
                .Register<ITaskExtension, TaskExtension>();
        }
    }
}
