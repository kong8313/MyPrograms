using System.Web.Http;
using System.Web.Http.Description;
using SimulatorDialerDriver.WebApi.Models;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    public class RootController : ApiController
    {
        /// <summary>
        /// Basic information about the SurveyVoiceXml service
        /// </summary>
        /// <returns>Links</returns>
        [HttpGet, Route(""), ResponseType(typeof(ApiInfoModel))]
        public ApiInfoModel ServiceInfo()
        {
            return new ApiInfoModel
            {
                Id = "confirmit.dialer.simulator",
                Links = new ApiInfoModel.ServiceLinks
                {
                    Spec = "swagger",
                    InboundCall = "simulateInboundCall",
                    InboundCallDropped = "simulateInboundCallDroppedByRespondent",
                    GetInboundCalls = "getInboundCalls"
                }
            };
        }
    }
}
