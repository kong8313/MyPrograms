using System;
using Confirmit.CATI.Core.Export;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID", SeparatorCharacter = ",")]
    public partial class ExportQuotaStatusReport : SurveyFormBase
    {
        private const string ExportQuotaStatusReportFileName = "QuotaStatusReport.txt";

        private const string ExportQuotaStatusReportPackageFileName = "QuotaStatusReport.zip";

        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        private int SurveyId
        {
            get
            {
                return (int)(ViewState["ID"] ?? 0);
            }
            set
            {
                ViewState["ID"] = value;
            }
        }

        private string ProjectId
        {
            get
            {
                return (string)(ViewState["ProjectId"] ?? String.Empty);
            }
            set
            {
                ViewState["ProjectId"] = value;
            }
        }
    
    
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!IsPostBack)
            {
                SurveyId = int.Parse(Request.Params["ID"]);
                var survey = SurveyRepository.GetById(SurveyId);
                ProjectId = survey.ProjectId;

                CallManager.AttachSurveyDb(survey.ProjectId);

                divExportQuotaStatusReportHelp.InnerHtml = Strings.ExportQuotaStatusReportHelp;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            hintExportQuotaStatusReportHint.Text = Strings.ExportQuotaStatusReportHintText;
        }

        protected void btnExportClick(object sender, EventArgs e)
        {
            try
            {
                var dataProvider = new SurveyQuotasExportInfoProvider(SurveyId, ProjectId);

                string report = new SurveyQuotasReportGenerator(dataProvider,
                                                                () => _timezoneProvider.GetCurrentLocalTime().ToString("dd MMMM yyyy, hh:mm"),
                                                                SurveyService.GetFormattedSurveyName).Generate();

                string packageFilePath = String.Empty;

                try
                {
                    packageFilePath = new Packaging().CreatePackage(ExportQuotaStatusReportFileName, report);
                }
                catch (Exception ex)
                {
                    ExceptionTraceHelper.TraceException(ex);
                    throw new Exception("Error on creating export file, contact the administrator");
                }

                FileToClientSender.SendFileContent(packageFilePath, ExportQuotaStatusReportPackageFileName);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
      
    }
}
