using System;
using System.Net;
using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices.Logging
{
    public class RestApiMonitorInfo
    {        
        /// <example>
        /// Confirmit.Authoring.RestService/projects/p1015570/ProjectInfo?view=Content
        /// </example>
        public Uri Uri { get; set; }

        public long TimeTakenInMs { get; set; }

        /// <example>
        /// Author
        /// TaskSystem
        /// </example>
        public string Application { get; set; }

        /// <example>        
        /// SurveyDesigner:209030377
        /// LaunchSurvey:18
        /// confirm/authoring/Confirmit.aspx:11404313
        /// SurveyEngineSynchronizationBase:DownloadSurveyPackage:442805034        
        ///</example>>        
        public string UnitOfWork { get; set; }

        /// <example>
        /// GET
        /// PUT
        /// POST
        /// </example>>        
        public HttpMethod Method { get; set; }

        /// <example>
        /// 200
        /// 404 
        ///</example>>
        public HttpStatusCode StatusCode { get; set; }

        /// <example>
        /// application/xml
        /// application/zip
        /// application/json
        /// </example>
        public string ContentType { get; set; }

        public long? UserId { get; set; }

        public int? CompanyId { get; set; }

        /// <example>
        /// projects
        /// featuretoggles
        /// surveylayouts
        ///</example>
        public string ResourceCollectionName { get; set; }

        /// <example>
        /// If ResourceCollectionName = 
        /// projects:
        ///     p3123455 
        ///     p1335454
        /// featuretoggles:
        ///     SmartHub_SmartHub
        ///     Authoring_TextAnalytics
        /// surveylayouts:
        ///     1
        ///     12
        ///</example>>
        public string ResourceIdentifier { get; set; }
        
        public Exception Exception { get; set; }

        public string WebServerName { get; set; }

        /// <summary>
        /// Get/set Authentication Type
        /// </summary>        
        /// <remarks>
        /// In CF AuthenticationHeaderType enum is used, so it cannot be arbitrary filled 
        /// because some filters in UI can rely on this enum
        /// </remarks>
        public string AuthenticationHeaderType { get { return null; } }
    }
}
