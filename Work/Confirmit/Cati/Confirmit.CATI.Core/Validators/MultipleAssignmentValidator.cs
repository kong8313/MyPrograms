using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Validators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Validators
{
    public class MultipleAssignmentValidator : IMultipleAssignmentValidator
    {
        public MultipleAssignmentValidationResult ValidateMultipleAssignment(string[] keys)
        {
            List<KeyValuePair<int, bool>> idsAndIsGroups = keys.Select(
                k =>
                {
                    var items = k.Split('_');
                    return new KeyValuePair<int, bool>(
                        int.Parse(items[0]),
                        bool.Parse(items[1])
                        );
                }
                ).ToList();

            int groupsCount = idsAndIsGroups.Count(pair => pair.Value);
            int personsCount = idsAndIsGroups.Count(pair => !pair.Value);

            return ValidateMultipleAssignmentByCounts(groupsCount, personsCount);
        }

        public MultipleAssignmentValidationResult ValidateMultipleAssignmentByCounts(int groupsInAssignmentCount, int personsInAssignmentCount)
        {
            if (groupsInAssignmentCount > 0 && personsInAssignmentCount > 0)
            {
                return MultipleAssignmentValidationResult.GroupsAssignmentContainsPersons;
            }

            if (groupsInAssignmentCount == 0 && personsInAssignmentCount > 1)
            {
                return MultipleAssignmentValidationResult.ContainsMultiplePersons;
            }

            return MultipleAssignmentValidationResult.Success;
        }

        public bool IsMultipleAssignmentGroup(int sid)
        {
            return BvSpIsMultipleAssignmentGroupAdapter.ExecuteScalar<int>(sid) > 0;
        }

    }
}
