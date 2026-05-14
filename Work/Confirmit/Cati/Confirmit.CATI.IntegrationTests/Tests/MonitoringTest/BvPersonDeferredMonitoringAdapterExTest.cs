using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Linq;
using DeferredService = Confirmit.CATI.Core.Services.DeferredMonitoringService;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringTest
{
    /// <summary>
    /// Summary description for BvPersonDeferredMonitoringAdapterExTest
    /// </summary>
    [TestClass]
    public class BvPersonDeferredMonitoringAdapterExTest : BaseMonitoringTest
    {
    
        private static void CompareBvPersonDeferredMonitoringEntity(BvPersonDeferredMonitoringEntity left, BvPersonDeferredMonitoringEntity right)
        { 
            Assert.AreEqual(left.PersonSID, right.PersonSID, "PersonSID");
            Assert.AreEqual(left.InterviewID, right.InterviewID, "InterviewID");
            Assert.AreEqual(left.SurveySID, right.SurveySID, "SurveySID");
            Assert.AreEqual(left.TimeStamp, right.TimeStamp, "TimeStamp");
            Assert.AreEqual(left.HasAudio, right.HasAudio, "HasAudio");
            Assert.IsTrue(left.EventsFile.Except(right.EventsFile).Count() == 0, "EventsFile");            
            Assert.AreEqual(left.StartingFile, right.StartingFile, "StartingFile");
            Assert.AreEqual(left.IsRecording, right.IsRecording, "IsRecording");
            Assert.AreEqual(left.IsComplete, right.IsComplete, "IsComplete");
            Assert.AreEqual(left.ClientTimeUtc, right.ClientTimeUtc, "IsComplete");
            Assert.AreEqual(left.ServerTimeUtc, right.ServerTimeUtc, "IsComplete");    
        }


        #region Insert

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void Insert_ValidEntity_RecordInserted()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity;
            BvPersonDeferredMonitoringEntity insertedEntity = CreateRecordInDatabase(out entity);

            CompareBvPersonDeferredMonitoringEntity(entity, insertedEntity);

        }
        #endregion

        #region UpdateIsComplete

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void UpdateIsComplete_IsCompleteTrue_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity insertedEntity = CreateRecordInDatabase();
            Assert.IsFalse(insertedEntity.IsComplete, "Initinally IsComplete is not false.");

            BvPersonDeferredMonitoringAdapterEx.UpdateIsComplete(insertedEntity.ID, true);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(insertedEntity.ID);
            Assert.IsTrue(updatedEntity.IsComplete, "IsComplete was not changed.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void UpdateIsComplete_IsCompleteFalse_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity insertedEntity = CreateRecordInDatabase(ent => { ent.IsComplete = true; });
            Assert.IsTrue(insertedEntity.IsComplete, "Initinally IsComplete is not true.");

            BvPersonDeferredMonitoringAdapterEx.UpdateIsComplete(insertedEntity.ID, false);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(insertedEntity.ID);
            Assert.IsFalse(updatedEntity.IsComplete, "IsComplete was not changed.");
        }

        #endregion

        #region UpdateIsRecording

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void UpdateIsRecording_IsRecordingTrue_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            var insertedEntity = CreateRecordInDatabase(ent =>
            {
                ent.IsRecording = false;
            });
            Assert.IsFalse(insertedEntity.IsRecording, "Initinally IsRecording is not false.");

            BvPersonDeferredMonitoringAdapterEx.UpdateIsRecording(insertedEntity.ID, true);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(insertedEntity.ID);
            Assert.IsTrue(updatedEntity.IsRecording, "IsRecording was not changed.");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void UpdateIsRecording_IsRecordingFalse_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            var insertedEntity = CreateRecordInDatabase();
            Assert.IsTrue(insertedEntity.IsRecording, "Initinally IsRecording is not true.");

            BvPersonDeferredMonitoringAdapterEx.UpdateIsRecording(insertedEntity.ID, false);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(insertedEntity.ID);
            Assert.IsFalse(updatedEntity.IsRecording, "IsRecording was not changed.");
        }

        #endregion

        #region AppendToEventsFile

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AppendToEventsFile_ValidPacket_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity = CreateRecordInDatabase();

            var ta = new byte[100];
            ta[99] = 99;
            ta[50] = 50;
            ta[01] = 01;
            BvPersonDeferredMonitoringAdapterEx.AppendToEventsFile(entity.ID, ta);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(entity.ID);
            Assert.IsTrue(ta.Except(updatedEntity.EventsFile).Count() == 0, "EventsFile");
            Assert.IsTrue(updatedEntity.EventsFile.Length==ta.Length, "Wrong data length.");
            Assert.IsTrue(ta.Except(updatedEntity.EventsFile).Count() == 0, "EventsFile");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AppendToEventsFile_NotExistingRecord_NothingHappens()
        {
            CreateSurveyPersonInterviewCall();
            var ta = new byte[100];
            BvPersonDeferredMonitoringAdapterEx.AppendToEventsFile(54190782, ta);            
        }

        #endregion


        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void CompleteDeferredMonitoringRecord_ValidPacket_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity = CreateRecordInDatabase();

            var ta = new byte[100];
            ta[99] = 99;
            ta[50] = 50;
            ta[01] = 01;
            BvPersonDeferredMonitoringAdapterEx.CompleteDeferredMonitoringRecord(entity.ID, ta, true, true, 1);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(entity.ID);
            
            Assert.IsNotNull(updatedEntity.EventsFile);
            Assert.IsTrue(updatedEntity.EventsFile.Length == ta.Length, "Wrong data length.");
            Assert.IsTrue(ta.Except(updatedEntity.EventsFile).Count() == 0, "EventsFile");

            Assert.IsTrue(updatedEntity.IsComplete);
            Assert.IsTrue(updatedEntity.HasAudio);
            Assert.IsTrue(updatedEntity.RequestAudio);
            Assert.AreEqual(1, updatedEntity.InterviewDuration);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void CompleteDeferredMonitoringRecord_NoAudioEmptyPacket_RecordUpdated()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity = CreateRecordInDatabase();

            var ta = new byte[0];
            BvPersonDeferredMonitoringAdapterEx.CompleteDeferredMonitoringRecord(entity.ID, ta, false, true, 2);

            BvPersonDeferredMonitoringEntity updatedEntity = GetPersonDeferredMonitoringEntityByIdWithCheck(entity.ID);

            Assert.IsNotNull(updatedEntity.EventsFile);
            Assert.IsTrue(updatedEntity.EventsFile.Length == ta.Length, "Wrong data length.");
            
            Assert.IsTrue(updatedEntity.IsComplete);
            Assert.IsFalse(updatedEntity.HasAudio);
            Assert.IsTrue(updatedEntity.RequestAudio);
            Assert.AreEqual(2, updatedEntity.InterviewDuration);
        }


        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void IsThereAnyNonemptyMonitoringRecords_NoRecords_ReturnFalse()
        {
            CreateSurveyPersonInterviewCall();
            bool res = BvPersonDeferredMonitoringAdapterEx.AreThereAnyNonEmptyMonitoringRecords(0, 0);

            Assert.IsFalse(res);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void IsThereAnyNonemptyMonitoringRecords_ExistingNonemptyRecord_ReturnTrue()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity = CreateRecordInDatabase();
            bool res = BvPersonDeferredMonitoringAdapterEx.AreThereAnyNonEmptyMonitoringRecords(entity.PersonSID, entity.ID);

            Assert.IsTrue(res);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsThereAnyNonemptyMonitoringRecords_ExistingEmptyRecord_ReturnFalse()
        {
            CreateSurveyPersonInterviewCall();
            var entity = CreateIdentitiesAndEmptyRecord();
            bool res = BvPersonDeferredMonitoringAdapterEx.AreThereAnyNonEmptyMonitoringRecords(entity.PersonSID, entity.ID);

            Assert.IsFalse(res);
        }
    }
}
