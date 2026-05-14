using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.WcfServices.Clients;
using Microsoft.SqlServer.Management.Smo;

using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestQuota
    {
        private const string QuotaTableFormat = "quota_{0}";
        private const string ReplicatedTableFormat = "BvReplicatedData_{0}";

        private readonly IQuotaBalancingService _quotaBalancingService;

        public int QuotaId { get; set; }

        public string[] FieldNames { get; set; }

        public int[] AnswerCounts { get; set; }

        public DatabaseEngine Db { get; set; }

        public int SurveySid { get; set; }

        public string ProjectId { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public class CellInfo
        {
            public int CellId;
            public string[] FieldPrecodes;
        }

        public IEnumerable<CellInfo> GetAllCells()
        {
            string quotaTable = String.Format(QuotaTableFormat, QuotaId);
            string query = String.Format("SELECT * FROM {0}", quotaTable);

            using (var reader = Db.ExecuteReaderInNewConnection(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    yield return new CellInfo
                    {
                        CellId = (int)reader["quotaId"],
                        FieldPrecodes = FieldNames.Select(x => (string)reader[x]).ToArray()
                    };
                }
            }
        }

        private TestQuota(DatabaseEngine db, int surveySid, int quotaId, string[] fieldNames, int[] answerCounts)
        {
            QuotaId = quotaId;
            FieldNames = fieldNames;
            AnswerCounts = answerCounts;
            Db = db;
            SurveySid = surveySid;
            _quotaBalancingService = ServiceLocator.Resolve<IQuotaBalancingService>();

            ProjectId = SurveyRepository.GetById(SurveySid).Name;
        }

        public static TestQuota Create(
            DatabaseEngine db,
            int surveySid,
            int quotaId,
            string[] columnNames,
            int[] answerCounts,
            bool withReplicationTable = false)
        {
            var cellsCount = answerCounts.Any() ? answerCounts.Aggregate((x, y) => x * y) : 0;
            var counters = Enumerable.Repeat(1, cellsCount).ToArray();
            var limits = Enumerable.Repeat(2, cellsCount).ToArray();
            return Create(db, surveySid, quotaId, columnNames, answerCounts, counters, limits, withReplicationTable);
        }

        public void MarkQuotaAsBalanced(int promotionPriority, string[] filterField, int promotionThreshold)
        {
            var configuration = _quotaBalancingService.GetQuotaBalancingConfiguration(SurveySid);
            configuration.Quotas.Single(x => x.QuotaId == QuotaId).IsEnabled = true;
            foreach (var field in filterField)
            {
                configuration.Fields.Single(x => x.FieldName.Equals(field, StringComparison.OrdinalIgnoreCase)).IsEnabled = true;
            }

            configuration.PromotionPriority = promotionPriority;
            configuration.PromotionThreshold = promotionThreshold;

            _quotaBalancingService.SetQuotaBalancingConfiguration(SurveySid, configuration);
        }

        public static TestQuota Create(
            DatabaseEngine db,
            int surveySid,
            int quotaId,
            string[] columnNames,
            int[] answerCounts,
            int[] counter,
            int[] limit,
            bool withReplicationTable = false)
        {
            //correct replicated data table
            string replicatedTable = String.Format(ReplicatedTableFormat, surveySid);

            IntegrationTestingFramework.Instance.DbEngine.AddColumnsToTable(replicatedTable,
                columnNames.Select(x => new KeyValuePair<string, DataType>(x, DataType.NVarChar(100))).ToArray());

            if (withReplicationTable)
            {
                int tableId = ReplicationTablesRepository.Insert(
                    new BvReplicationTablesEntity
                    {
                        LastVersion = 0,
                        PrimaryKey = "respid",
                        SurveySid = surveySid,
                        TableName = quotaId.ToString(CultureInfo.InvariantCulture)
                    });

                int columnId = 1;

                foreach (var columnName in columnNames)
                {
                    ReplicationColumnsRepository.Insert(new BvReplicationColumnsEntity
                    {
                        ColumnID = columnId++,
                        ColumnMaxLength = 4,
                        ColumnName = columnName,
                        ColumnType = (int) SqlDataType.Int,
                        TableID = tableId
                    });
                }
            }

            if (columnNames.Length > 0)
            {
                IntegrationTestingFramework.Instance.DbEngine.ExecuteNonQuery(
                    String.Format(@"if not exists( select * from sys.indexes WHERE name = '{0}' AND object_id = OBJECT_ID('{1}') )
                                  create nonclustered index {0} on {1} ({2}) include (respid)",
                        ServiceLocator.Resolve<IReplicationIndexService>().GetQuotaIndexName(quotaId),
                        replicatedTable,
                        String.Join(",", columnNames)),
                    CommandType.Text);
            }

            var name = "q" + quotaId.ToString(CultureInfo.InvariantCulture);

            //create quota
            string quotaTable = String.Format(QuotaTableFormat, quotaId);
            db.ExecuteNonQuery("INSERT INTO quotas VALUES(@quotaid, @quotaname, @tablename, 1)",
                CommandType.Text,
                new SqlParameter("@quotaid", quotaId),
                new SqlParameter("@quotaname", name),
                new SqlParameter("@tablename", quotaTable));

            foreach (var columnName in columnNames)
            {
                db.ExecuteNonQuery("INSERT INTO quota_field VALUES(@quotaid, @fieldname)",
                    CommandType.Text,
                    new SqlParameter("@quotaid", quotaId),
                    new SqlParameter("@fieldname", columnName));
            }

            db.CreateTable(quotaTable,
                new[]
                {
                    new KeyValuePair<string, DataType>("quotaid", DataType.Int),
                    new KeyValuePair<string, DataType>("limit", DataType.Int),
                    new KeyValuePair<string, DataType>("counter", DataType.Int)
                }.Union(columnNames.Select(x => new KeyValuePair<string, DataType>(x, DataType.NVarChar(32)))).ToArray());

            int cellid = 1;
            if (answerCounts.Length > 0)
            {
                foreach (var answer in GetAllPermutations(answerCounts))
                {
                    string query = "INSERT INTO " + quotaTable +
                                   " VALUES(@quotaid, @limit, @counter";
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@quotaid", cellid),
                        new SqlParameter("@limit", limit[cellid-1]),
                        new SqlParameter("@counter", counter[cellid-1])
                    };

                    cellid++;

                    for (int i = 0; i < answerCounts.Length; ++i)
                    {
                        query += ", @" + columnNames[i];
                        parameters.Add(new SqlParameter("@" + columnNames[i], answer[i]));
                    }

                    query += ")";

                    db.ExecuteNonQuery(query,
                        CommandType.Text,
                        parameters.ToArray());
                }
            }

            ServiceLocator.Resolve<IFcdQuotaService>().OnLaunchSurvey(surveySid, true, default);

            return new TestQuota(
                db,
                surveySid,
                quotaId,
                columnNames,
                answerCounts)
            {
                Name = name,
                TableName = quotaTable
            };
        }

        public int GetQuotaCell(params string[] answers)
        {
            string quotaTable = String.Format(QuotaTableFormat, QuotaId);
            string query = String.Format("SELECT quotaId FROM {0} WHERE {1}",
                quotaTable,
                String.Join(" AND ", FieldNames.Select((x, i) => String.Format("[{0}] = '{1}'", x, answers[i])).ToArray()));

            return Db.ExecuteScalar<int>(query, CommandType.Text);
        }

        public void PutInterviewsInCells(
            int[] interviewIds,
            int[] cellIds)
        {
            string quotaTable = String.Format(QuotaTableFormat, QuotaId);
            string replicatedTable = IntegrationTestingFramework.Instance.DbEngine.DatabaseName + ".dbo." +
                                     String.Format(ReplicatedTableFormat, SurveySid);

            string delemitedColumns = String.Join(", ", FieldNames);
            string delemitedColumnsWithTableName = String.Join(", ", FieldNames.Select(x => "source." + x).ToArray());

            for (int i = 0; i < interviewIds.Length; ++i)
            {
                string query = "merge " + replicatedTable + " as target " +
                               "USING (SELECT @respid, " + delemitedColumns +
                               "       FROM " + quotaTable +
                               "       WHERE quotaid = @cellId) " +
                               "AS source (respid, " + delemitedColumns + " ) ON target.respid=source.respid " +
                               "WHEN NOT MATCHED THEN " +
                               "   INSERT(respid, " + delemitedColumns + ") VALUES(source.respid, " + delemitedColumnsWithTableName + ") " +
                               "WHEN MATCHED THEN " +
                               "   UPDATE SET " + String.Join(", ", FieldNames.Select(
                                   x => String.Format("target.{0} = source.{0}", x)).ToArray()) + ";";

                Db.ExecuteNonQuery(query,
                    CommandType.Text,
                    new SqlParameter("@respid", interviewIds[i]),
                    new SqlParameter("@cellId", cellIds[i]));
            }
        }

        public void ConfirmitCloseCell(int cellId)
        {
            string quotaTable = String.Format(QuotaTableFormat, QuotaId);
            
            Db.ExecuteNonQuery(String.Format(
                "update {0} set counter = limit where quotaid = {1}",
                quotaTable,
                cellId
            ), CommandType.Text);
        }

        public void CloseCell(int cellId)
        {
            ConfirmitCloseCell(cellId);
            new ManagementService().OnQuotaCellsChanged(ProjectId, QuotaId, new int[] { }, new[] { cellId }, new int[] { });
            BackendTools.ExecuteAllAsyncOperations();
        }

        public void QuotaUpdate()
        {
            new ManagementService().OnQuotaChanged(ProjectId, QuotaId);
            BackendTools.ExecuteAllAsyncOperations();
        }

        private static IEnumerable<List<int>> GetAllPermutations(int[] maxValues)
        {
            var permutations = new List<List<int>>();

            //first permutation
            int[] permutation = maxValues.Select(x => 1).ToArray();
            permutations.Add(new List<int>(permutation));

            for (int i = maxValues.Length - 1; i >= 0; --i)
            {
                permutation[i]++;

                if (permutation[i] <= maxValues[i])
                {
                    for (int j = i + 1; j < maxValues.Length; ++j)
                        permutation[j] = 1;

                    i = maxValues.Length;
                    permutations.Add(new List<int>(permutation));
                }
                else
                {
                    permutation[i] = 1;
                }
            }
            return permutations;
        }

        public void PutInterviewInCell(int interviewId, string[] quotaAnswers)
        {
            string replicatedTable = IntegrationTestingFramework.Instance.DbEngine.DatabaseName + ".dbo." +
                                     String.Format(ReplicatedTableFormat, SurveySid);

            string delemitedColumns = String.Join(", ", FieldNames);
            string delemitedData = String.Join(", ", quotaAnswers.Select(x => x != null ? "'" + x + "'" : "NULL").ToArray());


            string query = "merge " + replicatedTable + " as target " +
                           "USING (SELECT " + interviewId + ")" +
                           "AS source (respid) ON target.respid=source.respid " +
                           "WHEN NOT MATCHED THEN " +
                           "   INSERT(respid, " + delemitedColumns + ") VALUES(source.respid, " + delemitedData + ") " +
                           "WHEN MATCHED THEN " +
                           "   UPDATE SET " + String.Join(", ", FieldNames.Select(
                           (x, i) => String.Format("target.{0} = {1}", x, quotaAnswers[i] == null ? "NULL" : "'" + quotaAnswers[i] + "'"))) + ";";

            Db.ExecuteNonQuery(query, CommandType.Text);
        }

        public void MockWs()
        {
            var rows = GetAllCells().ToArray();

            var designQuota =
                new QuotaList
                {
                    QuotaRows = rows.Select(x => new QuotaRow { Target = 10, Counter = 1, FieldPrecodes = x.FieldPrecodes, QuotaRowId = x.CellId }).ToArray(),
                    FieldNames = FieldNames,
                    QuotaFullEmailAddress = "123",
                    QuotaId = 123,
                    QuotaName = "quota1"
                };

            var questions = FieldNames.Select((x, i) => new SingleForm
            {
                Name = x,
                SingleAnswers = new SingleAnswers
                {
                    Items = Enumerable.Range(0, AnswerCounts[i]).Select(p => new Answer
                    {
                        Precode = p.ToString(),
                        Texts = new[] { new AnswerText { Value = x + p.ToString() } }
                    }).ToArray()
                }
            }).ToArray();

            var originalAuthoringService = ServiceLocator.Resolve<IAuthoringService>();
            var stubIAuthoringService = new StubIAuthoringService
            {
                Inner = originalAuthoringService,
                GetQuotaListStringStringQuotaMode = (id, name, mode) => designQuota,
                GetQuotaFormsStringString = (id, name) => questions,
                GetFormInfosStringIEnumerableOfStringSchemaSourceType = (id, names, type) =>
                {
                    return names.Select(x => questions.Single(y => y.Name == x)).Cast<FormBase>().ToArray();
                },
            };
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoringService);

        }
    }
}