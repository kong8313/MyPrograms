using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class NielsenSchedulingScriptTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "NielsenSchedulingScript";

            TestInitialize();

            var sample = SampleGenerator.Generate(3, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(sample);
            var scriptId = Confirmit.Cati.Scheduling.Load(PathToSchedule);
            Confirmit.Cati.Surveys[ProjectId].AssignSchedulingScript(scriptId);
        }

        [TestMethod]
        public void RunSchedulingScript_HistoryRecordAreCreatedSuccessful()
        {

            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      NULL             
2      NULL             
3      NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", String.Empty);
            CheckForEquality(ProjectId, "SELECT * FROM response1", String.Empty);

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(23, 2);

            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      NULL             
2      1                
3      NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid Q13VC11 Q9VC51 Q1   Q2   Q27VC25 Q3   Q3VC5 Q3VC5_1_other Q4   Q4VC05 Q4VC1 Q6A  Q6B  Q6C  Q6D  Q6E  Q6VC5 Q6VC5B Q6VC5B_1_other Q7A  Q7B  Q7C  Q8A  Q8B  Q8C  Q8D  Q8E  Q9A  Q9B  Q9C  Q10A Q10B Q11  Q12  D0   D0_1_other D1   D1_1_other D1A  D1A_1_other D1B  D1B_1_other D2   D2_1_other D2A  D2A_1_other D2B  D2B_1_other D3   D3_1_other D4   D4_1_other D5   D5_1_other D6   D6_1_other D7   D7_1_other D8   D8_1_other D9   D9_1_other D9A  D9A_1_other D9B  D9B_1_other D11  D11_1_other D12  D13  D14  D15  D16  D16_77_other D16a D16a_1_other D17  D18  D19  D19_1_other D19c D20  D21  D22  D22c D23  D23_1_other D24  D24_1_other D25  D26  D26_1_other CallAttemptCount CurrAttempts TimeZone EverHuman Queue StudyCode ExtendedStatusCode ResumePoint TextState Text1D22 Text2D22 TextQ4 callhistoryinfo CollectionMode LastCallExtendedStatus Q260VC00 Q310VC50 apptyp contact_person MakeComments Comments 
1          2      NULL    NULL   NULL NULL NULL    NULL NULL  NULL          NULL NULL   NULL  NULL NULL NULL NULL NULL NULL  NULL   NULL           NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL       NULL NULL       NULL NULL        NULL NULL        NULL NULL       NULL NULL        NULL NULL        NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL        NULL NULL        NULL NULL        NULL NULL NULL NULL NULL NULL         NULL NULL         NULL NULL NULL NULL        NULL NULL NULL NULL NULL NULL NULL        NULL NULL        NULL NULL NULL        NULL             NULL         NULL     NULL      NULL  NULL      NULL               NULL        NULL      NULL     NULL     NULL   NULL            NULL           NULL                   NULL     NULL     NULL   NULL           NULL         NULL     ");
            CheckForEquality(ProjectId, "SELECT responseid, respid, callhistoryinfo, CallExtendedStatus FROM response1", @"
responseid respid callhistoryinfo CallExtendedStatus 
1          2      1               23                 ");

            Confirmit.Cati.Surveys[ProjectId].CallManagement.MoveAndResedule(42, 2);

            CheckForEquality(ProjectId, "SELECT respid, CallAttemptCount FROM respondent", @"
respid CallAttemptCount 
1      NULL             
2      2                
3      NULL             ");
            CheckForEquality(ProjectId, "SELECT * FROM response0", @"
responseid respid Q13VC11 Q9VC51 Q1   Q2   Q27VC25 Q3   Q3VC5 Q3VC5_1_other Q4   Q4VC05 Q4VC1 Q6A  Q6B  Q6C  Q6D  Q6E  Q6VC5 Q6VC5B Q6VC5B_1_other Q7A  Q7B  Q7C  Q8A  Q8B  Q8C  Q8D  Q8E  Q9A  Q9B  Q9C  Q10A Q10B Q11  Q12  D0   D0_1_other D1   D1_1_other D1A  D1A_1_other D1B  D1B_1_other D2   D2_1_other D2A  D2A_1_other D2B  D2B_1_other D3   D3_1_other D4   D4_1_other D5   D5_1_other D6   D6_1_other D7   D7_1_other D8   D8_1_other D9   D9_1_other D9A  D9A_1_other D9B  D9B_1_other D11  D11_1_other D12  D13  D14  D15  D16  D16_77_other D16a D16a_1_other D17  D18  D19  D19_1_other D19c D20  D21  D22  D22c D23  D23_1_other D24  D24_1_other D25  D26  D26_1_other CallAttemptCount CurrAttempts TimeZone EverHuman Queue StudyCode ExtendedStatusCode ResumePoint TextState Text1D22 Text2D22 TextQ4 callhistoryinfo CollectionMode LastCallExtendedStatus Q260VC00 Q310VC50 apptyp contact_person MakeComments Comments 
1          2      NULL    NULL   NULL NULL NULL    NULL NULL  NULL          NULL NULL   NULL  NULL NULL NULL NULL NULL NULL  NULL   NULL           NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL       NULL NULL       NULL NULL        NULL NULL        NULL NULL       NULL NULL        NULL NULL        NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL       NULL NULL        NULL NULL        NULL NULL        NULL NULL NULL NULL NULL NULL         NULL NULL         NULL NULL NULL NULL        NULL NULL NULL NULL NULL NULL NULL        NULL NULL        NULL NULL NULL        NULL             NULL         NULL     NULL      NULL  NULL      NULL               NULL        NULL      NULL     NULL     NULL   NULL            NULL           NULL                   NULL     NULL     NULL   NULL           NULL         NULL     ");
            CheckForEquality(ProjectId, "SELECT responseid, respid, callhistoryinfo, CallExtendedStatus FROM response1", @"
responseid respid callhistoryinfo CallExtendedStatus 
1          2      1               23                 
1          2      2               42                 ");
        }
    }
}
