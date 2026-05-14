using System.Net;

namespace Confirmit.CATI.REST.SDK.Exceptions
{
    /// <summary>
    /// An exception which occurs when an http request to CATI REST API returns 400 error code (HttpStatusCode.BadRequest)
    /// </summary>
    public class BadRequestException : RestClientException
    {
        /// <summary>
        ///  Creates and initializes an instance of BadRequestException exception by url, reason and content
        /// </summary>
        /// <param name="url">Problem URL</param>
        /// <param name="reason">Why the error occured</param>
        /// <param name="content">Received data if it exists</param>
        public BadRequestException(string url, string reason, string content)
            : base(url, HttpStatusCode.BadRequest, reason, content)
        {

        }
    }
}