using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;
using Confirmit.CATI.Supervisor.Core.Activity.Fakes;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class SurveyActivityViewCustomizableColumnsServiceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();

            ISupervisorSettingsRepository stubISupervisorSettingsRepository = new StubISupervisorSettingsRepository
            {
                ReadSurveyActivityViewColumnSettings = () => new List<ColumnDescription>()
            };

            IActivityManager stubIActivityManager = new StubIActivityManager
            {
                GetStatusAlertsListBoolean = (i) => new List<StatusAlertInfo>()
            };

            var serviceceRegistrator = ServiceLocator.Resolve<IServiceRegistrator>();
            serviceceRegistrator
                .Register<ICustomizableColumnsService, SurveyActivityViewCustomizableColumnsService>()
                .RegisterInstance(stubISupervisorSettingsRepository)
                .RegisterInstance(stubIActivityManager);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ServiceLocator.StaticCleanup();
        }

        [TestMethod]
        public void GetGridFields_should_merge_columns_correctly()
        {
            // arrange
            var columnService = ServiceLocator.Resolve<ICustomizableColumnsService>();
            // act
            var defaultFields = columnService.GetGridFields();
            var changedColumnKey = defaultFields[0].DataField;
            columnService.SaveColumnSettings(new List<GridColumnSetting> {new GridColumnSetting { Key = "Test123", Active = true }, new GridColumnSetting { Key = changedColumnKey, Active = false } });
            // assert
            var mergedColumns = columnService.GetGridFields();

            Assert.IsFalse(mergedColumns.First(x => x.DataField == changedColumnKey).Visible);
            Assert.IsNull(mergedColumns.FirstOrDefault(x => x.DataField == "Test123")); // do not add column if it's not in predefined list
        }

        [TestMethod]
        public void GetColumnSettings_should_be_return_correct_settings()
        {
            // arrange
            var columnService = ServiceLocator.Resolve<ICustomizableColumnsService>();
            // act
            var fields = columnService.GetGridFields();
            var settings = columnService.GetColumnSettings();
            // assert
            foreach (var field in fields)
            {
                Assert.AreEqual(settings.First(x => x.Key == field.DataField).Active, field.Visible);
            }
        }

        [TestMethod]
        public void SaveColumnSettings_should_be_update_correctlyY()
        {
            // arrange
            var columnService = ServiceLocator.Resolve<ICustomizableColumnsService>();
            var oldsettings = columnService.GetColumnSettings();
            oldsettings[0].Active = false;
            oldsettings[1].Active = false;
            var newSettings = new List<GridColumnSetting> { oldsettings[0], oldsettings[1], new GridColumnSetting { Key = "Test123" } };
            // act
            columnService.SaveColumnSettings(newSettings);
            var settings = columnService.GetColumnSettings();
            // assert
            Assert.IsFalse(settings[0].Active);
            Assert.IsFalse(settings[1].Active);
            Assert.IsNull(settings.FirstOrDefault(x => x.Key == "Test123"));
        }
    }
}