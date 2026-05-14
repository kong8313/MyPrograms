using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class IvrSettings : BaseForm
    {
        private readonly IIvrSettingsRepository _ivrSettingsRepository;

        public override string TopTitle => Strings.IvrSettings;

        public IvrSettings()
        {
            _ivrSettingsRepository = ServiceLocator.Resolve<IIvrSettingsRepository>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GetPage += GetPage;
            RegisterStartupScript("openIvrStaticSettingsFrame()");
            m_grid.GridName = TopTitle;
        }
 
        private object GetPage(out int totalCount)
        {
            var ivrSettings = _ivrSettingsRepository.GetAll();

            var models = ivrSettings.Select(entity => new GridIvrSettingsModel
            {
                LanguageId = entity.LanguageId,
                LanguageDescription = entity.LanguageDescription,
                WrongInputAudioUrl = entity.WrongInputAudioUrl,
                WrongInputText = entity.WrongInputText,
                WrongInputExitAudioUrl = entity.WrongInputExitAudioUrl,
                WrongInputExitText = entity.WrongInputExitText
            });

            return BaseMethods.GetPage(models, m_grid.PageArguments, out totalCount);
        }

        protected void DeleteIvrSettings(object sender, EventArgs e)
        {
            if (m_grid.SelectedKeys.Length == 0) return;

            try
            {
                _ivrSettingsRepository.Delete(m_grid.SelectedKeysInt);

                m_grid.ClearSelectedKeys();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected class GridIvrSettingsModel
        {
            public int LanguageId { get; set; }
            public string LanguageDescription { get; set; }
            public string WrongInputAudioUrl { get; set; }
            public string WrongInputText { get; set; }
            public string WrongInputExitAudioUrl { get; set; }
            public string WrongInputExitText { get; set; }
        }
    }
}