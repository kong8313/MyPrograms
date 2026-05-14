using System;
using System.Linq;
using System.Web.Http;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("dialers")]
    public class DialersController : ApiController
    {
        public class DialerDto
        {
            public int CompanyId { get; set; }
            public int DialerId { get; set; }
            public DateTime InitializationTime { get; set; }
            public int AgentsCount { get; set; }
            public int CampaignsCount { get; set; }
        }

        [HttpGet]
        [Route("")]
        public DialerDto[] GetAll()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values.Select(dialer => new DialerDto()
                {
                    CompanyId = dialer.CompanyId,
                    DialerId = dialer.DialerId,
                    InitializationTime = dialer.InitializationTime,
                    AgentsCount = dialer.InterviewersManager.Interviewers.Count,
                    CampaignsCount = dialer.CampaignsManager.Campaigns.Count
                }).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "DialersController.GetAll",
                    ex.ToString());
                throw;
            }
        }

        [HttpDelete]
        [Route("")]
        public void Delete(int companyId, int dialerId)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.Release(dialerId, companyId);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "DialersController.Delete",
                    ex.ToString());
                throw;
            }
        }
    }
}
