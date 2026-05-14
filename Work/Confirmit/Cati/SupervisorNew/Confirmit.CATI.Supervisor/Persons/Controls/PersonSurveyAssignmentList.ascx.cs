using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.Backend.Assignment;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;
using Infragistics.Web.UI.GridControls;

using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Persons.Controls
{
    /// <summary>
    /// Represents list of surveys assigned on the interviewer
    /// </summary>
    public partial class PersonSurveyAssignmentList : BaseWUC
    {
        private readonly IAssignmentManager _assignmentsManager;
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly IAssignmentWithEventLoggingPerformer _assignmentWithEventLoggingPerformer;

        public PersonSurveyAssignmentList()
        {
            _assignmentsManager = ServiceLocator.Resolve<IAssignmentManager>();
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _assignmentWithEventLoggingPerformer = ServiceLocator.Resolve<IAssignmentWithEventLoggingPerformer>();
        }

        
        /// <summary>
        /// Occurs when user specifies Automatic survey
        /// </summary>
        public event EventHandler AutomaticSurveyChanged;

        /// <summary>
        /// Person's or person group's SID.
        /// </summary>
        public int SID
        {
            get
            {
                return ViewState["Id"] == null ? 0 : (int)ViewState["Id"];
            }
            set
            {
                ViewState["Id"] = value;
            }
        }

        /// <summary>
        /// Determins if current object is group.
        /// </summary>
        public bool IsGroup
        {
            get
            {
                return ViewState["IsGroup"] == null ? false : (bool)ViewState["IsGroup"];
            }
            set
            {
                ViewState["IsGroup"] = value;
            }
        }

        /// <summary>
        /// Current person information. 
        /// </summary>
        private CatiUserItem Person
        {
            get
            {
                if (IsGroup)
                    throw new InvalidOperationException(Strings.GetPersonInsteadOfGroupExceptionMessage);

                if (ViewState["Person"] == null)
                {
                    CatiUserItem catiUserItem = new CatiUserItem(SID);
                    ViewState["Person"] = catiUserItem;
                }

                return (CatiUserItem)ViewState["Person"];
            }
        }

        /// <summary>
        /// Current group information.
        /// </summary>
        private CatiGroupItem Group
        {
            get
            {
                if (!IsGroup)
                    throw new InvalidOperationException(Strings.GetGroupInsteadOfPersonExceptionMessage);
                if (ViewState["Group"] == null)
                {
                    CatiGroupItem catiGroupItem = new CatiGroupItem(SID);
                    catiGroupItem.Init();
                    ViewState["Group"] = catiGroupItem;
                }
                return (CatiGroupItem)ViewState["Group"];
            }
        }

        /// <summary>
        /// Person's or person group's name.
        /// </summary>
        public string ObjectName
        {
            get
            {
                return IsGroup ? Group.Name : Person.Name;
            }
        }

        /// <summary>
        /// Page load event handler.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SID == 0)
            {
                return;
            }

            if (IsGroup)
            {
                m_grid.GridName = GetResString("GroupAssignedSurveys", ObjectName);
            }
            else
            {
                m_grid.GridName = PersonRepository.GetById(SID).Type == (byte) AgentType.IvrAgent
                    ? GetResString("IvrAgentAssignedSurveys", ObjectName)
                    : GetResString("PersonAssignedSurveys", ObjectName);
            }

            m_grid.GetPage += GetPage;
            m_grid.InitializeRow += Grid_InitializeRow;

            foreach (var overlayCommand in from Command command in m_grid.Commands
                                     where command.Key == "New" || command.Key == "Replace"
                                     select (OverlayCommand)command)
            {
                overlayCommand.ExternalDynamicParams.Add("ObjectSid", SID.ToString(CultureInfo.InvariantCulture));
                overlayCommand.ExternalDynamicParams.Add("IsGroup", IsGroup.ToString(CultureInfo.InvariantCulture));
                overlayCommand.ExternalDynamicParams.Add("ReplaceAssignment", (overlayCommand.Key == "Replace").ToString(CultureInfo.InvariantCulture));
            }

            if (IsGroup)
            {
                // hidding assignment group column for groups
                GeneralGridColumn column = (GeneralGridColumn)m_grid.Columns.FromKey("AssignmentGroup");
                column.Hidden = true;

                m_grid.HideCommand("SetAutomaticSurvey");
            }
        }

        /// <summary>
        /// Handles InitializeRow event of the grid.
        /// Used to change boolean value in the "Type" column to "Group" or "Person" text.
        /// </summary>
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int assignedCount = Int32.Parse(e.Row.Items.FindItemByKey("AssignedCallsCount").Value.ToString());
            int sid = Int32.Parse(e.Row.Items.FindItemByKey("SurveySID").Value.ToString());
            string group = e.Row.Items.FindItemByKey("AssignmentGroup").Value.ToString();

            if (e.Row.Items.FindItemByKey("SID_Calls") != null)
                e.Row.Items.FindItemByKey("SID_Calls").Text = sid + "_" + assignedCount + "_" + group;

            if (assignedCount == 0)
            {
                e.Row.Items.FindItemByKey("AssignedCallsCount").Text = string.Format("{0} (0)", Strings.Any);
            }

            // highlighting automatic survey for current person
            BvSurveyEntity survey = null;
            if (IsGroup == false &&
                // TODO: Performance issue - DB read for each row in grid!
                (survey = PersonService.GetPersonAutomaticSurvey(SID)) != null &&
                sid == survey.SID)
            {
                e.Row.CssClass += " AutoSurveyRow";
            }
        }

        protected void SetAutomaticSurvey(object sender, EventArgs e)
        {
            // processing automatic survey only for persons
            if (!IsGroup)
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.SetAutomaticSurvey", DeadlockPriority.Supervisor))
                {
                    int surveyId = Int32.Parse(m_grid.HighlightedKey.Split('_')[0]);
                    PersonService.SetAutomaticSurvey(SID, surveyId, true);

                    OnAutomaticSurveyChanged(EventArgs.Empty);

                    transaction.Commit();
                }
            }
        }

        protected void RefreshHandler(object sender, EventArgs e)
        {
            m_grid.RefreshHandler(sender, e);
        }

        public void EnableAutomaticSurveyButton(bool enabled)
        {
            if (enabled)
                m_grid.EnableCommand("SetAutomaticSurvey");
            else
                m_grid.DisableCommand("SetAutomaticSurvey");            
        }

        /// <summary>
        /// Deassign selected surveys.
        /// </summary>
        protected void DeassignSurveys(object sender, EventArgs e)
        {
            List<string> surveys = new List<string>();

            using (var transaction = new DatabaseTransactionScope("DeassignSurveysFromResource", DeadlockPriority.Supervisor))
            {
                foreach (string key in m_grid.SelectedKeys)
                {
                    string[] parts = key.Split(new[] { '_' }, 3);
                    int surveySid = Int32.Parse(parts[0]);
                    int assignedCallsCount = Int32.Parse(parts[1]);
                    string groupOrPersonName = parts[2];
                    
                    if (assignedCallsCount == 0)
                    {
                        if (String.IsNullOrEmpty(groupOrPersonName))
                        {
                            AssignmentWithEventLoggingPerformer.DeassignResourcesFromSurvey(surveySid, new[] { SID });
                        }
                        else
                        {
                            // implicit assignment by group. Do nothing, show warning
                            BvSurveyEntity survey = SurveyRepository.GetById(surveySid);
                            surveys.Add(survey.Description);
                        }
                    }
                    else
                    {
                        if (ObjectName != groupOrPersonName)
                        {
                            // implicit assignment by group. Do nothing, show warning
                            BvSurveyEntity survey = SurveyRepository.GetById(surveySid);
                            surveys.Add(survey.Description);
                        }
                        else
                        {
                            _assignmentWithEventLoggingPerformer.DeassignResourcesFromSurveyCalls(surveySid, new[] { SID });
                        }
                    }
                }

                transaction.Commit();
            }

            if (surveys.Count > 0)
            {
                ShowClientMessage(
                    GetResString("GroupAssignmentWarning", String.Join(", ", surveys.ToArray()))
                );
            }

            m_grid.ClearSelectedKeys();
            m_grid.BindData();

            Page.RefreshInfoFrame();
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int totalCount)
        {
            List<PersonAssignmentInfoItemWithGroupName> list = _assignmentsManager.GetPersonAssignments(SID, User.Name, _callCenterProvider.GetCurrentId());
            list.ForEach(x =>
            {
                x.ParentGroupName = x.ParentGroupName.Replace(',', ';');
            });

            SortingArgsCollection sortingArgs = new SortingArgsCollection();
            
            if (!String.IsNullOrEmpty(m_grid.SortedColumnName))
            {
                sortingArgs.Add(
                    new SortingArgs(
                        m_grid.SortedColumnName,
                        m_grid.SortIndicatorAsc
                    )
                );
            }
            sortingArgs.Add(
                new SortingArgs(
                    "AssignmentType",
                    true
                )
            );

            MultiSortPagingArgs args = new MultiSortPagingArgs(
                m_grid.PageIndex,
                m_grid.PageSize,
                sortingArgs,
                m_grid.SearchParameterCollection
            );
            return BaseMethods.GetPage(list, args, out totalCount);
        }

        /// <summary>
        /// Raises AutomaticSurveyChanged event
        /// </summary>
        /// <param name="e"></param>
        private void OnAutomaticSurveyChanged(EventArgs e)
        {
            if (AutomaticSurveyChanged != null)
            {
                AutomaticSurveyChanged(this, e);
            }
        
        }
    }
}