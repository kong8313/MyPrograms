using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement.Fakes;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Tests.Services
{
    [TestClass]
    public class SupervisorSettingsRepositoryTest : BaseMockedIntegrationTest
    {
        public const string UserName = "Super1";
        public const int SurveyId = 123;

        [TestMethod, Owner(@"Firm\EgorS")]
        public void SupervisorSettingsRepository_ReadWhenSettingsAreNotSaved_NotNullReturned()
        {
            var supervisorProvider = new StubISupervisorNameProvider();
            supervisorProvider.NameGet = () => UserName;
            var repository = new SupervisorSettingsRepository(supervisorProvider, new StubICallManagementViewsProvider(), new StubIConnectionStrings());

            var settings = repository.ReadQuotaSettings(SurveyId);

            Assert.IsNotNull(settings);
        }

        [TestMethod, Owner(@"Firm\EgorS")]
        public void SupervisorSettingsRepository_WriteAndReadSettings_ActualSettingsObjectReturned()
        {
            var supervisorProvider = new StubISupervisorNameProvider();
            supervisorProvider.NameGet = () => UserName;
            var repository = new SupervisorSettingsRepository(supervisorProvider, new StubICallManagementViewsProvider(), new StubIConnectionStrings());

            var writeSettings = new QuotaPageViewSettings();
            writeSettings.NumberOfColumns = 12345;
            writeSettings.QuotasExclusion.Add("Quota_Exclusion_1");
            writeSettings.QuotasOrder.Add("Quota_Order_1");

            repository.WriteQuotaSettings(SurveyId, writeSettings);

            var readSettings = repository.ReadQuotaSettings(SurveyId);

            Assert.IsNotNull(readSettings);
            Assert.AreEqual(12345, readSettings.NumberOfColumns);
            Assert.AreEqual("Quota_Exclusion_1", readSettings.QuotasExclusion.SingleOrDefault());
            Assert.AreEqual("Quota_Order_1", readSettings.QuotasOrder.SingleOrDefault());
        }

        [TestMethod, Owner(@"Firm\EgorS")]
        public void SupervisorSettingsRepository_WriteSettingsTwiceAndReadSettings_ActualSettingsObjectReturned()
        {
            var supervisorProvider = new StubISupervisorNameProvider();
            supervisorProvider.NameGet = () => UserName;
            var repository = new SupervisorSettingsRepository(supervisorProvider, new StubICallManagementViewsProvider(), new StubIConnectionStrings());

            var writeSettings = new QuotaPageViewSettings();
            writeSettings.NumberOfColumns = 12345;
            writeSettings.QuotasExclusion.Add("Quota_Exclusion_1");
            writeSettings.QuotasOrder.Add("Quota_Order_1");
            repository.WriteQuotaSettings(SurveyId, writeSettings);

            writeSettings.NumberOfColumns = 999;
            repository.WriteQuotaSettings(SurveyId, writeSettings);

            var readSettings = repository.ReadQuotaSettings(SurveyId);

            Assert.IsNotNull(readSettings);
            Assert.AreEqual(999, readSettings.NumberOfColumns);
            Assert.AreEqual("Quota_Exclusion_1", readSettings.QuotasExclusion.SingleOrDefault());
            Assert.AreEqual("Quota_Order_1", readSettings.QuotasOrder.SingleOrDefault());
        }

        [TestMethod]
        public void SupervisorSettingsRepository_CallManagement_SaveSettings_should_result_in_equal_result()
        {
            // arrange
            var supervisorProvider = new StubISupervisorNameProvider();
            supervisorProvider.NameGet = () => UserName;
            var repository = new SupervisorSettingsRepository(supervisorProvider, new StubICallManagementViewsProvider(), new StubIConnectionStrings());

            var settings = new CallManagementColumnSettings();
            settings.Columns = new Dictionary<string, List<ColumnSetting>>();
            settings.Columns["View1"] = new List<ColumnSetting> { new ColumnSetting { Key = "Col1", Width = 123 } };
            settings.Columns["View2"] = new List<ColumnSetting> { new ColumnSetting { Key = "Col2", Width = 223 } };
            // act
            repository.WriteCallManagementColumnSettings(settings);
            var resultSettings = repository.ReadCallManagementColumnSettings();
            // assert
            CollectionAssert.AreEquivalent(settings.Columns.Keys, resultSettings.Columns.Keys);
            CollectionAssert.AreEquivalent(settings.Columns["View1"].Select(x => x.Key).ToList(), resultSettings.Columns["View1"].Select(x => x.Key).ToList());
            CollectionAssert.AreEquivalent(settings.Columns["View1"].Select(x => x.Width).ToList(), resultSettings.Columns["View1"].Select(x => x.Width).ToList());
            CollectionAssert.AreEquivalent(settings.Columns["View2"].Select(x => x.Key).ToList(), resultSettings.Columns["View2"].Select(x => x.Key).ToList());
            CollectionAssert.AreEquivalent(settings.Columns["View2"].Select(x => x.Width).ToList(), resultSettings.Columns["View2"].Select(x => x.Width).ToList());
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void SupervisorSettingsRepository_ReadWhenCustomViewsSettingsAreNotSaved_ReturnedDefaultCustomViews()
        {
            var repository = new SupervisorSettingsRepository(new StubISupervisorNameProvider(), new CallManagementViewsProvider(), new StubIConnectionStrings());

            var settings = repository.ReadCallManagementViews();

            Assert.AreEqual(5, settings.Views.Count, "ReadCallManagementViews work incorrect if table doesn't contain information about vustom views");
        }
        
        [TestMethod, Owner(@"firm\grigoryk")]
        public void SupervisorSettingsRepository_ReadAndWriteCustomViewsSettings_DataIsCorrectAndMisssedColumnsWereAddedToCustomView()
        {
            var callManagementViewProvider = new CallManagementViewsProvider();
            var repository = new SupervisorSettingsRepository(new StubISupervisorNameProvider(), callManagementViewProvider, new StubIConnectionStrings());
            
            CallManagementViews writeSettings = callManagementViewProvider.GetDefaultViews();

            writeSettings.Views.Add(new CallManagementView
            {
                Name = "Test",
                Columns = new List<CallManagementColumn>
                {
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.ApptTimeText, IsVisible = true},
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.QuestionColumnsPosition, IsVisible = false}
                },
                IsDefault = true
            });

            repository.WriteCallManagementViews(writeSettings);

            var readSettings = repository.ReadCallManagementViews();

            Assert.AreEqual(6, readSettings.Views.Count);
            Assert.AreEqual("Scheduled", readSettings.Views[0].Name);
            Assert.AreEqual("High priority", readSettings.Views[1].Name);
            Assert.AreEqual("Not Scheduled", readSettings.Views[2].Name);
            Assert.AreEqual("All", readSettings.Views[3].Name);
            Assert.AreEqual("Sent to dialer", readSettings.Views[4].Name);
            Assert.AreEqual("Test", readSettings.Views[5].Name);
            Assert.AreEqual(true, readSettings.Views[5].IsDefault);
            Assert.AreEqual(18, readSettings.Views[5].Columns.Count);
            Assert.AreEqual(true, readSettings.Views[5].Columns[0].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.ApptTimeText, readSettings.Views[5].Columns[0].ColumnKey);
            Assert.AreEqual(false, readSettings.Views[5].Columns[1].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.QuestionColumnsPosition, readSettings.Views[5].Columns[1].ColumnKey);
            Assert.AreEqual(false, readSettings.Views[5].Columns[2].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.InterviewID, readSettings.Views[5].Columns[2].ColumnKey);
            Assert.AreEqual(false, readSettings.Views[5].Columns[17].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.CallState, readSettings.Views[5].Columns[17].ColumnKey);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void SupervisorSettingsRepository_WriteSettingsWithDefaultAndCustomViews_OnlyCustomSettignsInDatabase()
        {
            var callManagementViewProvider = new CallManagementViewsProvider();
            var repository = new SupervisorSettingsRepository(new StubISupervisorNameProvider(), callManagementViewProvider, new StubIConnectionStrings());

            CallManagementViews writeSettings = callManagementViewProvider.GetDefaultViews();

            writeSettings.Views.Add(new CallManagementView
            {
                Name = "Test",
                Columns = new List<CallManagementColumn>
                {
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.ApptTimeText, IsVisible = true},
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.QuestionColumnsPosition, IsVisible = false}
                },
                IsDefault = true
            });

            repository.WriteCallManagementViews(writeSettings);

            var entity = BvSupervisorSettingsAdapter.GetByCondition("[SettingType] = @SettingType",
                    new SqlParameter("@SettingType", SupervisorSettingType.CallManagementCustomViews))
                .FirstOrDefault();

            Assert.IsNotNull(entity);
            var settingsFromDatabase = JsonConvert.DeserializeObject<CallManagementViews>(entity.Settings);

            Assert.AreEqual(1, settingsFromDatabase.Views.Count);
            Assert.AreEqual("Test", settingsFromDatabase.Views[0].Name);
            Assert.AreEqual(true, settingsFromDatabase.Views[0].IsDefault);
            Assert.AreEqual(2, settingsFromDatabase.Views[0].Columns.Count);
            Assert.AreEqual(true, settingsFromDatabase.Views[0].Columns[0].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.ApptTimeText, settingsFromDatabase.Views[0].Columns[0].ColumnKey);
            Assert.AreEqual(false, settingsFromDatabase.Views[0].Columns[1].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.QuestionColumnsPosition, settingsFromDatabase.Views[0].Columns[1].ColumnKey);
        }
    }
}
