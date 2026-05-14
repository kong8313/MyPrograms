using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.PersonLogin;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IStationInfoFactory
    {
        StationInfo Create(string stationId, BvPersonEntity person);
    }
}