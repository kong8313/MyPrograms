using System;
using Confirmit.CATI.Core.Services.PersonImport;

namespace Confirmit.CATI.Core.Services.PersonImport.Fakes
{
    public class StubIInvalidSymbolsRepairer : IInvalidSymbolsRepairer 
    {
        private IInvalidSymbolsRepairer _inner;

        public StubIInvalidSymbolsRepairer()
        {
            _inner = null;
        }

        public IInvalidSymbolsRepairer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AssignmentData RepairAssignmentDataImportResultDelegate(AssignmentData rowData, ImportResult importResult);
        public RepairAssignmentDataImportResultDelegate RepairAssignmentDataImportResult;

        AssignmentData IInvalidSymbolsRepairer.Repair(AssignmentData rowData, ImportResult importResult)
        {


            if (RepairAssignmentDataImportResult != null)
            {
                return RepairAssignmentDataImportResult(rowData, importResult);
            } else if (_inner != null)
            {
                return ((IInvalidSymbolsRepairer)_inner).Repair(rowData, importResult);
            }

            return default(AssignmentData);
        }

    }
}