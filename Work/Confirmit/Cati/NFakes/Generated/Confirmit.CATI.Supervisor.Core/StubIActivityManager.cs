using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Activity.Fakes
{
    public class StubIActivityManager : IActivityManager 
    {
        private IActivityManager _inner;

        public StubIActivityManager()
        {
            _inner = null;
        }

        public IActivityManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<InterviewerPerformanceInfo> GetInterviewerPerformanceDataBooleanBooleanBooleanInt32ArrayOfInt32IEnumerableOfInt32Delegate(bool onlyLogged, bool filterBySurveys, bool activeSurveysOnly, int callCenterId, int[] interviewersId, IEnumerable<int> surveysId);
        public GetInterviewerPerformanceDataBooleanBooleanBooleanInt32ArrayOfInt32IEnumerableOfInt32Delegate GetInterviewerPerformanceDataBooleanBooleanBooleanInt32ArrayOfInt32IEnumerableOfInt32;

        List<InterviewerPerformanceInfo> IActivityManager.GetInterviewerPerformanceData(bool onlyLogged, bool filterBySurveys, bool activeSurveysOnly, int callCenterId, int[] interviewersId, IEnumerable<int> surveysId)
        {


            if (GetInterviewerPerformanceDataBooleanBooleanBooleanInt32ArrayOfInt32IEnumerableOfInt32 != null)
            {
                return GetInterviewerPerformanceDataBooleanBooleanBooleanInt32ArrayOfInt32IEnumerableOfInt32(onlyLogged, filterBySurveys, activeSurveysOnly, callCenterId, interviewersId, surveysId);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetInterviewerPerformanceData(onlyLogged, filterBySurveys, activeSurveysOnly, callCenterId, interviewersId, surveysId);
            }

            return default(List<InterviewerPerformanceInfo>);
        }

        public delegate void TerminateTaskByPersonInt32StringDelegate(int personId, string reason);
        public TerminateTaskByPersonInt32StringDelegate TerminateTaskByPersonInt32String;

        void IActivityManager.TerminateTaskByPerson(int personId, string reason)
        {

            if (TerminateTaskByPersonInt32String != null)
            {
                TerminateTaskByPersonInt32String(personId, reason);
            } else if (_inner != null)
            {
                ((IActivityManager)_inner).TerminateTaskByPerson(personId, reason);
            }
        }

        public delegate List<StatusInfo> GetStatusBreakdownInt32Delegate(int surveyId);
        public GetStatusBreakdownInt32Delegate GetStatusBreakdownInt32;

        List<StatusInfo> IActivityManager.GetStatusBreakdown(int surveyId)
        {


            if (GetStatusBreakdownInt32 != null)
            {
                return GetStatusBreakdownInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetStatusBreakdown(surveyId);
            }

            return default(List<StatusInfo>);
        }

        public delegate List<SurveyActivityInfo> GetSurveyActivityDataStringBooleanBooleanIEnumerableOfInt32BooleanArrayOfInt32Delegate(string sortExpression, bool sortOrderAsc, bool showOnlyActiveSurveys, IEnumerable<int> surveys, bool onlyCatiInterviews, int[] its);
        public GetSurveyActivityDataStringBooleanBooleanIEnumerableOfInt32BooleanArrayOfInt32Delegate GetSurveyActivityDataStringBooleanBooleanIEnumerableOfInt32BooleanArrayOfInt32;

        List<SurveyActivityInfo> IActivityManager.GetSurveyActivityData(string sortExpression, bool sortOrderAsc, bool showOnlyActiveSurveys, IEnumerable<int> surveys, bool onlyCatiInterviews, int[] its)
        {


            if (GetSurveyActivityDataStringBooleanBooleanIEnumerableOfInt32BooleanArrayOfInt32 != null)
            {
                return GetSurveyActivityDataStringBooleanBooleanIEnumerableOfInt32BooleanArrayOfInt32(sortExpression, sortOrderAsc, showOnlyActiveSurveys, surveys, onlyCatiInterviews, its);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetSurveyActivityData(sortExpression, sortOrderAsc, showOnlyActiveSurveys, surveys, onlyCatiInterviews, its);
            }

            return default(List<SurveyActivityInfo>);
        }

        public delegate List<TaskActivityInfo> GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringDelegate(string sortExpression, bool sortOrderAsc, bool alertsOnTop, IEnumerable<int> surveys, IEnumerable<int> interviewers, string superName);
        public GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringDelegate GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32String;

        List<TaskActivityInfo> IActivityManager.GetTasksActivityData(string sortExpression, bool sortOrderAsc, bool alertsOnTop, IEnumerable<int> surveys, IEnumerable<int> interviewers, string superName)
        {


            if (GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32String != null)
            {
                return GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32String(sortExpression, sortOrderAsc, alertsOnTop, surveys, interviewers, superName);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetTasksActivityData(sortExpression, sortOrderAsc, alertsOnTop, surveys, interviewers, superName);
            }

            return default(List<TaskActivityInfo>);
        }

        public delegate List<TaskActivityInfo> GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringBooleanDelegate(string sortExpression, bool sortOrderAsc, bool alertsOnTop, IEnumerable<int> surveys, IEnumerable<int> interviewers, string superName, bool allCalcenters);
        public GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringBooleanDelegate GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringBoolean;

        List<TaskActivityInfo> IActivityManager.GetTasksActivityData(string sortExpression, bool sortOrderAsc, bool alertsOnTop, IEnumerable<int> surveys, IEnumerable<int> interviewers, string superName, bool allCalcenters)
        {


            if (GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringBoolean != null)
            {
                return GetTasksActivityDataStringBooleanBooleanIEnumerableOfInt32IEnumerableOfInt32StringBoolean(sortExpression, sortOrderAsc, alertsOnTop, surveys, interviewers, superName, allCalcenters);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetTasksActivityData(sortExpression, sortOrderAsc, alertsOnTop, surveys, interviewers, superName, allCalcenters);
            }

            return default(List<TaskActivityInfo>);
        }

        public delegate List<StatusAlertInfo> GetStatusAlertsListBooleanDelegate(bool includeDefault);
        public GetStatusAlertsListBooleanDelegate GetStatusAlertsListBoolean;

        List<StatusAlertInfo> IActivityManager.GetStatusAlertsList(bool includeDefault)
        {


            if (GetStatusAlertsListBoolean != null)
            {
                return GetStatusAlertsListBoolean(includeDefault);
            } else if (_inner != null)
            {
                return ((IActivityManager)_inner).GetStatusAlertsList(includeDefault);
            }

            return default(List<StatusAlertInfo>);
        }

    }
}