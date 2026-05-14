using System;
using Confirmit.CATI.Core.Services;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIDatabaseObjectService : IDatabaseObjectService 
    {
        private IDatabaseObjectService _inner;

        public StubIDatabaseObjectService()
        {
            _inner = null;
        }

        public IDatabaseObjectService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CopyColumnsAndConstraintsStringStringDelegate(string sourceTableName, string destinationTableName);
        public CopyColumnsAndConstraintsStringStringDelegate CopyColumnsAndConstraintsStringString;

        void IDatabaseObjectService.CopyColumnsAndConstraints(string sourceTableName, string destinationTableName)
        {

            if (CopyColumnsAndConstraintsStringString != null)
            {
                CopyColumnsAndConstraintsStringString(sourceTableName, destinationTableName);
            } else if (_inner != null)
            {
                ((IDatabaseObjectService)_inner).CopyColumnsAndConstraints(sourceTableName, destinationTableName);
            }
        }

        public delegate void CopyTriggersStringStringDelegate(string sourceTableName, string destinationTableName);
        public CopyTriggersStringStringDelegate CopyTriggersStringString;

        void IDatabaseObjectService.CopyTriggers(string sourceTableName, string destinationTableName)
        {

            if (CopyTriggersStringString != null)
            {
                CopyTriggersStringString(sourceTableName, destinationTableName);
            } else if (_inner != null)
            {
                ((IDatabaseObjectService)_inner).CopyTriggers(sourceTableName, destinationTableName);
            }
        }

        public delegate List<string> GetCreateIndexQueriesStringStringDelegate(string sourceTableName, string destinationTableName);
        public GetCreateIndexQueriesStringStringDelegate GetCreateIndexQueriesStringString;

        List<string> IDatabaseObjectService.GetCreateIndexQueries(string sourceTableName, string destinationTableName)
        {


            if (GetCreateIndexQueriesStringString != null)
            {
                return GetCreateIndexQueriesStringString(sourceTableName, destinationTableName);
            } else if (_inner != null)
            {
                return ((IDatabaseObjectService)_inner).GetCreateIndexQueries(sourceTableName, destinationTableName);
            }

            return default(List<string>);
        }

    }
}