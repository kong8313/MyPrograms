using System.Net;

namespace Confirmit.CATI.REST.SDK.Exceptions
{
    /// <summary>
    /// An exception which occurs when an http request to CATI REST API returns 404 error code (HttpStatusCode.NotFound)
    /// </summary>
    public class NotFoundException : RestClientException
    {
        /// <summary>
        ///  Creates and initializes an instance of NotFoundException exception by url, reason and content
        /// </summary>
        /// <param name="url">Problem URL</param>
        /// <param name="reason">Why the error occured</param>
        /// <param name="content">Received data if it exists</param>
        public NotFoundException(string url, string reason, string content):
            base(url, HttpStatusCode.NotFound, reason, content)
        {
            
        }
    }
}