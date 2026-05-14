using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIDatabaseWorker : IDatabaseWorker 
    {
        private IDatabaseWorker _inner;

        public StubIDatabaseWorker()
        {
            _inner = null;
        }

        public IDatabaseWorker Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string CreateConnectionStringStringDelegate(string databaseName);
        public CreateConnectionStringStringDelegate CreateConnectionStringString;

        string IDatabaseWorker.CreateConnectionString(string databaseName)
        {


            if (CreateConnectionStringString != null)
            {
                return CreateConnectionStringString(databaseName);
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).CreateConnectionString(databaseName);
            }

            return default(string);
        }

        public delegate bool KillProcessesStringDelegate(string databaseName);
        public KillProcessesStringDelegate KillProcessesString;

        bool IDatabaseWorker.KillProcesses(string databaseName)
        {


            if (KillProcessesString != null)
            {
                return KillProcessesString(databaseName);
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).KillProcesses(databaseName);
            }

            return default(bool);
        }

        public delegate bool IsDatabaseExistsStringDelegate(string databaseName);
        public IsDatabaseExistsStringDelegate IsDatabaseExistsString;

        bool IDatabaseWorker.IsDatabaseExists(string databaseName)
        {


            if (IsDatabaseExistsString != null)
            {
                return IsDatabaseExistsString(databaseName);
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).IsDatabaseExists(databaseName);
            }

            return default(bool);
        }

        public delegate DatabaseUserAccess GetUserAccessStringDelegate(string databaseName);
        public GetUserAccessStringDelegate GetUserAccessString;

        DatabaseUserAccess IDatabaseWorker.GetUserAccess(string databaseName)
        {


            if (GetUserAccessString != null)
            {
                return GetUserAccessString(databaseName);
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).GetUserAccess(databaseName);
            }

            return default(DatabaseUserAccess);
        }

        public delegate string ExecuteSqlScriptStringStringDelegate(string sqlQuery, string databaseName);
        public ExecuteSqlScriptStringStringDelegate ExecuteSqlScriptStringString;

        string IDatabaseWorker.ExecuteSqlScript(string sqlQuery, string databaseName)
        {


            if (ExecuteSqlScriptStringString != null)
            {
                return ExecuteSqlScriptStringString(sqlQuery, databaseName);
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).ExecuteSqlScript(sqlQuery, databaseName);
            }

            return default(string);
        }

        public delegate void UpdateRegenerateIsRequiredFlagStringDelegate(string databaseName);
        public UpdateRegenerateIsRequiredFlagStringDelegate UpdateRegenerateIsRequiredFlagString;

        void IDatabaseWorker.UpdateRegenerateIsRequiredFlag(string databaseName)
        {

            if (UpdateRegenerateIsRequiredFlagString != null)
            {
                UpdateRegenerateIsRequiredFlagString(databaseName);
            } else if (_inner != null)
            {
                ((IDatabaseWorker)_inner).UpdateRegenerateIsRequiredFlag(databaseName);
            }
        }

        public delegate string[] GetAllDatabaseNamesDelegate();
        public GetAllDatabaseNamesDelegate GetAllDatabaseNames;

        string[] IDatabaseWorker.GetAllDatabaseNames()
        {


            if (GetAllDatabaseNames != null)
            {
                return GetAllDatabaseNames();
            } else if (_inner != null)
            {
                return ((IDatabaseWorker)_inner).GetAllDatabaseNames();
            }

            return default(string[]);
        }

    }
}