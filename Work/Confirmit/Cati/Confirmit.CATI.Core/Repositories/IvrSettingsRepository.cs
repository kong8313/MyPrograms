using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class IvrSettingsRepository : IIvrSettingsRepository
    {
        public List<BvIvrSettingsEntity> GetAll()
        {
            return BvIvrSettingsAdapter.GetAll().OrderBy(entity => entity.LanguageId).ToList();
        }

        public BvIvrSettingsEntity TryGetByLanguageId(int languageId)
        {
            return BvIvrSettingsAdapter.GetAll().FirstOrDefault(entity => entity.LanguageId == languageId);
        }

        public void Insert(BvIvrSettingsEntity entity)
        {
            var evt = new AddIvrSettingEvent(entity);

            BvIvrSettingsAdapter.Insert(entity);

            evt.Finish();
        }

        public void Update(int languageId, BvIvrSettingsEntity entity)
        {
            var evt = new UpdateIvrSettingEvent(languageId, entity);

            BvIvrSettingsAdapter.DeleteByCondition($"[LanguageId] = {languageId}");
            BvIvrSettingsAdapter.Insert(entity);

            evt.Finish();
        }

        public void Delete(List<int> languageIds)
        {
            string languageIdsString = string.Join(", ", languageIds);
            var evt = new DeleteIvrSettingsEvent(languageIdsString);

            BvIvrSettingsAdapter.DeleteByCondition($"[LanguageId] IN ({languageIdsString})");

            evt.Finish();
        }
    }
}