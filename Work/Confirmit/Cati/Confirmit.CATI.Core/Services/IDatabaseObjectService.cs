using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services
{
    public interface IDatabaseObjectService
    {
        void CopyColumnsAndConstraints(string sourceTableName, string destinationTableName);

        void CopyTriggers(string sourceTableName, string destinationTableName);

        List<string> GetCreateIndexQueries(string sourceTableName, string destinationTableName);
    }
}
