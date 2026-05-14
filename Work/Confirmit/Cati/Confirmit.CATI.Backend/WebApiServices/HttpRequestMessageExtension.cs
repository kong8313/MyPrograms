using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public static class HttpRequestMessageExtensions
    {
        public static T Resolve<T>(this HttpRequestMessage requestMessage)
        {
            return (T)requestMessage.GetDependencyScope().GetService(typeof(T));
        }
    }
}
