using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FilterServiceTest
    {
        //[TestMethod, Owner(@"FIRM\AlexanderL")]
        //public void GetScheduledInterviews_WithCFVariablesWithoutAliases()
        //{
        //    string query = FilterAndPagingTools.GetFilterQuery(surveyId, CallStates.Scheduled, new[] { "q1", "q2" }, new[] { "", "" });
        //    DataTable actualRecordSet = ExecFilter<DataTable>(query);
        //    IsCorrectRecordSet(respondentlist.Skip(1).Select(x => x.InterviewId).ToArray(), actualRecordSet, "q1", "q2");
        //}

        //[TestMethod, Owner(@"FIRM\AlexanderL")]
        //public void GetScheduledInterviews_WithCFVariablesWithAliasesForSomeFields()
        //{
        //    string query = FilterAndPagingTools.GetFilterQuery(surveyId, CallStates.Scheduled, new[] { "q1", "q2" }, new[] { "", "Varq2" });
        //    DataTable actualRecordSet = ExecFilter<DataTable>(query);
        //    IsCorrectRecordSet(respondentlist.Skip(1).Select(x => x.InterviewId).ToArray(), actualRecordSet, "q1", "Varq2");
        //}
    }
}
