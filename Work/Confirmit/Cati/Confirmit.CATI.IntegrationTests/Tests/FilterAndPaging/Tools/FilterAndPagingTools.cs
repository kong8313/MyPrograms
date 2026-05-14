using System;
using System.IO;
using System.Collections.Generic;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;

using Confirmit.CATI.IntegrationTests.Framework.Tools;
using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools
{
    public class FilterAndPagingTools
    {
        private readonly IntegrationTestingFramework _framework;
        private readonly BackendTools _backendTools;

        private static int _filtersCount;

        internal enum SampleType
        {
            SmallSample,
            MiddleSample,
            LargeSample
        }

        public FilterAndPagingTools(IntegrationTestingFramework framework, BackendTools backendTools)
        {
            _framework = framework;
            _backendTools = backendTools;
        }

        internal DatabaseEngine CreateCFSurveyDatabaseEngine()
        {
            return new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(_framework.TestSurveyDatabaseName));
        }

        public void AddAdditionalColumnsToRespondentTable(DatabaseEngine db, string[] additionalRespondentColumns)
        {
            foreach (var columnName in additionalRespondentColumns)
            {
                db.ExecuteNonQuery(
                    String.Format("ALTER TABLE respondent ADD [{0}] NVARCHAR(10)", columnName),
                    CommandType.Text);
            }
        }

        internal int CreateSurveyWithSample(string projId, SampleType? sampleType = null)
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 3, Name = "q1", QuotaIds = new[] { 1 } };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 4, Name = "q2", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };

            var t1 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c1 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c2, c3 }, PrimaryKeyColumns = new[] { p2 } };

            return CreateSurveyWithSample(projId, new[] { t1, t2 }, sampleType);
        }

        internal int CreateSurveyWithSample(string projId, IEnumerable<TableInfo> tables, SampleType? sampleType = null)
        {
            var connectionString = sampleType.HasValue
                                       ? _framework.GetCatiSqlServerConnectionString(_framework.TestSurveyDatabaseName)
                                       : _framework.DbEngine.ConnectionString;
            int surveySid = _backendTools.CreateSurvey(projId, connectionString);

            new ManagementService().UpdateSurveyReplicationScheme(projId, tables.ToArray());

            const int batchId = 1;

            _backendTools.AddSample(
                projId,
                batchId,
                (int)SchedulingMode.Simple);

            var defaultCallCenterId = ServiceLocator.Resolve<ICallCenterRepository>().Default.ID;
            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(defaultCallCenterId, surveySid);

            return surveySid;
        }

        /// <summary>
        /// Creates new interview for given survey. All interview data fields are filled with default values.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="interviewId">interview id</param>
        /// <returns>Interview identifier.</returns>
        public static BvInterviewEntity CreateEmptyInterview(int surveyId, int interviewId)
        {
            var interview = new BvInterviewEntity
            {
                ID = interviewId,
                SurveySID = surveyId,
                TransientState = 16,
                RespondentName = String.Empty,
                TelephoneNumber = String.Empty
            };
            BackendTools.CreateInterview(interview);

            return interview;
        }

        internal static int CreateSimpleFilter(FilterField[] filterFields)
        {
            return CreateSimpleFilter(0, AndOrOperator.And, filterFields);
        }

        internal static int CreateSimpleFilter(int surveySid, FilterField[] filterFields)
        {
            return CreateSimpleFilter(surveySid, AndOrOperator.And, filterFields);
        }

        internal static int CreateSimpleFilter(int surveySid, AndOrOperator operation, FilterField[] filterFields)
        {
            var bvFiltersEntity = new BvFiltersEntity
            {
                SurveySID = surveySid,
                Name = "SimpleFilter" + _filtersCount++,
                AndOrOperator = (byte)operation
            };
            int filterSid = FilterRepository.Insert(bvFiltersEntity);

            var fields = new List<BvFilterFieldsEntity>();

            foreach (FilterField filterField in filterFields)
            {
                fields.Add(filterField.GetBvFilterFieldsEntity(filterSid));
            }

            FilterService.SetFields(filterSid, fields);

            return filterSid;
        }

        internal static void Compare(DataTable actualTable, string path, string orderedBy)
        {
            const string defaultTableName = "table";

            if (String.IsNullOrEmpty(actualTable.TableName))
                actualTable.TableName = defaultTableName;

            var expectedTable = DatasetEngine.ReadDataTableFromXml<DataTable>(
                Path.Combine(path, "test.xsd"),
                Path.Combine(path, "test.xml"),
                defaultTableName);

            DatasetEngine.AreEqual(expectedTable, actualTable, orderedBy);
        }

        internal static void Compare(DataTable actualTable, string path)
        {
            Compare(actualTable, path, (string)null);
        }

        internal static void Compare(DataTable actualTable, string path, string[] ignoreColumns)
        {
            DataTable table;
            if (ignoreColumns != null && ignoreColumns.Length > 0)
            {
                DataTable tmp = actualTable.Copy();
                foreach (string columnName in ignoreColumns)
                {
                    if (tmp.Columns.Contains(columnName))
                    {
                        tmp.Columns.Remove(columnName);
                    }
                }

                table = tmp;
            }
            else
            {
                table = actualTable;
            }

            Compare(table, path);
        }

        //set priority = id calls. (in this case we will know which of this calls will be activated)
        internal static void UpdatePriority(DatabaseEngine db)
        {
            const string query = @"update bvsvyschedule
                             SET priority = ID";
            db.ExecuteNonQuery(query, CommandType.Text);
        }

        internal static void UpdateCallAttemptCount(DatabaseEngine db)
        {
            const string query = @"update respondent
                             SET CallAttemptCount = respid";
            db.ExecuteNonQuery(query, CommandType.Text);

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();
        }

        internal static void UpdateFilterFields(int filterSid, FilterField[] filterFields)
        {
            UpdateFilterFields(0, filterSid, filterFields);
        }

        internal static void UpdateFilterFields(int surveySid, int filterSid, FilterField[] filterFields)
        {
            BvFiltersEntity filterEntity = FilterRepository.GetById(filterSid);
            filterEntity.SurveySID = surveySid;
            FilterRepository.Update(filterEntity);

            FilterService.SetFields(filterSid, filterFields.Select(x => x.GetBvFilterFieldsEntity(filterSid)).ToList());
        }

    }
}
