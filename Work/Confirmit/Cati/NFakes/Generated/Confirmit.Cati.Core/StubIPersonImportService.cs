using System;
using System.Data;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.PersonImport;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.PersonImport.Fakes
{
    public class StubIPersonImportService : IPersonImportService 
    {
        private IPersonImportService _inner;

        public StubIPersonImportService()
        {
            _inner = null;
        }

        public IPersonImportService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ImportResult ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);
        public ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions;

        ImportResult IPersonImportService.ImportPersons(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {


            if (ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions != null)
            {
                return ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions(callCenterID, dataTable, columnRoleMap, importOptions);
            } else if (_inner != null)
            {
                return ((IPersonImportService)_inner).ImportPersons(callCenterID, dataTable, columnRoleMap, importOptions);
            }

            return default(ImportResult);
        }

        public delegate bool ParsePersonModeStringAgentTaskChoiceModeOutDelegate(string value, out AgentTaskChoiceMode mode);
        public ParsePersonModeStringAgentTaskChoiceModeOutDelegate ParsePersonModeStringAgentTaskChoiceModeOut;

        bool IPersonImportService.ParsePersonMode(string value, out AgentTaskChoiceMode mode)
        {
            mode = default(AgentTaskChoiceMode);


            if (ParsePersonModeStringAgentTaskChoiceModeOut != null)
            {
                return ParsePersonModeStringAgentTaskChoiceModeOut(value, out mode);
            } else if (_inner != null)
            {
                return ((IPersonImportService)_inner).ParsePersonMode(value, out mode);
            }

            return default(bool);
        }

    }
}