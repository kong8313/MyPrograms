using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Controllers;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver
{
    public class InterviewersManager
    {
        private readonly ISimulator _simulator;
        private readonly Dialer _dialer;
        public ConcurrentDictionary<int, IInterviewerController> Interviewers = new ConcurrentDictionary<int, IInterviewerController>();

        public InterviewersManager(ISimulator simulator, Dialer dialer)
        {
            _simulator = simulator;
            _dialer = dialer;
        }

        public IInterviewerController Get(int agentId)
        {
            if (!Interviewers.TryGetValue(agentId, out var result))
                throw new DialerException(DialerErrorCode.AgentIsNotLoggedin, "Interviewer is not logged in to dialer");

            return result;
        }

        public IInterviewerController TryGet(int agentId)
        {
            Interviewers.TryGetValue(agentId, out var result);
            
            return result;
        }

        public IEnumerable<IInterviewerController> GetAll()
        {
            return Interviewers.Values;
        }

        public IInterviewerController Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            var interviewer = new Interviewer(agentId)
            {
                CompanyId = companyId,
                DialerId = dialerId,
                CampaignId = campaignId,
                Name = agentName,
                Type = agentType,
                ConnectionString = agentConnectionString,
                IsPredictive = isPredictive,
                Attributes = agentAttributes
            };

            var controller = isPredictive ?
                    (IInterviewerController)InterviewerPredictiveController.Create(_simulator, _dialer, interviewer) :
                    new InterviewerNotPredictiveController(_simulator, _dialer, interviewer);
            
            if (!Interviewers.TryAdd(agentId, controller))
                throw new DialerException(DialerErrorCode.AgentAlreadyLoggedIn, "Agent already logged in to dialer");

            AsyncManager.Execute(_simulator.Logger, () =>
            {
                var context = new ContextInfo(interviewer);
                var delay = Generators.NotifyAgentStateDelay.GetValue(context,TimeSpan.Zero);
                Thread.Sleep(delay);

                _simulator.DialerEvents.NotifyAgentState(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    Generators.NotifyAgentStateValue.GetValue(context, AgentState.LoggedIn));

                if (_simulator.Scenario.SendNotReadyNotificationOnLogin)
                {
                    _simulator.DialerEvents.NotifyAgentState(
                        companyId,
                        dialerId,
                        campaignId,
                        agentId,
                        Generators.NotifyAgentStateValue.GetValue(context, AgentState.NotReady));
                }
            });
            return controller;
        }

        public void Logout(int companyId, int dialerId, long campaignId, int agentId)
        {
            IInterviewerController result;


            if (Interviewers.TryRemove(agentId, out result))
            {
                result.Destroy();

            }
                
            _simulator.DialerEvents.NotifyAgentState(
                companyId,
                dialerId,
                campaignId,
                agentId,
                AgentState.LoggedOut);


        }

        public void DestroyByCampaign(Campaign campaign)
        {
            var interviewersToRemove = Interviewers.Values.Where(x => x.Interviewer.CampaignId == campaign.CampaignId).ToArray();
            foreach (var interviewer in interviewersToRemove)
            {
                IInterviewerController result;
                Interviewers.TryRemove(interviewer.Interviewer.AgentId, out result);
                interviewer.Destroy();
            }
        }

        public int GetCountOfInterviewersByCampaignId(long campaignId)
        {
            return Interviewers.Count(x => x.Value.Interviewer.CampaignId == campaignId);
        }
    }
}