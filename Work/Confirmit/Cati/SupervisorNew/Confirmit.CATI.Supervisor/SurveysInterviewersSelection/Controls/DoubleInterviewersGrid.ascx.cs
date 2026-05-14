using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls
{
    public partial class DoubleInterviewersGrid : BaseWUC
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly IPersonGroupManager _personGroupManager;

        public DoubleInterviewersGrid()
        {
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _personGroupManager = ServiceLocator.Resolve<IPersonGroupManager>();
        }

        private IEnumerable<PersonAndGroupInfoItem> _allInterviewers;
        private IDictionary<int, List<int>> _groupToPersonMapping;

        private  readonly SessionVariable<int[]> _selectedIds =
                                new SessionVariable<int[]>("_doubleGridSelectedInterviewersIds");

        public int[] SelectedIds
        {
            get { return _selectedIds.Value; }
            set { _selectedIds.Value = value != null ? value.ToArray() : null; }
        }

        private IEnumerable<PersonAndGroupInfoItem> AllInterviewers
        {
            get
            {
                if (_allInterviewers == null)
                {
                    _allInterviewers = PersonManager.GetAllPersonsAndGroups();
                }

                return _allInterviewers;
            }
        }

        private IDictionary<int, List<int>> GroupToPersonMapping
        {
            get
            {
                if (_groupToPersonMapping == null)
                {
                    var currentCallCenterId = _callCenterProvider.GetCurrentId();
                    _groupToPersonMapping = _personGroupManager.GetPersonsInGroups(currentCallCenterId);
                }

                return _groupToPersonMapping;
            }
        }

        private IEnumerable<PersonAndGroupInfoItem> EmptyGroups
        {
            get
            {
                return AllInterviewers.Where(x => x.IsGroup && GroupToPersonMapping[x.Id].All(personId => SelectedIds.Contains(personId)));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            selectedGrid.GetPage += GetSelectedListPage;
            allGrid.GetPage += GetAllAvailableListPage;
            allGrid.InitializeRow += Grid_InitializeRow;
            
            if (SelectedIds == null)
            {
                SelectedIds = new int[0];
            }

            var column = allGrid.Columns.FromKey("IsGroup") as GeneralGridColumn;

            if (column != null)
            {
                column.Items.Add(new ListItem(Strings.Group, "1"));
                column.Items.Add(new ListItem(Strings.Person, "0"));
            }
        }        
        
        protected void Add(object sender, EventArgs e)
        {
            var keys = allGrid.SelectedKeys.Select(Int32.Parse).ToList();
            var interviewersId = keys.SelectMany(key =>
            {
                var personOrGroup = AllInterviewers.FirstOrDefault(x => x.Id == key);
                if (personOrGroup != null)
                {
                    return personOrGroup.IsGroup
                        ? GroupToPersonMapping[personOrGroup.Id]
                        : new List<int> { personOrGroup.Id };
                }

                return null;
            }).Distinct();

            SelectedIds = SelectedIds.Union(interviewersId).ToArray();
            allGrid.ClearSelectedKeys();
        }

        protected void Remove(object sender, EventArgs e)
        {
            var keys = selectedGrid.SelectedKeys.Select(Int32.Parse).ToList();
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
            var list = AllInterviewers.Where(x => !SelectedIds.Contains(x.Id)).ToList();
            list = list.Except(EmptyGroups).ToList();

            return GetListPage(list, allGrid, out totalCount);
        }

        protected object GetSelectedListPage(out int totalCount)
        {
            var list = AllInterviewers.Where(x => SelectedIds.Contains(x.Id));

            return GetListPage(list, selectedGrid, out totalCount);
        }

        private object GetListPage(IEnumerable<PersonAndGroupInfoItem> list, GeneralGrid grid, out int totalCount)
        {
            var sortingArgs = new SortingArgsCollection { new SortingArgs("IsGroup", false) };

            if (!String.IsNullOrEmpty(grid.SortedColumnName))
            {
                sortingArgs.Add(new SortingArgs(grid.SortedColumnName,grid.SortIndicatorAsc));
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
            var dataItem = (PersonAndGroupInfoItem)e.Row.DataItem;
            isGroupCell.Text = dataItem.IsGroup ? Strings.Group : Strings.Person;
        }
    }
}
