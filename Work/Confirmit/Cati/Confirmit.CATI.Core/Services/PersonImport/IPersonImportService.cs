using System.Collections.Generic;
using System.Data;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.PersonImport
{
    public interface IPersonImportService
    {
        ImportResult ImportPersons(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);
        bool ParsePersonMode(string value, out AgentTaskChoiceMode mode);
    }
}