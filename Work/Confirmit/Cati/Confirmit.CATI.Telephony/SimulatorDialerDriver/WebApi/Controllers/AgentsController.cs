using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SimulatorDialerDriver.Models;

using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("agents")]
    public class AgentsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public Interviewer[] GetAll()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values.SelectMany(dialer => dialer.InterviewersManager.GetAll())
                    .Select(x => x.Interviewer).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "AgentsController.GetAll",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Logout agent from dialer 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dialerId"></param>
        /// <param name="agentId"></param>
        [HttpDelete]
        [Route("")]
        public void Delete(int companyId, int dialerId, int agentId)
        {
            try
            {
                var dialer = SimulatorDialerDriverClass.Instance.GetDialerWithCheck(companyId, dialerId);
                var agent = dialer.InterviewersManager.Get(agentId);
                dialer.InterviewersManager.Logout(companyId, dialerId, agent.Interviewer.CampaignId, agentId);

            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "AgentsController.Delete",
                    ex.ToString());
                throw;
            }
        }
    }
}
