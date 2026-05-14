using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    /// <summary>
    /// Class responsible for performing assign, deassign, replace assignment actions for surveys and persons 
    /// with logging corresponding events in the CATI Management Event Log.
    /// </summary>
    public class AssignmentWithEventLoggingPerformer : IAssignmentWithEventLoggingPerformer
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentWithEventLoggingPerformer(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public static int AssignResourcesToSurvey(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {
            var survey = SurveyRepository.GetById(surveyId);
            var evt = new AssignResourcesToSurveyEvent(survey.SID, survey.Name, interviewerOrGroupIds);
            
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var count = AssignmentService.AssignResourcesToSurvey(surveyId, interviewerOrGroupIds, callCenterId);

            evt.Finish();

            return count;
        }

        public int AssignResourcesToSurveyUsingSurveyAssignmentsDialog(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {
            var survey = SurveyRepository.GetById(surveyId);
            var evt = new AssignResourcesToSurveyUsingSurveyAssignmentsDialogEvent(survey.SID, survey.Name, interviewerOrGroupIds);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var count = AssignmentService.AssignResourcesToSurvey(surveyId, interviewerOrGroupIds, callCenterId);

            evt.Finish();

            return count;
        }

        public static void AssignSurveysToResource(bool isGroup, int interviewerOrGroupId, List<int> surveysIds)
        {
            var name = isGroup
                           ? PersonGroupRepository.GetById(interviewerOrGroupId).Name
                           : PersonRepository.GetById(interviewerOrGroupId).Name;

            var evt = new AssignSurveysToResourceEvent(interviewerOrGroupId, name, surveysIds);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            foreach (int surveySid in surveysIds)
            {
                AssignmentService.AssignResourceToSurvey(surveySid, interviewerOrGroupId, callCenterId);
            }

            evt.Finish();
        }

        public static void AssignSurveysToResources(bool isGroup, IEnumerable<int> interviewerOrGroupIds, List<int> surveysIds)
        {
            foreach (var interviewerOrGroupId in interviewerOrGroupIds)
            {
                AssignSurveysToResource(isGroup, interviewerOrGroupId, surveysIds);
            }
        }
        
        public static int DeassignResourcesFromSurvey(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {
            var survey = SurveyRepository.GetById(surveyId);
            var evt = new DeassignResourcesFromSurveyEvent(surveyId, survey.Name, interviewerOrGroupIds);
            
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var count = AssignmentService.DeassignResourcesFromSurvey(surveyId, interviewerOrGroupIds, callCenterId);

            evt.Finish();

            return count;
        }

        public int DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {
            var survey = SurveyRepository.GetById(surveyId);
            var evt = new DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogEvent(surveyId, survey.Name, interviewerOrGroupIds);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var count = AssignmentService.DeassignResourcesFromSurvey(surveyId, interviewerOrGroupIds, callCenterId);

            evt.Finish();

            return count;
        }

        public void DeassignResourcesFromSurveyCalls(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {
            var survey = SurveyRepository.GetById(surveyId);
            
            var evt = new DeassignResourcesFromSurveyCallsEvent(survey.SID, survey.Name, interviewerOrGroupIds);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            _assignmentService.DeassignResourcesFromSurveyCalls(surveyId, interviewerOrGroupIds, callCenterId);

            evt.Finish();
        }

        public void ReplaceSurveyPersonAssignments(int surveyId, List<int> interviewerOrGroupIds)
        {
            using (var transaction = new DatabaseTransactionScope("ReplaceSurveyPersonAssignments", DeadlockPriority.Supervisor))
            {
                var evt = new ReplaceSurveyPersonAssignmentEvent(surveyId, SurveyRepository.GetById(surveyId).Name, interviewerOrGroupIds);
                var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
                ClearSurveyAssignments(surveyId);
                AssignmentService.AssignResourcesToSurvey(surveyId, interviewerOrGroupIds, callCenterId);

                evt.Finish();

                transaction.Commit();
            }
        }

        public void ReplacePersonSurveyAssignments(bool isGroup, int interviewerOrGroupId, List<int> surveysIds, string supervisorName)
        {
            using (var transaction = new DatabaseTransactionScope("ReplacePersonSurveyAssignments", DeadlockPriority.Supervisor))
            {
                var name = isGroup
                               ? PersonGroupRepository.GetById(interviewerOrGroupId).Name
                               : PersonRepository.GetById(interviewerOrGroupId).Name;

                var evt = new ReplacePersonSurveyAssignmentEvent(interviewerOrGroupId, name, surveysIds);

                var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

                _assignmentService.ClearPersonAssignments(interviewerOrGroupId, supervisorName, callCenterId);

                foreach (int surveySid in surveysIds)
                {
                    AssignmentService.AssignResourceToSurvey(surveySid, interviewerOrGroupId, callCenterId);
                }

                evt.Finish();

                transaction.Commit();
            }
        }

        public void ReplacePersonSurveyAssignments(bool isGroup, IEnumerable<int> interviewerOrGroupIds, List<int> surveysIds, string supervisorName)
        {
            foreach (var interviewerOrGroupId in interviewerOrGroupIds)
            {
                ReplacePersonSurveyAssignments(isGroup, interviewerOrGroupId, surveysIds, supervisorName);
            }
        }
        

        private void ClearSurveyAssignments(int surveyId)
        {
            var assignedPersons = ServiceLocator.Resolve<IAssignmentManager>().GetAssignedInterviewersAndGroupsList(surveyId);

            var resourcesToDeassignFromSurvey = assignedPersons.Where(x => x.AssignedCallsCount == 0).Select(y => y.SID);

            var resourcesToDeassignFromSurveyCalls = assignedPersons.Where(x => x.AssignedCallsCount != 0).Select(y => y.SID);

            if (resourcesToDeassignFromSurvey.Any())
            {
                DeassignResourcesFromSurvey(surveyId, resourcesToDeassignFromSurvey);
            }

            if (resourcesToDeassignFromSurveyCalls.Any())
            {
                DeassignResourcesFromSurveyCalls(surveyId, resourcesToDeassignFromSurveyCalls);
            }
        }
    }
}
