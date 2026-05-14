using System;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringTest
{
    /// <summary>
    /// Summary description for BvPersonDeferredMonitoringAdapterExTest
    /// </summary>
    [TestClass]
    public class BvPersonDeferredMonitoringPartAdapterExTest : BaseMonitoringTest
    {

        private static void CompareBvPersonDeferredMonitoringPartEntity(BvPersonDeferredMonitoringPartEntity left, BvPersonDeferredMonitoringPartEntity right)
        { 
            Assert.AreEqual(left.PersonSID, right.PersonSID, "PersonSID");
            Assert.AreEqual(left.InterviewID, right.InterviewID, "InterviewID");
            Assert.AreEqual(left.SurveySID, right.SurveySID, "SurveySID");
            Assert.AreEqual(left.TimeStamp, right.TimeStamp, "TimeStamp");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetMonitoringRecord_ValidId_ReturnsRecord()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity;

            var pe = CreatePartRecordInDatabase(out entity);

            var re = BvPersonDeferredMonitoringPartAdapterEx.GetByIdWithCheck(pe.ID, entity.PersonSID);

            CompareBvPersonDeferredMonitoringPartEntity(pe, re);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetMonitoringRecord_InvalidId_ExceptionIsThrown()
        {
            CreateSurveyPersonInterviewCall();
            BvPersonDeferredMonitoringEntity entity;

            var pe = CreatePartRecordInDatabase(out entity);

            try
            {
                var re = BvPersonDeferredMonitoringPartAdapterEx.GetByIdWithCheck(pe.ID + 1, entity.PersonSID);

                Assert.Fail("Exception was expected but not thrown.");
            }
            catch (Exception ex)
            {
                // Check if it's expected exception

                const string expected = "System.Exception: Deferred record [2] is not found";

                Assert.IsTrue(ex.ToString().Contains(expected),
                    string.Format("Wrong exception message. Expected to be contained: [{0}]. Actual: [{1}]", expected, ex));
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetByCondition_NoConditons1Record_Returns1Record()
        {
            CreateSurveyPersonInterviewCall();

            var et = CreatePartRecordInDatabase();
            var list = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition(null);

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count, "Wrong number of records");

            CompareBvPersonDeferredMonitoringPartEntity(et, list[0]);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetByCondition_NoConditons2Records_Returns2Record()
        {
            CreateSurveyPersonInterviewCall();
            CreatePartRecordInDatabase();
            CreatePartRecordInDatabase();

            var list = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition(null);

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count, "Wrong number of records");
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetByCondition_SimpleConditon2Records_Returns1Record()
        {
            CreateSurveyPersonInterviewCall();
            var en = CreatePartRecordInDatabase();

            BvPersonDeferredMonitoringAdapterEx.UpdateIsRecording(en.ID, false);

            var e2 = CreatePartRecordInDatabase();

            var list = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition("(IsRecording=1)");

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count, "Wrong number of records");

            CompareBvPersonDeferredMonitoringPartEntity(e2, list[0]);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetByCondition_ConditonWithParameter2Records_Returns1Record()
        {
            CreateSurveyPersonInterviewCall();
            var en = CreatePartRecordInDatabase();

            BvPersonDeferredMonitoringAdapterEx.UpdateIsRecording(en.ID, false);

            var en2= CreatePartRecordInDatabase();
            var list = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition("(IsRecording=@IsRecording)", new SqlParameter("@IsRecording", false));

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count, "Wrong number of records");

            CompareBvPersonDeferredMonitoringPartEntity(en, list[0]);

            list = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition("(IsRecording=@IsRecording)", new SqlParameter("@IsRecording", 0) { Value = 0 });

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count, "Wrong number of records");

            CompareBvPersonDeferredMonitoringPartEntity(en2, list[0]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetById_DeferredRecordExistsInDatabase_RecordIsReturned()
        {
            CreateSurveyPersonInterviewCall();
            var en = CreatePartRecordInDatabase();

            var found = BvPersonDeferredMonitoringPartAdapterEx.GetById(en.ID);

            Assert.IsNotNull(found, "Deferred record is not found by id");

            CompareBvPersonDeferredMonitoringPartEntity(en, found);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetById_DeferredRecordDoesntExistInDatabase_NullIsReturned()
        {
            CreateSurveyPersonInterviewCall();
            var en = CreatePartRecordInDatabase();

            var found = BvPersonDeferredMonitoringPartAdapterEx.GetById(en.ID + 1);

            Assert.IsNull(found, "Deferred record shouldn't be found");
        }
    }
}
