using System;
using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    /// <summary>
    /// Manager is responsible for survey permission
    /// </summary>
    public class SurveyPermissionProvider : ISurveyPermissionProvider
    {
        private readonly IUserSurveyPermissionRepository _permissionRepository;
        private readonly ICallCenterService _callCenterService;
        private readonly ICallCenterProvider _callCenterProvider;

        public SurveyPermissionProvider(IUserSurveyPermissionRepository permissionRepository,
                                        ICallCenterService callCenterService,
                                        ICallCenterProvider callCenterProvider)
        {
            if (permissionRepository == null)
            {
                throw new ArgumentNullException("permissionRepository");
            }

            if (callCenterService == null)
            {
                throw new ArgumentNullException("permissionRepository");
            }

            if (callCenterProvider == null)
            {
                throw new ArgumentNullException("callCenterProvider");
            }

            _permissionRepository = permissionRepository;
            _callCenterService = callCenterService;
            _callCenterProvider = callCenterProvider;
        }

        /// <summary>
        /// Init user survey permission 
        /// Calls CF WS UserManagementWS for get ones
        /// Writes them to DB
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="companyId">Company identifier.</param>
        public void InitUserSurveyPermissions(string userName, int companyId)
        {
            var evt = new InitUserSurveyPermissionsEvent();

            var authoringService = ServiceLocator.Resolve<IAuthoringService>();

            string[] allowedSurveys = authoringService.GetProjectsWithSuperviseCATIProjectPermissionForUser(userName, companyId);

            using (var transaction = new DatabaseTransactionScope("UpdateSurveyPermissions", DeadlockPriority.Supervisor))
            {
                WriteUserSurveyPermissionToDb(userName, allowedSurveys);

                transaction.Commit();
            }

            evt.Details.AllowedSurveys = allowedSurveys;
            evt.Finish();
        }

        /// <summary>
        /// Determines if user has permissions for survey.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="surveySid">Fusion survey SID.</param>
        public bool IsSurveyAccessible(string userName, int surveySid)
        {
            return GetUserSurveyPermission(userName).Contains(surveySid);
        }

        /// <summary>
        /// Returns list of survey SIDs user has CATI supervisor permissions for.
        /// </summary>
        /// <param name="userName">User name to get permissions for.</param>
        public List<int> GetUserSurveyPermission(string userName)
        {
            return
                _permissionRepository.GetListByUserName(userName)
                                     .Select(entity => entity.SurveySID.Value)
                                     .ToList()
                                     .Intersect(
                                         _callCenterService.GetSurveyAssignments(_callCenterProvider.GetCurrentId()))
                                     .ToList();
        }

        /// <summary>
        /// Writes info about allowed surveys for specified user to database.
        /// </summary>
        private void WriteUserSurveyPermissionToDb(string userName, IEnumerable<string> allowedSurveys)
        {
            _permissionRepository.Delete(userName);

            foreach (String surveyName in allowedSurveys)
            {
                _permissionRepository.Insert(userName, surveyName);
            }
        }
    }
}
