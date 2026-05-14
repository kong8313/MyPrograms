using System;
using System.Linq;
using System.Web.Http;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("inbound")]
    public class InboundController : ApiController
    {
        public class InboundDdiNumberDto
        {
            public int CompanyId { get; set; }
            public int DialerId { get; set; }
            public string Number;
        }
        [HttpGet]
        [Route("ddi")]
        public InboundDdiNumberDto[] GetDdiAll()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values.SelectMany(dialer => dialer.InboundDdiNumbers.Select(
                    ddi =>
                    new InboundDdiNumberDto
                    {
                        CompanyId = dialer.CompanyId,
                        DialerId = dialer.DialerId,
                        Number = ddi.Number
                    })).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "InboundController.GetDdiAll",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Returns list of existing inbound calls.
        /// </summary>
        /// <returns>List of existing inbound calls.</returns>
        [HttpGet, Route("calls")]
        public InboundCall[] GetInboundCalls()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Dialers.Values.SelectMany(dialer => dialer.GlobalInboundCalls.GetInboudCalls()).ToArray();
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "InboundController.GetInboundCalls",
                    ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Simulates new inbound call. I.e. creates new InboundCall object and sends NotifyInboundCall notification.
        /// </summary>
        /// <param name="companyId">Commpany id.</param>
        /// <param name="dialerId">Dialer id.</param>
        /// <param name="ddiNumber">DDI number - number of the called('incoming') phone line.</param>
        /// <param name="cliNumber">CLI number - number of the calling phone (respondent).</param>
        /// <param name="inboundCallId">Optional. Call id of the generated inbound call. 
        /// The id is being generated automatically if the parameter is omitted</param>
        /// <returns>Inbound call id of the generated call.</returns>
        [HttpPost, Route("calls")]
        public string SimulateInboundCall(InboundCall call)
        {
            try
            {
                var dialer = SimulatorDialerDriverClass.Instance.GetDialerWithCheck(call.CompanyId, call.DialerId);
                return dialer.GlobalInboundCalls.GenerateInboundCall(call);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "InboundController.SimulateInboundCall",
                    ex.ToString());

                throw;
            }
        }

        /// <summary>
        /// Simulates drop of inbound call from respondent side.
        /// </summary>
        /// <param name="companyId">Commpany id.</param>
        /// <param name="dialerId">Dialer id.</param>
        /// <param name="inboundCallId">Inbound call id to be dropped.</param>
        /// <returns>Inbound call id of the dropped call.</returns>
        [HttpDelete, Route("calls")]
        public string DropInboundCalls(
            int companyId,
            int dialerId,
            string inboundCallId)
        {
            try
            {
                var dialer = SimulatorDialerDriverClass.Instance.GetDialerWithCheck(companyId, dialerId);
                SimulatorDialerDriverClass.Instance.DialerEvents.NotifyInboundCallDroppedByRespondent(
                    companyId, dialerId, inboundCallId);
                dialer.GlobalInboundCalls.RemoveInboundCall(inboundCallId);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "InboundController.DropInboundCalls",
                    ex.ToString());
                throw;
            }

            return inboundCallId;
        }


    }
}
