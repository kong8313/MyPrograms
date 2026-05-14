using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns
{
    public class GridColumnSetting
    {
        public bool Active { get; set; }

        public string Key { get; set; }

        public Control SettingControl { get; set; }
    }
}