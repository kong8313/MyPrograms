using System;
using Confirmit.CATI.Supervisor.Core.Assignment;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Backend.Assignment;

namespace Confirmit.CATI.Supervisor.Core.Assignment.Fakes
{
    public class StubIAssignmentManager : IAssignmentManager 
    {
        private IAssignmentManager _inner;

        public StubIAssignmentManager()
        {
            _inner = null;
        }

        public IAssignmentManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsListInt32Delegate(int surveySID);
        public GetAssignedInterviewersAndGroupsListInt32Delegate GetAssignedInterviewersAndGroupsListInt32;

        List<SurveyAssignmentInfoItem> IAssignmentManager.GetAssignedInterviewersAndGroupsList(int surveySID)
        {


            if (GetAssignedInterviewersAndGroupsListInt32 != null)
            {
                return GetAssignedInterviewersAndGroupsListInt32(surveySID);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).GetAssignedInterviewersAndGroupsList(surveySID);
            }

            return default(List<SurveyAssignmentInfoItem>);
        }

        public delegate List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsListInt32Int32Delegate(int surveySID, int callCenterId);
        public GetAssignedInterviewersAndGroupsListInt32Int32Delegate GetAssignedInterviewersAndGroupsListInt32Int32;

        List<SurveyAssignmentInfoItem> IAssignmentManager.GetAssignedInterviewersAndGroupsList(int surveySID, int callCenterId)
        {


            if (GetAssignedInterviewersAndGroupsListInt32Int32 != null)
            {
                return GetAssignedInterviewersAndGroupsListInt32Int32(surveySID, callCenterId);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).GetAssignedInterviewersAndGroupsList(surveySID, callCenterId);
            }

            return default(List<SurveyAssignmentInfoItem>);
        }

        public delegate List<PersonAssignmentInfoItem> GetAssignedSurveyListInt32StringDelegate(int sid, string supervisorName);
        public GetAssignedSurveyListInt32StringDelegate GetAssignedSurveyListInt32String;

        List<PersonAssignmentInfoItem> IAssignmentManager.GetAssignedSurveyList(int sid, string supervisorName)
        {


            if (GetAssignedSurveyListInt32String != null)
            {
                return GetAssignedSurveyListInt32String(sid, supervisorName);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).GetAssignedSurveyList(sid, supervisorName);
            }

            return default(List<PersonAssignmentInfoItem>);
        }

        public delegate List<SurveyInfoItem> GetNotAssignedSurveysListInt32StringBooleanDelegate(int sid, string supervisorName, bool isGroup);
        public GetNotAssignedSurveysListInt32StringBooleanDelegate GetNotAssignedSurveysListInt32StringBoolean;

        List<SurveyInfoItem> IAssignmentManager.GetNotAssignedSurveysList(int sid, string supervisorName, bool isGroup)
        {


            if (GetNotAssignedSurveysListInt32StringBoolean != null)
            {
                return GetNotAssignedSurveysListInt32StringBoolean(sid, supervisorName, isGroup);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).GetNotAssignedSurveysList(sid, supervisorName, isGroup);
            }

            return default(List<SurveyInfoItem>);
        }

        public delegate List<SurveyInfoItem> RemoveAssignedSurveysFromListListOfSurveyInfoItemInt32StringBooleanDelegate(List<SurveyInfoItem> surveysList, int sid, string supervisorName, bool isGroup);
        public RemoveAssignedSurveysFromListListOfSurveyInfoItemInt32StringBooleanDelegate RemoveAssignedSurveysFromListListOfSurveyInfoItemInt32StringBoolean;

        List<SurveyInfoItem> IAssignmentManager.RemoveAssignedSurveysFromList(List<SurveyInfoItem> surveysList, int sid, string supervisorName, bool isGroup)
        {


            if (RemoveAssignedSurveysFromListListOfSurveyInfoItemInt32StringBoolean != null)
            {
                return RemoveAssignedSurveysFromListListOfSurveyInfoItemInt32StringBoolean(surveysList, sid, supervisorName, isGroup);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).RemoveAssignedSurveysFromList(surveysList, sid, supervisorName, isGroup);
            }

            return default(List<SurveyInfoItem>);
        }

        public delegate bool IsPersonOrGroupAssignedInt32Int32Delegate(int surveySID, int personOrGroupSID);
        public IsPersonOrGroupAssignedInt32Int32Delegate IsPersonOrGroupAssignedInt32Int32;

        bool IAssignmentManager.IsPersonOrGroupAssigned(int surveySID, int personOrGroupSID)
        {


            if (IsPersonOrGroupAssignedInt32Int32 != null)
            {
                return IsPersonOrGroupAssignedInt32Int32(surveySID, personOrGroupSID);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).IsPersonOrGroupAssigned(surveySID, personOrGroupSID);
            }

            return default(bool);
        }

        public delegate void ClearSurveyAssignmentsInt32Int32Delegate(int surveyId, int callCenterId);
        public ClearSurveyAssignmentsInt32Int32Delegate ClearSurveyAssignmentsInt32Int32;

        void IAssignmentManager.ClearSurveyAssignments(int surveyId, int callCenterId)
        {

            if (ClearSurveyAssignmentsInt32Int32 != null)
            {
                ClearSurveyAssignmentsInt32Int32(surveyId, callCenterId);
            } else if (_inner != null)
            {
                ((IAssignmentManager)_inner).ClearSurveyAssignments(surveyId, callCenterId);
            }
        }

        public delegate List<PersonAssignmentInfoItemWithGroupName> GetPersonAssignmentsInt32StringInt32Delegate(int sid, string supervisorName, int callCenterId);
        public GetPersonAssignmentsInt32StringInt32Delegate GetPersonAssignmentsInt32StringInt32;

        List<PersonAssignmentInfoItemWithGroupName> IAssignmentManager.GetPersonAssignments(int sid, string supervisorName, int callCenterId)
        {


            if (GetPersonAssignmentsInt32StringInt32 != null)
            {
                return GetPersonAssignmentsInt32StringInt32(sid, supervisorName, callCenterId);
            } else if (_inner != null)
            {
                return ((IAssignmentManager)_inner).GetPersonAssignments(sid, supervisorName, callCenterId);
            }

            return default(List<PersonAssignmentInfoItemWithGroupName>);
        }

    }
}