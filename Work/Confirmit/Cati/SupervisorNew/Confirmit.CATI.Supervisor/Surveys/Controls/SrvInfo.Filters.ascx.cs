using System;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    /// <summary>
    ///		Summary description for SrvInfo_Filters.
    /// </summary>
    public partial class SrvInfo_Filters: SrvInfoChild
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            filtersList.SurveyID = Survey.SID;
            filtersList.Survey = Survey;
        }
    }
}