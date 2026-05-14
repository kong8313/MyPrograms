using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.SearchableFields;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Microsoft.Practices.ObjectBuilder2;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyView : SurveyFormBase
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerSurveyParametersManager _dialerSurveyParametersManager;

        public SurveyView()
        {
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _dialerSurveyParametersManager = ServiceLocator.Resolve<IDialerSurveyParametersManager>();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            DisableControlsOnPostback = false;
        }

        private BvSurveyEntity _survey;
        public BvSurveyEntity Survey
        {
            get
            {
                if (_survey == null)
                {
                    _survey = SurveyRepository.GetById(Int32.Parse(Request["ID"]));
                }

                return _survey;
            }
        }

        public string SurveyPropertiesTab
        {
            get { return Request["SurveyPropertiesTab"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CallManager.AttachSurveyDb(Survey.Name);
            }

            tabs.FindTabFromKey("Quotas").Hidden = !QuotaManager.GetQuotaNames(Survey.SID).Any();
            tabs.FindTabFromKey("DialerSettings").Hidden =
                !(_dialersRepository.IsAnyDialerConfigured() &&
                _dialerSurveyParametersManager.DoesDialerHaveSurveyParameters);

            if (IsPostBack == false)
            {
                // We need do it after visibility of some tabs changed
                string tabKey = SurveyPropertiesTab;
                if (tabKey == "Quotas")
                {
                    tabs.TabItems.First(x => x.Key == "Quotas").ContentUrl += "?startAllQuotas=true";
                }

                if (string.IsNullOrEmpty(tabKey))
                {
                    tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.SurveyProperties);
                }
                
                if (!string.IsNullOrEmpty(tabKey))
                {
                    tabs.SelectTabByKey(tabKey);
                }

                CleanChanged();
            }

            dialog.Title = string.Format(Strings.SurveyInfo, Survey.Description, Survey.Name);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.SurveyProperties, tabKey);
        }
    }
}
