using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class ControllersTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Controllers";

            TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void ConfirmitController_Login()
        {

        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SurveysController_ImportSurvey()
        {
            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);

            Cleanup(false);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SurveyController_Launch()
        {
            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SurveyController_AddRespondents()
        {
            var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(file);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SchedulingController_Load()
        {
            Confirmit.Cati.Scheduling.Load(PathToSchedule);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void CatiSurveyController_AssignSchedulingScript()
        {
            var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber, ColumnType.Email);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(file);
            var scriptId = Confirmit.Cati.Scheduling.Load(PathToSchedule);
            Confirmit.Cati.Surveys[ProjectId].AssignSchedulingScript(scriptId);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void CallManagementController_MoveAndResedule()
        {
            var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber, ColumnType.Email);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(file);
            var scriptId = Confirmit.Cati.Scheduling.Load(PathToSchedule);
            Confirmit.Cati.Surveys[ProjectId].AssignSchedulingScript(scriptId);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(1, 1);
            CheckForEquality(ProjectId,
                "SELECT respid FROM respondent", @"
respid 
1      ");

            Cleanup();
        }
    }
}