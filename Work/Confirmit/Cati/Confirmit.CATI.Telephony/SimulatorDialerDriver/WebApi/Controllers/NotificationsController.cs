using System;
using System.Linq;
using System.Web.Http;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("notifications")]
    public class NotificationsController : ApiController
    {
        [HttpPost]
        [Route("agentState")]
        public void AgentState(int companyId, int dialerId, long campaignId, int agentId, AgentState agentState)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyAgentState(companyId, dialerId, campaignId, agentId, agentState);
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "NotificationsController.AgentState",
                    ex.ToString());
                throw;
            }
        }
    }
}
