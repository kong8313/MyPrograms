using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyScriptParameterProperties : BaseForm
    {
        [StoreInViewState]
        protected int ParameterId;

        [StoreInViewState]
        protected int SurveyId;

        private readonly IScheduleService _scheduleService;

        public SurveyScriptParameterProperties()
        {
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlType.Items.Clear();
                foreach (SchedulingParameterType type in Enum.GetValues(typeof(SchedulingParameterType)))
                {
                    ddlType.Items.Add(new ListItem(StringHelper.GetStringForEnum(type), ((int)type).ToString(CultureInfo.InvariantCulture)));
                }

                SurveyId = Int32.Parse(Request["ID"]);
                ParameterId = Int32.Parse(Request["ParameterId"]);

                BindData();
            }

            dialog.OKButton.Text = "Save";
        }

        private void BindData()
        {
            var parameter = SurveyService.GetSchedulingParametersList(SurveyId).First(x => x.ParamID == ParameterId);

            tbParamName.Text = parameter.Name;
            tbDescription.Text = parameter.Description;
            ddlType.SelectedValue = parameter.Type.ToString();
            neDefaultValue.Value = parameter.Value;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                var survey = SurveyRepository.GetById(SurveyId);
                var evt = new SetSurveySchedulingParametersEvent(SurveyId, survey.Name, ParameterId, neDefaultValue.ValueInt);

                _scheduleService.SetParamValue(SurveyId, ParameterId, neDefaultValue.ValueInt);

                evt.Finish();

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}