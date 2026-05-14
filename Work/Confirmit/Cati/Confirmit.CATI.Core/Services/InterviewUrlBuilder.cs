using System;
using System.Web;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewUrlBuilder
    {
        private readonly UriBuilder _builder;        

        /// <summary>
        /// InterviewUrlBuilder constructor.
        /// </summary>
        /// <param name="urlStartPart">
        /// Start part of interview url. 
        /// It should have following format: http://localhost/wix/cati_
        /// </param>
        /// <param name="projectId">Project identifier.</param>
        /// <param name="enforceHttps">If true 'https' schema will be used otherwise 'http' one.</param>
        public InterviewUrlBuilder(string urlStartPart, string projectId, bool enforceHttps)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }

            _builder = new UriBuilder(urlStartPart + projectId + ".aspx");
            
            if (_builder.Uri.IsDefaultPort)
            {
                //need to prevent incorrect port number after schema changing
                _builder.Port = -1;
            }

            _builder.Scheme = enforceHttps ? "https" : "http";            
        }

        public string Url
        {
            get { return _builder.Uri.AbsoluteUri; }
        }

        public void AddParameterWithUrlEncode(string name, object value)
        {
            var token = HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode(value.ToString());

            if (string.IsNullOrEmpty(_builder.Query))
            {
                _builder.Query = token;
            }
            else
            {
                _builder.Query = _builder.Query.Substring(1) + "&" + token;
            }
        }
    }
}
