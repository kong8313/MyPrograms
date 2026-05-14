using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public interface IFilterFactory
    {
        BvFiltersEntity Create(int id, string name, string description, string operatorString);
    }
}
