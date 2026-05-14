using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Confirmit.SystemTestFramework.Controllers;
using Confirmit.SystemTestFramework.Samples;
using Confirmit.SystemTestFramework.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.SystemTestFramework
{
    public abstract class BaseSystemTests
    {
        private string CurrentDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        protected string TestsGroupName;

        protected string PathToSurvey => Path.Combine(CurrentDirectory, @"..\Confirmit.CATI.SystemTests\TestData\" + TestsGroupName + @"\survey.xml");

        protected string PathToSchedule => Path.Combine(CurrentDirectory, @"..\Confirmit.CATI.SystemTests\TestData\" + TestsGroupName + @"\Schedule.xml");

        private readonly DataProvider _dataProvider;
        protected readonly SampleGenerator SampleGenerator;
        protected ConfirmitController Confirmit;
        protected EnvironmentController Environment;
        protected string ProjectId;

        protected BaseSystemTests()
        {
            _dataProvider = new DataProvider();
            SampleGenerator = new SampleGenerator();
            ProjectId = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        protected void CheckForEquality(string pid, string query, string expected)
        {
            var actual = _dataProvider.GetDataFromDb(pid, query);
            Assert.AreEqual(expected, actual);
        }

        protected void CheckForContent(string pid, string query, string pattern, bool isContains)
        {
            var result = _dataProvider.GetDataFromDb(pid, query);
            var actual = result.Contains(pattern);
            Assert.AreEqual(isContains, actual);
        }

        protected void Cleanup(bool isSurveyLaunched = true)
        {
            if (ProjectId != null)
            {
                if (isSurveyLaunched)
                {
                    Confirmit.Cati.Surveys[ProjectId].Close();
                }

                bool isSurveyRemoved;
                int cnt = 0;
                do
                {
                    try
                    {
                        Confirmit.Surveys.Delete(ProjectId);
                        isSurveyRemoved = true;
                    }
                    catch
                    {
                        isSurveyRemoved = false;
                        cnt++;
                        Thread.Sleep(500);
                    }
                } while (!isSurveyRemoved && cnt < 10);
            }
        }

        protected static int GetCompanyId()
        {
            return int.Parse(Properties.Settings.Default.CompanyId);
        }

        protected void TestInitialize()
        {
            Confirmit = ConfirmitController.Login(GetAdminSettings());
            Environment = new EnvironmentController();
        }

        private UserSettings GetAdminSettings()
        {
            return new UserProvider().GetUserSettings();
        }
    }
}