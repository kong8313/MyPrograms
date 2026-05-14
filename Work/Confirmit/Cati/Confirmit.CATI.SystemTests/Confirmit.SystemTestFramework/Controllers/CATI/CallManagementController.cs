using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class CallManagementController : TestController
    {
        private IInterviewRepository _interviewRepository;        
        
        private readonly string _pid;
        private readonly int _surveySid;

        public CallManagementController(UserInfo userInfo, string pid)
        {
            UserInfo = userInfo;
            _pid = pid;
            _surveySid = ServiceLocator.Resolve<ISurveyRepository>().GetByName(_pid).SID;

            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
        }

        public void MoveAndResedule(int its, params int[] interviewIds)
        {
            var parameters = new Parameters
            {
                SurveyId = SurveyRepository.GetByName(_pid).SID,
                BatchParameters = new SelectedBatchParameters(interviewIds),
                StateId = its
            };

            var operation = StartAsyncOperation(parameters, "System test operation");

            operation = ServiceLocator.Resolve<IAsyncOperationAwaiter>().Await(operation);
        }

        public static BvAsyncOperationQueueEntity StartAsyncOperation(IAsyncOperationParameters parameters, string title)
        {
            var supervisorName = ServiceLocator.Resolve<ISupervisorNameProvider>().Name;

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                callCenterId,
                title,
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                supervisorName);

            return operationEntity;
        }

        public BvInterviewWithOriginEntity GetInterview(int interviewId)
        {
            return _interviewRepository.GetById(_surveySid, interviewId);
        }

        public List<BvInterviewEntity> GetInterviews()
        {
            return BvInterviewAdapter.GetByCondition("[SurveySID] = @SurveySID", new SqlParameter("@SurveySID", _surveySid));
        }

        public BvSvyScheduleEntity GetCall(int interviewId)
        {
            return BvSvyScheduleAdapter.GetByCondition(
                "[SurveySID] = @SurveySID and [InterviewID] = @InterviewID", 
                new SqlParameter("@SurveySID", _surveySid), 
                new SqlParameter("@InterviewID", interviewId)).FirstOrDefault(x => x.CallState != 0);
        }
    }
}