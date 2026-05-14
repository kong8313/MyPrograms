using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Assignment;

namespace Confirmit.CATI.Supervisor.Core.Assignment.Fakes
{
    public class StubIAssignmentWithEventLoggingPerformer : IAssignmentWithEventLoggingPerformer 
    {
        private IAssignmentWithEventLoggingPerformer _inner;

        public StubIAssignmentWithEventLoggingPerformer()
        {
            _inner = null;
        }

        public IAssignmentWithEventLoggingPerformer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ReplaceSurveyPersonAssignmentsInt32ListOfInt32Delegate(int surveyId, List<int> interviewerOrGroupIds);
        public ReplaceSurveyPersonAssignmentsInt32ListOfInt32Delegate ReplaceSurveyPersonAssignmentsInt32ListOfInt32;

        void IAssignmentWithEventLoggingPerformer.ReplaceSurveyPersonAssignments(int surveyId, List<int> interviewerOrGroupIds)
        {

            if (ReplaceSurveyPersonAssignmentsInt32ListOfInt32 != null)
            {
                ReplaceSurveyPersonAssignmentsInt32ListOfInt32(surveyId, interviewerOrGroupIds);
            } else if (_inner != null)
            {
                ((IAssignmentWithEventLoggingPerformer)_inner).ReplaceSurveyPersonAssignments(surveyId, interviewerOrGroupIds);
            }
        }

        public delegate void ReplacePersonSurveyAssignmentsBooleanInt32ListOfInt32StringDelegate(bool isGroup, int interviewerOrGroupId, List<int> surveysIds, string supervisorName);
        public ReplacePersonSurveyAssignmentsBooleanInt32ListOfInt32StringDelegate ReplacePersonSurveyAssignmentsBooleanInt32ListOfInt32String;

        void IAssignmentWithEventLoggingPerformer.ReplacePersonSurveyAssignments(bool isGroup, int interviewerOrGroupId, List<int> surveysIds, string supervisorName)
        {

            if (ReplacePersonSurveyAssignmentsBooleanInt32ListOfInt32String != null)
            {
                ReplacePersonSurveyAssignmentsBooleanInt32ListOfInt32String(isGroup, interviewerOrGroupId, surveysIds, supervisorName);
            } else if (_inner != null)
            {
                ((IAssignmentWithEventLoggingPerformer)_inner).ReplacePersonSurveyAssignments(isGroup, interviewerOrGroupId, surveysIds, supervisorName);
            }
        }

        public delegate void ReplacePersonSurveyAssignmentsBooleanIEnumerableOfInt32ListOfInt32StringDelegate(bool isGroup, IEnumerable<int> interviewerOrGroupIds, List<int> surveysIds, string supervisorName);
        public ReplacePersonSurveyAssignmentsBooleanIEnumerableOfInt32ListOfInt32StringDelegate ReplacePersonSurveyAssignmentsBooleanIEnumerableOfInt32ListOfInt32String;

        void IAssignmentWithEventLoggingPerformer.ReplacePersonSurveyAssignments(bool isGroup, IEnumerable<int> interviewerOrGroupIds, List<int> surveysIds, string supervisorName)
        {

            if (ReplacePersonSurveyAssignmentsBooleanIEnumerableOfInt32ListOfInt32String != null)
            {
                ReplacePersonSurveyAssignmentsBooleanIEnumerableOfInt32ListOfInt32String(isGroup, interviewerOrGroupIds, surveysIds, supervisorName);
            } else if (_inner != null)
            {
                ((IAssignmentWithEventLoggingPerformer)_inner).ReplacePersonSurveyAssignments(isGroup, interviewerOrGroupIds, surveysIds, supervisorName);
            }
        }

        public delegate void DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Delegate(int surveyId, IEnumerable<int> interviewerOrGroupIds);
        public DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Delegate DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32;

        void IAssignmentWithEventLoggingPerformer.DeassignResourcesFromSurveyCalls(int surveyId, IEnumerable<int> interviewerOrGroupIds)
        {

            if (DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32 != null)
            {
                DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32(surveyId, interviewerOrGroupIds);
            } else if (_inner != null)
            {
                ((IAssignmentWithEventLoggingPerformer)_inner).DeassignResourcesFromSurveyCalls(surveyId, interviewerOrGroupIds);
            }
        }

        public delegate int AssignResourcesToSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32Delegate(int surveySid, IEnumerable<int> personSids);
        public AssignResourcesToSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32Delegate AssignResourcesToSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32;

        int IAssignmentWithEventLoggingPerformer.AssignResourcesToSurveyUsingSurveyAssignmentsDialog(int surveySid, IEnumerable<int> personSids)
        {


            if (AssignResourcesToSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32 != null)
            {
                return AssignResourcesToSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32(surveySid, personSids);
            } else if (_inner != null)
            {
                return ((IAssignmentWithEventLoggingPerformer)_inner).AssignResourcesToSurveyUsingSurveyAssignmentsDialog(surveySid, personSids);
            }

            return default(int);
        }

        public delegate int DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32Delegate(int surveySid, IEnumerable<int> personSids);
        public DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32Delegate DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32;

        int IAssignmentWithEventLoggingPerformer.DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog(int surveySid, IEnumerable<int> personSids)
        {


            if (DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32 != null)
            {
                return DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogInt32IEnumerableOfInt32(surveySid, personSids);
            } else if (_inner != null)
            {
                return ((IAssignmentWithEventLoggingPerformer)_inner).DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog(surveySid, personSids);
            }

            return default(int);
        }

    }
}