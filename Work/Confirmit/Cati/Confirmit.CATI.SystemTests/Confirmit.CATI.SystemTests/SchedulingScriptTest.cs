using System.IO;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class SchedulingScriptTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "SchedulingScript";

            TestInitialize();

            var sample = SampleGenerator.Generate(1, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(sample);
            var scriptId = Confirmit.Cati.Scheduling.Load(PathToSchedule);
            Confirmit.Cati.Surveys[ProjectId].AssignSchedulingScript(scriptId);
        }

        private void CheckForEqualityRespondentAndResponseControl(string projectId)
        {
            CheckForEquality(projectId,
                "SELECT respid FROM respondent", @"
respid 
1      ");
            CheckForEquality(projectId,
                "SELECT responseid,respid FROM response_control", @"
responseid respid 
1          1      ");

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Single_Root()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(31, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      1           NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(31, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      2           NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Single_Level1()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(32, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId,
                "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  1         NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(32, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId,
                "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  2         NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Single_Level1_UpdateSeveralIterations()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(38, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      1                ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId,
                "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  2         NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(38, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      2                ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId,
                "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  2         NULL              NULL       NULL       NULL       NULL             
1          1      2  3         NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [Ignore, TestMethod, Owner(@"FIRM\KirillV")]
        public void Single_Level2()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(33, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", @"
responseid respid l1 l2 single_l2 single_l2_3_other multi_l2_1 multi_l2_2 multi_l2_3 multi_l2_3_other 
1          1      1  1  1         NULL              NULL       NULL       NULL       NULL             ");

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(33, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", @"
responseid respid l1 l2 single_l2 single_l2_3_other multi_l2_1 multi_l2_2 multi_l2_3 multi_l2_3_other 
1          1      1  1  2         NULL              NULL       NULL       NULL       NULL             ");

            Cleanup();
        }


        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Single_RootLevel1()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(31, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      1           NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(32, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      1           NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId,
                "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  1         NULL              NULL       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Multi_Root()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(34, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                True         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(34, 1);

            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                False        NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Multi_Level1()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(35, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              True       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(35, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              False      NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [Ignore, TestMethod, Owner(@"FIRM\KirillV")]
        public void Multi_Level2()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(36, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              True       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(35, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              False      NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Multi_RootLevel1()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(34, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                True         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(35, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                True         NULL         NULL         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 single_l1 single_l1_3_other multi_l1_1 multi_l1_2 multi_l1_3 multi_l1_3_other 
1          1      1  NULL      NULL              True       NULL       NULL       NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void Multi_Root_OpenEnd()
        {
            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(37, 1);

            CheckForEqualityRespondentAndResponseControl(ProjectId);
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         True         NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(37, 1);

            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid single_root single_root_3_other multi_root_1 multi_root_2 multi_root_3 multi_root_3_other l1   l2   
1          1      NULL        NULL                NULL         NULL         False        NULL               NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", string.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response2", string.Empty);

            Cleanup();
        }
    }
}