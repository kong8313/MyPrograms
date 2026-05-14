using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.SearchableFields;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SelectConfirmitVariables : BaseForm
    {
        // maximum rows in grid that can be selected.
        protected int MaxSelectedRows = ServiceLocator.Resolve<ICallManagementSettings>().MaximumConfirmitVariables;

        /// <summary>
        /// Fusion survey ID.
        /// </summary>
        public int SurveyId
        {
            get
            {
                if (ViewState["SurveyID"] == null && Request.Params["ID"] != null)
                    ViewState["SurveyID"] = Convert.ToInt32(Request.Params["ID"]);
                return ViewState["SurveyID"] == null ? 0 : (int)ViewState["SurveyID"];
            }
            set
            {
                if (SurveyId != value)
                {
                    ViewState["SurveyID"] = value;
                }
            }
        }

        List<SearchableFieldForSelection> CallManagementSearchableColumns
        {
            get
            {
                if (ViewState["CallManagementSearchableColumns"] == null)
                {
                    ViewState["CallManagementSearchableColumns"] =
                        new SearchableFieldsProvider().GetCallManagementSearchableFields(SurveyId);
                }

                return (List<SearchableFieldForSelection>)ViewState["CallManagementSearchableColumns"];
            }
            set
            {
                ViewState["CallManagementSearchableColumns"] = value;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            dialog.OKButton.Click += OkButton_Clicked;

            m_grid.Refresh += delegate
            {
                RefreshAvailableFields();
            };

            m_grid.GetPage +=
                 delegate(out int totalCount)
                 {
                     m_grid.SelectedKeys = CallManagementSearchableColumns.Where(x => x.IsSelected).Select(x => x.Key).ToArray();

                     CallManagementSearchableColumns.Sort(new CommonComparer<SearchableFieldForSelection>(m_grid.SortedColumnName, m_grid.SortIndicatorAsc));
                     totalCount = CallManagementSearchableColumns.Count;

                     return CallManagementSearchableColumns;
                 };
        }

        private void OkButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string[] selectedKeys = m_grid.CheckedKeys;

                var evt = new SaveCallMangementSearchableFieldsEvent(
                    SurveyId,
                    SurveyRepository.GetById(SurveyId).Name,
                    CallManagementSearchableColumns.Where(x => selectedKeys.Contains(x.Key)).Select(x => x.Name).ToArray());

                using (var transactionScope = new DatabaseTransactionScope("UpdateCallManagementSearchField", DeadlockPriority.Supervisor))
                {
                    new SearchableFieldsService().DeleteBySurveyId(SurveyId);

                    foreach (var selectedKey in selectedKeys)
                    {
                        string key = selectedKey;
                        var column = CallManagementSearchableColumns.Single(x => x.Key == key);
                        new SearchableFieldsService().Add(SurveyId, column.TableId, column.ColumnId);
                    }

                    transactionScope.Commit();
                }

                evt.Finish();

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void RefreshAvailableFields()
        {
            CallManagementSearchableColumns = null;
        }
    }
}
