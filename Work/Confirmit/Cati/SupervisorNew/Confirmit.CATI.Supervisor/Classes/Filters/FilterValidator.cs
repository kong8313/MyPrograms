using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    internal class FilterValidator : IFilterValidator
    {
        public void Validate(BvFiltersEntity filter)
        {
            if (string.IsNullOrEmpty(filter.Name))
            {
                throw new UserMessageException(Strings.Err_NameMustNotBeEmpty);
            }
        }
    }
}