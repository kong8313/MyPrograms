using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.PersonImport;

using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.Telephony;
using DialerCommon;

namespace Confirmit.CATI.Core.SupervisorService
{
    public class SupervisorServiceClient : ISupervisorServiceClient
    {
        private static readonly ChannelFactoryWrapper<ISupervisorService> ChannelFactory =
            new ChannelFactoryWrapper<ISupervisorService>(new SupervisorServiceChannelFactoryWrapperConfiguration(), new CatiLogger());

        private static void DoServiceCall(Action<ISupervisorService> action, [CallerMemberName]string methodName = "")
        {
            CheckTransaction(methodName);
            ChannelFactory.Execute(action, methodName);
        }

        private static T DoServiceCall<T>(Func<ISupervisorService, T> function, [CallerMemberName]string methodName = "")
        {
            CheckTransaction(methodName);
            return ChannelFactory.Execute(function, methodName);
        }

        private static void CheckTransaction(string methodName)
        {
            if (DatabaseTransactionScope.Current != null)
            {
                string serviceAndMethodName = WcfExecutor.GetServiceAndMethodName<ISupervisorService>(methodName);

                Trace.TraceWarning(
                    "Web service method '{0}' is called inside transaction scope '{1}'.",
                    serviceAndMethodName,
                    DatabaseTransactionScope.Current.TransactionName);
            }
        }

        public void SendMessage(FeedbackForm mailMessage)
        {
            DoServiceCall(x => x.SendMessage(mailMessage));
        }

        public void ConfigureInboundDdiNumbers(int dialerId)
        {
            DoServiceCall(x => x.ConfigureInboundDdiNumbers(dialerId));
        }

        public void OpenSurvey(int surveySid)
        {
            DoServiceCall(x => x.OpenSurvey(surveySid));
        }

        public void CloseSurvey(int surveySid)
        {
            DoServiceCall(x => x.CloseSurvey(surveySid));
        }

        public void ShutdownSurvey(int surveySid)
        {
            DoServiceCall(x => x.ShutdownSurvey(surveySid));
        }

        public BvTasksEntity TerminateTaskByPerson(int personSid, CallOutcome? explicitIts)
        {
            return DoServiceCall(x => x.TerminateTaskByPerson(personSid, explicitIts));
        }

        public void TerminateTasksByDialerId(int dialerId)
        {
            DoServiceCall(x => x.TerminateTasksByDialerId(dialerId));
        }

        public void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            DoServiceCall(x => x.SetDialerDefaultSurveyParameters(parameters));
        }

        public void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            DoServiceCall(x => x.ValidateDialerSurveyParameters(parameters));
        }

        public void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {
            DoServiceCall(x => x.SetDialerSurveyParameters(surveySid, parameters));
        }

        public bool EnableDialer(int dialerId)
        {
            return DoServiceCall(x => x.EnableDialer(dialerId));
        }

        public bool DisableDialer(int dialerId)
        {
            return DoServiceCall(x => x.DisableDialer(dialerId));
        }

        public void SetPersonParentGroups(int personSid, int[] parentGroupsSids)
        {
            DoServiceCall(x => x.SetPersonParentGroups(personSid, parentGroupsSids));
        }
        
        public void DeletePersons(List<int> personSids)
        {
            DoServiceCall(x => x.DeletePersons(personSids));
        }
        
        public void LockPersonsBySupervisor(List<int> personSids)
        {
            DoServiceCall(x => x.LockPersonsBySupervisor(personSids));
        }

        public bool IsDialerOperational(int dialerId)
        {
            return DoServiceCall(x => x.IsDialerOperational(dialerId));
        }

        public void SaveSchedule(int scheduleSid, string serializedSchedule)
        {
            DoServiceCall(x => x.SaveSchedule(scheduleSid, serializedSchedule));
        }

        public void LaunchSchedule(int scheduleSid)
        {
            DoServiceCall(x => x.LaunchSchedule(scheduleSid));
        }

        public void CheckSchedule(string serializedSchedule)
        {
            DoServiceCall(x => x.CheckSchedule(serializedSchedule));
        }

        public bool Schedule()
        {
            return DoServiceCall(x => x.Schedule());
        }

        public void StartMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {
            DoServiceCall(x => x.StartMonitor(supervisorName, interviewerId, telephoneNumber));
        }

        public void StopMonitor(string supervisorName, int interviewerId)
        {
            DoServiceCall(x => x.StopMonitor(supervisorName, interviewerId));
        }

        public void CreateOrUpdatePerson(
            int callCenterId,
            int personSid,
            string name,
            string description,
            string fullName,
            string password,
            AgentTaskChoiceMode mode,
            PersonAssignmentListMode assignmentListMode,
            TaskChoicePermissions? permissions,
            List<int> parentGroups,
            int? autoSurveyId,
            int callGroupId,
            string location,
            DialType dialType,
            AgentType agentType,
            bool enableSoftphoneIntegration = true,
            string[] attributes = null)
        {
            DoServiceCall(
                x => x.CreateOrUpdatePerson(
                    callCenterId,
                    personSid,
                    name,
                    description,
                    fullName,
                    password,
                    mode,
                    assignmentListMode,
                    permissions,
                    parentGroups,
                    autoSurveyId,
                    callGroupId,
                    location,
                    dialType,
                    agentType,
                    enableSoftphoneIntegration,
                    attributes));
        }

        public IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveyId, int interviewId)
        {
            return DoServiceCall(x => x.GetInterviewRecordings(surveyId, interviewId));
        }

        public bool[] AreRecordsExists(int surveySid, int[] interviewIds)
        {
            return DoServiceCall(x => x.AreRecordsExists(surveySid, interviewIds));
        }

        public ImportResult ImportPersons(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {
            return DoServiceCall(x => x.ImportPersons(callCenterID, dataTable, columnRoleMap, importOptions));
        }

        public IEnumerable<LogFileInfo> GetLogFiles(int dialerId)
        {
            return DoServiceCall(x => x.GetLogFiles(dialerId));
        }

        public byte[] GetLogFileBodyZipped(int dialerId, string fileName)
        {
            return DoServiceCall(x => x.GetLogFileBodyZipped(dialerId, fileName));
        }

        public string GetDialerVersion(int dialerId)
        {
            return DoServiceCall(x => x.GetDialerVersion(dialerId));
        }

        public DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId)
        {
            return DoServiceCall(x => x.GetAvailableExtendedFunctionality(dialerId));
        }

        public DialerFeatures GetDialerSupportedFeatures(int dialerId)
        {
            return DoServiceCall(x => x.GetDialerSupportedFeatures(dialerId));
        }

        public IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId)
        {
            return DoServiceCall(x => x.GetOverridenDialerSupportedFeatures(dialerId));
        }

        public void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {
            DoServiceCall(x => x.UpdateOverridenDialerSupportedFeature(dialerId, featureName, overridenFeatureValue));
        }
    }
}
