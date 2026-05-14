using System;
using System.Web;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;

namespace Confirmit.CATI.Supervisor.MasterPages
{
    public partial class Main: System.Web.UI.MasterPage
    {
        public string TableDensityCssClass { get; set; } = "";

        public string NewUiCssClass { get; set; } = HttpContext.Current.Request.QueryString["newui"] == "true" ? "newui" : "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SetTableDensity();
        }

        private void SetDensityCssClass(string classToAdd, string classToRemove)
        {
            TableDensityCssClass = TableDensityCssClass.Replace(classToRemove, "");
            TableDensityCssClass += classToAdd;
        }

        private void SetTableDensity()
        {
            const string densityNormalClassName = " density-normal";
            const string densityCondensedClassName = " density-condensed";

            var currentDensity = SupervisorSettingsManager.GetTableDensity();

            switch (currentDensity)
            {
                case "normal" when !TableDensityCssClass.Contains(densityNormalClassName):
                    SetDensityCssClass(densityNormalClassName, densityCondensedClassName);
                    break;
                case "condensed" when !TableDensityCssClass.Contains(densityCondensedClassName):
                    SetDensityCssClass(densityCondensedClassName, densityNormalClassName);
                    break;
            }
        }


    }
}