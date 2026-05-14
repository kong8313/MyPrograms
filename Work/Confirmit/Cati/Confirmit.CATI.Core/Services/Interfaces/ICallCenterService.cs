using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ICallCenterService
    {
        void AssignSurvey(int callCenterId, int surveyId);
        bool DeassignSurvey(int callCenterId, int surveyId);
        void AssignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds);
        IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ReassignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds);

        void AssignSupervisors(int callCenterId, params string[] name);
        void DeassignSupervisor(int callCenterId, string name);
        void ClearSupervisorAssignment(string name);

        List<int> GetSurveyAssignments(int callCenterId);
        IEnumerable<BvSurveyAssignmentOnCallCenterEntity> GetAssignmentsBySurvey(int surveyId);
        List<string> GetSupervisorAssignments(int callCenterId);
        IEnumerable<BvSupervisorAssignmentEntity> GetAllSupervisorCallCenterAssignments();

        BvCallCenterEntity GetSupervisorCallCenter(string superName);

        bool IsNameAlreadyInUse(string callCenterName);
        bool HasLoggedInPersons(int callCenterId, int? surveyId);

        bool IsNeedToHidePii();
    }
}
