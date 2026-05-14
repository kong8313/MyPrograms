using System;
using System.Linq;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public class CellItsCounters
    {
        public int ItsId { get; set; }
        public string StatusName { get; set; }
        public int Count { get; set; }
    }

    public partial class StatusBreakdown: BaseForm
    {       
        [StoreInViewState] 
        public int CellId;

        [StoreInViewState]
        public ExtraQuotaCounterTypes ExtraCounter;        

        private  IExtraQuotaCounterParameters ExtraQuotaCounterParameters
        {
            get { return SessionVariables.ExtraQuotaCounterParameters; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CellId = Convert.ToInt32(Request["CellId"]);
                ExtraCounter = (ExtraQuotaCounterTypes)Int32.Parse(Request["ExtraCounter"]);
            }

            switch (ExtraCounter)
            {                
                case ExtraQuotaCounterTypes.Scheduled:                                        
                    grid.GridName = Strings.ScheduledCalls; break;
                case ExtraQuotaCounterTypes.ScheduledWithSpecificStatuses:
                    grid.GridName = Strings.ScheduledCallsWithSpecificStatuses; break;
                case ExtraQuotaCounterTypes.InterviewsWithSpecificStatuses:
                    grid.GridName = Strings.InterviewsWithSpecificStatuses; break;                
            }

            var itses = SurveyService.GetTransientStates(ExtraQuotaCounterParameters.SurveyId);                    
            var calculator = ExtraQuotaCounterService.Create(ExtraQuotaCounterParameters);            
                        
            grid.GetPage =
                delegate(out int totalCount)
                {
                    var result = calculator.GetItsCountersForCell(CellId).ToArray();

                    var list =
                        (from record in result
                        let its = itses.SingleOrDefault(x => x.StateID == record.Key)
                        where its != null
                        select new CellItsCounters { ItsId = record.Key, StatusName = its.Name, Count = record.Value }).ToList();

                    grid.ExtraStatusBarText = String.Format(Strings.StatusBreakdown_TotalCallCount, list.Sum(x => x.Count));

                    if (String.IsNullOrEmpty(grid.SortedColumnName) == false)
                    {
                        list.Sort(new CommonComparer<CellItsCounters>(grid.SortedColumnName, grid.SortIndicatorAsc));                        
                    }                                        

                    totalCount = list.Count();                    

                    return list;
                };
        }
    }
}