using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Framework.Tools;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.FusionInteraction;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Monitoring.Common;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.ConnectionObject;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.ContractImplementation;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringTest
{
    /// <summary>
    /// Summary description for SupervisorProcessorUnitTest
    /// </summary>
    [TestClass]
    public class SupervisorProcessorUnitTest : BaseMonitoringTest
    {

        #region GetInterviewerEvents

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetInterviewerEvents_MonitoringIsNotStarted_ReturnMonitoringEndMessageInArray()
        {
            CreateSurveyPersonInterviewCall();
            CreateIdentities();

            SupervisorProcessor processor = new SupervisorProcessor();
            StateEventInfo[] events = processor.GetInterviewerEvents(identity);
            Assert.IsNotNull(events, "Events' array is not defined.");
            Assert.AreEqual<int>(1, events.Length, "No events should be returned.");
            Assert.AreEqual<MonitoringMessageTypes>(MonitoringMessageTypes.MonitoringEndMessage, events[0].MessageType, "Wrong message type.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetInterviewerEvents_MonitoringIsStartedNoEventWasSaved_ReturnEmptyArray()
        {
            CreateSurveyPersonInterviewCall();
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                "sadmin"
                );

            CreateIdentities();
            identity.MonitoringSessionID = monitoringSessionID;

            SupervisorProcessor processor = new SupervisorProcessor();
            StateEventInfo[] events = processor.GetInterviewerEvents(identity);
            Assert.IsNotNull(events, "Events' array is not defined.");
            Assert.AreEqual<int>(0, events.Length, "No events should be returned.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetInterviewerEvents_MonitoringIsStartedSomeEventWasSaved_ReturnArrayWithEvents()
        {
            CreateSurveyPersonInterviewCall();
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                "sadmin"
                );

            CreateIdentities();
            identity.MonitoringSessionID = monitoringSessionID;

            int eventCount = 2;
            int eventSize = 100;
            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, false);

            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();

            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            SupervisorProcessor processor = new SupervisorProcessor();
            StateEventInfo[] events = processor.GetInterviewerEvents(identity);
            Assert.IsNotNull(events, "Events' array is not defined.");
            Assert.AreEqual<int>(eventInfos.Length, events.Length, "Wrong number of events was returned.");

            #region Check implementation details: maxID is updated.

            BvSpPersonMonitoring_GetLastIDEntity maxIDRecord = BvSpPersonMonitoring_GetLastIDAdapter.ExecuteEntityList(identity.InterviewerID, identity.MonitoringSessionID).FirstOrDefault();
            Assert.IsNotNull(maxIDRecord);
            Assert.IsTrue(maxIDRecord.LastSentID.HasValue);

            #endregion
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetInterviewerEvents_MonitoringIsStartedSomeEventWasSavedAndRecieved_ReturnEmptyArray()
        {
            CreateSurveyPersonInterviewCall();
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                "sadmin"
                );

            CreateIdentities();
            identity.MonitoringSessionID = monitoringSessionID;

            int eventCount = 2;
            int eventSize = 100;
            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, false);

            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();

            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            SupervisorProcessor processor = new SupervisorProcessor();
            StateEventInfo[] events = processor.GetInterviewerEvents(identity);
            Assert.IsNotNull(events, "Events' array is not defined.");
            Assert.AreEqual<int>(eventInfos.Length, events.Length, "Wrong number of events was returned.");

            events = processor.GetInterviewerEvents(identity);
            Assert.IsNotNull(events, "Events' array is not defined.");
            Assert.AreEqual<int>(0, events.Length, "No events should be returned.");
        }

        #endregion GetInterviewerEvents

        #region StopMonitoring

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void StopMonitoring_MonitoringIsNotStarted_NothingHappens()
        {
            CreateSurveyPersonInterviewCall();
            CreateIdentities();
            SupervisorProcessor processor = new SupervisorProcessor();
            processor.StopMonitoring(identity, "sadmin");

        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void StopMonitoring_MonitoringIsStartedByDifferentSupervisor_NothingHappens()
        {
            CreateSurveyPersonInterviewCall();
            string supervisor = "sadmin";
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                supervisor
                );
            CreateIdentities();
            identity.MonitoringSessionID = monitoringSessionID;

            SupervisorProcessor processor = new SupervisorProcessor();
            processor.StopMonitoring(identity, "sadmin1");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void StopMonitoring_MonitoringIsStartedInDifferentSession_NothingHappens()
        {
            CreateSurveyPersonInterviewCall();

            string supervisor = "sadmin";
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                supervisor
                );
            CreateIdentities();

            SupervisorProcessor processor = new SupervisorProcessor();
            processor.StopMonitoring(identity, supervisor);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void StopMonitoring_ValidParameters_AllIsClearedFromDatabase()
        {
            CreateSurveyPersonInterviewCall();

            string supervisor = "sadmin";
            long monitoringSessionID = FusionConnection.Instance.StartMonitoring(
                BackendInstance.Current.ConnectionString,
                personId,
                supervisor
                );
            CreateIdentities();
            identity.MonitoringSessionID = monitoringSessionID;

            int eventCount = 2;
            int eventSize = 100;
            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, false);

            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();

            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            SupervisorProcessor processor = new SupervisorProcessor();
            processor.StopMonitoring(identity, supervisor);

            bool isMonitored = FusionConnection.Instance.IsMonitored(
                BackendInstance.Current.ConnectionString,
                personId
                );

            Assert.IsFalse(isMonitored, "Monitoring isn't stopped.");


            bool isActiveMontoring = FusionConnection.Instance.IsActiveMonitoringSession(
                BackendInstance.Current.ConnectionString,
                personId,
                monitoringSessionID
                );

            Assert.IsFalse(isMonitored, "Monitoring is active.");

            #region Check implementation details: all records are removed from the database.

            var oevents = BvPersonMonitoringEventsAdapter.GetByCondition(
                "[MonitoringSessionID]=@MonitoringSessionID",
                new[] { new SqlParameter("@MonitoringSessionID", monitoringSessionID) });
            Assert.IsTrue(oevents.Count == 0, "Events are not deleted.");

            var orecord = BvPersonMonitoringAdapter.GetByCondition(
                "[MonitoringSessionID]=@MonitoringSessionID",
                new[] { new SqlParameter("@MonitoringSessionID", monitoringSessionID) });
            Assert.IsTrue(orecord.Count == 0, "The monitoring record is not deleted.");

            var olastevent = BvPersonMonitoringLastIDAdapter.GetByCondition("[MonitoringSessionID]=@MonitoringSessionID",
                new[] { new SqlParameter("@MonitoringSessionID", monitoringSessionID) });
            Assert.IsTrue(olastevent.Count == 0, "The last id record is not deleted.");

            #endregion

        }

        #endregion StopMonitoring

        #region GetVideoFile
        [TestMethod, Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(FaultException<Exception>))]
        public void GetVideoFile_NoDeferredMonitoringRecord_ThrowException()
        {
            CreateSurveyPersonInterviewCall();
            SupervisorProcessor processor = new SupervisorProcessor();
            FileResponse response = processor.GetVideoFile(0, 0, 0, 0);

        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetVideoFile_MonitoringRecordExistsCountIsZero_ReturnDataLength()
        {
            CreateSurveyPersonInterviewCall();
            CreateIdentities();
            int eventCount = 2;
            int eventSize = 100;

            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, true);
            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();
            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            int monitoringRecordId = GetDeferredMonitoringRecordIdBySessionId(deferredIdentity.DeferredSessionID);

            SupervisorProcessor processor = new SupervisorProcessor();
            FileResponse response = processor.GetVideoFile(0, monitoringRecordId, 0, 0);
            Assert.IsNotNull(response, "Response is not defined.");
            Assert.IsTrue(response.Total > eventSize * eventCount, "Wrong data size.");
            Assert.IsNull(response.Data, "Data should not be returned.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetVideoFile_MonitoringRecordExistsCountIsNotZero_ReturnDataLimitedByCount()
        {
            CreateSurveyPersonInterviewCall();
            CreateIdentities();
            int eventCount = 2;
            int eventSize = 100;
            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, true);

            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();
            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            int monitoringRecordId = GetDeferredMonitoringRecordIdBySessionId(deferredIdentity.DeferredSessionID);

            SupervisorProcessor processor = new SupervisorProcessor();
            FileResponse response = processor.GetVideoFile(0, monitoringRecordId, 0, eventSize);
            Assert.IsNotNull(response, "Response is not defined.");
            Assert.IsTrue(response.Total > eventSize * eventCount, "Wrong data size.");
            Assert.IsNotNull(response.Data, "Data should is not defined.");
            Assert.IsTrue(response.Data.Length == eventSize, "Wrong data size.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetVideoFile_MonitoringRecordExistsCountIsNotZeroFromIsNotZero_ReturnDataFromDefinedPosition()
        {
            CreateSurveyPersonInterviewCall();
            CreateIdentities();
            int eventCount = 2;
            int eventSize = 100;
            StateEventInfo[] eventInfos = CreateSomeEvents(eventCount, eventSize, true);

            DatabaseConnectionObject databaseConnection = new DatabaseConnectionObject();
            databaseConnection.SaveEvents(identity, eventInfos);

            //TODO. DeferredMonitoringFileCreator uses ThreadPool.QueueUserWorkItem insted of AsyncManager,
            //so it cannot be faked. Bad solution just wait 3 secs is used. Need to change this in Boomer.
            Thread.Sleep(3000);

            int monitoringRecordId = GetDeferredMonitoringRecordIdBySessionId(deferredIdentity.DeferredSessionID);

            SupervisorProcessor processor = new SupervisorProcessor();

            FileResponse preResponse = processor.GetVideoFile(0, monitoringRecordId, 0, int.MaxValue);
            Assert.IsNotNull(preResponse, "PreResponse is not defined.");
            FileResponse response = processor.GetVideoFile(0, monitoringRecordId, eventSize, int.MaxValue);
            Assert.IsNotNull(response, "Response is not defined.");
            Assert.IsTrue(response.Total == preResponse.Total, "Wrong total.");
            Assert.IsNotNull(response.Data, "Data should is not defined.");
            Assert.IsTrue(response.Data.Length == preResponse.Total - eventSize, "Wrong data size.");
            Assert.IsTrue(preResponse.Data.Skip(eventSize).SequenceEqual(response.Data), "Wrong data");
        }

        #endregion

    }
}
