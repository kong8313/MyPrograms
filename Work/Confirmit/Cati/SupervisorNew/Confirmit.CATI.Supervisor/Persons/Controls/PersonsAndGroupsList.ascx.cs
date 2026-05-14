using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls.Grid;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Persons.Controls
{
    public partial class PersonsAndGroupsList : BaseWUC
    {
        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

        /// <summary>
        /// List of persons and groups items to show in the control.
        /// </summary>
        public List<PersonAndGroupInfoItem> Data
        {
            get;
            set;
        }

        public bool DialTypeVisible
        {
            get { return DialTypeTable.Visible; }
            set { DialTypeTable.Visible = value; }
        }

        public DialType? DialTypeId
        {
            get { return ddlDialType.SelectedDialType; }
        }

        /// <summary>
        /// List name to show in the header.
        /// </summary>
        public string ListName
        {
            get
            {
                return personsAndGroupsListGrid.GridName;
            }
            set
            {
                personsAndGroupsListGrid.GridName = value;
            }
        }

        /// <summary>
        /// Grid hint text to show below the toolbar.
        /// </summary>
        public string HintText
        {
            get
            {
                return personsAndGroupsListGrid.HintText;
            }
            set
            {
                personsAndGroupsListGrid.HintText = value;
            }
        }

        /// <summary>
        /// Selected keys.
        /// </summary>
        public string[] SelectedKeys
        {
            get
            {
                return personsAndGroupsListGrid.SelectedKeys;
            }
            set
            {
                personsAndGroupsListGrid.SelectedKeys = value;
            }
        }

        /// <summary>
        /// If true, multiselection is enabled. Checkbox column exists in the grid.
        /// If false, selecting of only one record is enabled. Checkbox column does not exist in the grid. Highlighted record is selected.
        /// </summary>
        public bool AllowMultiSelection
        {
            get
            {
                return !personsAndGroupsListGrid.HideSelectedColumn;
            }
            set
            {
                personsAndGroupsListGrid.HideSelectedColumn = !value;
            }
        }

        /// <summary>
        /// Gets the SIDs of interviewers that are explicitly selected it the list and that are contained in selected groups ang their subgroups. .
        /// </summary>
        public List<int> AllInterviewersIDs
        {
            get
            {
                var interviewerSids = new List<int>();

                foreach (string key in personsAndGroupsListGrid.SelectedKeys)
                {
                    int sid = Int32.Parse(key.Split('_')[0]);
                    bool isGroup = Boolean.Parse(key.Split('_')[1]);
                    interviewerSids.AddRange(PredictiveHelper.GetAllPersonSidsList(sid, isGroup));
                }

                interviewerSids = interviewerSids.Distinct().ToList();
                return interviewerSids;
            }
        }

        /// <summary>
        /// Gets the SIDs of interviewers that are explicitly selected.
        /// </summary>
        public List<int> SelectedInterviewersIDs
        {
            get
            {
                return SelectedKeys.Select(x => Int32.Parse(x.Split('_')[0])).ToList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            personsAndGroupsListGrid.GetPage += GetPage;
            personsAndGroupsListGrid.InitializeRow += Grid_InitializeRow;

            var column = personsAndGroupsListGrid.Columns.FromKey("IsGroup") as GeneralGridColumn;

            if (column != null)
            {
                column.Items.Add(new ListItem(Strings.Group, "1"));
                column.Items.Add(new ListItem(Strings.Person, "0"));
            }

            DialTypeTable.Visible = DialTypeVisible && _toggleSettings.ShowDialType;
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int totalCount)
        {
            List<PersonAndGroupInfoItem> usersList = Data;

            var sortingArgs = new SortingArgsCollection { new SortingArgs("IsGroup", false) };
            if (!String.IsNullOrEmpty(personsAndGroupsListGrid.SortedColumnName))
            {
                sortingArgs.Add(new SortingArgs(personsAndGroupsListGrid.SortedColumnName, personsAndGroupsListGrid.SortIndicatorAsc));
            }

            var args = new MultiSortPagingArgs(
                personsAndGroupsListGrid.PageIndex,
                personsAndGroupsListGrid.PageSize,
                sortingArgs,
                personsAndGroupsListGrid.SearchParameterCollection
            );

            return BaseMethods.GetPage(usersList, args, out totalCount);
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var isGroupCell = e.Row.Items.FindItemByKey("IsGroup");
            var keyCell = e.Row.Items.FindItemByKey("Id_IsGroup");
            var dataItem = (PersonAndGroupInfoItem)e.Row.DataItem;
            isGroupCell.Column.Type = typeof(string);

            var keyCellValue = String.Format("{0}_{1}", dataItem.Id, dataItem.IsGroup);
            keyCell.Text = keyCellValue;
            keyCell.Value = keyCellValue;
            isGroupCell.Text = dataItem.IsGroup ? Strings.Group : Strings.Person;

            if (AllowMultiSelection)
            {
                var checkbox = e.Row.Items.FindItemByKey("Selected").FindControl("cbxSelection") as NotSubmitCheckBox;
                if (checkbox != null)
                {
                    checkbox.Checked = SelectedKeys.Contains(keyCellValue);
                }
            }

        }
    }
}
