namespace SimulatorDialerDriver.WebApi.Models
{
    public class ApiInfoModel
    {
        /// <summary>
        /// List of API links
        /// </summary>
        public class ServiceLinks
        {
            public string Spec;
            public string InboundCall;
            public string InboundCallDropped;
            public string GetInboundCalls;
        }

        /// <summary>
        /// confirmit.dialer.simulator
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// List of API links
        /// </summary>
        public ServiceLinks Links { get; set; }
    }
}