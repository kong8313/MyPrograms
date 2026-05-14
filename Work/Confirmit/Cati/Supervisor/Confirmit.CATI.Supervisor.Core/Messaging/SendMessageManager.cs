using System.Collections.Generic;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using System.Linq;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.UsersApi;
using Confirmit.CATI.Supervisor.Messaging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.Core.Messaging
{
    /// <summary>
    /// Class is responsible for messging operations.
    /// </summary>
    public class SendMessageManager : ISendMessageManager
    {
        private readonly IInterviewerApiClient _interviewerApiClient;
        private readonly IUsersApiService _usersApiService;
        private readonly ITaskRepository _taskRepository;
        private readonly ICompanyInfo _companyInfo;

        public SendMessageManager(
            IInterviewerApiClient interviewerApiClient,
            IUsersApiService usersApiService,
            ITaskRepository taskRepository,
            ICompanyInfo companyInfo)
        {
            _interviewerApiClient = interviewerApiClient;
            _usersApiService = usersApiService;
            _taskRepository = taskRepository;
            _companyInfo = companyInfo;
        }

        #region Methods

        public void SendMessage(string userName, string messageText, MessageRecipientType recipientType, List<int> interviewerIds, bool onlineOnly)
        {
            string supervisorName = GetUserName(userName);

            var message = new Message
            {
                Body = messageText,
                SupervisorName = supervisorName
            };

            IEnumerable<int> bbccInterviewersToNotify = interviewerIds;

            using (var transactionScope = new DatabaseTransactionScope("Messaging.SendMessage", DeadlockPriority.Supervisor))
            {
                switch (recipientType)
                {
                    case MessageRecipientType.Interviewer:
                        SendMessageToInterviewers(interviewerIds, onlineOnly, message);
                        break;
                    case MessageRecipientType.InterviewerGroup:
                        bbccInterviewersToNotify = SendMessageToGroups(interviewerIds, onlineOnly, message);
                        break;
                    case MessageRecipientType.Survey:
                        bbccInterviewersToNotify = SendMessageToSurveys(interviewerIds, message);
                        break;
                }

                transactionScope.Commit();
            }

            NotifyBBCCUsers(bbccInterviewersToNotify, messageText, supervisorName);
        }

        /// <summary>
        /// Sends message for several interviewers
        /// </summary>
        /// <param name="interviewerIds">Array with interviewer ids </param>
        /// <param name="onlineOnly">If true message will be sent only to online interviewers</param>
        /// <param name="message">Message for sending</param>        
        public static void SendMessageToInterviewers(IEnumerable<int> interviewerIds, bool onlineOnly, Message message)
        {
            var evt = new SendMessageToInterviewersEvent(interviewerIds);

            using (TransferBatch batch = TransferBatch.Create())
            {
                batch.Insert(interviewerIds);
                SendMessageToInterviewers(batch.Value, onlineOnly, message);
            }

            evt.Finish();
        }

        /// <summary>
        /// Sends message for several interviewer groups
        /// </summary>
        /// <param name="interviewerIds">Array with interviewer groups ids </param>
        /// <param name="onlineOnly">If true message will be sent only to online interviewers</param>
        /// <param name="message">Message for sending</param>                
        public static IEnumerable<int> SendMessageToGroups(IEnumerable<int> groupIds, bool onlineOnly, Message message)
        {
            var evt = new SendMessageToGroupsEvent(groupIds);
            IEnumerable<int> result;

            using (TransferBatch batch = TransferBatch.Create())
            {
                batch.Insert(groupIds);
                result = SendMessageToGroups(batch.Value, onlineOnly, message);
            }

            evt.Finish();
            return result;
        }

        /// <summary>
        /// Sends message for several interviewer groups
        /// </summary>
        /// <param name="interviewerIds">Array with surveys ids </param>
        /// <param name="onlineOnly">If true message will be sent only to online interviewers</param>
        /// <param name="message">Message for sending</param>              
        public static IEnumerable<int> SendMessageToSurveys(IEnumerable<int> surveyIds, Message message)
        {
            var evt = new SendMessageToSurveysEvent(surveyIds);
            IEnumerable<int> result;
            using (TransferBatch batch = TransferBatch.Create())
            {
                batch.Insert(surveyIds);
                result = SendMessageToSurveys(batch.Value, message);
            }

            evt.Finish();

            return result;
        }

        private static void SendMessageToInterviewers(int batchId, bool onlineOnly, Message message)
        {
            BvSpSendMessageToInterviewersAdapter.ExecuteNonQuery(batchId, onlineOnly, message.Body, message.SupervisorName);
            PersonRepository.RefreshCache();
        }

        private static IEnumerable<int> SendMessageToGroups(int batchId, bool onlineOnly, Message message)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var res =  BvSpSendMessageToGroupsAdapter.ExecuteEntityList(batchId, onlineOnly, message.Body, message.SupervisorName, callCenterId).Select(x => (int)x.InterviewerId);
            PersonRepository.RefreshCache();
            return res;
        }

        private static IEnumerable<int> SendMessageToSurveys(int batchId, Message message)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var res =  BvSpSendMessageToSurveysAdapter.ExecuteEntityList(batchId, message.Body, message.SupervisorName, callCenterId).Select(x => (int)x.InterviewerId);
            PersonRepository.RefreshCache();
            return res;
        }

        private string GetUserName(string userName)
        {
            var user = _usersApiService.GetUsersByName(userName).FirstOrDefault();
            string result;
            if (!string.IsNullOrEmpty(user?.FullName))
            {
                result = user.FullName.Replace(",", string.Empty);
            }
            else
            {
                result = userName;
            }
            return result;
        }

        private void NotifyBBCCUsers(IEnumerable<int> bbccInterviewersToNotify, string message, string supervisorName)
        {
            var BBCCUsers = _taskRepository.GetPersonIdsFromBBCC().Intersect(bbccInterviewersToNotify).ToList();

            if (BBCCUsers.Any())
            {
                _interviewerApiClient.NotifyNewMessage(_companyInfo.CompanyId, BBCCUsers, message, supervisorName);
            }
        }

        #endregion
    }
}