using System.Net;

namespace Confirmit.CATI.REST.SDK.Exceptions
{
    /// <summary>
    /// An exception which occurs when an http request to CATI REST API returns 500 error code (HttpStatusCode.InternalServerError)
    /// </summary>
    public class InternalServerErrorException : RestClientException
    {
        /// <summary>
        ///  Creates and initializes an instance of InternalServerErrorException exception by url, reason and content
        /// </summary>
        /// <param name="url">Problem URL</param>
        /// <param name="reason">Why the error occured</param>
        /// <param name="content">Received data if it exists</param>
        public InternalServerErrorException(string url, string reason, string content)
            : base(url, HttpStatusCode.InternalServerError, reason, content)
        {

        }

    }
}