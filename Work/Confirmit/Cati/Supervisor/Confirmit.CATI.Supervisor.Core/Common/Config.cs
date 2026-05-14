using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    /// <summary>
    /// Class that encapsulates access to web.config settings of application
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Settings from these groups can't be changed from system settings section.
        /// </summary>
        public const string NotOverridableSystemSettingsGroups = "Setup,System";

        private Config()
        {
        }

        public static bool DebugMode
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]);
            }
        }        

        /// <summary>
        /// If true, recording retrieval button in call management is hidden.
        /// </summary>
        public static bool HideRecordingRetrievalButton
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["HideRecordingRetrievalButton"]);
            }
        }

        /// <summary>
        /// if true, restricts IE popup menu.
        /// </summary>
        public static bool DisablePopupMenu
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["DisableIEPopupMenu"]);
            }
        }

        /// <summary>
        /// Defines a port used for SSL connections. On load balancer environment it may be different from default 443.
        /// </summary>
        public static int SSLPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SSLPort"]);
            }
        }

        /// <summary>
        /// Gets confirmit keepsession.aspx page url.
        /// </summary>
        public static string ConfirmitKeepSessionAspxUrl
        {
            get { return ConfigurationManager.AppSettings["ConfirmitKeepSessionAspxUrl"]; }
        }
    }
}