using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IAssignmentService
    {
        int GetAssignmentResourceId(int[] resourceIds);

        int[] GetResourceIds(int assignmentResourceId);

        void ClearPersonAssignments(int personId, string supervisorName, int callCenterId);

        void DeassignResourcesFromSurveyCalls(int surveySid, IEnumerable<int> personOrGroupSids, int callCenterId);

        CallAssignemntInfo GetAssignemntInfo(BvCallEntity call);
    }

    public class CallAssignemntInfo
    {
        private CallAssignemntInfo() { }

        public CallAssignemntType Type { get; private set; }
        public BvPersonEntity Person { get; private set; }
        public BvPersonGroupEntity[] Groups { get; private set; }

        public static CallAssignemntInfo CreateSurveyAssignment()
        {
            return new CallAssignemntInfo() { Type = CallAssignemntType.Survey };
        }

        public static CallAssignemntInfo CreatePersonAssignment(BvPersonEntity person)
        {
            return new CallAssignemntInfo() { Type = CallAssignemntType.Person, Person = person };
        }

        public static CallAssignemntInfo CreateGroupAssignment(BvPersonGroupEntity group)
        {
            return new CallAssignemntInfo() { Type = CallAssignemntType.Group, Groups = new[] { group } };
        }

        public static CallAssignemntInfo CreateMultiAssignment(IEnumerable<BvPersonGroupEntity> groups)
        {
            return new CallAssignemntInfo() { Type = CallAssignemntType.Multi, Groups = groups.ToArray() };
        }
    }
}
