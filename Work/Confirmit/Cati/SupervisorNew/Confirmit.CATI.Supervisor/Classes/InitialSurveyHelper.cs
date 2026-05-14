using System;
using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Helps to manage with initial survey.
    /// Stores flag indicated that survey has been already shown.
    /// </summary>
    /// <remarks>
    /// It is needed because information (in the bottom frame) about initial survey 
    /// should be shown only first time. 
    /// But if window with CATI Surpervisor has been closed this information should  be shown again.
    /// </remarks>
    public static class InitialSurveyHelper
    {
        private const string m_key = "HasInitialSurveyAlreadyBeenShown";

        /// <summary>
        /// Gets/sets flag indicated that survey has been already shown.
        /// </summary>
        public static bool HasSurveyBeenShown
        {
            get
            {
                return (bool)(HttpContext.Current.Session[m_key] ?? false);
            }
            set
            {
                HttpContext.Current.Session[m_key] = value;
            }
        }
    }
}
