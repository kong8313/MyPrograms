using System;
using System.IO;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Confirmit.SystemTestFramework.SurveyData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class SchedulingScriptWithLoopsTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "SchedulingScriptWithLoopsTest";

            TestInitialize();

            var sample = SampleGenerator.Generate(3, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            var scriptId = Confirmit.Cati.Scheduling.Load(PathToSchedule);
            Confirmit.Cati.Surveys[ProjectId].AssignSchedulingScript(scriptId);
            Confirmit.Surveys[ProjectId].AddRespondents(sample, CatiScheduling.Full);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CreateDifferentIterationInLoop_ResultAreCorrect()
        {
            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      NULL             
2      NULL             
3      NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid q3   l1   
1          1      NULL NULL 
2          2      NULL NULL 
3          3      NULL NULL ");
            CheckForEquality(ProjectId, "SELECT * FROM response1", @"
responseid respid l1 q2   q1   
1          1      2  NULL a    
1          1      3  aasd NULL 
2          2      2  NULL a    
2          2      3  aasd NULL 
3          3      2  NULL a    
3          3      3  aasd NULL ");

        }
    }
}