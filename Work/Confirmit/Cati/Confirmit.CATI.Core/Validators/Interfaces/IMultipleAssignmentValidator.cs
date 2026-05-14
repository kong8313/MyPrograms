using System.Collections.Generic;

namespace Confirmit.CATI.Core.Validators.Interfaces
{
    public interface IMultipleAssignmentValidator
    {
        MultipleAssignmentValidationResult ValidateMultipleAssignment(string[] keys);

        MultipleAssignmentValidationResult ValidateMultipleAssignmentByCounts(int groupsInAssignmentCount, int personsInAssignmentCount);

        bool IsMultipleAssignmentGroup(int sid);
    }
}
