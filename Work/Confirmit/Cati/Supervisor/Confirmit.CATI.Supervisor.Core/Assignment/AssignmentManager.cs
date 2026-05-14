using System;
using System.Collections.Generic;

using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Backend.Assignment;

namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    /// <summary>
    /// Class contains common methods for interviewers assignment to surveys and vice versa.
    /// </summary>
    public class AssignmentManager : IAssignmentManager
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly IAssignmentService _assignmentService;

        public AssignmentManager(ICallCenterProvider callCenterProvider, IAssignmentService assignmentService)
        {
            _callCenterProvider = callCenterProvider;
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Gets the list of CATI interviewers and groups assigned to specific survey.
        /// List includes information both for explisit and implicit assignments.
        /// </summary>
        /// <param name="surveySID">The survey SID to retive data for.</param>
        /// <returns>The list of interviewers and groups assigned to specific survey.</returns>
        public List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsList(int surveySID)
        {
            var callCenterId = _callCenterProvider.GetCurrentId();

            return GetAssignedInterviewersAndGroupsList(surveySID, callCenterId);
        }

        public List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsList(int surveySID, int callCenterId)
        {
            if (surveySID == 0)
            {
                throw new ArgumentOutOfRangeException("surveySID");
            }

            return (from c in BvSpAssignment_ListAdapter.ExecuteEntityList(surveySID, callCenterId)
                    select new SurveyAssignmentInfoItem
                    {
                        SID = c.PersonSID.Value,
                        Name = c.Name,
                        IsGroup = c.IsPersonGroup.Value == 1,
                        AssignedCallsCount = c.Counts.Value                        
                    }
                ).Distinct().ToList();
        }

      
        /// <summary>
        /// Returns list of surveys assigned to specific interviewer or group.
        /// </summary>
        /// <param name="sid">Person or group SID to get surveys assigned to.</param>
        /// <param name="supervisorName">Supervisor user name.</param>
        /// <returns>List of assignments.</returns>
        public List<PersonAssignmentInfoItem> GetAssignedSurveyList(int sid, string supervisorName)
        {
            List<PersonAssignmentInfoItem> result = new List<PersonAssignmentInfoItem>();

            var callCenterId = _callCenterProvider.GetCurrentId();

            foreach (BvSpPerson_GetAssignedSurveyListEntity entity in
                AssignmentService.GetPersonAssignedSurveys(sid, supervisorName, callCenterId))
            {
                result.Add(
                    new PersonAssignmentInfoItem
                    {
                        SurveySID = entity.SID.Value,
                        ProjectID = entity.Name,
                        ProjectName = entity.Description,
                        AssignedCallsCount = entity.AssignedCallsCount.Value,
                        AssignmentType = entity.AssignmentType.Value
                    }
                );

            }

            return result;
        }


        /// <summary>
        /// Returns list of surveys assigned to specific interviewer or group.
        /// Eithere Explicitly, implicitly (via groups) or assignments to survey calls
        /// </summary>
        /// <param name="sid">Person or group SID to get surveys assigned to</param>
        /// <param name="supervisorName">Supervisor user name</param>
        /// <param name="callCenterId">ID of call center</param>
        public List<PersonAssignmentInfoItemWithGroupName> GetPersonAssignments(int sid, string supervisorName, int callCenterId)
        {
            return AssignmentService.GetPersonAssignments(sid, supervisorName, callCenterId).Select(entity => new PersonAssignmentInfoItemWithGroupName
            {
                SurveySID = entity.SurveyId.Value,
                ProjectID = entity.ProjectId,
                ProjectName = entity.SurveyName,
                AssignedCallsCount = entity.Count.Value,
                AssignmentType = entity.Type.Value,
                ParentGroupName = (AssignmentType)entity.Type.Value == AssignmentType.Explicit ? string.Empty : entity.ResourceName
            }).ToList();
        }

        /// <summary>
        /// Gets list of groups names which are parent for given person
        /// and assigned to given survey.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="personId">Person identifier.</param>
        /// <returns>List of groups names or empty collection.</returns>
        private string[] GetParentGroupsNamesWithAssignment(int surveyId, int personId)
        {
            // getting list of persons/surveys assigned to survey
            List<SurveyAssignmentInfoItem> assignedGroups = GetAssignedInterviewersAndGroupsList(surveyId).
                Where(c => c.IsGroup == true && c.AssignedCallsCount == 0 ).ToList();

            // getting parent groups for person
            int[] parentGroups = PersonService.GetParentGroups(personId);

            var intersection = parentGroups.Intersect(from c in assignedGroups select c.SID);

            return (from c in assignedGroups from x in intersection where c.SID == x select c.Name).Distinct().ToArray();
        }

        /// <summary>
        /// Gets the list of surveys not assigned to specific interviewer or group.
        /// </summary>
        /// <param name="sid">The sid of interviewer or group to retive data for.</param>
        /// <param name="supervisorName">Name of current supervisor.</param>
        /// <param name="isGroup">true, if we pass group identifier as sid; otherwise false.</param>
        /// <returns>List of surveys not assigned to specific interviewer or group.</returns>
        public List<SurveyInfoItem> GetNotAssignedSurveysList(int sid, string supervisorName, bool isGroup)
        {
            List<SurveyInfoItem> surveysList = SurveyManager.GetSurveys(supervisorName, String.Empty);
            // we should remove surveys which are explicitly assigned to person from full survey list
            return RemoveAssignedSurveysFromList(surveysList, sid, supervisorName, isGroup);
        }

        public List<SurveyInfoItem> RemoveAssignedSurveysFromList(List<SurveyInfoItem> surveysList, int sid,
            string supervisorName, bool isGroup)
        {
            var callCenterId = _callCenterProvider.GetCurrentId();
            var assigned = (from c in GetPersonAssignments(sid, supervisorName, callCenterId)
                    where c.AssignmentType == 1
                    select new PersonAssignmentInfoItem
                    {
                        SurveySID = c.SurveySID,
                        ProjectName = c.ProjectName,
                        ProjectID = c.ProjectID,
                    })
                .Distinct();

            foreach (PersonAssignmentInfoItem item in assigned)
            {
                surveysList.Remove(new SurveyInfoItem(item.SurveySID, item.ProjectName, item.ProjectID, 0));
            }

            return surveysList;
        }

        /// <summary>
        /// Determines whether specific person or group of persons assigned to the specified survey.
        /// </summary>
        /// <param name="surveySID">The survey SID to get data for.</param>
        /// <param name="personOrGroupSID">The person or group SID to get data for.</param>
        /// <returns>
        /// 	<c>true</c> if person or group assigned to the specified survey; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPersonOrGroupAssigned(int surveySID, int personOrGroupSID)
        {
            return BvSpSurvey_IsPersonAssignedAdapter.ExecuteEntity(surveySID, personOrGroupSID) != null;
        }


        public void ClearSurveyAssignments(int surveyId, int callCenterId)
        {
            var assignedPersons = GetAssignedInterviewersAndGroupsList(surveyId);

            var resourcesToDeassignFromSurvey = assignedPersons.Where(x => x.AssignedCallsCount == 0).Select(y => y.SID);

            var resourcesToDeassignFromSurveyCalls = assignedPersons.Where(x => x.AssignedCallsCount != 0).Select(y => y.SID);

            if (resourcesToDeassignFromSurvey.Any())
            {
                AssignmentService.DeassignResourcesFromSurvey(surveyId, resourcesToDeassignFromSurvey, callCenterId);
            }

            if (resourcesToDeassignFromSurveyCalls.Any())
            {
                _assignmentService.DeassignResourcesFromSurveyCalls(surveyId, resourcesToDeassignFromSurveyCalls, callCenterId);
            }
        }
    }
}