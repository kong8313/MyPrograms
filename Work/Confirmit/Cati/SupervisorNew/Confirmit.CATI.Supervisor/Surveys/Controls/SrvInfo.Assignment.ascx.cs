using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Paging;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Infragistics.Web.UI.GridControls;

using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    public partial class SrvInfo_Assignment : SrvInfoChild
    {
        private readonly IAssignmentManager _assignmentManager;
        private readonly IAssignmentWithEventLoggingPerformer _assignmentWithEventLoggingPerformer;

        #region Lifecycle

        /// <summary>
        /// Page load event handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GridName = string.Format(Strings.AssignmentsForSurvey, Survey.Description, Survey.Name);

            m_grid.GetPage = new GetPageDelegate(GetPage);
            m_grid.InitializeRow += Grid_InitializeRow;

            GeneralGridColumn column = m_grid.Columns.FromKey("IsGroup") as GeneralGridColumn;

            if (column != null)
            {
                column.Items.Add(new ListItem(Strings.Group, "1"));
                column.Items.Add(new ListItem(Strings.Person, "0"));
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles InitializeRow event of the grid.
        /// Used to change boolean value in the "Type" column to "Group" or "Person" text.
        /// </summary>
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Items.FindItemByKey("IsGroup").Column.Type = typeof(string);
            bool isGroup = (bool)e.Row.Items.FindItemByKey("IsGroup").Value;
            int assignedCount = Int32.Parse(e.Row.Items.FindItemByKey("AssignedCallsCount").Value.ToString());
            int sid = Int32.Parse(e.Row.Items.FindItemByKey("SID").Value.ToString());
            if (e.Row.Items.FindItemByKey("SID_Calls") != null)
                e.Row.Items.FindItemByKey("SID_Calls").Text = sid + "_" + assignedCount;
            e.Row.Items.FindItemByKey("IsGroup").Text = isGroup ? Strings.Group : Strings.Person;

            if (assignedCount == 0)
            {
                e.Row.Items.FindItemByKey("AssignedCallsCount").Text = string.Format("{0} (0)", Strings.Any);
            }
        }
        #endregion

        #region Methods

        public SrvInfo_Assignment()
        {
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
            _assignmentWithEventLoggingPerformer = ServiceLocator.Resolve<IAssignmentWithEventLoggingPerformer>();
        }

        protected void Refresh(object sender, EventArgs e)
        {
            m_grid.RefreshHandler(sender, e);
        }

        /// <summary>
        /// Deassign selected users and groups.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeassignUsers(object sender, EventArgs e)
        {
            var resourcesToDeassign =
                from key in m_grid.SelectedKeys
                let keyArray = key.Split('_')
                select new {ResourceId = Int32.Parse(keyArray[0]), Calls = Int32.Parse(keyArray[1])};

            var resourcesToDeassignFromSurvey =
                from resource in resourcesToDeassign
                where resource.Calls == 0
                select resource.ResourceId;

            var resourcesToDeassignFromSurveyCalls =
                from resource in resourcesToDeassign
                where resource.Calls != 0
                select resource.ResourceId;

            using (var transaction = new DatabaseTransactionScope("DeassignResourceFromSurvey", DeadlockPriority.Supervisor))
            {
                if (resourcesToDeassignFromSurvey.Any())
                {
                    AssignmentWithEventLoggingPerformer.DeassignResourcesFromSurvey(
                    Survey.SID,
                    resourcesToDeassignFromSurvey);
                }

                if (resourcesToDeassignFromSurveyCalls.Any())
                {
                    _assignmentWithEventLoggingPerformer.DeassignResourcesFromSurveyCalls(
                    Survey.SID,
                    resourcesToDeassignFromSurveyCalls);
                }

                transaction.Commit();
            }

            m_grid.ClearSelectedKeys();
            m_grid.BindData();
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int count)
        {
            List<SurveyAssignmentInfoItem> list = _assignmentManager.GetAssignedInterviewersAndGroupsList(Survey.SID);

            SortingArgsCollection sortingArgs = new SortingArgsCollection();
            sortingArgs.Add(new SortingArgs("IsGroup", false));
            if (!String.IsNullOrEmpty(m_grid.SortedColumnName))
            {
                sortingArgs.Add(new SortingArgs(m_grid.SortedColumnName, m_grid.SortIndicatorAsc));
            }

            MultiSortPagingArgs args = new MultiSortPagingArgs(
                m_grid.PageIndex,
                m_grid.PageSize,
                sortingArgs,
                m_grid.SearchParameterCollection
            );
            return BaseMethods.GetPage(list, args, out count);
        }

        #endregion
    }
}