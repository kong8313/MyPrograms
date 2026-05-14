using System.ServiceModel;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    /// <summary>
    /// It is a contract for PRO-T-S WCF service working on Confirmit PRO-T-S server side
    /// </summary>
    [ServiceContract]
    public interface IDialerService : IDialerServiceCore, IDialerServiceRecording
    {
    }
}
