using System;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class SurveyQuotasReportGeneratorTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void QuotasReportGenerator_GenerateReports_Success()
        {
            var columns = new[] { "c1", "c2", "c3" };
            var valus = new[] { new[] { "1", "2", "3" }, 
                                new[] { "11", "12", "13" } 
                              };

            var dataProvider = new SurveyQuotasExportInfoProviderStub(string.Empty, columns, valus);

            string report = new SurveyQuotasReportGenerator(dataProvider,
                                                            () => String.Empty,
                                                            x => string.Empty).Generate();

            var result = report.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual("c1\tc2\tc3", result[5]);
            Assert.AreEqual("1\t2\t3", result[6]);
            Assert.AreEqual("11\t12\t13", result[7]);

        }
    }
}
