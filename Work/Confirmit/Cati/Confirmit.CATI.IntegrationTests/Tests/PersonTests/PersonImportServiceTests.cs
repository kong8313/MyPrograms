using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonImportServiceTests
    {
        private DataTable _dataTable;

        private Dictionary<string, ColumnRole> _columnRoleMap;

        private ImportOptions _importOptions;

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private BackendTools _backendTools;

        private int _surveySid;

        private const string SurveyName = "p0000123";

        private const string PersonName = "new_person";

        private const string Password = "password";

        private IPersonImportService _personImportService;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);

            _backendTools = new BackendTools(_framework);
            _surveySid = _backendTools.CreateSurvey(SurveyName);
            _dataTable = new DataTable();

            for (var i = 0; i < 5; i++)
            {
                _dataTable.Columns.Add(new DataColumn("DataColumn" + i));
            }

            _columnRoleMap = new Dictionary<string, ColumnRole>()
            {
                {"DataColumn0", ColumnRole.Login},
                {"DataColumn1", ColumnRole.AutomaticSurvey},
                {"DataColumn2", ColumnRole.Password},
                {"DataColumn3", ColumnRole.Group},
                {"DataColumn4", ColumnRole.TaskChoice}
            };

            _importOptions = new ImportOptions()
            {
                ImportFirstRow = true,
                OverwriteExistentData = false,
                OverwriteExistentRelations = false
            };

            _personImportService = ServiceLocator.Resolve<IPersonImportService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ImportPersons_ImportNewPersonWithAutomaticSurvey_AutomaticSurveyIdAdded()
        {
            AddRow(new object[] { PersonName, SurveyName, Password, string.Empty, "3" });

            var result = _personImportService.ImportPersons(CallCenterTools.DefaultId, _dataTable, _columnRoleMap,
                _importOptions);

            Assert.AreEqual(_dataTable.Rows.Count, result.RowsProcessed);
            Assert.AreEqual(true, result.Interviewers.Any());

            var personId = result.Interviewers.First().Id;
            var person = PersonRepository.GetById(personId);

            Assert.IsNotNull(person);
            Assert.AreEqual(_surveySid, person.AutomaticSurveyID);
            Assert.AreEqual((int)AgentTaskChoiceMode.CampaignAssignment, person.ManualSelection);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ImportPersons_ImportNewPersonWithAutomaticSurveyAndManualTaskChoice_AutomaticSurveyIdNotAdded()
        {
            AddRow(new object[] { PersonName, SurveyName, Password, string.Empty, "2" });

            var result = _personImportService.ImportPersons(CallCenterTools.DefaultId, _dataTable, _columnRoleMap,
                _importOptions);

            Assert.AreEqual(_dataTable.Rows.Count, result.RowsProcessed);
            Assert.AreEqual(true, result.Interviewers.Any());

            var personId = result.Interviewers.First().Id;
            var person = PersonRepository.GetById(personId);

            Assert.IsNotNull(person);
            Assert.AreEqual(null, person.AutomaticSurveyID);
            Assert.AreEqual((int)AgentTaskChoiceMode.Manual, person.ManualSelection);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ImportPersons_UpdatePersonWithAutomaticSurvey_AutomaticSurveyIdUpdatedWithLog()
        {
            AddRow(new object[] { PersonName, string.Empty, Password, string.Empty, "2" });
            AddRow(new object[] { PersonName, SurveyName, Password, string.Empty, "3" });
            _importOptions.OverwriteExistentData = true;
            var result = _personImportService.ImportPersons(CallCenterTools.DefaultId, _dataTable, _columnRoleMap,
                _importOptions);

            Assert.AreEqual(_dataTable.Rows.Count, result.RowsProcessed);
            Assert.AreEqual(true, result.Interviewers.Any());

            var personId = result.Interviewers.First().Id;
            var person = PersonRepository.GetById(personId);

            Assert.IsNotNull(person);
            Assert.AreEqual(_surveySid, person.AutomaticSurveyID);
            Assert.AreEqual((int)AgentTaskChoiceMode.CampaignAssignment, person.ManualSelection);
            Assert.IsTrue(result.Log.Contains(String.Format(Strings.PersonImport_AutomaticSurveyWasSet, SurveyName)));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ImportPersons_UpdatePersonWithoutAutomaticSurvey_AutomaticSurveyIdUpdatedWithLog()
        {
            AddRow(new object[] { PersonName, SurveyName, Password, string.Empty, "3" });
            AddRow(new object[] { PersonName, string.Empty, Password, string.Empty, "3" });
            _importOptions.OverwriteExistentData = true;
            var result = _personImportService.ImportPersons(CallCenterTools.DefaultId, _dataTable, _columnRoleMap,
                _importOptions);

            Assert.AreEqual(_dataTable.Rows.Count, result.RowsProcessed);
            Assert.AreEqual(true, result.Interviewers.Any());

            var personId = result.Interviewers.First().Id;
            var person = PersonRepository.GetById(personId);

            Assert.IsNotNull(person);
            Assert.AreEqual(null, person.AutomaticSurveyID);
            Assert.AreEqual((int)AgentTaskChoiceMode.CampaignAssignment, person.ManualSelection);
            Assert.IsTrue(result.Log.Contains(String.Format(Strings.PersonImport_AutomaticSurveyWasSet, SurveyName)));
        }

        private void AddRow(object[] items)
        {
            var row = _dataTable.NewRow();
            row.ItemArray = items;
            _dataTable.Rows.Add(row);
        }
    }
}