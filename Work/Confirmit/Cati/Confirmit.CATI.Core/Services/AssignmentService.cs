using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;
        private readonly IPersonGroupRepository _personGroupRepository;
        private readonly ICallQueueService _callQueueService;

        public AssignmentService(
            IPersonService personService,
            IPersonRepository personRepository,
            IPersonGroupRepository personGroupRepository,
            ICallQueueService callQueueService)
        {
            _personService = personService;
            _personRepository = personRepository;
            _personGroupRepository = personGroupRepository;
            _callQueueService = callQueueService;
        }

        public static int AssignResourcesToSurvey(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId)
        {
            return personOrGroupSids.Sum(
                resourceSid => AssignResourceToSurvey(surveySid, resourceSid, callCenterId));
        }

        public static int DeassignResourcesFromSurvey(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId)
        {
            return personOrGroupSids.Sum(
                resourceSid => DeassignResourceFromSurvey(surveySid, resourceSid, callCenterId));
        }

        public void DeassignResourcesFromSurveyCalls(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId)
        {
            foreach (int resourceSid in personOrGroupSids)
            {
                DeassignResourceFromSurveyCalls(surveySid, resourceSid, callCenterId);
            }
            _callQueueService.SyncRuntimeStatistics(DeadlockPriority.Supervisor);
        }

        /// <summary>
        /// Add assignment of CATI person or person group on survey
        /// </summary>
        /// <param name="surveySID">SID of survey</param>
        /// <param name="personOrGroupSID">SID person or person group</param>
        /// <param name="callCenterId">Id of call center. Assignment will be created inside this call center</param>
        public static int AssignResourceToSurvey(int surveySID, int personOrGroupSID, int callCenterId)
        {
            int assignedOperationsCount;
            BvSpAssignment_InsertAdapter.ExecuteNonQuery(
                 0/*@SID - rudiment*/,
                 surveySID,
                 0/*@InterviewSID*/,
                 personOrGroupSID,
                 2   /*@RoleID*/,
                 0/*@FromCall*/,
                 callCenterId,
                 out assignedOperationsCount);

            return assignedOperationsCount;
        }

        public static void AssignResourceToInterview(
            int surveySid,
            int interviewId,
            int personOrGroupSID,
            int callCenterId)
        {
            BvSpAssignment_InsertAdapter.ExecuteNonQuery(
                0 /* always 0 */,
                surveySid,
                interviewId,
                personOrGroupSID,
                (int)Common.Role.Interviewer,
                1/* FromCall*/,
                callCenterId);
        }

        /// <summary>
        /// Remove assignment of CATI person or person group from survey. Note: Does not delete assignment from survey calls
        /// </summary>
        /// <param name="surveySID">SID of survey</param>
        /// <param name="personOrGroupSID">SID person or person group</param>
        /// <param name="callCenterId">Id of call center. Assignment will be deleted only from specific call center</param>
        public static int DeassignResourceFromSurvey(int surveySID, int personOrGroupSID, int callCenterId)
        {
            int deassignedOperationsCount;

            BvSpAssignment_DeleteAdapter.ExecuteNonQuery(surveySID, 0/*Count*/, personOrGroupSID, 2/*RoleID*/, callCenterId, out deassignedOperationsCount);

            ClearResourceAutoSurvey(surveySID, personOrGroupSID, callCenterId, false);

            return deassignedOperationsCount;
        }

        /// <summary>
        /// Remove assignment of CATI person or person group from survey calls. Note: Does not delete assignment of this person(group) from this survey
        /// </summary>
        /// <param name="surveySID">SID of survey</param>
        /// <param name="personOrGroupSID">SID person or person group</param>
        /// <param name="callCenterId">Id of call center. Assignment will be deleted only from this call center</param>
        private static void DeassignResourceFromSurveyCalls(int surveySID, int personOrGroupSID, int callCenterId)
        {
            BvSpAssignment_DeleteAdapter.ExecuteNonQuery(surveySID, 1/*Count*/, personOrGroupSID, 2/*RoleID*/, callCenterId);

            ClearResourceAutoSurvey(surveySID, personOrGroupSID, callCenterId, true);
        }

        public static void ClearResourceAutoSurvey(int surveySid, int resourceSid, int callCenterId, bool isSurveyCallsAssignmentChanged)
        {
            if (isSurveyCallsAssignmentChanged)
                BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            var persons =
                BvSpPerson_GetPersonsWithWrongAutomaticSurveyAdapter.ExecuteEntityList(surveySid, resourceSid,
                    callCenterId);
            foreach (var person in persons)
            {
                if (person.PersonSID.HasValue)
                {
                    PersonService.ClearAutomaticSurvey(person.PersonSID.Value, false);
                }
            }
            
            PersonRepository.RefreshCache();
        }

        /// <summary>
        /// Gets the list of persons assigned on survey.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="roleId">The role ID.</param>
        /// <param name="callCenterId">The ID of call center. </param>
        public static List<BvSpSurvey_GetAssignedPersonListEntity> GetSurveyAssignedPersons(int surveySid, int roleId, int callCenterId)
        {
            return BvSpSurvey_GetAssignedPersonListAdapter.ExecuteEntityList(surveySid, roleId, callCenterId);
        }

        public static List<BvSpPerson_GetAssignedSurveyListEntity> GetPersonAssignedSurveys(int personId, string supervisorName, int callCenterId)
        {
            return BvSpPerson_GetAssignedSurveyListAdapter.ExecuteEntityList(personId, supervisorName, callCenterId);
        }

        /// <summary>
        /// Improved version of <see cref="GetPersonAssignedSurveys"/> with bugfixes
        /// </summary>
        /// <param name="personId">SID person</param>
        /// <param name="supervisorName">Supervisor</param>
        /// <param name="callCenterId">The ID of call center</param>
        /// <returns></returns>
        public static List<BvSpPerson_GetAssignmentsEntity> GetPersonAssignments(int personId, string supervisorName, int callCenterId)
        {
            return BvSpPerson_GetAssignmentsAdapter.ExecuteEntityList(personId, supervisorName, callCenterId);
        }

        public void ClearPersonAssignments(int personId, string supervisorName, int callCenterId)
        {
            var assignedSurveys = GetPersonAssignedSurveys(personId, supervisorName, callCenterId);

            var surveysToDeassignFromResource = assignedSurveys.Where(x => x.AssignedCallsCount == 0 && x.AssignmentType == (int)AssignmentType.Explicit).Select(y => y.SID);

            var surveysToDeassignCallsFromRecource = assignedSurveys.Where(x => x.AssignedCallsCount != 0).Select(y => y.SID);

            foreach (var surveyId in surveysToDeassignFromResource)
            {
                DeassignResourcesFromSurvey(surveyId.Value, personId.CreateList(), callCenterId);
            }

            foreach (var surveyId in surveysToDeassignCallsFromRecource)
            {
                DeassignResourcesFromSurveyCalls(surveyId.Value, personId.CreateList(), callCenterId);
            }

            _callQueueService.SyncRuntimeStatistics(DeadlockPriority.Supervisor);
        }

        public int GetAssignmentResourceId(int[] resourceIds)
        {
            if (resourceIds.Length == 0)
                return 0;
            if (resourceIds.Length == 1)
                return resourceIds[0];

            var resourceQualifier = StringService.Join(",", x => x.ToString(CultureInfo.InvariantCulture),
                resourceIds.OrderBy(x => x));

            int result;

            var persons = BvSpAssignmentResource_InsertAdapter.ExecuteEntityList(resourceQualifier, out result);
            foreach (var person in persons)
            {
                _personService.OnPersonMemberShipUpdate((int)person.ID);
            }

            return result;
        }

        public int[] GetResourceIds(int assignmentResourceId)
        {
            if (assignmentResourceId == 0)
                return new int[] { };

            return BvSpAssignmentResource_GetResourcesAdapter.ExecuteEntityList(assignmentResourceId)
                .Select(x => (int)x.ID)
                .ToArray();
        }

        public CallAssignemntInfo GetAssignemntInfo(BvCallEntity call)
        {
            if (call.ResourceType == (int)CallExplicitType.Survey)
                return CallAssignemntInfo.CreateSurveyAssignment();

            var group = _personGroupRepository.TryGetById(call.Resource);
            if (group != null)
            {
                return CallAssignemntInfo.CreateGroupAssignment(group);
            }

            var person = _personRepository.TryGetById(call.Resource);
            if (person != null)
            {
                return CallAssignemntInfo.CreatePersonAssignment(person);
            }

            var multiResourceIds = GetResourceIds(call.Resource)
                                    .Select(id => _personGroupRepository.TryGetById(id))
                                    .Where(g => g != null)
                                    .ToArray();

            return CallAssignemntInfo.CreateMultiAssignment(multiResourceIds);
        }
    }
}
