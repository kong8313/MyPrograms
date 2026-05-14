using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Monitoring.Common.Contracts;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public static class ConsoleControllerExtensions
    {
        private static int _stationId;

        private static string GenerateStationId()
        {
            return $"stid{++_stationId}";
        }

        public static ConsoleController Login(this PersonController person)
        {
            return person.Console.Login();
        }

        public static ConsoleController Login(this PersonController person, SurveyController survey)
        {
            return person.Console.Login(survey);
        }

        public static ConsoleController Login(this PersonController person, string surveyTag)
        {
            return person.Console.Login(person.Context.GetSurvey(surveyTag));
        }

        public static ConsoleController SetStationId<T>(this T console, string stationId) where T : ConsoleController
        {
            console.StationId = stationId;
            return console;
        }

        public static T Login<T>(this T console) where T : ConsoleController 
        {
            console.StationId = console.StationId ?? GenerateStationId();

            console.Services = new CatiWsHelper(console.Person.Data.Name, console.Person.Data.Password);

            var consoleDescriptor = new ConsoleDescription();

            console.Services.ConsoleService.Login(console.StationId, consoleDescriptor, out var personInfo, out var dialerInfo, out var properties);

            console.DialerInfo = dialerInfo;
            console.PersonInfo = personInfo;
            console.Properties = properties;

            return console;
        }

        public static T Login<T>(this T console, SurveyController survey) where T : ConsoleController
        {
            console.Login();

            console.Survey = survey;

            PersonService.LoginPersonOnSurveyForSurveySelectionMode(console.Person.Id, survey.Id);

            return console;
        }

        public static T Login<T>(this T console, string surveyTag) where T : ConsoleController
        {
            return console.Login(console.Context.GetSurvey(surveyTag));
        }

        public static T LoginToDialer<T>(this T console) where T : ConsoleController
        {
            console.Services.ConsoleService.LoginToDialer(
                "ex1", console.Survey != null ? console.Survey.Model.Name : "", out _);

            console.Dialer = console.Context.Dialers.First();

            return console;
        }

        public static T Start<T>(this T console) where T : ConsoleController
        {
            if (console.Survey != null)
            {
                console.Services.ConsoleService.StartInterview(console.Survey.Model.Name, 0);
            }
            else
            {
                console.Services.ConsoleService.StartInterview(null, 0);
            }

            console.Dialer?.ProcessAllPosponedNotification();

            return console;
        }

        public static T Dial<T>(this T console, string phone = null,int attempt = 1) where T : ConsoleController
        {
            console.Services.ConsoleService.Dial(phone, 0, attempt);
            
            return console;
        }

        /// <summary>
        /// Respondent hangup.
        /// CATI console can call this method only if there is a dialer in the system.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="initiator"> 0 - script, 1 - telephone menu, 2 - dial cancellation </param>
        /// <returns>
        /// <c>true</c> if hangup succeeded, <c>false</c> if an error occurred during hangup.
        /// </returns>
        /// <remarks>
        /// We suppose Hangup operation is synchronous.
        /// </remarks>
        public static T Hangup<T>(this T console, int initiator = 1) where T : ConsoleController
        {
            console.Services.ConsoleService.Hangup(initiator);

            return console;
        }

        public static T Wait<T>(this T console, InterviewState interviewState) where T : ConsoleController
        {
            return Wait(console, state => state.interviewState == (int)interviewState );
        }
        public static T Wait<T>(this T console) where T : ConsoleController
        {
            return Wait(console, state => 
                state.interviewState == (int) InterviewState.NO_CALLS ||
                state.interviewState == (int) InterviewState.INTERVIEWING ||
                state.interviewState == (int) InterviewState.OUTGOING_TRANSFER);
        }

        private static T Wait<T>(this T console, Func<State, bool> condition) where T : ConsoleController
        {
            console.Dialer?.ProcessAllPosponedNotification();
            console.State = console.Services.ConsoleStateService.GetState();

            if (console.State.interviewState == (int) InterviewState.NO_CALLS && console.State.interviewerLoginState != (int)LoginState.BREAK)
            {
                Start(console);
                console.State = console.Services.ConsoleStateService.GetState();
            }


            var deadTime = DateTime.UtcNow.AddSeconds(30);

            while (!condition(console.State))
            {
                if (deadTime < DateTime.UtcNow)
                {
                    throw new Exception($"Console was hanged. ConsoleState:{JsonConvert.SerializeObject(console.State)}");
                }

                if (console.State.interviewState == (int)InterviewState.WAITING &&
                    console.State.callOutcome == (int)CallOutcome.Blacklist)
                {
                    Start(console);
                }
                else
                {
                    Thread.Sleep(10);
                }

                console.Dialer?.ProcessAllPosponedNotification();

                console.State = console.Services.ConsoleStateService.GetState();
            }

            if (console.State.interviewId == 0)
            {
                console.Interview = null;
            }
            else
            {
                var surveyId = SurveyRepository.GetByName(console.State.surveyId).SID;
                console.Interview = console.Context.Interviews.Single(x => x.Survey.Id == surveyId && x.Id == console.State.interviewId);
            }

            return console;
        }
        private const string privateDummyString = "You should always specify parameter name(s) in Check method like following: Check(interviewState: InterviewState.WAITING)";
        public static T Check<T>(this T console, string dummy = privateDummyString,  InterviewState? interviewState = null, string interviewTag = null) where T : ConsoleController
        {
            Assert.AreEqual(privateDummyString, dummy);

            if (interviewState != null)
            {
                Assert.AreEqual((int)interviewState, console.State.interviewState, "Wrong interview state");
            }

            if (interviewTag != null)
            {
                Assert.AreEqual(interviewTag, console.Interview.Tag, "Wrong interview tag");
            }

            return console;
        }

        public static T Do<T>(this T console, Action<T> action) where T : ConsoleController
        {
            action(console);

            return console;
        }

        public static T AreEqual<T, P>(this T console, P expected, Func<T, P> actual, string message = null)
            where T : ConsoleController
        {
            Assert.AreEqual(expected, actual(console), message);

            return console;
        }

        public static T WrapUp<T>(this T console, CompletedInterviewDetails details = null, int attemptNumber = 1) where T : ConsoleController
        {
            Assert.IsNotNull(console.Interview, "Can't call WrapUp method, becouse console controller doesn't have active interview.");

            console.Services.ConsoleService.WrapUp(console.Interview.Id, true, attemptNumber, details ?? new CompletedInterviewDetails(){Its = "13"});

            return console;
        }

        public static T InternalColdTransfer<T>(this T console, string group) where T : ConsoleController
        {
            console.Services.ConsoleService.TransferStart(new TransferOptions() { Type = ConsoleTransferType.InternalCold, Resource = group });

            console.Services.ConsoleService.TransferComplete();

            return console;
        }

        public static T StartTransfer<T>(this T console, ConsoleTransferType type, string resource) where T : ConsoleController
        {
            console.Services.ConsoleService.TransferStart(new TransferOptions() { Type = type, Resource = resource });

            return console;
        }

        public static T CompleteTransfer<T>( this T console) where T : ConsoleController
        {
            console.Services.ConsoleService.TransferComplete();

            return console;
        }

        public static T CancelTransfer<T>(this T console) where T : ConsoleController
        {
            console.Services.ConsoleService.TransferCancel();

            return console;
        }


        public static InternalTransferTarget[] GetInternalTransferTargets<T>(this T console) where T : ConsoleController
        {
            return console.Services.ConsoleService.GetInternalTransferTargets();
        }

        public static ExternalTransferTarget[] GetExternalTransferTargets<T>(this T console) where T : ConsoleController
        {
            return console.Services.ConsoleService.GetExternalTransferTargets();
        }

        public static T TerminateConsole<T>(this T console) where T : ConsoleController
        {
            TaskService.TerminateTask(console.Person.Id, new DatabaseTransactionOptions("TerminateTaskWhileAutoLogout", DeadlockPriority.PeriodicalThread));

            return console;
        }

        public static T Break<T>(this T console, bool enable = true, int? type = null) where T : ConsoleController
        {
            console.Services.ConsoleService.SetPendingBreakStatus(enable ? PendingBreakStatus.Break : PendingBreakStatus.None, type);

            return console;
        }
        
    }

    
}