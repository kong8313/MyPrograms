using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Supervisor.Classes
{
    public static class SessionVariables
    {        
        private static readonly SessionVariable<IExtraQuotaCounterParameters> _extraQuotaCounterParameters = 
                                new SessionVariable<IExtraQuotaCounterParameters>("ExtraQuotaCounterParameters");

        private static readonly SessionVariable<int[]> _taskListSelectedSurveysIds =
                                new SessionVariable<int[]>("TaskListSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _taskListSelectedInterviewersIds =
                                new SessionVariable<int[]>("TaskListSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _surveysActivityViewSelectedSurveysIds =
                                new SessionVariable<int[]>("SurveysActivityViewSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _appointmentsListSelectedSurveysIds =
                                new SessionVariable<int[]>("AppointmentsListSelectedSurveysIds");  
        
        private static readonly SessionVariable<int[]> _performanceListSelectedSurveysIds =
                                new SessionVariable<int[]>("PerformanceListSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _performanceListSelectedInterviewersIds =
                                new SessionVariable<int[]>("PerformanceListSelectedInterviewersIds");
        
                        
        public static IExtraQuotaCounterParameters ExtraQuotaCounterParameters
        {
            get
            {
                return _extraQuotaCounterParameters.Value;
            }
            set
            {
                _extraQuotaCounterParameters.Value = value; 
            }
        }

        public static int[] TaskListSelectedSurveysIds
        {
            get
            {
                return _taskListSelectedSurveysIds.Value;
            }
            set
            {
                _taskListSelectedSurveysIds.Value = value != null ? value.ToArray() : null;
            }
        }

        public static int[] TaskListSelectedInterviewersIds
        {
            get
            {
                return _taskListSelectedInterviewersIds.Value;
            }
            set
            {
                _taskListSelectedInterviewersIds.Value = value != null ? value.ToArray() : null;
            }
        }

        public static int[] SurveysActivityViewSelectedSurveysIds
        {
            get
            {
                return _surveysActivityViewSelectedSurveysIds.Value;
            }
            set
            {
                _surveysActivityViewSelectedSurveysIds.Value = value != null ? value.ToArray() : null;
            }
        }

        public static int[] AppointmentsListSelectedSurveysIds
        {
            get
            {
                return _appointmentsListSelectedSurveysIds.Value;
            }
            set
            {
                _appointmentsListSelectedSurveysIds.Value = value != null ? value.ToArray() : null;
            }
        }     
        
        public static int[] PerformanceListSelectedSurveysIds
        {
            get
            {
                return _performanceListSelectedSurveysIds.Value;
            }
            set
            {
                _performanceListSelectedSurveysIds.Value = value != null ? value.ToArray() : null;
            }
        }

        public static int[] PerformanceListSelectedInterviewersIds
        {
            get
            {
                return _performanceListSelectedInterviewersIds.Value;
            }
            set
            {
                _performanceListSelectedInterviewersIds.Value = value != null ? value.ToArray() : null;
            }
        }  
    }
}
