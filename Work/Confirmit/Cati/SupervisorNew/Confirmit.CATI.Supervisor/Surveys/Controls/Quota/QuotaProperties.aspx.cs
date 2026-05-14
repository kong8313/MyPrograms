using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys.Controls.Quota
{
    public partial class QuotaProperties : BaseForm
    {
        private readonly ISupervisorSettingsRepository _supervisorSettingsRepository;
        private readonly IQuotaSettingsProvider _quotaSettingsProvider;

        public QuotaProperties()
        {
            _supervisorSettingsRepository = ServiceLocator.Resolve<ISupervisorSettingsRepository>();
            _quotaSettingsProvider = ServiceLocator.Resolve<IQuotaSettingsProvider>();
        }

        private int SurveySid
        {
            get
            {
                return (int)(ViewState["SurveyID"] ?? 0);
            }
            set
            {
                ViewState["SurveyID"] = value;
            }
        }

        [StoreInViewState]
        protected List<QuotaDetails> QuotaList;

        [StoreInViewState]
        protected ObservableCollection<string> SortedQuotas;

        [StoreInViewState]
        protected List<string> ExcludedQuotas;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveySid = Convert.ToInt32(Request.Params["SurveyID"]);
                var settings = _supervisorSettingsRepository.ReadQuotaSettings(SurveySid);
                tbxColumns.Text = settings.NumberOfColumns != 0 ? settings.NumberOfColumns.ToString() : "3";

                FillData();
            }

            quotasGrid.GetPage = GetPage;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (quotasGrid.CheckedKeys.Length == 0)
                {
                    AddUserMessage(Strings.QuotaNoneSelectedWarning);
                    return;
                }

                if (Convert.ToInt32(tbxColumns.Text) < 0)
                {
                    AddUserMessage(Strings.NegativeNumberForQuotasProperty);
                    return;
                }

                var excluded = SortedQuotas.Except(quotasGrid.CheckedKeys);

                var settings = new QuotaPageViewSettings
                {
                    QuotasExclusion = excluded.ToList(),
                    QuotasOrder = SortedQuotas.ToList(),
                    NumberOfColumns = int.Parse(tbxColumns.Text)
                };

                _supervisorSettingsRepository.WriteQuotaSettings(SurveySid, settings);

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected object GetPage(out int totalCount)
        {
            var list = QuotaList ?? FillData();

            if (!IsPostBack)
            {
                quotasGrid.SelectedKeys = SortedQuotas.Except(ExcludedQuotas).ToArray();
            }

            var orderedList = (from quotaName in SortedQuotas
                               join quota in list
                                   on quotaName equals quota.Name
                               select quota).Select((value, index) => new
                               {
                                   Priority = index,
                                   value.Name
                               });

            return BaseMethods.GetPage(orderedList, quotasGrid.PageArguments, out totalCount);
        }

        private List<QuotaDetails> FillData()
        {
            var settings = _quotaSettingsProvider.UpdateAndGetSettings(SurveySid);

            ExcludedQuotas = settings.QuotasExclusion;
            SortedQuotas = new ObservableCollection<string>(settings.QuotasOrder);
            QuotaList = QuotaManager.GetQuotaNamesAndIds(SurveySid).ToList();

            return QuotaList;
        }

        protected void MoveUp(object sender, EventArgs e)
        {
            var position = SortedQuotas.IndexOf(quotasGrid.HighlightedKey);
            var newPosition = position - 1 >= 0 ? position - 1 : 0;
            SortedQuotas.Move(position, newPosition);
        }

        protected void MoveDown(object sender, EventArgs e)
        {
            var position = SortedQuotas.IndexOf(quotasGrid.HighlightedKey);
            var newPosition = position + 1 <= SortedQuotas.Count - 1 ? position + 1 : SortedQuotas.Count - 1;
            SortedQuotas.Move(position, newPosition);
        }
    }
}
