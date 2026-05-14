using System;
using System.Linq;
using System.Web.Http;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("campaigns")]
    public class CampaignsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public Campaign[] GetAll()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values.SelectMany(dialer => dialer.CampaignsManager.GetAllCampaign()).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "CampaignsController.GetAll",
                    ex.ToString());
                throw;
            }
        }
    }
}
