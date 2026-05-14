using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls
{
    public partial class DoubleGroupsInterviewersGrid : BaseWUC
    {
        private IEnumerable<PersonAndGroupInfoItem> _allGroupsAndInterviewers;

        private  readonly SessionVariable<int[]> _selectedIds =
                                new SessionVariable<int[]>("_doubleGridSelectedInterviewersGroupsIds");

        public int[] SelectedIds
        {
            get { return _selectedIds.Value; }
            set { _selectedIds.Value = value != null ? value.ToArray() : null; }
        }

        private IEnumerable<PersonAndGroupInfoItem> AllGroupsAndInterviewers
        {
            get { return _allGroupsAndInterviewers ?? (_allGroupsAndInterviewers =  PersonManager.GetAllPersonsAndGroups()); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            selectedGrid.GetPage += GetSelectedListPage;
            allGrid.GetPage += GetAllAvailableListPage;

            selectedGrid.InitializeRow += Grid_InitializeRow;
            allGrid.InitializeRow += Grid_InitializeRow;            

            var column = selectedGrid.Columns.FromKey("IsGroup") as GeneralGridColumn;
            if (column != null) column.Items.AddRange(new[] { new ListItem(Strings.Group, "1"), new ListItem(Strings.Person, "0") });

            column = allGrid.Columns.FromKey("IsGroup") as GeneralGridColumn;
            if (column != null) column.Items.AddRange(new[] { new ListItem(Strings.Group, "1"), new ListItem(Strings.Person, "0") });

            if (SelectedIds == null)
            {
                SelectedIds = new int[0];
            }
        }        
        
        protected void Add(object sender, EventArgs e)
        {
            var keys = allGrid.SelectedKeys.Select(x => Int32.Parse(x.Split('_')[0])).ToList();
            
            SelectedIds = SelectedIds.Union(keys).ToArray();
            allGrid.ClearSelectedKeys();
        }

        protected void Remove(object sender, EventArgs e)
        {            
            var keys = selectedGrid.SelectedKeys.Select(x => Int32.Parse(x.Split('_')[0])).ToList();
            SelectedIds = SelectedIds.Except(keys).ToArray();
            selectedGrid.ClearSelectedKeys();
        }

        protected void RemoveAll(object sender, EventArgs e)
        {
            SelectedIds = Array.Empty<int>();
            selectedGrid.ClearSelectedKeys();
        }
        
        protected object GetAllAvailableListPage(out int totalCount)
        {
            var list = AllGroupsAndInterviewers.Where(x => !SelectedIds.Contains(x.Id)).ToList();

            return GetistPage(list, allGrid, out totalCount);
        }

        protected object GetSelectedListPage(out int totalCount)
        {
            var list = AllGroupsAndInterviewers.Where(x => SelectedIds.Contains(x.Id));

            return GetistPage(list, selectedGrid, out totalCount);
        }      

        private object GetistPage(IEnumerable<PersonAndGroupInfoItem> list, GeneralGrid grid, out int totalCount)
        {
            var sortingArgs = new SortingArgsCollection {new SortingArgs("IsGroup", false)};

            if (!String.IsNullOrEmpty(grid.SortedColumnName))
            {
                sortingArgs.Add(new SortingArgs(grid.SortedColumnName,
                                                grid.SortIndicatorAsc));
            }

            var args = new MultiSortPagingArgs(
                grid.PageIndex,
                grid.PageSize,
                sortingArgs,
                grid.SearchParameterCollection
                );

            return BaseMethods.GetPage(list, args, out totalCount);
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var isGroupCell = e.Row.Items.FindItemByKey("IsGroup");
            var keyCell = e.Row.Items.FindItemByKey("Id_IsGroup");
            var dataItem = (PersonAndGroupInfoItem)e.Row.DataItem;
            isGroupCell.Column.Type = typeof(string);

            keyCell.Text = String.Format("{0}_{1}", dataItem.Id, dataItem.IsGroup);
            isGroupCell.Text = dataItem.IsGroup ? Strings.Group : Strings.Person;
        }
        
    }
}
