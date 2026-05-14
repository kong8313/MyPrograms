using System.ServiceModel;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    /// <summary>
    /// CODI WCF service contract
    /// </summary>
    [ServiceContract]
    public interface IDialerService : IDialerServiceCore, IDialerServiceRecording
    {
    }
}
