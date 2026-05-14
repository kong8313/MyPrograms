using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
    public partial class SurveyActivityColumnSettings : BaseForm
    {
        private ICustomizableColumnsService _customizableColumnsService;
        private string _columnSettingCustomControlId = "customControlId";

        public string SettingsFor
        {
            get { return Request["SettingsFor"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _customizableColumnsService = ServiceLocator.ResolveByName<ICustomizableColumnsService>(SettingsFor);

            grid.RowDataBound += grid_RowDataBound;

            var rows = _customizableColumnsService.GetColumnSettings();
            AddRowsControls(rows);
        }

        private void AddRowsControls(List<GridColumnSetting> rows)
        {
            int i = 0;
            foreach (GridViewRow gridViewRow in grid.Rows)
            {
                if (gridViewRow.RowType == DataControlRowType.DataRow)
                {
                    rows[i].SettingControl.ID = _columnSettingCustomControlId;
                    gridViewRow.Cells[1].Controls.Add(rows[i].SettingControl);
                }

                i++;
            }
        }

        private void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var columnRow = (GridColumnSetting)e.Row.DataItem;

                var checkBox = (ServerControls.CheckBox)e.Row.Cells[0].FindControl("cbIsActive");
                checkBox.Checked = columnRow.Active;
                columnRow.SettingControl.ID = _columnSettingCustomControlId;
                e.Row.Cells[1].Controls.Add(columnRow.SettingControl);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
            {
                grid.DataSource = _customizableColumnsService.GetColumnSettings();
                grid.DataBind();
            }
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            var columnSettings = PrepareSettingsToSave();
            _customizableColumnsService.SaveColumnSettings(columnSettings);

            CloseOverlay(true);
        }

        private List<GridColumnSetting> PrepareSettingsToSave()
        {
            var result = new List<GridColumnSetting>();
            int i = 0;
            foreach (Control gridRow in grid.Rows)
            {
                result.Add(new GridColumnSetting
                {
                    Key = (string)grid.DataKeys[i].Value, 
                    Active = ((ServerControls.CheckBox) gridRow.FindControl("cbIsActive")).Checked,
                    SettingControl = gridRow.FindControl(_columnSettingCustomControlId)
                });

                i++;
            }

            return result;
        }
    }
}