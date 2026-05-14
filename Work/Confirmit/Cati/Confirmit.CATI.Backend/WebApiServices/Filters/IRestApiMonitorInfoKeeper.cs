using Confirmit.CATI.Backend.WebApiServices.Logging;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    public interface IRestApiMonitorInfoKeeper
    {
        void Store(RestApiMonitorInfo restApiMonitorInfo);
        RestApiMonitorInfo GetInfo();
    }
}
