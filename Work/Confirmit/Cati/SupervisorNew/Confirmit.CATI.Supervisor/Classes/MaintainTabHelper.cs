using System;
using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    public enum ViewWithTabs
    {
        SurveyProperties,

        PersonProperties,

        PersonGroupProperties,

        CallGroupView,

        AsyncOperationView,

        SiteSettings,

        CallListHistoryTabs
    }

    /// <summary>
    /// Helps maintain state of selected tab
    /// Used on pages with several tabs
    /// </summary>
    public static class MaintainTabHelper
    {
        private const string m_key = "MaintainTab";        

        /// <summary>
        /// Gets stored telephony number from session
        /// </summary>
        public static string GetTabKey(ViewWithTabs view)
        {            
            return (string)(HttpContext.Current.Session[ GetKey(view)]?? String.Empty);            
        }

        /// <summary>
        /// Sets telephony number in session
        /// </summary>        
        public static void SetTabKey(ViewWithTabs view, string selectedTabKey)
        {                        
            HttpContext.Current.Session[GetKey(view)] = selectedTabKey;
        }

        private static string GetKey(ViewWithTabs view)
        {
            return m_key + view.ToString();
        }
     
    }
}
