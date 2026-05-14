using System.ServiceModel;
using Confirmit.CATI.Common.ConsoleService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    /// <summary>
    /// WCF service for CATI console. Contains methods that does not require SSL encryption.
    /// </summary>
    [ErrorContextHandler(WebServiceType.External)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ConsoleStateService : IConsoleStateService
    {
        private readonly IConsoleStateWsRequestsAuthoriser _consoleStateWsRequestsAuthoriser;
        private readonly IMonitoringService _monitoringService;
        private readonly IConsoleStateProvider _consoleStateProvider;
        private readonly IConsoleTransferProcessProcessor _consoleTransferProcessProcessor;
        private readonly IAsyncManager _asyncManager;

        public ConsoleStateService()
        {
            _consoleStateWsRequestsAuthoriser = ServiceLocator.Resolve<IConsoleStateWsRequestsAuthoriser>();
            _monitoringService = ServiceLocator.Resolve<IMonitoringService>();
            _consoleStateProvider = ServiceLocator.Resolve<IConsoleStateProvider>();
            _consoleTransferProcessProcessor = ServiceLocator.Resolve<IConsoleTransferProcessProcessor>();
            _asyncManager = ServiceLocator.Resolve<IAsyncManager>();
        }

        /// <summary>
        /// It is the request for the current interview and interviewer state.
        /// </summary>
        /// <remarks>
        /// CATI console calls this method to find out:
        /// - the login state of the interviewer
        /// - the login to dialer state of the interviewer
        /// - the previously started by <code>StartInterview</code> method interview and obtain 
        /// the interview URL in order to start it on Confirmit
        /// - that another a new interview was started
        /// - that there are no calls for the interviewer 
        /// - ...
        /// </remarks>
        /// <returns>
        /// The state parameters are returned. 
        /// </returns>
        public State GetState()
        {
            var evt = new GetStateEvent();
            var activityEvent = new UrlGeneratedInGetStateEvent();

            // Authorization
            _consoleStateWsRequestsAuthoriser.AuthoriseRequest(out var person, out var task);

            evt.AddTiming("Authorise Request");
            evt.UpdateEventPropertiesFromTask(task);

            ProcessIdleTask(person, task);

            var result = _consoleStateProvider.GetState(task, person, evt, activityEvent);

            evt.SaveIfEventTookLongerThan(5000);

            return result;
        }

        private void ProcessIdleTask(BvPersonEntity person, BvTasksEntity task)
        {
            if(_consoleTransferProcessProcessor.ShouldProcessTransfer(task))
            {
                _asyncManager.QueueWorkItem(() => _consoleTransferProcessProcessor.ProcessTransfer(person));
            }
        }

        /// <summary>
        /// CATI console must call this function each "KeepAliveTimeout" seconds.
        /// (CATI console obtains "KeepAliveTimeout" from ConsoleService at login).
        /// It allows Fusion realize that CATI console user is alive.
        /// </summary>
        /// <example>
        /// User didn't log out via "Logout" but closed the CATI console or turned off the computer.
        /// So in "KeepAliveTimeout" seconds ConsoleService will find out that user does not keep alive and ConsoleService
        /// will logout the user automatically.
        /// </example>
        public KeepAliveResult KeepAlive()
        {
            var evt = new KeepAliveEvent();

            // Authorization
            BvTasksEntity task;
            BvPersonEntity person;
            _consoleStateWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            evt.AddTiming("Authorise Request");
            evt.UpdateEventPropertiesFromTask(task);

            var result = new KeepAliveResult();

            int rowsAffected;
            BvSpTasks_UpdateKeepAliveAdapter.ExecuteNonQuery(person.SID, out rowsAffected);
            evt.AddTiming("BvSpTasks_UpdateKeepAliveAdapter");

            result.m_NewMessage = PersonService.CheckNewMessages(person.SID);
            evt.AddTiming("PersonService.CheckNewMessages");

            result.m_interviwerSID = person.SID;

            var monitoringDescription = _monitoringService.GetActiveMonitoring(person.SID);
            evt.AddTiming("MonitoringService.GetActiveMonitoring");

            if (monitoringDescription != null)
            {
                result.m_isMonitored = true;
                result.m_monitoringSessionID = monitoringDescription.MonitoringSessionId;
            }
            else
            {
                result.m_isMonitored = false;
                result.m_monitoringSessionID = 0;
            }

            evt.SaveIfEventTookLongerThan(5000);

            return result;
        }
    }
}
