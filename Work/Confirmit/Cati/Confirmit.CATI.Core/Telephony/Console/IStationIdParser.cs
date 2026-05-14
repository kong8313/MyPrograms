namespace Confirmit.CATI.Core.PersonLogin
{
    public interface IStationIdParser
    {
        StationInfo Parse(string stationId);
    }
}