using System.Collections.Concurrent;
using System.Linq;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class InboundCalls : IInboundCalls
    {
        private readonly ILogger _logger;
        private readonly IDialerEvents _dialerEvents;

        private readonly ConcurrentDictionary<string, InboundCall> _inboundCalls = new ConcurrentDictionary<string, InboundCall>();

        private readonly RequestId _requestId = new RequestId();

        public InboundCalls(
            ILogger logger,
            IDialerEvents dialerEvents)
        {
            _logger = logger;
            _dialerEvents = dialerEvents;
        }

        public int Count
        {
            get
            {
                return _inboundCalls.Count;
            }
        }

        public string GenerateInboundCall(InboundCall inboundCall)
        {
            if (inboundCall.InboundCallId == null)
            {
                // Generate an id if no id provided
                inboundCall.InboundCallId = _requestId.Next().ToString();
            }

            _inboundCalls[inboundCall.InboundCallId] = inboundCall;

            _dialerEvents.NotifyInboundCall(
                inboundCall.CompanyId,
                inboundCall.DialerId,
                inboundCall.DdiNumber,
                inboundCall.CliNumber,
                inboundCall.InboundCallId);

            return inboundCall.InboundCallId;
        }

        public void RemoveInboundCall(string inboundCallId)
        {
            if (!_inboundCalls.TryRemove(inboundCallId, out var inboundCall))
            {
                _logger.Warning("InboundCalls.RemoveInboundCall", "Remove inbound call with id '{0}' is failed", inboundCallId);

            }
        }

        public InboundCall[] GetInboudCalls()
        {
            return _inboundCalls.Values.ToArray();
        }
    }
}