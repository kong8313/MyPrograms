using System;
using System.Linq;
using System.Web.Http;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("transfers")]
    public class TransfersController : ApiController
    {
        public class TransferDto
        {
            public int CompanyId { get; set; }
            public int DialerId { get; set; }
            public string TransferId { get; set; }
            public string Initiator { get; set; }
            public string Target { get; set; }
            public TransferType Type { get; set; }
            public TransferState State { get; set; }
        }
        [HttpGet]
        [Route("")]
        public TransferDto[] GetAll()
        {
            try
            {
                
                return SimulatorDialerDriverClass.Instance.Dialers.Values.SelectMany(dialer => dialer.Transfers.GetAll())
                    .Select(x => new TransferDto()
                    {
                        CompanyId = x.CompanyId,
                        DialerId = x.DialerId,
                        TransferId = x.TransferId,
                        Initiator = GetInitiator(x),
                        Target = GetTarget(x),
                        Type = x.TransferType,
                        State = x.TransferState,
                    }).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "TransfersController.GetAll",
                    ex.ToString());
                throw;
            }
        }

        private string GetAgentDisplayName(int companyId, int dialerId, int agentId)
        {
            var dialer = SimulatorDialerDriverClass.Instance.TryGetDialer(companyId, dialerId);
            var interviewer = dialer?.InterviewersManager.TryGet(agentId)?.Interviewer;
            return interviewer?.DisplayName ?? $"<NotFound>({agentId})";
        }

        private string GetInitiator(Transfer transfer)
        {
            return GetAgentDisplayName(transfer.CompanyId, transfer.DialerId, transfer.InitiatorAgentId);
        }

        private string GetTarget(Transfer transfer)
        {
            switch (transfer.TransferState.TargetType)
            {
                case TargetType.Agent:
                    int.TryParse(transfer.TransferState.TargetResource, out var agentId);
                    return "Agent:" + GetAgentDisplayName(transfer.CompanyId, transfer.DialerId, agentId);
                default:
                    return $"{transfer.TransferState.TargetType}:{transfer.TransferState.TargetResource}";

            }
        }
    }
}
