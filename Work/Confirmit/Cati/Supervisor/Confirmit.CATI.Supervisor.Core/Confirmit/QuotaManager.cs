using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;
using Confirmit.CATI.Supervisor.Core.Resources;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    /// <summary>
    /// Class contains additional business logic for quota-related management via CF web services.
    /// </summary>
    /// <remarks>
    /// CATI has no access to the test database/quotas, so we always use production counters in <see cref="QuotaMode"/>.
    /// </remarks>
    public static class QuotaManager
    {
        private const string Any = "[Any]";
        private const string Counter = "Counter";
        public const string CounterPercentage = "CounterPercentage";

        private const string PrecodePostfix = "_precode";

        /// <summary>
        /// Name of the <see cref="DataColumn"/> extended property that determines if column is hidden.
        /// </summary>
        public const string Hidden = "Hidden";
        private const string Id = "ID";
        public const string Priority = "QuotaBalancingPriority";
        private const string Limit = "Limit";
        public const string Remaining = "Remaining";
        public const string IsDisabled = "IsDisabled";
        public const string InProgress = "InProgress";
        public const string OptimisticTotalLimit = "OptimisticTotalLimit";
        public const string ExtraCounter = "ExtraCounter";
        public const string UsedCalls = "UsedCalls";
        public const string BurnRate = "BurnRate";
        public const string DailyQuotaCounter = "DailyQuotaCounter";

        /// <summary>
        /// Gets the design quota list for a quota.
        /// Note. The QuotaRowId on the QuotaRow object (which is contained by QuotaList) is only set if the quota is synchronized. If not available, it is set to -1.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        public static QuotaList GetQuotaList(string projectId, string quotaName)
        {
            QuotaList quotaList;
            try
            {
                var authoringService = ServiceLocator.Resolve<IAuthoringService>();

                quotaList = authoringService.GetQuotaList(projectId, quotaName, QuotaMode.DesignWithProductionCounter);
            }
            catch (Exception ex)
            {
                throw new QuotaNotInSyncException(Strings.QuotaDefinitionIsNotInSyncWithTheDatabase, ex);
            }
            return quotaList;
        }

        public static IExtraQuotaCounterParameters GetExtraCounterParameters(ExtraQuotaCounterTypes extraCounter, int surveyId, int quotaId, bool includeDisabledCalls, int[] itses, (DateTime startDate, DateTime endDate)? period = null)
        {
            var quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
            switch (extraCounter)
            {
                case ExtraQuotaCounterTypes.None:
                    return null;
                case ExtraQuotaCounterTypes.DailyCounter:
                    return new DailyCounterParameter(surveyId, quotaId, itses, quotaInfoService.GetQuotaFields(surveyId, quotaId), period);
                case ExtraQuotaCounterTypes.Scheduled:
                    return new CallsCounterParameter(surveyId, quotaId, includeDisabledCalls, null, quotaInfoService.GetQuotaFields(surveyId, quotaId));
                case ExtraQuotaCounterTypes.ScheduledWithSpecificStatuses:
                    return new CallsCounterParameter(surveyId, quotaId, includeDisabledCalls, itses, quotaInfoService.GetQuotaFields(surveyId, quotaId));
                case ExtraQuotaCounterTypes.InterviewsWithSpecificStatuses:
                    return new InterviewsCounterParameter(surveyId, quotaId, itses, quotaInfoService.GetQuotaFields(surveyId, quotaId));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the <see cref="DataTable"/> with quota information to show in grid.
        /// </summary>
        /// <remarks>
        /// It constructs a data table with all needed info using GetQuotaList and GetFormInfosWithText web methods.
        /// Column captions contains both question ID and title: 'q1 (question 1)'
        /// Data in these columns are the answer texts and not precodes.
        /// If there are multiple languages in the survey - the first available language is used.
        /// </remarks>
        /// <exception cref="QuotaNotInSyncException">Quota definition is not in sync with the database.</exception>
        public static DataTable CreateQuotaDataTable(List<SingleForm> fields, IQuotaViewAdditionalColumnsBuilder additionalColumnsBuilder)
        {
            // Create resulting data table.
            var result = new DataTable();

            // Fill columns.
            result.Columns.Add(Id, typeof(int)).ExtendedProperties[Hidden] = true;

            foreach (var field in fields)
            {
                string caption = string.Format("{0} ({1})", field.Name, field.FormTexts.First().Title);
                DataColumn column = new DataColumn(field.Name) { Caption = caption };
                result.Columns.Add(column);
                var precodeColumn = new DataColumn(field.Name + PrecodePostfix);
                precodeColumn.ExtendedProperties[Hidden] = true;
                result.Columns.Add(precodeColumn);
            }

            result.Columns.Add(Limit, typeof(int));
            result.Columns.Add(Counter, typeof(int));
            result.Columns.Add(CounterPercentage, typeof(int));
            result.Columns.Add(Remaining, typeof(int));
            result.Columns.Add(IsDisabled, typeof(bool)).ExtendedProperties[Hidden] = true;
            additionalColumnsBuilder.AddColumns(result.Columns);

            return result;
        }

        public static void FillQuotaDataTable(DataTable result, QuotaList quotaList, List<SingleForm> fields, IQuotaViewAdditionalColumnsBuilder additionalColumnsBuilder)
        {
            foreach (QuotaRow row in quotaList.QuotaRows)
            {
                var dataRow = result.Rows.Add();

                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];

                    var answer = field.SingleAnswers.Items.FirstOrDefault(x => x.Precode == row.FieldPrecodes[i]) as Answer;

                    dataRow[field.Name] = answer != null ? answer.Texts.First().Value : Any;
                    dataRow[field.Name + PrecodePostfix] = answer != null ? answer.Precode : Any;
                }

                dataRow[Id] = row.QuotaRowId;
                dataRow[Limit] = row.Target;
                dataRow[Counter] = row.Counter == -1 ? (object)DBNull.Value : row.Counter;
                dataRow[CounterPercentage] = CalculateQuotaPercentage(row);
                dataRow[Remaining] = row.Counter == -1 ? (object)DBNull.Value : row.Target - row.Counter;
                dataRow[IsDisabled] = row.IsDisabled ?? false;

                additionalColumnsBuilder.FillRow(dataRow, quotaList, row);
            }
        }


        private static int CalculateQuotaPercentage(QuotaRow quotaRow)
        {
            var quotaPercentage = 101;
            if (quotaRow.Target > 0)
            {
                quotaPercentage = quotaRow.Counter == -1 ? 0 : (int)(Math.Round(quotaRow.Counter / (float)quotaRow.Target * 100));
            }

            return quotaPercentage > 100 ? 100 : quotaPercentage;
        }

        /// <summary>
        /// Updates the quota list in design mode.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        /// <param name="quotaList">The quota list to update with.</param>
        public static void UpdateQuotaList(string projectId, string quotaName, QuotaList quotaList)
        {
            var authoringService = ServiceLocator.Resolve<IAuthoringService>();
            authoringService.UpdateQuotaList(projectId, quotaName, quotaList, DatabaseType.Production);
        }

        /// <summary>
        /// Gets the names of production CATI quotas.
        /// </summary>
        /// <param name="surveySid">The survey SID to get quotas for.</param>
        /// <returns></returns>
        public static string[] GetQuotaNames(int surveySid)
        {
            return GetQuotaNamesAndIds(surveySid).Select(x => x.Name).ToArray();
        }

        /// <summary>
        /// Synchronizes the quota from design to production.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        public static void SynchronizeQuota(string projectId, string quotaName)
        {
            var authoringService = ServiceLocator.Resolve<IAuthoringService>();
            authoringService.SynchronizeQuota(projectId, quotaName, DatabaseType.Production);
        }

        /// <summary>
        /// Determines whether design quota is synchronized with the production one.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        public static QuotaSyncState GetQuotaState(string projectId, string quotaName)
        {
            QuotaList designQuota;
            QuotaList productionQuota;
            try
            {
                var authoringService = ServiceLocator.Resolve<IAuthoringService>();
                designQuota = authoringService.GetQuotaList(projectId, quotaName, QuotaMode.DesignWithProductionCounter);
                productionQuota = authoringService.GetQuotaList(projectId, quotaName, QuotaMode.Production);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.ToString());
                return QuotaSyncState.NotSynchronized;
            }

            var designCells = designQuota.QuotaRows.OrderBy(x => x.QuotaRowId).ToList();
            var productionCells = productionQuota.QuotaRows.OrderBy(x => x.QuotaRowId).ToList();

            if (designCells.Count() != productionCells.Count())
            {
                return QuotaSyncState.NotSynchronized;
            }

            if (productionQuota.QuotaRows.Any(x => x.IsDisabled == null) && designQuota.QuotaRows.Any(x => x.IsDisabled == true))
            {
                return QuotaSyncState.NotSynchronized;
            }

            if (productionQuota.QuotaRows.Any(x => x.Priority == null) && designQuota.QuotaRows.Any(x => x.Priority != QuotaLimitPriority.Medium))
            {
                return QuotaSyncState.NotSynchronized;
            }

            var designCellsIds = designCells.Select(x => x.QuotaRowId);
            var productionCellsIds = productionCells.Select(x => x.QuotaRowId);

            for (int i = 0; i < designCells.Count(); i++)
            {
                if (designCellsIds.ElementAt(i) != productionCellsIds.ElementAt(i))
                {
                    return QuotaSyncState.NotSynchronized;
                }
            }

            return QuotaSyncState.Synchronized;
        }

        public static IEnumerable<QuotaDetails> GetQuotaNamesAndIds(int surveySid)
        {
            var survey = SurveyRepository.GetById(surveySid);
            var authoringService = ServiceLocator.Resolve<IAuthoringService>();
            var designQuotaNames = authoringService.GetQuotaNames(survey.Name, QuotaMode.DesignWithProductionCounter);

            var seQuotaRepository = ServiceLocator.Resolve<ISeQuotaRepository>();
            return seQuotaRepository
                .GetAll(surveySid)
                .Where(x => designQuotaNames.Contains(x.Name))
                .Select(x => new QuotaDetails()
                {
                    Id = x.QuotaID,
                    Name = x.Name
                })
                .ToArray();
        }

        public static string[] GetBalancedQuotaNames(int surveyId)
        {
            var configuration = ServiceLocator.Resolve<IQuotaBalancingService>().GetQuotaBalancingConfiguration(surveyId);

            return configuration.Quotas.Where(x => x.IsEnabled).Select(x => x.QuotaName).ToArray();
        }

        public static string[][] GetCellsValues(QuotaList quotaList, List<int> cellIds, string[] usedFields)
        {
            var cells = cellIds;
            var fields = quotaList.FieldNames.Select((x, i) => new { Name = x, Pos = i }).ToArray();
            var fieldIndexes = usedFields.Select(x => fields.Single(y => y.Name == x).Pos).ToArray();

            var cellsFields = from row in quotaList.QuotaRows
                              where cells.Contains(row.QuotaRowId)
                              select fieldIndexes.Select(i => row.FieldPrecodes[i]).ToArray();

            //distinct
            cellsFields = from f in cellsFields group f by String.Join(",", f) into g select g.First();
            return cellsFields.ToArray();
        }
    }
}
