using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubICallCenterService : ICallCenterService 
    {
        private ICallCenterService _inner;

        public StubICallCenterService()
        {
            _inner = null;
        }

        public ICallCenterService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AssignSurveyInt32Int32Delegate(int callCenterId, int surveyId);
        public AssignSurveyInt32Int32Delegate AssignSurveyInt32Int32;

        void ICallCenterService.AssignSurvey(int callCenterId, int surveyId)
        {

            if (AssignSurveyInt32Int32 != null)
            {
                AssignSurveyInt32Int32(callCenterId, surveyId);
            } else if (_inner != null)
            {
                ((ICallCenterService)_inner).AssignSurvey(callCenterId, surveyId);
            }
        }

        public delegate bool DeassignSurveyInt32Int32Delegate(int callCenterId, int surveyId);
        public DeassignSurveyInt32Int32Delegate DeassignSurveyInt32Int32;

        bool ICallCenterService.DeassignSurvey(int callCenterId, int surveyId)
        {


            if (DeassignSurveyInt32Int32 != null)
            {
                return DeassignSurveyInt32Int32(callCenterId, surveyId);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).DeassignSurvey(callCenterId, surveyId);
            }

            return default(bool);
        }

        public delegate void AssignSurveysIEnumerableOfInt32IEnumerableOfInt32Delegate(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds);
        public AssignSurveysIEnumerableOfInt32IEnumerableOfInt32Delegate AssignSurveysIEnumerableOfInt32IEnumerableOfInt32;

        void ICallCenterService.AssignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
        {

            if (AssignSurveysIEnumerableOfInt32IEnumerableOfInt32 != null)
            {
                AssignSurveysIEnumerableOfInt32IEnumerableOfInt32(callCenterIds, surveyIds);
            } else if (_inner != null)
            {
                ((ICallCenterService)_inner).AssignSurveys(callCenterIds, surveyIds);
            }
        }

        public delegate IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ReassignSurveysIEnumerableOfInt32IEnumerableOfInt32Delegate(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds);
        public ReassignSurveysIEnumerableOfInt32IEnumerableOfInt32Delegate ReassignSurveysIEnumerableOfInt32IEnumerableOfInt32;

        IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ICallCenterService.ReassignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
        {


            if (ReassignSurveysIEnumerableOfInt32IEnumerableOfInt32 != null)
            {
                return ReassignSurveysIEnumerableOfInt32IEnumerableOfInt32(callCenterIds, surveyIds);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).ReassignSurveys(callCenterIds, surveyIds);
            }

            return default(IEnumerable<BvSurveyAssignmentOnCallCenterEntity>);
        }

        public delegate void AssignSupervisorsInt32ArrayOfStringDelegate(int callCenterId, string[] name);
        public AssignSupervisorsInt32ArrayOfStringDelegate AssignSupervisorsInt32ArrayOfString;

        void ICallCenterService.AssignSupervisors(int callCenterId, string[] name)
        {

            if (AssignSupervisorsInt32ArrayOfString != null)
            {
                AssignSupervisorsInt32ArrayOfString(callCenterId, name);
            } else if (_inner != null)
            {
                ((ICallCenterService)_inner).AssignSupervisors(callCenterId, name);
            }
        }

        public delegate void DeassignSupervisorInt32StringDelegate(int callCenterId, string name);
        public DeassignSupervisorInt32StringDelegate DeassignSupervisorInt32String;

        void ICallCenterService.DeassignSupervisor(int callCenterId, string name)
        {

            if (DeassignSupervisorInt32String != null)
            {
                DeassignSupervisorInt32String(callCenterId, name);
            } else if (_inner != null)
            {
                ((ICallCenterService)_inner).DeassignSupervisor(callCenterId, name);
            }
        }

        public delegate void ClearSupervisorAssignmentStringDelegate(string name);
        public ClearSupervisorAssignmentStringDelegate ClearSupervisorAssignmentString;

        void ICallCenterService.ClearSupervisorAssignment(string name)
        {

            if (ClearSupervisorAssignmentString != null)
            {
                ClearSupervisorAssignmentString(name);
            } else if (_inner != null)
            {
                ((ICallCenterService)_inner).ClearSupervisorAssignment(name);
            }
        }

        public delegate List<int> GetSurveyAssignmentsInt32Delegate(int callCenterId);
        public GetSurveyAssignmentsInt32Delegate GetSurveyAssignmentsInt32;

        List<int> ICallCenterService.GetSurveyAssignments(int callCenterId)
        {


            if (GetSurveyAssignmentsInt32 != null)
            {
                return GetSurveyAssignmentsInt32(callCenterId);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).GetSurveyAssignments(callCenterId);
            }

            return default(List<int>);
        }

        public delegate IEnumerable<BvSurveyAssignmentOnCallCenterEntity> GetAssignmentsBySurveyInt32Delegate(int surveyId);
        public GetAssignmentsBySurveyInt32Delegate GetAssignmentsBySurveyInt32;

        IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ICallCenterService.GetAssignmentsBySurvey(int surveyId)
        {


            if (GetAssignmentsBySurveyInt32 != null)
            {
                return GetAssignmentsBySurveyInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).GetAssignmentsBySurvey(surveyId);
            }

            return default(IEnumerable<BvSurveyAssignmentOnCallCenterEntity>);
        }

        public delegate List<string> GetSupervisorAssignmentsInt32Delegate(int callCenterId);
        public GetSupervisorAssignmentsInt32Delegate GetSupervisorAssignmentsInt32;

        List<string> ICallCenterService.GetSupervisorAssignments(int callCenterId)
        {


            if (GetSupervisorAssignmentsInt32 != null)
            {
                return GetSupervisorAssignmentsInt32(callCenterId);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).GetSupervisorAssignments(callCenterId);
            }

            return default(List<string>);
        }

        public delegate IEnumerable<BvSupervisorAssignmentEntity> GetAllSupervisorCallCenterAssignmentsDelegate();
        public GetAllSupervisorCallCenterAssignmentsDelegate GetAllSupervisorCallCenterAssignments;

        IEnumerable<BvSupervisorAssignmentEntity> ICallCenterService.GetAllSupervisorCallCenterAssignments()
        {


            if (GetAllSupervisorCallCenterAssignments != null)
            {
                return GetAllSupervisorCallCenterAssignments();
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).GetAllSupervisorCallCenterAssignments();
            }

            return default(IEnumerable<BvSupervisorAssignmentEntity>);
        }

        public delegate BvCallCenterEntity GetSupervisorCallCenterStringDelegate(string superName);
        public GetSupervisorCallCenterStringDelegate GetSupervisorCallCenterString;

        BvCallCenterEntity ICallCenterService.GetSupervisorCallCenter(string superName)
        {


            if (GetSupervisorCallCenterString != null)
            {
                return GetSupervisorCallCenterString(superName);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).GetSupervisorCallCenter(superName);
            }

            return default(BvCallCenterEntity);
        }

        public delegate bool IsNameAlreadyInUseStringDelegate(string callCenterName);
        public IsNameAlreadyInUseStringDelegate IsNameAlreadyInUseString;

        bool ICallCenterService.IsNameAlreadyInUse(string callCenterName)
        {


            if (IsNameAlreadyInUseString != null)
            {
                return IsNameAlreadyInUseString(callCenterName);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).IsNameAlreadyInUse(callCenterName);
            }

            return default(bool);
        }

        public delegate bool HasLoggedInPersonsInt32NullableOfInt32Delegate(int callCenterId, int? surveyId);
        public HasLoggedInPersonsInt32NullableOfInt32Delegate HasLoggedInPersonsInt32NullableOfInt32;

        bool ICallCenterService.HasLoggedInPersons(int callCenterId, int? surveyId)
        {


            if (HasLoggedInPersonsInt32NullableOfInt32 != null)
            {
                return HasLoggedInPersonsInt32NullableOfInt32(callCenterId, surveyId);
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).HasLoggedInPersons(callCenterId, surveyId);
            }

            return default(bool);
        }

        public delegate bool IsNeedToHidePiiDelegate();
        public IsNeedToHidePiiDelegate IsNeedToHidePii;

        bool ICallCenterService.IsNeedToHidePii()
        {


            if (IsNeedToHidePii != null)
            {
                return IsNeedToHidePii();
            } else if (_inner != null)
            {
                return ((ICallCenterService)_inner).IsNeedToHidePii();
            }

            return default(bool);
        }

    }
}