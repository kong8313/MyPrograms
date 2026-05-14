
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class SurveysList: Confirmit.CATI.Supervisor.Classes.BaseForm
    {
        public override string TopTitle
        {
            get { return Strings.SurveysList; }
        }

        protected Confirmit.CATI.Supervisor.Surveys.Controls.SurveysList SrvList;
    }
}
