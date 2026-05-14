using System.Collections.Generic;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;

namespace Confirmit.CATI.Supervisor.Classes
{
    public interface ICustomizableColumnsService
    {
        List<BoundField> GetGridFields();

        List<GridColumnSetting> GetColumnSettings();

        void SaveColumnSettings(List<GridColumnSetting> settings);

        object GetGridData(params object[] searchParams);
    }
}