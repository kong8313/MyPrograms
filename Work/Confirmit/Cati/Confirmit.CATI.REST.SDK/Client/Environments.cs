using System.ServiceModel;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.LogOn;

namespace Confirmit.CATI.REST.SDK.Client
{
    /// <summary>
    /// Class to establish a connection with CATI REST API on different Forsta sites
    /// </summary>
    public static class Environments
    {
        private const string CatiServiceUrlTemplateHttps = "https://{0}/";
        private const string LogonWsServiceUrlTemplateHttps = "https://{0}/confirmit/webservices/18.0/logon.asmx";

        /// <summary>
        /// Creates LongOn SOAP client to login to Horizons system
        /// </summary>
        /// <param name="url">URL to LogOn web service</param>
        /// <returns>Instance of LogOnSoapClient object</returns>
        public static LogOnSoapClient CreateLogOnSoapClient(string url)
        {
            var securityMode = url.StartsWith("https", System.StringComparison.OrdinalIgnoreCase)
                ? BasicHttpSecurityMode.Transport 
                : BasicHttpSecurityMode.None;

            var logOnSoapClient = new LogOnSoapClient(
                new BasicHttpBinding(securityMode),
                new EndpointAddress(url));

            return logOnSoapClient;
        }

        /// <summary>
        /// Login user to Horizons system
        /// </summary>
        /// <param name="url">URL to LogOn web service</param>
        /// <param name="userName">User login</param>
        /// <param name="password">User password</param>
        /// <returns>Value of xConfirmitApiKey to send with http requests to the CATI REST API</returns>
        public static string LogonUser(
            string url, 
            string userName, 
            string password)
        {
            var logOnSoapClient = CreateLogOnSoapClient(url);

            var key = logOnSoapClient.LogOnUser(userName, password);

            return key;
        }

        /// <summary>
        /// Creates instance of CATI REST client to send HTTP requests to CATI REST API by xConfirmitApiKey
        /// </summary>
        /// <param name="url">URL to CATI web service</param>
        /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
        /// <param name="companyId">Unique identifier of the company</param>
        /// <param name="proxy">Proxy address if needed</param>
        /// <returns>Instance of the RestClient object</returns>
        public static IRestClient CreateCatiRestClient(
            string url, 
            string xConfirmitApiKey, 
            int companyId, 
            string proxy)
        {
            var client = new RestClient(
                url,
                proxy,
                xConfirmitApiKey,
                companyId);

            return client;
        }

        /// <summary>
        /// Creates CATI REST client to send http requests to CATI REST API by user login and password
        /// </summary>
        /// <param name="url">URL to CATI web service</param>
        /// <param name="logonUrl">URL to LogOn web service</param>
        /// <param name="userName">Name of the user</param>
        /// <param name="password">Password</param>
        /// <param name="companyId">Unique identifier of the company</param>
        /// <param name="proxy">Proxy address if needed</param>
        /// <returns>Instance of the RestClient object</returns>
        public static IRestClient CreateCatiRestClient(
            string url, 
            string logonUrl, 
            string userName, 
            string password, 
            int companyId, 
            string proxy)
        {
            var key = LogonUser(
                logonUrl, 
                userName, 
                password);

            var client = CreateCatiRestClient(
                url,
                key,
                companyId,
                proxy);

            return client;
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons EURO site
        /// </summary>
        public static class Euro
        {
            private const string CatiLbUrl = "cati.euro.confirmit.com";
            private const string WsLbUrl = "ws.euro.confirmit.com";

            /// <summary>
            /// URL to CATI web service on Euro
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on Euro
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to Euro
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Euro by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Euro by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons US site
        /// </summary>
        public static class Us
        {
            private const string CatiLbUrl = "cati.us.confirmit.com";
            private const string WsLbUrl = "ws.us.confirmit.com";

            /// <summary>
            /// URL to CATI web service on US
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on US
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to US
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on US by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on US by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons Australia site
        /// </summary>
        public static class Australia
        {
            private const string CatiLbUrl = "cati.confirmit.com.au";
            private const string WsLbUrl = "ws.confirmit.com.au";

            /// <summary>
            /// URL to CATI web service on Australia
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on Australia
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to Australia
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Australia by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Australia by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons Germany site
        /// </summary>
        public static class Germany
        {
            private const string CatiLbUrl = "cati.confirmit.de";
            private const string WsLbUrl = "ws.confirmit.de";

            /// <summary>
            /// URL to CATI web service on Germany
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on Germany
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to Germany
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Germany by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Germany by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons Testlab site
        /// </summary>
        public static class Testlab
        {
            private const string CatiLbUrl = "cati.testlab.firmglobal.net";
            private const string WsLbUrl = "ws.testlab.firmglobal.net";

            /// <summary>
            /// URL to CATI web service on Testlab
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on Testlab
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to Testlab
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Testlab by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Testlab by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }

        /// <summary>
        /// Class to help establish a connection to the Horizons Nordic site
        /// </summary>
        public static class Nordic
        {
            private const string CatiLbUrl = "cati.nordic.confirmit.com";
            private const string WsLbUrl = "ws.nordic.confirmit.com";

            /// <summary>
            /// URL to CATI web service on Nordic
            /// </summary>
            public static string CatiServiceUrl => string.Format(CatiServiceUrlTemplateHttps, CatiLbUrl);

            /// <summary>
            /// URL to LogOn web service on Nordic
            /// </summary>
            public static string LogonWsUrl => string.Format(LogonWsServiceUrlTemplateHttps, WsLbUrl);

            /// <summary>
            /// Create LongOn soap client to login to Nordic
            /// </summary>
            /// <returns>Instance of LogOnSoapClient object</returns>
            public static LogOnSoapClient CreateLogOnSoapClient()
            {
                return Environments.CreateLogOnSoapClient(LogonWsUrl);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Nordic by user login and password
            /// </summary>
            /// <param name="userName">Name of the user</param>
            /// <param name="password">Password</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string userName, string password, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, LogonWsUrl, userName, password, companyId, proxy);
            }

            /// <summary>
            /// Creates CATI REST client to send http requests to CATI REST API on Nordic by xConfirmitApiKey
            /// </summary>
            /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
            /// <param name="companyId">Unique identifier of the company</param>
            /// <param name="proxy">Proxy address if needed</param>
            /// <returns>Instance of the RestClient object</returns>
            public static IRestClient CreateCatiRestClient(string xConfirmitApiKey, int companyId, string proxy = null)
            {
                return Environments.CreateCatiRestClient(CatiServiceUrl, xConfirmitApiKey, companyId, proxy);
            }
        }
    }
}
