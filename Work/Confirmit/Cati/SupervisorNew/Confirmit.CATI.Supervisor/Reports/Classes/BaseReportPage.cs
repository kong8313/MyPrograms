using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using System.Web.Script.Serialization;
using Panel = System.Web.UI.WebControls.Panel;
using SqlDataSource = Telerik.Reporting.SqlDataSource;

namespace Confirmit.CATI.Supervisor.Reports.Classes
{
    public abstract class BaseReportPage : BaseForm, IPostBackEventHandler
    {
        protected const int CompletedItsId = 13;
        protected const int MaxNamesCount = 50;
        protected const int MaxLineLength = 100;

        private const string ReportDataKeyPrefix = "ReportData_";
        protected const string _SurveysSelected = "_SurveysSelected";
        protected const string _PersonsSelected = "_PersonSelected";
        
        protected readonly ICachedLocalTimezoneManager LocalTimezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        protected readonly ISystemSettings SystemSettings = ServiceLocator.Resolve<ISystemSettings>();

        protected event EventHandler SurveysSelectedByUser;
        protected event EventHandler PersonsSelectedByUser;

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new LightSessionPageStatePersister(this);
            }
        }

        protected abstract Panel ReportPanel { get; }
        protected abstract Button BuildButton { get; }        
        protected abstract Report Report { get; }
        protected abstract ReportViewer ReportViewer { get; }
        protected abstract UpdatePanel UpdatePanel { get; }        
        protected abstract void  BuildReport();

        public string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        public bool IsBuildButtonPressed
        {
            get { return BuildButton != null && IsPostBack && Request["__EVENTTARGET"] == BuildButton.UniqueID; }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer.Visible = false;
            }

            UpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ID });

            base.OnInit(e);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            // Clear cached report data if "Build report" button is pressed.
            // Should be done when ViewState is loaded but before PageLoad where we build report.
            if (IsBuildButtonPressed)
            {
                if (ViewState.Keys != null)
                {
                    var reportDataKeys =
                        ViewState.Keys.OfType<string>().Where(x => x.StartsWith(ReportDataKeyPrefix)).ToArray();

                    foreach (var key in reportDataKeys)
                    {
                        ViewState.Remove(key);
                    }
                }
            }

            base.OnPreLoad(e);
        }   

        protected override void OnPreRender(EventArgs e)
        {
            if (ReportViewer != null)
            {
                PageHelper.RegisterClientLibrary("Reports/Client/CatiReport.js");

                ((BaseForm) Page).RegisterScriptBlock(string.Format("var {0} = new CatiReport({1});", ClientControllerName, GetClientSettings()),
                                                      "Controller" + ClientID, GetType());
            }

            if (IsBuildButtonPressed)
            {
                BuildButton.Attributes.Add("disabled", "disabled");
            }
            else
            {
                BuildButton.Attributes.Remove("disabled");
            }

            base.OnPreRender(e);
        }

        protected void InitReportDataSource(SqlDataSource dataSource)
        {
            dataSource.ConnectionString = BackendInstance.Current.ConnectionString;
            dataSource.CommandTimeout = SystemSettings.Reports.ReportGenerationTimeout;
        }

        private object GetClientSettings()
        {
            var settings = new
            {
                ReportViewerClientId = ReportViewer.ClientID,
                ReportPanelClientId = ReportPanel.ClientID,
                BuildButtonClinetId = BuildButton.ClientID,
                IsBuildButtonPressed
            };

            return new JavaScriptSerializer().Serialize(settings);
        }
        
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == _SurveysSelected)
            {
                if (SurveysSelectedByUser != null)
                    SurveysSelectedByUser(this, EventArgs.Empty);
            }
            else if (eventArgument == _PersonsSelected)
            {
                if (PersonsSelectedByUser != null)
                    PersonsSelectedByUser(this, EventArgs.Empty);
            }
        }
    }
}
