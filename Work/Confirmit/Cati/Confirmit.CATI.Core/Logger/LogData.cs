using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.Logger
{
    public class LogData
    {
        private readonly bool _isLogDataDefined;
        private readonly Exception _exceptionDuringLogDataDefinition;

        private readonly DateTime _serverTime;
        private readonly DateTime _utcTime;

        private readonly bool _isBackendInitialized;
        private readonly string _companyName;
        private readonly int _companyId;

        private readonly string _processName;
        private readonly int _processId;
        private readonly string _catiVersion;

        private readonly string _threadInfo;

        private readonly bool _isUserDefined;
        private readonly string _userName;

        private readonly bool _isHttpRequestDefined;
        private readonly string _requestHttpMethod;
        private readonly Uri _requestUrl;
        private readonly Uri _requestUrlReferrer;
        private readonly string _requestUserAgent;
        private readonly string _requestUserHostName;
        private readonly bool _isFormVariablesDefined;
        private readonly string _formVariables;
        
        private LogData()
            : this(BackendInstance.IsInitialized ? BackendInstance.Current.CompanyId : 0)
        {
            
        }

        private LogData(int companyId)
        {
            try
            {
                _serverTime = DateTime.Now;
                _utcTime = DateTime.UtcNow;
                _companyId = companyId;
                
                _isBackendInitialized = BackendInstance.IsInitialized;
                if (_isBackendInitialized)
                {
                    _companyName = BackendInstance.Current.CompanyName;
                }

                var processInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
                _processName = processInfo.ProcessName;
                _processId = processInfo.ProcessId;
                _catiVersion = processInfo.Version;

                Thread thread = Thread.CurrentThread;
                _threadInfo = string.IsNullOrEmpty(thread.Name)
                    ? thread.ManagedThreadId.ToString()
                    : $"{thread.Name} ({thread.ManagedThreadId})";
                _threadInfo += thread.IsThreadPoolThread ? " from ThreadPool" : string.Empty;
                
                HttpRequest request = null;
                SupervisorPrincipal user = null;
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.ApplicationInstance != null)
                    {
                        request = HttpContext.Current.ApplicationInstance.Request;
                    }

                    if (HttpContext.Current.User != null && HttpContext.Current.User is SupervisorPrincipal)
                    {
                        user = (SupervisorPrincipal)HttpContext.Current.User;
                    }
                }

                if (user != null)
                {
                    _isUserDefined = true;
                    _userName = user.Name;
                }

                if (request != null)
                {
                    _isHttpRequestDefined = true;
                    _requestHttpMethod = request.HttpMethod;
                    _requestUrl = request.Url;
                    _requestUrlReferrer = request.UrlReferrer;
                    _requestUserAgent = request.UserAgent;
                    _requestUserHostName = request.UserHostName;

                    if (request.Form.Count > 0)
                    {
                        _isFormVariablesDefined = true;
                        _formVariables = NameValueCollectionFormatter.FormatToString(request.Form);
                    }
                }

                _isLogDataDefined = true;
            }
            catch (Exception ex)
            {
                _isLogDataDefined = false;
                _exceptionDuringLogDataDefinition = ex;
            }
        }
        
        public static string ToMessageFooter() => new LogData().BuildMessageFooter();
        
        public static CustomField[] ToCustomFields() => new LogData().BuildCustomFields();
        
        public static CustomField[] ToCustomFields(int companyId) => new LogData(companyId).BuildCustomFields();

        /// <summary>
        /// Gets the event log message footer. Contains additional information for log entry.
        /// </summary>
        private string BuildMessageFooter()
        {
            if (!_isLogDataDefined)
            {
                return $"Error during creation of message footer: \r\n{_exceptionDuringLogDataDefinition}";
            }

            var message = new StringBuilder();

            message.AppendLine("----------");
            message.AppendLine("Server time: " + _serverTime);
            message.AppendLine("UTC time: " + _utcTime);
            message.AppendLine($"Process: {_processName} ({_processId})");

            message.AppendLine($"Thread: {_threadInfo}");

            if (_isUserDefined)
            {
                message.AppendLine("User name: " + _userName);
            }

            if (_isHttpRequestDefined)
            {
                message.AppendLine("HTTP method: " + _requestHttpMethod);
                message.AppendLine("URL: " + _requestUrl);
                message.AppendLine("Referrer: " + _requestUrlReferrer);
                message.AppendLine("User agent: " + _requestUserAgent);
                message.AppendLine("User host: " + _requestUserHostName);

                if (_isFormVariablesDefined)
                {
                    message.AppendLine("Form variables: " + _formVariables);
                }
            }

            message.AppendLine(_isBackendInitialized
                ? $"Company: {_companyName} ({_companyId})"
                : "Company: <Unknown as BackendInstance.Current is not initialized.>");

            message.AppendLine("CATI version: " + _catiVersion);

            return message.ToString();
        }
        
        private CustomField[] BuildCustomFields()
        {
            var fields = new List<CustomField>();
            if (!_isLogDataDefined)
            {
                fields.Add(new CustomField("ErrorDuringCollectingCustomData", "" + _exceptionDuringLogDataDefinition));
                return fields.ToArray();
            }
            
            fields.AddRange(new[]
            {
                new CustomField("CompanyId", _companyId),
                new CustomField("ProcessName", _processName),
                new CustomField("ProcessId", _processId),
                new CustomField("Thread", _threadInfo),
                new CustomField("CatiVersion", _catiVersion)
            });

            if (_isUserDefined)
            {
                fields.Add(new CustomField("Username", _userName));
            }

            if (_isHttpRequestDefined)
            {
                fields.AddRange(new[]
                {
                    new CustomField("DoesRequestExist", "" + _isHttpRequestDefined),
                    new CustomField("UserAgent", _requestUserAgent),
                    new CustomField("HttpMethod", _requestHttpMethod),
                    new CustomField("Url", "" + _requestUrl),
                    new CustomField("Referrer", "" + _requestUrlReferrer),
                    new CustomField("UserHost", _requestUserHostName),
                });
                
                if (_isFormVariablesDefined)
                {
                    fields.Add(new CustomField("FormVariables", _formVariables));
                }
            }
                
            if (_isBackendInitialized)
            {
                fields.Add(new CustomField("CompanyName", _companyName));
            }

            return fields.ToArray();
        }
    }
}