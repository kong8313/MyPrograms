using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public interface ISrvInfo
    {
        BvSurveyEntity Survey
        {
            get;
        }
    }
}