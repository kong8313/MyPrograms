using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Supervisor.Reports.Classes
{    
    public abstract class SingleSurveyReportBase : SurveyReportBase
    {                  
        protected override void InitSelectedSurveys(bool isInitial)
        {
            SelectedSurveys = new List<int>();

            if (isInitial && SurveyId.HasValue)
            {
                SelectedSurveys = new[] { (SurveyId.Value) }.ToList();                
            }
            else
            {
                SelectedSurveys = (GetSurveysSelectedByUser() ?? new List<int>()).ToList();    
            }
        }        
    }
}