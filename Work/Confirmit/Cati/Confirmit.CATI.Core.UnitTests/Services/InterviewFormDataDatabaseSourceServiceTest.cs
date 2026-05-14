using System.Data;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Types;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class InterviewFormDataDatabaseSourceServiceTest
    {
        private StubISurveyDatabaseEngine _stubISurveyDatabaseService;

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            ServiceLocator.RegisterSingleton<ISurveyDatabaseEngine, StubISurveyDatabaseEngine>();
            ServiceLocator.Register<IInterviewFormDataDatabaseSourceService, InterviewFormDataDatabaseSourceService>();

            _stubISurveyDatabaseService = ServiceLocator.Resolve<ISurveyDatabaseEngine>() as StubISurveyDatabaseEngine;
        }

        private void ReMockSurveyDatabaseService(object returnValue, string columnName)
        {
            _stubISurveyDatabaseService.ExecuteQueryInt32StringArrayOfSqlParameter = (id, query, parameters) =>
            {
                var table = new DataTable("response0");
                table.Columns.Add(columnName, returnValue.GetType());
                table.Rows.Add(returnValue);

                return table;
            };
        }

        [TestMethod, Owner(@"FIRM\OlegM")]
        public void GetFormValue_Should_Return_Correct_Value()
        {
            var formDesc = new OpenFormDesc(1, "1", new OpenForm { FormTexts = new FormText[0]}, new SurveyDatabaseFormInfo { LoopPath = new []{"responseid"}, Fields = new[] { new SurveyDatabaseFieldInfo { FieldName = "q1", TableName = "testName"} } });

            AssertFormValue(formDesc, '8', "8");
            AssertFormValue(formDesc, "10", "10");
            AssertFormValue(formDesc, 4, "4");
            AssertFormValue(formDesc, (byte)80, "80");

            var g = SqlGeography.Point(10, 10, 4326 /* WGS84 */);
            AssertFormValue(formDesc, g, g.ToString());
        }

        private void AssertFormValue(OpenFormDesc formDesc, object databaseValue, object actualValue)
        {
            // every time we should obtain new instance to invalidate cache
            var service = ServiceLocator.Resolve<IInterviewFormDataDatabaseSourceService>();
            ReMockSurveyDatabaseService(databaseValue, formDesc.DbFormInfo.Fields[0].FieldName);
            var expectedValue = service.GetFormValue(formDesc, null, new string[] { });
            Assert.AreEqual(actualValue, expectedValue);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }
    }
}