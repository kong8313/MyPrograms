using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DAL
{
    [TestClass]
    public class TableAdaptersTest
    {
        #region Initialize and Cleanup methods

        IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            framework.TestCleanup();
        }

        #endregion

        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public void TableAdapter_DeleteAndOutput_WithCondition_TableHasTimeStampColumn()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            BvDialersAdapter.Insert(entity1);
            BvDialersAdapter.Insert(entity2);

            var deletedRows = BvDialersAdapter.DeleteByConditionAndOutput(
                "Name = @Name",
                new SqlParameter("@Name", "E1"));

            Assert.AreEqual(1, deletedRows.Count);
            Assert.AreEqual("E1", deletedRows[0].Name);
        }

        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public void TableAdapter_DeleteAndOutput_WithNoCondition_TableHasTimeStampColumn()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            BvDialersAdapter.Insert(entity1);
            BvDialersAdapter.Insert(entity2);

            var deletedRows = BvDialersAdapter.DeleteByConditionAndOutput(null);

            Assert.AreEqual(2, deletedRows.Count);
            Assert.AreEqual("E1", deletedRows[0].Name);
            Assert.AreEqual("E2", deletedRows[1].Name);
        }

        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public void TableAdapter_UpdateAndOutput_WithCondition_TableHasTimeStampColumn()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            var entity3 = new BvDialersEntity() { Id = 3, Name = "E3" };
            BvDialersAdapter.Insert(entity1);
            BvDialersAdapter.Insert(entity3);

            var updatedRows = BvDialersAdapter.UpdateByConditionAndOutput(
                entity2,
                "Name = @NameValue",
                new SqlParameter("@NameValue", "E3"));

            Assert.AreEqual(1, updatedRows.Count);
            Assert.AreEqual("E2", updatedRows[0].Name);
        }

        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public void TableAdapter_UpdateAndOutput_WithNoCondition_TableHasTimeStampColumn()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            var entity3 = new BvDialersEntity() { Id = 3, Name = "E3" };
            BvDialersAdapter.Insert(entity1);
            BvDialersAdapter.Insert(entity2);

            var updatedRows = BvDialersAdapter.UpdateByConditionAndOutput(entity3, null);

            Assert.AreEqual(2, updatedRows.Count);

            Assert.AreEqual("E3", updatedRows[0].Name);
            Assert.AreEqual("E3", updatedRows[1].Name);
        }

        [TestMethod,  Owner(@"FIRM\EgorS")]
        public void TableAdapter_DeleteAndOutput_WithCondition()
        {
            var entity1 = new BvPersonEntity() { SID = 1, Name = "E1", PwdSetDate = DateTime.UtcNow };
            var entity2 = new BvPersonEntity() { SID = 2, Name = "E2", PwdSetDate = DateTime.UtcNow };
            BvPersonAdapter.Insert(entity1);
            BvPersonAdapter.Insert(entity2);

            var deletedRows = BvPersonAdapter.DeleteByConditionAndOutput(
                "Name = @Name",
                new SqlParameter("@Name", "E1"));

            Assert.AreEqual(1, deletedRows.Count);
            Assert.AreEqual("E1", deletedRows[0].Name);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void TableAdapter_DeleteAndOutput_WithNoCondition()
        {
            var entity1 = new BvPersonEntity() { SID = 1, Name = "E1", PwdSetDate = DateTime.UtcNow };
            var entity2 = new BvPersonEntity() { SID = 2, Name = "E2", PwdSetDate = DateTime.UtcNow };
            BvPersonAdapter.Insert(entity1);
            BvPersonAdapter.Insert(entity2);

            var deletedRows = BvPersonAdapter.DeleteByConditionAndOutput(null);

            Assert.AreEqual(2, deletedRows.Count);
            Assert.AreEqual("E1", deletedRows[0].Name);
            Assert.AreEqual("E2", deletedRows[1].Name);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void TableAdapter_UpdateAndOutput_WithCondition()
        {
            var entity1 = new BvPersonEntity() { SID = 1, Name = "E1", PwdSetDate = DateTime.UtcNow };
            var entity2 = new BvPersonEntity() { SID = 2, Name = "E2", PwdSetDate = DateTime.UtcNow };
            var entity3 = new BvPersonEntity() { SID = 3, Name = "E3", PwdSetDate = DateTime.UtcNow };
            BvPersonAdapter.Insert(entity1);
            BvPersonAdapter.Insert(entity3);

            var updatedRows = BvPersonAdapter.UpdateByConditionAndOutput(
                entity2,
                "Name = @NameValue",
                new SqlParameter("@NameValue", "E3"));

            Assert.AreEqual(1, updatedRows.Count);
            Assert.AreEqual("E2", updatedRows[0].Name);
        }

        // the most useless test our adapters. does not work with identity, timestamp, unique indexes 
        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public void TableAdapter_UpdateAndOutput_WithNoCondition()
        {
            var entity1 = new BvFilterFieldsEntity() { ID = 1, Value = "E1"};
            var entity2 = new BvFilterFieldsEntity() { ID = 2, Value = "E2" };
            var entity3 = new BvFilterFieldsEntity() { ID = 3, Value = "E3" };
            BvFilterFieldsAdapter.Insert(entity1);
            BvFilterFieldsAdapter.Insert(entity2);

            var updatedRows = BvFilterFieldsAdapter.UpdateByConditionAndOutput(entity3, null);

            Assert.AreEqual(2, updatedRows.Count);

            Assert.AreEqual("E3", updatedRows[0].Value);
            Assert.AreEqual("E3", updatedRows[1].Value);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TableAdapter_Merge_EntityIsInserted()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            BvDialersAdapter.Insert(entity1);

            BvDialersAdapter.Merge(entity2);

            var all = BvDialersAdapter.GetAll().OrderBy(x => x.Id).ToArray();

            Assert.AreEqual(2, all.Length);

            Assert.AreEqual("E1", all[0].Name);
            Assert.AreEqual("E2", all[1].Name);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TableAdapter_Merge_EntityIsUpdated()
        {
            var entity1 = new BvDialersEntity() { Id = 1, Name = "E1" };
            var entity2 = new BvDialersEntity() { Id = 2, Name = "E2" };
            BvDialersAdapter.Insert(entity1);
            BvDialersAdapter.Insert(entity2);

            entity2.Name = "E3";

            BvDialersAdapter.Merge(entity2);

            var all = BvDialersAdapter.GetAll().OrderBy(x => x.Id).ToArray();

            Assert.AreEqual(2, all.Length);

            Assert.AreEqual("E1", all[0].Name);
            Assert.AreEqual("E3", all[1].Name);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TableAdapter_Merge_EntityNotUpdatedBecauseThereAreNotUpdatableFields()
        {
            Assert.IsFalse(BvSurveyAssignmentOnCallCenterAdapter.IsUpdateSupported);
            var entity1 = new BvSurveyAssignmentOnCallCenterEntity() { SurveyId = 10, CallCenterId = 1};
            BvSurveyAssignmentOnCallCenterAdapter.Insert(entity1);

            BvSurveyAssignmentOnCallCenterAdapter.Merge(entity1);

            var all = BvSurveyAssignmentOnCallCenterAdapter.GetAll();

            Assert.AreEqual(1, all.Count);

            Assert.AreEqual(10, all[0].SurveyId);
            Assert.AreEqual(1, all[0].CallCenterId);
        }
    }
}
