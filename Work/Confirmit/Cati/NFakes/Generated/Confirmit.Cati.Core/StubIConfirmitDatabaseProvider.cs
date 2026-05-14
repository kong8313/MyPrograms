using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIConfirmitDatabaseProvider : IConfirmitDatabaseProvider 
    {
        private IConfirmitDatabaseProvider _inner;

        public StubIConfirmitDatabaseProvider()
        {
            _inner = null;
        }

        public IConfirmitDatabaseProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetSurveyDatabaseNameStringDelegate(string projectId);
        public GetSurveyDatabaseNameStringDelegate GetSurveyDatabaseNameString;

        string IConfirmitDatabaseProvider.GetSurveyDatabaseName(string projectId)
        {


            if (GetSurveyDatabaseNameString != null)
            {
                return GetSurveyDatabaseNameString(projectId);
            } else if (_inner != null)
            {
                return ((IConfirmitDatabaseProvider)_inner).GetSurveyDatabaseName(projectId);
            }

            return default(string);
        }

        public delegate string GetSqlServerNameStringBooleanDelegate(string projectId, bool updateLastConnectionTime);
        public GetSqlServerNameStringBooleanDelegate GetSqlServerNameStringBoolean;

        string IConfirmitDatabaseProvider.GetSqlServerName(string projectId, bool updateLastConnectionTime)
        {


            if (GetSqlServerNameStringBoolean != null)
            {
                return GetSqlServerNameStringBoolean(projectId, updateLastConnectionTime);
            } else if (_inner != null)
            {
                return ((IConfirmitDatabaseProvider)_inner).GetSqlServerName(projectId, updateLastConnectionTime);
            }

            return default(string);
        }

        public delegate string GetSchemaNameStringDelegate(string projectId);
        public GetSchemaNameStringDelegate GetSchemaNameString;

        string IConfirmitDatabaseProvider.GetSchemaName(string projectId)
        {


            if (GetSchemaNameString != null)
            {
                return GetSchemaNameString(projectId);
            } else if (_inner != null)
            {
                return ((IConfirmitDatabaseProvider)_inner).GetSchemaName(projectId);
            }

            return default(string);
        }

    }
}