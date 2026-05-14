using System;
using System.Net;

namespace Confirmit.CATI.REST.SDK.Exceptions
{
    /// <summary>
    /// Base class for all rest exception.
    /// It occurs when an http request to CATI REST API returns unknown code not supported by other exception classes
    /// </summary>
    public class RestClientException : Exception
    {
        /// <summary>
        ///  Creates and initializes an instance of RestClientException exception by url, status, reason and content
        /// </summary>
        /// <param name="url">Problem URL</param>
        /// <param name="status">Status code</param>
        /// <param name="reason">Why the error occured</param>
        /// <param name="content">Received data if it exists</param>
        public RestClientException(string url, HttpStatusCode status, string reason, string content)
        {
            Url = url;
            Status = status;
            Reason = reason;
            Content = content;
        }

        /// <summary>
        /// Problem URL
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Status code
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// Why the error occured
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Received data if it exists
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Convert the exception to string
        /// </summary>
        /// <returns>String with all information about the exception</returns>
        public override string ToString()
        {
            return $"Url: {Url}\r\nStatus {Status}\r\nReason {Reason}\r\nContent\r\n{Content}";
        }
    }
}
