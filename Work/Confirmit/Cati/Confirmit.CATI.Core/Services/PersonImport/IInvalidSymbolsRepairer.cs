using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.Services.PersonImport
{
    public interface IInvalidSymbolsRepairer
    {
        AssignmentData Repair(AssignmentData rowData, ImportResult importResult);
    }    
}
