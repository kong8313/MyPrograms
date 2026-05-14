using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IStateGroupRepository
    {
        BvStateGroupEntity GetDefault();
    }
}