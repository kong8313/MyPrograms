using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        [NotNull]
        BvPersonEntity GetById(int sid);

        [CanBeNull]
        BvPersonEntity TryGetById(int sid);

        [NotNull]
        BvPersonEntity GetByName(string name);

        [CanBeNull]
        BvPersonEntity TryGetByName(string name);

        List<BvPersonEntity> GetAll();

        List<BvPersonEntity> GetByType(AgentType type);

        int Insert([NotNull] BvPersonEntity person);
        void Update([NotNull] BvPersonEntity person, bool updateCache = true);
        void Delete(int sid, bool updateCache = true);


    }
}
