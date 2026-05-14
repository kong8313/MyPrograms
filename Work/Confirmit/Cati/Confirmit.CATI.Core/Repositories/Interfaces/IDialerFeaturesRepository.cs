using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IDialerFeaturesRepository
    {
        List<BvDialerFeaturesEntity> GetAll(int dialerId);
        void UpdateOrInsert([NotNull] BvDialerFeaturesEntity dialerFeaturesEntity);
        void Delete(int dialerId, string name);
        void DeleteAll(int dialerId);
    }
}
