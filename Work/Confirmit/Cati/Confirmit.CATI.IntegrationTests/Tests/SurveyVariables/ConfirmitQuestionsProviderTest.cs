using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.SearchableFields;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.SurveyVariables
{
    [TestClass]
    public class ConfirmitQuestionsProviderTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IConfirmitQuestionsProvider _confirmitQuestionsProvider;
        private BackendTools _backendTools;
        const string Project1 = "p0001231";
        private int _callsCount;
        private int _surveySid;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            _surveySid = _backendTools.CreateSurvey(Project1);

            var stubIAuthoring = _framework.RegistryStub<IAuthoringService, StubIAuthoringService>();
            stubIAuthoring.HasCatiAddonInt32 = id => true;
            stubIAuthoring.GetFormInfosStringIEnumerableOfStringSchemaSourceType = (id, formNames, type) =>
            {
                ++_callsCount;
                var result = new List<FormBase>();
                foreach (var formName in formNames)
                {
                    FormBase form = null;
                    if (formName.StartsWith("single", StringComparison.OrdinalIgnoreCase))
                    {
                        form = new SingleForm {Name = formName.ToLower()};
                    }

                    if (formName.StartsWith("open", StringComparison.OrdinalIgnoreCase))
                    {
                        form = new OpenForm {Name = formName.ToLower()};
                    }

                    if (formName.StartsWith("numeric", StringComparison.OrdinalIgnoreCase))
                    {
                        form = new OpenForm {Name = formName.ToLower(), Numeric = true};
                    }

                    result.Add(form);
                }

                return result.ToArray();
            };

            _framework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>().GetQuotaInfosInt32 = id => new QuotaInfo[]{};

            _confirmitQuestionsProvider = ServiceLocator.Resolve<ConfirmitQuestionsProvider>();
        }

        private void SetReplicationSchemaCallAttemptCount()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount" };
            SetReplicationSchemaForSpecifiedColumns(new[] { c1 });
        }

        private void SetReplicationSchemaFor5CatiColumns()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 33, Name = "TelephoneNumber" };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 34, Name = "RespondentName" };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 35, Name = "ExtensionNumber" };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 36, Name = "TimeZoneId" };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 37, Name = "DialType" };
            SetReplicationSchemaForSpecifiedColumns(new[] { c1, c2, c3, c4, c5 });
        }

        private void SetReplicationSchemaForSpecifiedColumns(ReplicationColumnInfo[] replicationColumns)
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount" };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "respondent", ReplicationColumns = replicationColumns, PrimaryKeyColumns = new[] { p1 } };

            new ManagementService().UpdateSurveyReplicationScheme(Project1, new[] { t1 });
        }

        private void SetReplicationSchema3VariablesAndCallAttemptCount()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount" };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 3, Name = "single" };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.NVarChar, MaxLength = 10, Id = 4, Name = "open" };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 5, Name = "numeric" };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };

            var t1 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c1 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c2, c3, c4 }, PrimaryKeyColumns = new[] { p2 } };

            new ManagementService().UpdateSurveyReplicationScheme(Project1, new[] { t1, t2 });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetConfirmitVariables_OnlyCallAttemptCountReplicated_EmptyCollection()
        {
            SetReplicationSchemaCallAttemptCount();

            var replicatedVariables = _confirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(_surveySid);

            Assert.IsNotNull(replicatedVariables);
            Assert.AreEqual(0, replicatedVariables.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetConfirmitVariables_AddAllColumnsForTrigger_EmptyCollection()
        {
            SetReplicationSchemaFor5CatiColumns();

            var replicatedVariables = _confirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(_surveySid);

            Assert.IsNotNull(replicatedVariables);
            Assert.AreEqual(0, replicatedVariables.Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetOrderedSearchableFields_OneFieldsWithDifferentCaseSensitive_1VariablesReturned()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount" };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "SinGLE" };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };

            var t1 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c1 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c2 }, PrimaryKeyColumns = new[] { p2 } };

            new ManagementService().UpdateSurveyReplicationScheme(Project1, new[] { t1, t2 });
            new OrderedSearchableFieldsService(new OrderedSearchableFieldsRepository()).RegenerateFields(_surveySid);
            
            var replicatedVariables = new SearchableFieldsProvider().GetOrderedSearchableFields(_surveySid);

            Assert.IsNotNull(replicatedVariables);
            Assert.AreEqual(5, replicatedVariables.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetConfirmitVariables_3VariablesAndCallAttemptCountReplicated_3VariablesWithCorrectTypes()
        {
            SetReplicationSchema3VariablesAndCallAttemptCount();

            var replicatedVariables = _confirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(_surveySid);

            Assert.IsNotNull(replicatedVariables);
            Assert.AreEqual(3, replicatedVariables.Count);

            var single = replicatedVariables.Single(x => x.Name == "single");
            Assert.AreEqual(ConfirmitVariableType.Single, single.ConfirmitVariableType);

            var open = replicatedVariables.Single(x => x.Name == "open");
            Assert.AreEqual(ConfirmitVariableType.Open, open.ConfirmitVariableType);

            var numeric = replicatedVariables.Single(x => x.Name == "numeric");
            Assert.AreEqual(ConfirmitVariableType.Numeric, numeric.ConfirmitVariableType);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetConfirmitVariables_3VariablesAndCallAttemptCountReplicated_3VariablesInCorrectOrder()
        {
            SetReplicationSchema3VariablesAndCallAttemptCount();

            var replicatedVariables = _confirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(_surveySid);

            Assert.IsNotNull(replicatedVariables);
            Assert.AreEqual(3, replicatedVariables.Count);

            Assert.AreEqual("numeric", replicatedVariables[0].Name);
            Assert.AreEqual("open", replicatedVariables[1].Name);
            Assert.AreEqual("single", replicatedVariables[2].Name);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetConfirmitVariables_3VariablesAndCallAttemptCountReplicated_1WsCall()
        {
            SetReplicationSchema3VariablesAndCallAttemptCount();

            _confirmitQuestionsProvider.GetReplicatedQuestionsOrderedByName(_surveySid);

            Assert.AreEqual(1,_callsCount);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetOrderedSearchableFields_3VariablesAndCallAttemptCountReplicated_3NotAvailableInConsoleFields()
        {
            var systemFields = new [] { "RespondentName", "TelephoneNumber", "ITSName", "TimeToCall"  };
            
            SetReplicationSchema3VariablesAndCallAttemptCount();

            var orderedSearchableFieldsService = new OrderedSearchableFieldsService(new OrderedSearchableFieldsRepository());
            orderedSearchableFieldsService.RegenerateFields(_surveySid);
            
            var consoleFields = new SearchableFieldsProvider().GetOrderedSearchableFields(_surveySid);
            
            Assert.AreEqual(7, consoleFields.Count);
            CollectionAssert.AreEqual(new [] { _surveySid, _surveySid, _surveySid, _surveySid, _surveySid, _surveySid, _surveySid }, 
                consoleFields.Select(x => x.SurveyId).ToArray());
            CollectionAssert.AreEqual(new List<string>(systemFields) { "numeric", "open", "single" },
                consoleFields.Select(x => x.FieldName).ToList());
            CollectionAssert.AreEqual(new List<string>(systemFields) { "numeric", "open", "single" },
                consoleFields.Select(x => x.DisplayName).ToList());
            CollectionAssert.AreEqual(new [] { true, true, true, true, false, false, false }, 
                consoleFields.Select(x => x.IsSystem).ToArray());
            CollectionAssert.AreEqual(new [] { true, true, true, false, false, false, false }, 
                consoleFields.Select(x => x.IsEnabled).ToArray());
            CollectionAssert.AreEqual(new [] { 0, 1, 2, 3, 4, 5, 6 }, 
                consoleFields.Select(x => x.OrderNumber).ToArray());
            CollectionAssert.AreEqual(new [] { "System", "System", "System", "System", "Numeric", "Open", "Single" }, 
                consoleFields.Select(x => x.FieldType).ToArray());

            var dbEngine = new DatabaseEngine();
            var sql = "UPDATE [BvReplicationColumns] SET [ColumnName] = 'single1' WHERE [ColumnName] = 'single'";
            dbEngine.ExecuteNonQuery(sql);
            
            orderedSearchableFieldsService.RegenerateFields(_surveySid);
            
            consoleFields = new SearchableFieldsProvider().GetOrderedSearchableFields(_surveySid);
            CollectionAssert.AreEqual(new List<string>(systemFields) { "numeric", "open", "single1" },
                consoleFields.Select(x => x.FieldName).ToList());
            CollectionAssert.AreEqual(new [] { 0, 1, 2, 3, 4, 5, 6 }, 
                consoleFields.Select(x => x.OrderNumber).ToArray());
         
            sql = @"UPDATE [BvSearchableFieldsOrdered] SET [OrderNumber] = 1 WHERE [FieldName] = 'open'
                    UPDATE [BvSearchableFieldsOrdered] SET [OrderNumber] = 5 WHERE [FieldName] = 'TelephoneNumber'";
            dbEngine.ExecuteNonQuery(sql);
         
            consoleFields = new SearchableFieldsProvider().GetOrderedSearchableFields(_surveySid);
            CollectionAssert.AreEqual(new List<string>{"RespondentName", "open", "ITSName", "TimeToCall", "numeric", "TelephoneNumber", "single1" },
                consoleFields.Select(x => x.FieldName).ToList());
            CollectionAssert.AreEqual(new [] { 0, 1, 2, 3, 4, 5, 6 }, 
                consoleFields.Select(x => x.OrderNumber).ToArray());
            
            sql = "UPDATE [BvReplicationColumns] SET [ColumnName] = 'open1' WHERE [ColumnName] = 'open'";
            dbEngine.ExecuteNonQuery(sql);
            
            orderedSearchableFieldsService.RegenerateFields(_surveySid);
            
            consoleFields = new SearchableFieldsProvider().GetOrderedSearchableFields(_surveySid);
            CollectionAssert.AreEqual(new List<string>{"RespondentName", "ITSName", "TimeToCall", "numeric", "TelephoneNumber", "single1", "open1" },
                consoleFields.Select(x => x.FieldName).ToList());
            CollectionAssert.AreEqual(new [] { 0, 1, 2, 3, 4, 5, 6 }, 
                consoleFields.Select(x => x.OrderNumber).ToArray());

        }
        
        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetIntegerBasedReplicatedColumns_3VariablesAndCallAttempCountInReplicatedTable_2IntBasedReturned()
        {
            SetReplicationSchema3VariablesAndCallAttemptCount();
            var intBasedColumns = _confirmitQuestionsProvider.GetIntegerBasedReplicatedColumns(_surveySid);

            Assert.AreEqual(2, intBasedColumns.Count);
            Assert.IsNotNull(intBasedColumns.Single(x => x.Name == "single"));
            Assert.IsNotNull(intBasedColumns.Single(x => x.Name == "numeric"));
        }
    }
}