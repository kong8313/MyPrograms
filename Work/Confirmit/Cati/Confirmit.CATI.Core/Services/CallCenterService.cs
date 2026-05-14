using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public class CallCenterService : ICallCenterService
    {
        private readonly ICallCenterRepository _callCenterRepository;

        public CallCenterService(ICallCenterRepository callCenterRepository)
        {
            if (callCenterRepository == null)
            {
                throw new ArgumentNullException("callCenterRepository");
            }

            _callCenterRepository = callCenterRepository;
        }

        public void AssignSurvey(int callCenterId, int surveyId)
        {
            using (var transaction = new DatabaseTransactionScope("AssignSurvey"))
            {
                BvSpSurvey_AssignToCallCenterAdapter.ExecuteNonQuery(surveyId, callCenterId);
                
                transaction.Commit();
            }
        }

        public bool DeassignSurvey(int callCenterId, int surveyId)
        {
            if (HasLoggedInPersons(callCenterId, surveyId))
            {
                return false;
            }

            using (var transaction = new DatabaseTransactionScope("AssignSurvey"))
            {
                BvSpSurvey_DeassignFromCallCenterAdapter.ExecuteNonQuery(surveyId, callCenterId);
                
                transaction.Commit();
            }

            return true;
        }

        public void AssignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
        {
            var evt = new AssignSurveysToCallCentersEvent(callCenterIds, surveyIds);

            foreach (var surveyId in surveyIds)
            {
                var assignments = GetAssignmentsBySurvey(surveyId);

                foreach (var callCenterId in callCenterIds)
                {
                    if (assignments.Any(x => x.CallCenterId == callCenterId))
                        continue;

                    AssignSurvey(callCenterId, surveyId);
                }
            }

            evt.Finish();
        }

        public bool HasLoggedInPersons(int callCenterId, int? surveyId)
        {
            return HasLoggedInPersons(callCenterId, surveyId, -1);
        }

        public IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ReassignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
        {
            var result = new List<BvSurveyAssignmentOnCallCenterEntity>();
            var evt = new AssignSurveysToCallCentersEvent(callCenterIds, surveyIds, true);

            foreach (var surveyId in surveyIds)
            {
                var assignments = GetAssignmentsBySurvey(surveyId);

                foreach (var callCenterId in callCenterIds)
                {
                    if (assignments.Any(x => x.CallCenterId == callCenterId))
                        continue; 
                    
                    AssignSurvey(callCenterId, surveyId);
                }

                foreach (var assignment in assignments)
                {
                    if (callCenterIds.Any(x => x == assignment.CallCenterId))
                        continue;

                    if (!DeassignSurvey(assignment.CallCenterId, surveyId))
                    {
                        result.Add(assignment);
                    }
                }
            }

            evt.Finish();

            return result;
        }

        public void AssignSupervisors(int callCenterId, params string[] names)
        {
            if (_callCenterRepository.Get(callCenterId) == null)
            {
                throw new ArgumentException(string.Format("Call center with id {0} does not exist", callCenterId), "callCenterId");
            }

            using (var transaction = new DatabaseTransactionScope("AssignSupersToCallCenter", DeadlockPriority.Supervisor))
            {
                var evt = new AssignSupervisorsToCallCenterEvent(callCenterId, names);

                foreach (var super in names)
                {
                    ClearSupervisorAssignment(super);
                    var entity = new BvSupervisorAssignmentEntity
                    {
                        CallCenterId = callCenterId,
                        Name = super
                    };

                    BvSupervisorAssignmentAdapter.Insert(entity);
                }

                evt.Finish();
                transaction.Commit();
            }
        }

        public void ClearSupervisorAssignment(string name)
        {
            BvSupervisorAssignmentAdapter.DeleteByCondition(
                "Name = @Name",
                new SqlParameter("@Name", name));
        }

        public void DeassignSupervisor(int callCenterId, string name)
        {
            BvSupervisorAssignmentAdapter.DeleteByCondition(
                "CallCenterId = @CallCenterId AND Name = @Name",
                new SqlParameter("@CallCenterId", callCenterId),
                new SqlParameter("@Name", name));
        }

        public List<int> GetSurveyAssignments(int callCenterId)
        {
            return BvSurveyAssignmentOnCallCenterAdapter.GetByCondition(
                "CallCenterId = @CallCenterId",
                new SqlParameter("@CallCenterId", callCenterId)).Select(x => x.SurveyId).ToList();
        }

        public List<string> GetSupervisorAssignments(int callCenterId)
        {
            return BvSupervisorAssignmentAdapter.GetByCondition(
                "CallCenterId = @CallCenterId",
                new SqlParameter("@CallCenterId", callCenterId)).Select(x => x.Name).ToList();
        }

        public IEnumerable<BvSupervisorAssignmentEntity> GetAllSupervisorCallCenterAssignments()
        {
            return BvSupervisorAssignmentAdapter.GetAll();
        }

        public BvCallCenterEntity GetSupervisorCallCenter(string superName)
        {
            var dbAssignment = BvSupervisorAssignmentAdapter.GetByCondition(
                "Name = @Name",
                new SqlParameter("@Name", superName ?? "")).SingleOrDefault();

            if (dbAssignment != null)
            {
                return _callCenterRepository.Get(dbAssignment.CallCenterId.Value);
            }

            return _callCenterRepository.Default;
        }

        public IEnumerable<BvSurveyAssignmentOnCallCenterEntity> GetAssignmentsBySurvey(int surveyId)
        {
            return BvSurveyAssignmentOnCallCenterAdapter.GetByCondition("SurveyId = @SurveyId",
                                                                        new SqlParameter("@SurveyId", surveyId));
        }

        public bool IsNameAlreadyInUse(string callCenterName)
        {
            return BvCallCenterAdapter.GetByCondition("Name = @Name", new SqlParameter("@name", callCenterName)).Any();
        }

        private bool HasLoggedInPersons(int callCenterId, int? surveyId, int taskChoiceMode)
        {
            return BvSpSurvey_GetCountOfLoggedPersonAdapter.ExecuteScalar<int>(
                surveyId,
                callCenterId,
                taskChoiceMode
                ) > 0;
        }

        public bool IsNeedToHidePii()
        {
            if (SupervisorPrincipal.Current.IsCatiAdministratorOrPros)
            {
                return false;
            }
            
            var callCenter = GetSupervisorCallCenter(SupervisorPrincipal.Current.Name);

            return callCenter.HidePii;
        }
    }
}
