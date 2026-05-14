using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    public partial class SrvInfoSchedulingParams : SrvInfoChild
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GetPage +=
                delegate(out int totalCount)
                {
                    var list = SurveyService.GetSchedulingParametersList(Survey.SID);
                    totalCount = list.Count;
                    return list;
                };
            
            m_grid.InitializeRow += m_grid_InitializeRow;

            m_grid.GridName = string.Format(Strings.SchedulingParametersForSurvey, Survey.Description, Survey.Name);
            m_grid.HintText = Strings.SurveyViewSchedulingParametersHint;
            m_grid.NoDataMessage = Strings.SurveyViewSchedulingParametersNoDataMessage;
        }
        
        public void ResetParams(object sender, EventArgs eventArgs)
        {
            try
            {
                var evt = new ResetSurveySchedulingParametersEvent(Survey.SID, Survey.Name);

                SurveyService.ResetSchedulingParametersValues(Survey.SID);

                evt.Finish();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
        
        void m_grid_InitializeRow(object sender, RowEventArgs e)
        {
            var type = ((BvScheduleParamEntity) e.Row.DataItem).Type;
            e.Row.Items.FindItemByKey("TypeName").Value = StringHelper.GetStringForEnum((SchedulingParameterType)type);
        }

        public void BindData()
        {
            m_grid.BindData();
        }
    }
}