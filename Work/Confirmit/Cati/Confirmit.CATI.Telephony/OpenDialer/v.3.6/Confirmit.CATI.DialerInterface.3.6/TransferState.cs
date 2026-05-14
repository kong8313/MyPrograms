using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class TransferState
    {
        [DataMember]
        public int InitiatorAgentId { get; set; }

        [DataMember]
        public InitiatorState InitiatorState { get; set; }

        [DataMember]
        public ConnectionState ConnectionState { get; set; }

        [DataMember]
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Contains telephone number, agentId, groupId depending on TargetType field
        /// </summary>
        [DataMember]
        public string TargetResource { get; set; }

        [DataMember]
        public TargetState TargetState { get; set; }

        [DataMember]
        public TargetOutcome TargetOutcome { get; set; }

        public override string ToString()
        {
            return $"InitiatorAgentId={InitiatorAgentId}, InitiatorState={InitiatorState}," +
                   $"ConnectionState={ConnectionState}, TargetType={TargetType}," +
                   $"TargetResource={TargetResource}, TargetState={TargetState}, TargetOutcome={TargetOutcome}";
        }
    }
}