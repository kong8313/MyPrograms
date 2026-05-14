using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IIvrSettingsRepository
    {
        List<BvIvrSettingsEntity> GetAll();

        BvIvrSettingsEntity TryGetByLanguageId(int languageId);

        void Insert(BvIvrSettingsEntity entity);

        void Update(int languageId, BvIvrSettingsEntity entity);

        void Delete(List<int> languageIds);
    }
}