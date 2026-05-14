using System.Text.RegularExpressions;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.Resources;
using System;

namespace Confirmit.CATI.Core.Services.PersonImport
{
    public class InvalidSymbolsRepairer: IInvalidSymbolsRepairer
    {
        private const string ReplacingSymbol = " ";
        private readonly IInputParameterValidator _validator;        

        public InvalidSymbolsRepairer(IInputParameterValidator validator)
        {
            _validator = validator;
        }        

        public AssignmentData Repair(AssignmentData rowData, ImportResult importResult)
        {
            ValidateAndRepair(rowData.GroupName, (v) => rowData.GroupName = v, importResult, Strings.GroupNameContainedInvalidSymbols);
            ValidateAndRepair(rowData.PersonName, (v) => rowData.PersonName = v, importResult, Strings.InterviewerNameContainedInvalidSymbols);
            ValidateAndRepair(rowData.PersonDescription, (v) => rowData.PersonDescription = v, importResult, Strings.InterviewerDescriptionContainedInvalidSymbols);
            ValidateAndRepair(rowData.PersonLocation, (v) => rowData.PersonLocation = v, importResult, Strings.InterviewerLocationContainedInvalidSymbols);
            
            return rowData;
        }

        private void ValidateAndRepair(string input, Action<string> update, ImportResult importResult, string warning)
        {
            if (String.IsNullOrEmpty(input) == false &&
                _validator.IsValid(input) == false)
            {
                update(Regex.Replace(input, _validator.InvalidSymbols, ReplacingSymbol));

                importResult.Warnings.Add(warning);
            }
        }        
    }
}
