using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIAssignmentService : IAssignmentService 
    {
        private IAssignmentService _inner;

        public StubIAssignmentService()
        {
            _inner = null;
        }

        public IAssignmentService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetAssignmentResourceIdArrayOfInt32Delegate(int[] resourceIds);
        public GetAssignmentResourceIdArrayOfInt32Delegate GetAssignmentResourceIdArrayOfInt32;

        int IAssignmentService.GetAssignmentResourceId(int[] resourceIds)
        {


            if (GetAssignmentResourceIdArrayOfInt32 != null)
            {
                return GetAssignmentResourceIdArrayOfInt32(resourceIds);
            } else if (_inner != null)
            {
                return ((IAssignmentService)_inner).GetAssignmentResourceId(resourceIds);
            }

            return default(int);
        }

        public delegate int[] GetResourceIdsInt32Delegate(int assignmentResourceId);
        public GetResourceIdsInt32Delegate GetResourceIdsInt32;

        int[] IAssignmentService.GetResourceIds(int assignmentResourceId)
        {


            if (GetResourceIdsInt32 != null)
            {
                return GetResourceIdsInt32(assignmentResourceId);
            } else if (_inner != null)
            {
                return ((IAssignmentService)_inner).GetResourceIds(assignmentResourceId);
            }

            return default(int[]);
        }

        public delegate void ClearPersonAssignmentsInt32StringInt32Delegate(int personId, string supervisorName, int callCenterId);
        public ClearPersonAssignmentsInt32StringInt32Delegate ClearPersonAssignmentsInt32StringInt32;

        void IAssignmentService.ClearPersonAssignments(int personId, string supervisorName, int callCenterId)
        {

            if (ClearPersonAssignmentsInt32StringInt32 != null)
            {
                ClearPersonAssignmentsInt32StringInt32(personId, supervisorName, callCenterId);
            } else if (_inner != null)
            {
                ((IAssignmentService)_inner).ClearPersonAssignments(personId, supervisorName, callCenterId);
            }
        }

        public delegate void DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Int32Delegate(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId);
        public DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Int32Delegate DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Int32;

        void IAssignmentService.DeassignResourcesFromSurveyCalls(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId)
        {

            if (DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Int32 != null)
            {
                DeassignResourcesFromSurveyCallsInt32IEnumerableOfInt32Int32(surveySid, personOrGroupSids, callCenterId);
            } else if (_inner != null)
            {
                ((IAssignmentService)_inner).DeassignResourcesFromSurveyCalls(surveySid, personOrGroupSids, callCenterId);
            }
        }

        public delegate CallAssignemntInfo GetAssignemntInfoBvCallEntityDelegate(BvCallEntity call);
        public GetAssignemntInfoBvCallEntityDelegate GetAssignemntInfoBvCallEntity;

        CallAssignemntInfo IAssignmentService.GetAssignemntInfo(BvCallEntity call)
        {


            if (GetAssignemntInfoBvCallEntity != null)
            {
                return GetAssignemntInfoBvCallEntity(call);
            } else if (_inner != null)
            {
                return ((IAssignmentService)_inner).GetAssignemntInfo(call);
            }

            return default(CallAssignemntInfo);
        }

    }
}