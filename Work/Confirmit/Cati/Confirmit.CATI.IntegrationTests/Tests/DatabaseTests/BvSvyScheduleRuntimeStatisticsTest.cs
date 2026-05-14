using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseTests
{
    [TestClass]
    public class BvSvyScheduleRuntimeStatisticsTest : BaseMockedIntegrationTest
    {
        public class CallData
        {
            public int SID;
            public int IID;
            public int ShiftTypeId;
            public int ExplicitSID;
            public int CallState;
        }


        [TestMethod, Owner(@"Firm\MaximL")]
        public void NoCalls_NoChanges_StatisticsAreEmpty()
        {
            CheckStatistics(new BvSvyScheduleRuntimeStatisticsEntity[]{});
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void NoCalls_InsertTwoRecords_StatisticsAreEmpty()
        {
            InsertData(new []
                       {
                           new CallData{SID = 1, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 1, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 1, IID = 3, ShiftTypeId = 1, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 1, IID = 4, ShiftTypeId = 1, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 1, IID = 5, ShiftTypeId = 1, ExplicitSID = 1, CallState = 1},
                           new CallData{SID = 1, IID = 6, ShiftTypeId = 1, ExplicitSID = 1, CallState = 2},
                           new CallData{SID = 2, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 2, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 2, IID = 3, ShiftTypeId = 2, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 2, IID = 4, ShiftTypeId = 2, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 2, IID = 5, ShiftTypeId = 2, ExplicitSID = 2, CallState = 1},
                           new CallData{SID = 2, IID = 6, ShiftTypeId = 2, ExplicitSID = 2, CallState = 2}
                       });

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            CheckStatistics(new []
                            {
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 1, ShiftTypeID = 1, ExplicitSID = 1, TotalCount = 2, FreeCount = 1},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 1, ExplicitSID = 1, TotalCount = 1, FreeCount = 0},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 2, ExplicitSID = 2, TotalCount = 1, FreeCount = 1}
                            });
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void TwelveCalls_UpdateCallState_StatisticsAreEmpty()
        {
            InsertData(new[]
                       {
                           new CallData{SID = 1, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 1, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 1, IID = 3, ShiftTypeId = 1, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 1, IID = 4, ShiftTypeId = 1, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 1, IID = 5, ShiftTypeId = 1, ExplicitSID = 1, CallState = 1},
                           new CallData{SID = 1, IID = 6, ShiftTypeId = 1, ExplicitSID = 1, CallState = 2},
                           new CallData{SID = 2, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 2, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 2, IID = 3, ShiftTypeId = 2, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 2, IID = 4, ShiftTypeId = 2, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 2, IID = 5, ShiftTypeId = 2, ExplicitSID = 2, CallState = 1},
                           new CallData{SID = 2, IID = 6, ShiftTypeId = 2, ExplicitSID = 2, CallState = 2}
                       });

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            TestingFramework.DbEngine.ExecuteNonQuery(@"UPDATE BvSvySchedule SET CallState = 2", CommandType.Text);

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            CheckStatistics(new[]
                            {
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 1, ShiftTypeID = 1, ExplicitSID = 1, TotalCount = 6, FreeCount =6},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 1, ExplicitSID = 1, TotalCount = 2, FreeCount = 2},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 2, ExplicitSID = 1, TotalCount = 2, FreeCount = 2},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 2, ExplicitSID = 2, TotalCount = 2, FreeCount = 2}
                            });
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void TwelveCalls_UpdateShiftTypeAndExplicitSID_StatisticsAreEmpty()
        {
            InsertData(new[]
                       {
                           new CallData{SID = 1, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 1, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 1, IID = 3, ShiftTypeId = 1, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 1, IID = 4, ShiftTypeId = 1, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 1, IID = 5, ShiftTypeId = 1, ExplicitSID = 1, CallState = 1},
                           new CallData{SID = 1, IID = 6, ShiftTypeId = 1, ExplicitSID = 1, CallState = 2},
                           new CallData{SID = 2, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 2, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 2, IID = 3, ShiftTypeId = 2, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 2, IID = 4, ShiftTypeId = 2, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 2, IID = 5, ShiftTypeId = 2, ExplicitSID = 2, CallState = 1},
                           new CallData{SID = 2, IID = 6, ShiftTypeId = 2, ExplicitSID = 2, CallState = 2}
                       });

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            TestingFramework.DbEngine.ExecuteNonQuery(@"UPDATE BvSvySchedule SET ShiftTypeId = 3, ExplicitSID = 4", CommandType.Text);

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            CheckStatistics(new[]
                            {
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 1, ShiftTypeID = 3, ExplicitSID = 4, TotalCount = 2, FreeCount =1},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 3, ExplicitSID = 4, TotalCount = 2, FreeCount =1}
                            });
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void TwelveCalls_DeleteSeveralCalls_StatisticsAreEmpty()
        {
            InsertData(new[]
                       {
                           new CallData{SID = 1, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 1, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 1, IID = 3, ShiftTypeId = 1, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 1, IID = 4, ShiftTypeId = 1, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 1, IID = 5, ShiftTypeId = 1, ExplicitSID = 1, CallState = 1},
                           new CallData{SID = 1, IID = 6, ShiftTypeId = 1, ExplicitSID = 1, CallState = 2},
                           new CallData{SID = 2, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 2, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 2, IID = 3, ShiftTypeId = 2, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 2, IID = 4, ShiftTypeId = 2, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 2, IID = 5, ShiftTypeId = 2, ExplicitSID = 2, CallState = 1},
                           new CallData{SID = 2, IID = 6, ShiftTypeId = 2, ExplicitSID = 2, CallState = 2}
                       });

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            TestingFramework.DbEngine.ExecuteNonQuery(@"DELETE FROM BvSvySchedule WHERE CallState IN (-3, -2, 2 ) AND SurveySID = 1", CommandType.Text);

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            CheckStatistics(new[]
                            {
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 1, ExplicitSID = 1, TotalCount = 1, FreeCount = 0},
                                new BvSvyScheduleRuntimeStatisticsEntity{SurveyId = 2, ShiftTypeID = 2, ExplicitSID = 2, TotalCount = 1, FreeCount = 1}
                            });
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void TwelveCalls_DeleteAllCalls_StatisticsAreEmpty()
        {
            InsertData(new[]
                       {
                           new CallData{SID = 1, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 1, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 1, IID = 3, ShiftTypeId = 1, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 1, IID = 4, ShiftTypeId = 1, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 1, IID = 5, ShiftTypeId = 1, ExplicitSID = 1, CallState = 1},
                           new CallData{SID = 1, IID = 6, ShiftTypeId = 1, ExplicitSID = 1, CallState = 2},
                           new CallData{SID = 2, IID = 1, ShiftTypeId = 1, ExplicitSID = 1, CallState = -3},
                           new CallData{SID = 2, IID = 2, ShiftTypeId = 1, ExplicitSID = 1, CallState = -2},
                           new CallData{SID = 2, IID = 3, ShiftTypeId = 2, ExplicitSID = 1, CallState = -1},
                           new CallData{SID = 2, IID = 4, ShiftTypeId = 2, ExplicitSID = 1, CallState = 0},
                           new CallData{SID = 2, IID = 5, ShiftTypeId = 2, ExplicitSID = 2, CallState = 1},
                           new CallData{SID = 2, IID = 6, ShiftTypeId = 2, ExplicitSID = 2, CallState = 2}
                       });

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            TestingFramework.DbEngine.ExecuteNonQuery(@"DELETE FROM BvSvySchedule", CommandType.Text);

            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

            CheckStatistics(new BvSvyScheduleRuntimeStatisticsEntity[]{});
        }


        private void InsertData(CallData[] data)
        {
            var query = String.Format(@"
;with data(SurveyId,InterviewId, ShiftTypeId,ExplicitSID, CallState) as 
(
	{0}
)
INSERT INTO BvSvySchedule(SurveySID, InterviewID, ShiftTypeID, ExplicitSID, ApptID, CallState, Priority, ExplicitType, DialTypeId ) 
	SELECT d.SurveyId, InterviewId, d.ShiftTypeId, d.ExplicitSID, 0, d.CallState, 1, 1, 0 FROM data as d

", StringService.Join(" UNION ALL ", x => String.Format("SELECT {0}, {1}, {2}, {3}, {4}", x.SID, x.IID, x.ShiftTypeId, x.ExplicitSID, x.CallState) , data));
        
            TestingFramework.DbEngine.ExecuteNonQuery(query, CommandType.Text);
        }

        private void CheckStatistics(BvSvyScheduleRuntimeStatisticsEntity[] expectedStat)
        {
            var actual = (from s in BvSvyScheduleRuntimeStatisticsAdapter.GetAll() 
                            orderby s.SurveyId, s.ShiftTypeID, s.ExplicitSID
                            select s).ToArray();

            var expected = (from s in expectedStat
                             orderby s.SurveyId, s.ShiftTypeID, s.ExplicitSID
                             select s).ToArray();
            
            
            TestAssert.AreEqual(expected, actual, 
                (x,y) => x.SurveyId == y.SurveyId && 
                    x.ShiftTypeID == y.ShiftTypeID && 
                    x.ExplicitSID == y.ExplicitSID &&
                    x.FreeCount == y.FreeCount &&
                    x.TotalCount == y.TotalCount );
        }

    }
}
