using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Core.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using Confirmit.CATI.Core.Services.Database.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.RoutineMaintenance.Actions
{
    
    [TestClass]
    public class RebuildIndexesActionTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_NoIndexesForOptimization_RequestIndexModeAreCorrect()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => ""
            };

            var indexes = new[] { new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 0, PageCount = 101 } };

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new []
                {
                    "getall(SIMPLED)"
                }, 
                actionTrace.ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_NoIndexesWithCorrespondingPageCount_RequestIndexModeAreCorrect()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => ""
            };

            var indexes = new[] { new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 50, PageCount = 99 } };

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new[]
                {
                    "getall(SIMPLED)"
                },
                actionTrace.ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_OneIndexForRebuild_ReorginizeMethodIsCalled()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => ""
            };

            var indexes = new[] {new IndexInfo() {TableName = "tbl1", IndexName = "ind1", Fragmentation = 40, PageCount = 101}};

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new[]
                {
                    "getall(SIMPLED)",
                    "rebuild(tbl1,ind1,False)",
                    "get(tbl1,ind1,SIMPLED)"
                },
                actionTrace.ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_OneIndexWithLobForRebuild_ReorginizeMethodIsCalled()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => ""
            };

            var indexes = new[] { new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 40, PageCount = 101,ContainsLob = true} };

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new[]
                {
                    "getall(SIMPLED)",
                    "rebuild(tbl1,ind1,True)",
                    "get(tbl1,ind1,SIMPLED)"
                },
                actionTrace.ToArray());
        }

        private static List<string> ExecuteDatabaseMaintence(IDatabaseMaintenanceSettings settings, IndexInfo[] indexInfo, bool isRebuildInShift = true)
        {
            List<string> actionTrace = new List<string>();
            
            var indexService = new StubIDatabaseIndexService()
            {
                GetAllIndexesString = (mode) =>
                {
                    actionTrace.Add(String.Format("getall({0})", mode));
                    return indexInfo;
                },
                RebuildIndexStringStringBoolean = (t, i, b) => actionTrace.Add(String.Format("rebuild({0},{1},{2})", t, i, b)),
                ReorginizeIndexStringString = (t, i) => actionTrace.Add(String.Format("reorginize({0},{1})", t, i)),
                GetIndexStringStringString = (t, i, m) =>
                {
                    actionTrace.Add(String.Format("get({0},{1},{2})", t, i, m));
                    return new IndexInfo() {TableName = t, IndexName = i};
                },
            };

            var statisticService = new StubIDatabaseStatisticService()
            {
                UpdateStatisticString = (mode) => actionTrace.Add(String.Format("update({0})", mode)),
            };

            var shiftService = new StubIRoutineMaintenanceShiftService()
            {
                IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftType = (z, y) => isRebuildInShift
            };

            new DatabaseMaintenanceAction(settings, indexService, statisticService, shiftService).Execute(RoutineMaintenanceShiftType.Monthly);
            
            return actionTrace;
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_OneIndexForReorganization_ReorginizeMethodIsCalled()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => ""
            };

            var indexes = new[] {new IndexInfo() {TableName = "tbl1", IndexName = "ind1", Fragmentation = 20, PageCount = 101}};
            
            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new[]
                {
                    "getall(SIMPLED)",
                    "reorginize(tbl1,ind1)",
                    "get(tbl1,ind1,SIMPLED)"
                },
                actionTrace.ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_SeveralIndexesAndStatistics_IndexesAndStatisticsAreUpdatedInRightOrder()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => "tbl0,tbl1,tbl10,tbl9"
            };

            var indexes = new[]
            {
                new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 20, PageCount = 101 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 40, PageCount = 10 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind3", Fragmentation = 0, PageCount = 101 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind4", Fragmentation = 40, PageCount = 101 },
                new IndexInfo() { TableName = "tbl2", IndexName = "ind1", Fragmentation = 40, PageCount = 101 },
                new IndexInfo() { TableName = "tbl3", IndexName = "ind1", Fragmentation = 20, PageCount = 101 },
                new IndexInfo() { TableName = "tbl4", IndexName = "ind1", Fragmentation = 5, PageCount = 101 }
            };

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes);

            CollectionAssert.AreEqual(
                new[]
                {
                    "getall(SIMPLED)",
                    "update(tbl0)",
                    "rebuild(tbl1,ind4,False)",
                    "reorginize(tbl1,ind1)",
                    "update(tbl1)",
                    "update(tbl10)",
                    "rebuild(tbl2,ind1,False)",
                    "reorginize(tbl3,ind1)",
                    "update(tbl9)",
                    "get(tbl1,ind1,SIMPLED)",
                    "get(tbl1,ind4,SIMPLED)",
                    "get(tbl2,ind1,SIMPLED)",
                    "get(tbl3,ind1,SIMPLED)"
                },
                actionTrace.ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Execute_SeveralIndexesAndStatistics_StatisticsAreUpdatedInRightOrderAndIndexAreNotTouched()
        {
            var settings = new StubIDatabaseMaintenanceSettings()
            {
                IndexFragmentationDetectModeGet = () => "SIMPLED",
                FragmentationIndexRebuildThresholdGet = () => 30,
                FragmentationIndexReorganizeThresholdGet = () => 10,
                RebuildIndexShiftTypeGet = () => (int)RoutineMaintenanceShiftType.Weekly,
                MinIndexPageCountGet = () => 100,
                UpdateStatisticTablesGet = () => "tbl0,tbl1,tbl10,tbl9"
            };

            var indexes = new[]
            {
                new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 20, PageCount = 101 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind1", Fragmentation = 40, PageCount = 10 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind3", Fragmentation = 0, PageCount = 101 },
                new IndexInfo() { TableName = "tbl1", IndexName = "ind4", Fragmentation = 40, PageCount = 101 },
                new IndexInfo() { TableName = "tbl2", IndexName = "ind1", Fragmentation = 40, PageCount = 101 },
                new IndexInfo() { TableName = "tbl3", IndexName = "ind1", Fragmentation = 20, PageCount = 101 },
                new IndexInfo() { TableName = "tbl4", IndexName = "ind1", Fragmentation = 5, PageCount = 101 }
            };

            var actionTrace = ExecuteDatabaseMaintence(settings, indexes, false);

            CollectionAssert.AreEqual(
                new[]
                {
                    "update(tbl0)",
                    "update(tbl1)",
                    "update(tbl10)",
                    "update(tbl9)",
                },
                actionTrace.ToArray());
        }
    }

    
}
