namespace Confirmit.CATI.Backend.WebApiServices.Logging
{
    public interface IRestApiMonitorLogger
    {
        void Log(RestApiMonitorInfo info);
    }
}
