using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public interface IFilterValidator
    {
        void Validate(BvFiltersEntity filter);
    }
}