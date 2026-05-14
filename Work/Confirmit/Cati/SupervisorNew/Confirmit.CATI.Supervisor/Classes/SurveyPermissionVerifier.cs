using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SurveyPermissionVerifier
    {
        private readonly IBaseForm _page;
        private readonly string _userName;

        public SurveyPermissionVerifier(IBaseForm page, string userName)
        {
            _page = page;
            _userName = userName;
        }

        public void Verify()
        {
            var attr = (CheckSurveyPermissionAttribute)_page.GetType().GetCustomAttributes(typeof(CheckSurveyPermissionAttribute), true).FirstOrDefault();

            if (attr != null)
            {
                var requestSurveyIds = _page.Request.Params[attr.RequestParameterName];

                if (string.IsNullOrEmpty(requestSurveyIds))
                {
                    if (!attr.IsRequired)
                    {
                        return;
                    }

                    throw new ArgumentException(String.Format("Parameter with name '{0}' is not present in request parameters",
                                                              attr.RequestParameterName));
                }

                CheckPermission(requestSurveyIds, attr.SeparatorCharacter, _userName);    
            }
        }
        
        public  static void CheckPermission(string requestSurveyIds, string separator, string userName)
        {
            var ids = new List<int>();

            if (String.IsNullOrEmpty(separator))
            {
                ids.Add(int.Parse(requestSurveyIds));
            }
            else
            {
                ids.AddRange(requestSurveyIds.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse));                                
            }

            var allowedSurveys = ServiceLocator.Resolve<ISurveyPermissionProvider>().GetUserSurveyPermission(userName);

            if (ids.Any(surveyId => allowedSurveys.Contains(surveyId) == false))
            {
                throw new UserMessageException(Strings.PermissionDenied);
            }            
        }
    }
}
