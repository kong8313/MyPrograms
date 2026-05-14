using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Core.SearchableFields;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    /// <summary>
    /// Represents list of available fields in console
    /// </summary>
    public partial class SrvInfoAvailableFieldsInConsole : SrvInfoChild
    {
        /// <summary>
        /// Gets/sets list of all available fields
        /// </summary>
        private List<SearchableFieldOrderedForSelection> AvailableFields
        {
            get
            {
                if (ViewState["AvailableFields"] == null)
                {
                    int surveyId = Survey.SID;

                    var searchableFieldsProvider = new SearchableFieldsProvider();
                    ViewState["AvailableFields"] = searchableFieldsProvider.GetOrderedSearchableFields(surveyId);
                }

                return (List<SearchableFieldOrderedForSelection>)ViewState["AvailableFields"];
            }
            set => ViewState["AvailableFields"] = value;
        }
        
        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            m_grid.InitializeRow += Grid_InitializeRow;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.Refresh += delegate
            {
                RefreshAvailableFields();
            };
            
            m_grid.GetPage +=
                 delegate(out int totalCount)
                 {
                     m_grid.SelectedKeys = AvailableFields.Where(x => x.IsEnabled).Select(x => x.FieldName).ToArray();
                     
                     totalCount = AvailableFields.Count;

                     return AvailableFields;
                 };

            m_grid.HintText = Strings.AvailableFieldsInConsoleHint;
            m_grid.GridName = string.Format(Strings.ManualSelectionModeSearchFieldsForSurvey, Survey.Description, Survey.Name); 

            stateChecker.AddSaveButton(btnSave);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedKeys = m_grid.CheckedKeys;

                foreach (var availableField in AvailableFields)
                {
                    availableField.IsEnabled = selectedKeys.Any(x => x == availableField.FieldName);
                }
                
                var evt = new SaveConsoleSearchableFieldsEvent(
                    Survey.SID, 
                    Survey.Name, 
                    AvailableFields.Select(x => $"{x.FieldName}-{x.IsEnabled}").ToArray());

                var orderedSearchableFieldsProvider = ServiceLocator.Resolve<IOrderedSearchableFieldsRepository>();
                orderedSearchableFieldsProvider.Update(GetDatabaseEntityFromAvailableFields());

                evt.Finish();

                stateChecker.MarkAsUnchanged();

                RefreshAvailableFields();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private List<BvSearchableFieldsOrderedEntity> GetDatabaseEntityFromAvailableFields()
        {
            return AvailableFields.Select(x => new BvSearchableFieldsOrderedEntity
            {
                FieldName = x.FieldName,
                SurveyId = x.SurveyId,
                IsEnabled = x.IsEnabled,
                IsSystem = x.IsSystem,
                OrderNumber = x.OrderNumber
            }).ToList();
        }

        private void RefreshAvailableFields()
        {
            AvailableFields = null;
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var fieldName = ((BvSearchableFieldsOrderedEntity)e.Row.DataItem).FieldName;
            var field = AvailableFields.Single(x => x.FieldName == fieldName);

            if (field.FieldType == "System")
            {
                var displayNameCell = e.Row.Items.FindItemByKey("DisplayName");
                displayNameCell.Text = GetResString("SearchableField_" + field.FieldName);

                var fieldTypeCell = e.Row.Items.FindItemByKey("FieldType");
                fieldTypeCell.CssClass += " boldLabel";
            }
        }
    }
}
