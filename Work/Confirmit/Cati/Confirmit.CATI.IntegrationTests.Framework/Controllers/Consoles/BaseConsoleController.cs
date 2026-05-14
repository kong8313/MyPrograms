using System;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Action = System.Action;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class BaseConsoleController
    {
        private static int _stationId = 0;

        public TestDataContext Context { get; set; }

        public PersonController Person { get; set; }
        public SurveyController Survey { get; set; }
        public DialerController Dialer { get; set; }

        protected PersonInfo _personInfo;
        protected CatiWsHelper _consoleServiceHelper;

        public BaseConsoleController(
            TestDataContext context,
            PersonController person,
            SurveyController survey,
            DialerController dialer = null
        )
        {
            Context = context;
            Person = person;
            Survey = survey;
            Dialer = dialer;
        }

        public void Login()
        {
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            string stationId = GenerateStationId(Dialer);

            _consoleServiceHelper = new CatiWsHelper(Person.Data.Name, Person.Data.Password);

            var consoleDescriptor = new ConsoleDescription();

            _consoleServiceHelper.ConsoleService.Login(stationId, consoleDescriptor, out _personInfo, out diallerInfo,
                out outProperties);

            if (Survey != null)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(Person.Id, Survey.Id);
            }
        }

        private string GenerateStationId(DialerController dialer)
        {
            if (dialer == null)
            {
                return string.Empty;
            }

            return string.Format("stid{0}", ++_stationId);
        }

        public void LoginToDialer()
        {
            LoginToDialer(Survey);
        }

        protected void LoginToDialer(SurveyController survey)
        {
            bool isPredictive;
            _consoleServiceHelper.ConsoleService.LoginToDialer(
                "ex1", Survey != null ? survey.Model.Name : "", out isPredictive);
        }

        public void TransferStart(string groupName)
        { 
            var transferOptions = new TransferOptions
            {
                Resource = groupName,
                Type = ConsoleTransferType.InternalCold
            };
            
            _consoleServiceHelper.ConsoleService.TransferStart(transferOptions);
        }

        public void TransferComplete()
        {
            _consoleServiceHelper.ConsoleService.TransferComplete();
        }


        public State State
        {
            get { return _consoleServiceHelper.ConsoleStateService.GetState(); }
        }

        public void Check(Action<State> checker)
        {
            checker(State);
        }

        public void FinishInterview(InterviewController interview, CompletedInterviewDetails details = null, int attemptNumber = 1)
        {
            _consoleServiceHelper.ConsoleService.WrapUp(interview.Id, false, 1, details ?? new CompletedInterviewDetails());
        }

        public bool SetLinkedInterview(InterviewController interview)
        {
            return new ManagementService().SetNextLinkedInterview(interview.Survey.Model.Name, interview.Id, Person.Id);
        }
        
        public void TerminateInterview()
        {
            _consoleServiceHelper.ConsoleService.TerminateTask();
        }

        public void TerminateConsole()
        {
            TaskService.TerminateTask( Person.Id, new DatabaseTransactionOptions("TerminateTask", DeadlockPriority.Normal));
        }
    }
}