using System.Data;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class InterviewFormDataWebSourceServiceTest : BaseTest
    {
        private StubISurveyDataService _stubISurveyDataService;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ServiceLocator.RegisterSingleton<ISurveyDataService, StubISurveyDataService>();
            ServiceLocator.Register<IInterviewFormDataWebSourceService, InterviewFormDataWebSourceService>();

            _stubISurveyDataService = ServiceLocator.Resolve<ISurveyDataService>() as StubISurveyDataService;
        }

        private void ReMockSurveyDataService(object returnValue, string columnName)
        {
            _stubISurveyDataService.GetDataTransferDefBaseResponseToken = (transfer, token) =>
            {
                var result = new TransferResult { ResponseToken = new ResponseToken(), Result = new DataSet() };

                var table = new DataTable("responseid");

                table.Columns.Add(columnName, returnValue.GetType());
                result.Result.Tables.Add(table);

                table.Rows.Add(returnValue);

                return result;
            };
        }

        [TestMethod, Owner(@"FIRM\OlegM")]
        public void GetFormValue_Should_Return_Correct_Value()
        {
            var service = ServiceLocator.Resolve<IInterviewFormDataWebSourceService>();
            var formDesc = new OpenFormDesc(1, "1", new OpenForm { FormTexts = new FormText[0] }, new SurveyDatabaseFormInfo { LoopPath = new[] { "responseid" }, Fields = new[] { new SurveyDatabaseFieldInfo { FieldName = "q1" } } });

            ReMockSurveyDataService('8', "q1");
            Assert.AreEqual("8", service.GetFormValue(formDesc, null, new string[] { }));

            ReMockSurveyDataService("10", "q1");
            Assert.AreEqual("10", service.GetFormValue(formDesc, null, new string[] { }));

            ReMockSurveyDataService(4, "q1");
            Assert.AreEqual("4", service.GetFormValue(formDesc, null, new string[] { }));

            ReMockSurveyDataService((byte)80, "q1");
            Assert.AreEqual("80", service.GetFormValue(formDesc, null, new string[] { }));
        }
    }
}