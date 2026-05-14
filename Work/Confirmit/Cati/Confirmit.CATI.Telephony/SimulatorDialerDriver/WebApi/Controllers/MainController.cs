using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;
using SimulatorDialerDriver.SurveyInstances;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;
using InboudCall = Confirmit.CATI.Telephony.SimulatorDialerDriver.InboundCall;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    public class MainController : ApiController
    {
        /// <summary>
        /// Simulates drop of call from respondent side.
        /// </summary>
        /// <param name="companyId">Commpany id.</param>
        /// <param name="dialerId">Dialer id.</param>
        /// <param name="campaignId">Survey name without p.</param>
        /// <param name="agentId">Interviewer id.</param>
        /// <param name="callId">Call id.</param>
        [HttpGet, Route("simulateCallDroppedByRespondent")]
        public void SimulateCallDroppedByRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyCallDroppedByRespondent(
                    companyId, dialerId, campaignId, agentId, callId);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateCallDroppedByRespondent",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Simulates drop of call from respondent side.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="callOutcome">The call outcome</param>
        [HttpGet, Route("simulateNotifyCustomIvrInterviewEnd")]
        public void SimulateNotifyCustomIvrInterviewEnd(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            CallOutcome callOutcome)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyCustomIvrInterviewEnd(
                    companyId, dialerId, campaignId, agentId, interviewId, callId, callOutcome);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateNotifyCustomIvrInterviewEnd",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">Commpany id.</param>
        /// <param name="dialerId">Dialer Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="interviewId">Interview Id</param>
        /// <param name="outcome">Call outcome</param>
        [HttpGet, Route("SimulateNotifyOutcome")]
        public void SimulateNotifyOutcome(
            int companyId,
            int dialerId,
            long campaignId,
            int interviewId,
            CallOutcome outcome,
            string callerId, 
            TimeSpan ringTime,
            Dictionary<string, string> callOutcomeMetadata,
            string correlationId)
        {
            try
            {
                var dialer = SimulatorDialerDriverClass.Instance.GetDialerWithCheck(companyId, dialerId);
                CallManager.CallInfoEx callInfoEx = dialer.GlobalCallManager.TryGetCallWithRemove(campaignId, interviewId);

                if (callInfoEx == null)
                {
                    var campaign = dialer.CampaignsManager.GetPredictive(campaignId);
                    callInfoEx = campaign.CallManager.TryGetCallWithRemove(campaignId, interviewId);
                }

                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyOutcome(
                    companyId, dialerId, callInfoEx.CampaignId, 0, callInfoEx.Info.interviewId, callInfoEx.Info.callId, outcome, callerId, ringTime, callOutcomeMetadata, correlationId);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateNotifyOutcome",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">Commpany id.</param>
        /// <param name="dialerId">Dialer Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="agentId">Agent Id</param>
        /// <param name="interviewId">Interview Id</param>
        /// <param name="callId">Call Id</param>
        /// <param name="outcome">Call outcome</param>
        [HttpGet, Route("SendRawNotifyOutcome")]
        public void SendRawNotifyOutcome(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            int callId,
            CallOutcome outcome,
            string callerId, 
            TimeSpan ringTime,
            Dictionary<string, string> callOutcomeMetadata,
            string correlationId)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyOutcome(
                    companyId, dialerId, campaignId, agentId, interviewId, callId, outcome, callerId, ringTime, callOutcomeMetadata, correlationId);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SendRawNotifyOutcome",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Return list of calls which were sent to dialer(simulator).
        /// </summary>
        /// <param name="campaignId">if param is specified, calls will be returned from ONLY QUEUE for specific campaign. So, moved calls from other campaigns
        /// will be also returned.</param>
        /// <param name="callType">if param is specified, calls will be filtered by specified type.</param>
        /// <returns></returns>
        [HttpGet, Route("getCalls")]
        public CallManager.CallInfoEx[] GetCalls(long campaignId = 0, CallManager.CallType? callType = null)
        {
            try
            {
                var result = new List<CallManager.CallInfoEx>();
                var predictiveSurveyInstances = SimulatorDialerDriverClass.Instance.Dialers.Values
                        .SelectMany(dialer => dialer.CampaignsManager.Campaigns.Values)
                        .Select(i => i as CampaignControllerPredictive)
                        .Where(x => x != null);

                foreach (var predictiveSurveyInstance in predictiveSurveyInstances)
                {
                    if (campaignId != 0 && predictiveSurveyInstance.CampaignId != campaignId )
                        continue;

                    var calls = predictiveSurveyInstance.CallManager.GetCalls()
                                    .Where(call => callType == null || call.Type == callType);

                    result.AddRange(calls);
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateInboundCall",
                    ex.ToString());
                throw;
            }
        }


        /// <summary>
        /// Return list of logged to dialer agents( interviewers).
        /// </summary>
        /// <param name="campaignId">If campaignId is specified, intervewers will be filtered by campaignId.</param>
        /// <returns></returns>
        [HttpGet, Route("GetInterviewers")]
        public Interviewer[] GetInterviewers(long campaignId = 0)
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values
                            .SelectMany(dialer => dialer.InterviewersManager.Interviewers.Values.Select(c => c.Interviewer))
                            .Where(i => campaignId == 0 || i.CampaignId == campaignId).ToArray();
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateInboundCall",
                    ex.ToString());
                throw;
            }
        }


        /// <summary>
        /// Move call from one campaign to another
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dialerId"></param>
        /// <param name="callId">CallId to move </param>
        /// <param name="fromCampaignId">Specify source campaign QUEUE</param>
        /// <param name="toCampaignId">Specify destination campaign QUEUE</param>
        /// <returns></returns>
        [HttpGet, Route("MoveCall")]
        public bool MoveCall(int companyId, int dialerId, int callId, long fromCampaignId, long toCampaignId)
        {
            try
            {
                var dialer = SimulatorDialerDriverClass.Instance.GetDialerWithCheck(companyId, dialerId);
                var from = dialer.CampaignsManager.GetPredictive(fromCampaignId);
                var to = dialer.CampaignsManager.GetPredictive(toCampaignId);
                return from.CallManager.MoveCallTo(callId, to.CallManager);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "MainController.SimulateInboundCall",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get call outcome distribution scenario
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("CallOutcomeDistributionScenario")]
        public CallOutcomeDistributionScenario GetCallOutcomeDistributionScenario()
        {
            return SimulatorDialerDriverClass.Instance.CallOutcomeDistributor.CallOutcomeDistributionScenario;
        }

        /// <summary>
        /// Get call outcome distribution scenario
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("CallOutcomeDistributionScenario")]
        public IHttpActionResult PutCallOutcomeDistributionScenario([FromBody]CallOutcomeDistributionScenario callOutcomeDistributionScenario)
        {
            if (callOutcomeDistributionScenario == null ||
                callOutcomeDistributionScenario.OutcomeList == null ||
                callOutcomeDistributionScenario.OutcomeList.Count <= 0)
            {
                return ResponseMessage( Request.CreateResponse( HttpStatusCode.BadRequest, "Call outcome can't be null or empty"));
            }

            if (!callOutcomeDistributionScenario.OutcomeList.Any(x =>
                x.CallOutcome == CallOutcome.Connected && x.DistributionWeight > 0))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Call outcome should contains at least one connected outcome with positive DistributionWeight"));
            }

            SimulatorDialerDriverClass.Instance.CallOutcomeDistributor.CallOutcomeDistributionScenario = callOutcomeDistributionScenario;

            return Ok();
        }
    }
}