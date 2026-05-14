using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools
{
    public class DBUpdateLibraryTestHelper
    {
        private readonly IntegrationTestingFramework _framework;
        private readonly string _dummyTableName;
        public const string TestLinkedServerName = "IT_LinkedServer_ForTest";
        private readonly IDatabaseWorker _databaseWorker;

        private string _tempPath;
        private string TempPath
        {
            get
            {
                if (string.IsNullOrEmpty(_tempPath))
                {
                    string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                    _tempPath = Path.Combine(executingPath, @"Database.Project.For.Tests\Temp_" + Process.GetCurrentProcess().Id);
                }

                return _tempPath;
            }
        }

        private readonly ManualResetEvent _runningLongScriptEvent = new ManualResetEvent(false);

        public DBUpdateLibraryTestHelper(IntegrationTestingFramework framework, string dummyTableName)
        {
            _framework = framework;
            _dummyTableName = dummyTableName;
        }

        public DBUpdateLibraryTestHelper(IntegrationTestingFramework framework, IDatabaseWorker databaseWorker)
        {
            _framework = framework;
            _databaseWorker = databaseWorker;
        }

        public void RunLongScript()
        {
            try
            {
                _runningLongScriptEvent.Reset();

                using (new ConnectionScope(_framework.DbEngine.ConnectionString))
                {
                    _runningLongScriptEvent.Set();

                    _framework.DbEngine.ExecuteNonQuery($@"
                        WAITFOR DELAY '00:00:01'; 
                        WAITFOR DELAY '00:00:01'; 
                        WAITFOR DELAY '00:00:01'; 
                        WAITFOR DELAY '00:00:01';
                        WAITFOR DELAY '00:00:01'; 
                        CREATE TABLE [dbo].[{_dummyTableName}]( [id] [int] )", CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("An error occured during execution of long script: " + ex);
            }
        }

        public void WaitWhileLongScriptIsStarted()
        {
            if (!_runningLongScriptEvent.WaitOne(10000))
            {
                Assert.Fail("Long script hasn't started for 10 sec");
            }
        }

        public string[] GetAllDatabaseNames()
        {
            string query = "sp_databases";
            var dt = _framework.DbEngine.ExecuteDataTable<DataTable>(query, CommandType.Text);

            return (from DataRow row in dt.Rows select row[0].ToString()).ToArray();
        }

        public void CreateTempFolderPath()
        {
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
        }

        public void RemoveTempFolderPath()
        {
            if (Directory.Exists(TempPath))
            {
                Directory.Delete(TempPath, true);
            }
        }

        public void RemoveFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void CreateTestLinkedServer(string dataSource)
        {
            RemoveTestLinkedServer();

            string query = string.Format(@"sp_addlinkedserver
                @server = '{0}',
                @provider = 'SQLNCLI11',
                @srvproduct = '{1}',
                @datasrc = '{1}';", TestLinkedServerName, dataSource);

            _framework.DbEngine.ExecuteNonQuery(query, CommandType.Text);
        }

        public void RemoveTestLinkedServer()
        {
            string query = $"select count(*) from sys.servers where name = '{TestLinkedServerName}'";

            if (_framework.DbEngine.ExecuteScalar<int>(query, CommandType.Text) > 0)
            {
                query = $"sp_dropserver @server = '{TestLinkedServerName}'";
                _framework.DbEngine.ExecuteNonQuery(query, CommandType.Text);
            }
        }
    }
}