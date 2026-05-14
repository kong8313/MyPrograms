using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class DatabaseMaintenanceAction : IRoutineMaintenanceAction
    {
        private readonly IDatabaseMaintenanceSettings _settings;
        private readonly IDatabaseIndexService _databaseIndexService;
        private readonly IDatabaseStatisticService _databaseStatisticService;
        private readonly IRoutineMaintenanceShiftService _routineMaintenanceShiftService;

        public DatabaseMaintenanceAction(
            IDatabaseMaintenanceSettings settings, 
            IDatabaseIndexService databaseIndexService,
            IDatabaseStatisticService databaseStatisticService,
            IRoutineMaintenanceShiftService routineMaintenanceShiftService
            )
        {
            _settings = settings;
            _databaseIndexService = databaseIndexService;
            _databaseStatisticService = databaseStatisticService;
            _routineMaintenanceShiftService = routineMaintenanceShiftService;
        }

        public string Name
        {
            get { return "Database maintenance."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            IndexInfo[] updatedIndexes = {};
            if (_routineMaintenanceShiftService.IsShiftTypeHitToAnother(
                (RoutineMaintenanceShiftType) _settings.RebuildIndexShiftType,
                curentShiftType)
                )
            {
                updatedIndexes = GetIndexesForDefragmentation();
                LogIndexStatistics(updatedIndexes, "Statistics for indexes which should be optimized");
            }

            var tables = GetTablesForUpdateStatistics();

            var plan = CreateExecutionPlan(updatedIndexes, tables);

            foreach (var item in plan)
            {
                item();
            }

            var afterUpdateIndexes = updatedIndexes.Select(x => _databaseIndexService.GetIndex(
                x.TableName, x.IndexName,
                _settings.IndexFragmentationDetectMode))
                .ToArray();

            if (afterUpdateIndexes.Length != 0)
            {
                LogIndexStatistics(afterUpdateIndexes, "Statistics for indexes after optimization");
            }
        }

        private void UpdateStatistic(string tableName)
        {
            try
            {
                _databaseStatisticService.UpdateStatistic(tableName);
                EventDetailsScope.Current.AddTiming(String.Format("    Statistics for [{0}] table were updated", tableName));
            }
            catch (Exception ex)
            {
                EventDetailsScope.Current.AddTiming(String.Format("    Statistics for [{0}] table weren't updated. See log for details", tableName));
                Trace.TraceError("Can't update statistics from [{0}] table. Exception: {1}", tableName, ex);
            }
        }

        private void RebuildIndex(IndexInfo index)
        {
            try
            {
                if (index.Fragmentation >= _settings.FragmentationIndexRebuildThreshold)
                {
                    _databaseIndexService.RebuildIndex(index.TableName, index.IndexName, index.ContainsLob);
                    EventDetailsScope.Current.AddTiming(String.Format("    Index [{0}].[{1}] was rebuilt",
                        index.TableName, index.IndexName));
                }
                else
                {
                    _databaseIndexService.ReorginizeIndex(index.TableName, index.IndexName);
                    EventDetailsScope.Current.AddTiming(String.Format("    Index [{0}].[{1}] was reorginized", index.TableName,
                        index.IndexName));
                }
            }
            catch (Exception ex)
            {
                EventDetailsScope.Current.AddTiming(String.Format(
                    "    Index [{0}].[{1}] wasn't reorginized. See log for details", index.TableName, index.IndexName));
                Trace.TraceError("Index [{0}].[{1}] wasn't reorginized. Exception: {2}", index.TableName, index.IndexName,
                    ex);
            }
        }

        public class ExecutionItem
        {
            public string TableName;
            public double Priority;
            public Action Action;
        }

        private Action[] CreateExecutionPlan(IndexInfo[] fragmentatedIndexes, string[] statisticTables)
        {
            var statisticsAction =
                statisticTables.Select(tableName => 
                    new ExecutionItem
                    {
                        TableName = tableName, 
                        Priority = double.MinValue, 
                        Action = () => UpdateStatistic(tableName)
                    });

            var indexActions = fragmentatedIndexes
                .Select(i => new ExecutionItem
                {
                    TableName = i.TableName, 
                    Priority = i.Fragmentation, Action = () => RebuildIndex(i)
                });

            return
                indexActions.Union(statisticsAction)
                    .OrderBy(x => x.TableName)
                    .ThenByDescending(y => y.Priority)
                    .Select(z => z.Action)
                    .ToArray();
        }

        private string[] GetTablesForUpdateStatistics()
        {
            return (_settings.UpdateStatisticTables ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
        
        private IndexInfo[] GetIndexesForDefragmentation()
        {
            var ignoredIndexNames =
                (_settings.IgnoredIndexes ?? "").Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var updatedIndexes = _databaseIndexService.GetAllIndexes(_settings.IndexFragmentationDetectMode)
                .Where(i => i.Fragmentation >= _settings.FragmentationIndexReorganizeThreshold &&
                            i.PageCount > _settings.MinIndexPageCount &&
                            !ignoredIndexNames.Contains(i.IndexName)).ToArray();
            return updatedIndexes;
        }

        private void LogIndexStatistics(IEnumerable<IndexInfo> indexes, string header)
        {
            EventDetailsScope.Current.AddMessage("    {0}:", header);

            foreach (var statistic in indexes)
            {
                EventDetailsScope.Current.AddMessage("    Index: [{0}].[{1}] ( FillFactor:{2}%, Fragmentation: {3}, RowCount: {4}, PageCount: {5} )",
                    statistic.TableName,
                    statistic.IndexName,
                    statistic.FillFactor,
                    statistic.Fragmentation,
                    statistic.RowCount,
                    statistic.PageCount);
            }
        }

    }
}

