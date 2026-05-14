using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes;
using Strings=Confirmit.CATI.Supervisor.Resources.Strings;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public enum AssignDirection
    {
        UserToSurvey = 1,
        SurveyToUser = 2,
        None = 3
    }

    public partial class SurveysAssignments : BaseForm
    {
        private IAssignmentManager _assignmentManager;

        public override string Title
        {
            get { return Strings.SurveysAssignments; }
        }

        /// <summary>
        /// set direction, in case direction User to Survey have true, else false
        /// </summary>
        protected AssignDirection Direction
        {
            get
            {
                if (ViewState["Direction"] == null)
                    ViewState["Direction"] = AssignDirection.None;
                return (AssignDirection)ViewState["Direction"];
            }
            set
            {
                ViewState["Direction"] = value;
            }
        }

        /// <summary>
        /// Gets or sets selected persons, that contained in central grid
        /// Save them into ViewState
        /// </summary>
        protected List<ICatiPersonItem> SelectedUsers
        {
            get
            {
                if (ViewState["SelectedUsers"] == null)
                    ViewState["SelectedUsers"] = new List<ICatiPersonItem>();
                return (List<ICatiPersonItem>)ViewState["SelectedUsers"];
            }
            set
            {
                ViewState["SelectedUsers"] = value;
            }
        }

        /// <summary>
        /// Gets or sets selected surveys, that contained in central grid
        /// Save them into ViewState
        /// </summary>
        protected List<SurveyInfo> SelectedSurveys
        {
            get
            {
                if (ViewState["SelectedSurveys"] == null)
                    ViewState["SelectedSurveys"] = new List<SurveyInfo>();
                return (List<SurveyInfo>)ViewState["SelectedSurveys"];
            }
            set
            {
                ViewState["SelectedSurveys"] = value;
            }
        }

        public SurveysAssignments()
        {
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UsersGrid.GetPage += GetUsersPage;
            SurveysGrid.GetPage += GetSurveysPage;

            personsTree.NodeDropped += personsTree_NodeDropped;
            surveysTree.NodeDropped += surveysTree_NodeDropped;

            personsTree.NodeDoubleClick += PersonsTreeNodeDoubleClick;
            surveysTree.NodeDoubleClick += SurveysTreeNodeDoubleClick;

            if (Direction == AssignDirection.SurveyToUser)
                RegisterStartupScript("ShowDiv('SurveysGrid');");
            else if (Direction == AssignDirection.UserToSurvey)
                RegisterStartupScript("ShowDiv('UsersGrid');");
        }           

        public void Page_PreRender(object sender, EventArgs e)
        {
            RegisterStartupScript(string.Format("resizeTreeScript('{0}','{1}', '{2}')", personsTree.TreeClientId, surveysTree.TreeClientId, updatePanelGrid.ClientID));            
        }                

        protected void SurveysTreeNodeDoubleClick(object sender, NodeDoubleClickEventArgs e)
        {
            Direction = AssignDirection.UserToSurvey;
            RegisterStartupScript("ShowDiv('UsersGrid');");
            lbGridName.Text = Strings.Interviewers;

            personsTree.UnselectAllPersons();
            surveysTree.UnselectAllSurveys();
            surveysTree.CheckSelectedNodes();
            var sid = Int32.Parse(e.DataKey);

            var resources = from resource in _assignmentManager.GetAssignedInterviewersAndGroupsList(sid)
                            where resource.AssignedCallsCount == 0
                            select new PersonGroupInfo(resource.IsGroup, resource.SID, resource.Name);

            SelectedUsers.Clear();
            resources.ToList().ForEach(AddUserToSelected);

            UsersGrid.BindData();
        }

        protected void PersonsTreeNodeDoubleClick(object sender, NodeDoubleClickEventArgs e)
        {
            Direction = AssignDirection.SurveyToUser;
            lbGridName.Text = Strings.Surveys;
            RegisterStartupScript("ShowDiv('SurveysGrid');");

            personsTree.UnselectAllPersons();
            surveysTree.UnselectAllSurveys();
            personsTree.CheckSelectedNodes();
            var sid = Int32.Parse(e.DataKey);
            
            var surveys = (from survey in _assignmentManager.GetAssignedSurveyList(sid, User.Name)
                           where survey.AssignmentType == 1 // explicit assignments only
                           select new {survey.SurveySID, survey.ProjectID, survey.ProjectName}).Distinct();

            SelectedSurveys =
                (from survey in surveys
                 select new SurveyInfo(survey.SurveySID, survey.ProjectName, survey.ProjectID, 0)).ToList();

            SurveysGrid.BindData();
        }

        #region Event handlers.

        /// <summary>
        /// Occurs when user selects surveys and tries to add them to central grid
        /// Save survey to SelectedSurveys collection
        /// </summary>
        protected void btnAddSurveys_Click(object sender, EventArgs e)
        {
            if (Direction == AssignDirection.UserToSurvey) return;
            if (Direction == AssignDirection.None)
            {
                Direction = AssignDirection.SurveyToUser;
                lbGridName.Text = "Surveys";
                RegisterStartupScript("ShowDiv('SurveysGrid');");
            }
            AddSurveys();
            SurveysGrid.BindData();
            UsersGrid.Visible = false;
        }

        /// <summary>
        /// Occurs when user selects users and tries to add them to central grid
        /// Save users to SelectedUsers collection
        /// </summary>
        protected void btnAddUsers_Click(object sender, EventArgs e)
        {
            if (Direction == AssignDirection.SurveyToUser) return;
            if (Direction == AssignDirection.None)
            {
                Direction = AssignDirection.UserToSurvey;
                RegisterStartupScript("ShowDiv('UsersGrid');");
               // UsersGrid.Grid.DisplayLayout.ColHeadersVisibleDefault = ShowMarginInfo.Yes;
                lbGridName.Text = Strings.Interviewers;
            }
            AddUsers();
            UsersGrid.BindData();
            SurveysGrid.Visible = false;
        }

        /// <summary>
        /// Occurs when user click reset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ResetAll();
        }

        /// <summary>
        /// Occurs when user click assign button
        /// Do assign, after reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAssign_Click(object sender, EventArgs e)
        {
            DoAssign();
            ResetAll();
        }

        protected void btnDeassign_Click(object sender, EventArgs e)
        {
            DoDeassign();
            ResetAll();
        }
        
        /// <summary>
        /// event occur when survey tree's node has been dropped
        /// </summary>
        void surveysTree_NodeDropped(object sender, NodeDroppedEventArgs e)
        {
            AddSurveys(e.NodeDataKey);
            Direction = AssignDirection.SurveyToUser; //suppose that in central grid there are surveys
            DoAssign();
            ResetAll();
        }
        
        void personsTree_NodeDropped(object sender, NodeDroppedEventArgs e)
        {
            AddUsers(e.NodeDataKey, e.NodeDataPath);
            Direction = AssignDirection.UserToSurvey; //suppose that in central grid there are persons
            DoAssign();
            ResetAll();
        }        

        protected void btnUserGridDeleteRows_Click(object sender, EventArgs e)
        {
            DeleteUsers();
            UsersGrid.BindData();
            UsersGrid.ClearSelectedKeys();
        }

        protected void btnSurveyGridDeleteRows_Click(object sender, EventArgs e)
        {
            DeleteSurveys();
            SurveysGrid.BindData();
            SurveysGrid.ClearSelectedKeys();
        }

        #endregion

        #region User's grid function

        private object GetUsersPage(out int iTotalCount)
        {
            iTotalCount = SelectedUsers.Count;
            return SelectedUsers;
        }

        //add all selected users
        private void AddUsers()
        {
            foreach (PersonGroupInfo user_info in personsTree.SelectedPersons)
            {
                AddUserToSelected(user_info);
            }
        }

        /// <summary>
        /// Add users for current node into SelectedUsers
        /// </summary>
        /// <remarks>It is used in drag&drop case for fill SelectedUsers</remarks>
        private void AddUsers(string nodeKey, string nodePath)
        {
            AddUserToSelected(personsTree.GetPersonsByNode(nodeKey, nodePath));
        }

        /// <summary>
        /// Add user or person to SelectedUsers collection
        /// </summary>
        /// <param name="user_info"></param>
        private void AddUserToSelected(PersonGroupInfo user_info)
        {
            ICatiPersonItem user = null;
            if (!user_info.IsGroup && user_info.SID > 0)
            {
                user = new CatiUserItem(user_info.SID, user_info.Name, user_info.Description);
            }
            else if (user_info.IsGroup && user_info.SID > 0)
            {
                user = new CatiGroupItem(user_info.SID, user_info.Name);
            }

            if (user != null)
            {
                // TODO: Refactor
                user.Init();
                bool bContain = false;
                foreach (ICatiPersonItem cui in SelectedUsers)
                    if (cui.Id == user.Id)
                        bContain = true;
                if (!bContain)
                    SelectedUsers.Add(user);
            }
        }

        private void DeleteUsers()
        {
            string personKey = UsersGrid.SelectedKeys[0];
            if (!String.IsNullOrEmpty(personKey))
            {
                foreach (ICatiPersonItem cui in SelectedUsers)
                {
                    if (cui.Id.ToString() == personKey)
                    {
                        SelectedUsers.Remove(cui);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Surveys's grid function

        private object GetSurveysPage(out int iTotalCount)
        {
            iTotalCount = SelectedSurveys.Count;
            return SelectedSurveys;
        }

        private void AddSurveys()
        {
            foreach (SurveyInfo selected_survey in surveysTree.CheckedSurveys)
            {
                bool bContain = false;
                foreach (SurveyInfo added_survey in SelectedSurveys)
                    if (selected_survey.Id == added_survey.Id)
                        bContain = true;
                if (!bContain)
                    SelectedSurveys.Add(selected_survey);
            }
        }

        private void AddSurveys(string nodeKey)
        {
            foreach (SurveyInfo selected_survey in surveysTree.GetSurveysByNode(nodeKey))
            {
                bool bContain = false;
                foreach (SurveyInfo added_survey in SelectedSurveys)
                    if (selected_survey.Id == added_survey.Id)
                        bContain = true;
                if (!bContain)
                    SelectedSurveys.Add(selected_survey);
            }
        }

        private void DeleteSurveys()
        {
            string surveyKey = SurveysGrid.SelectedKeys[0];
            if (!String.IsNullOrEmpty(surveyKey))
            {
                foreach (SurveyInfo si in SelectedSurveys)
                {
                    if (si.Id.ToString() == surveyKey)
                    {
                        SelectedSurveys.Remove(si);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Assignment's function
        /// <summary>
        /// Assign selected users to surveys, or vice versa, selected surveys to users.
        /// </summary>
        /// <returns>Count of successful assignments.</returns>
        protected int DoAssign()
        {
            int count = 0;
            var surveySids = new List<int>();
            var interviewerSids = new List<int>();

            if (Direction == AssignDirection.UserToSurvey)
            {
                count = AddMultipleAssignments(surveysTree.CheckedSurveys.Select(x => x.Id),
                                               SelectedUsers.Select(x => x.Id).ToArray());

                // Collect survey and person SIDs to show warning for predictive surveys.
                surveySids = surveysTree.CheckedSurveys.Select(x => x.Id).ToList();
                foreach (ICatiPersonItem item in SelectedUsers)
                {
                    interviewerSids.AddRange(PredictiveHelper.GetAllPersonSidsList(item.Id, item is CatiGroupItem));
                }
            }
            else if (Direction == AssignDirection.SurveyToUser)
            {
                count = AddMultipleAssignments(SelectedSurveys.Select(x => x.Id),
                                               personsTree.SelectedPersons.Select(x => x.SID).ToArray());

                // Collect survey and person SIDs to show warning for predictive surveys.
                surveySids = SelectedSurveys.Select(x => x.Id).ToList();
                foreach (PersonGroupInfo item in personsTree.SelectedPersons)
                {
                    interviewerSids.AddRange(PredictiveHelper.GetAllPersonSidsList(item.SID, item.IsGroup));
                }
            }

            // Show warning if needed.
            string warningText = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(surveySids, interviewerSids);
            warningText = warningText ?? String.Empty;
            ShowClientMessage(string.Format("Total assigned operations: {0}\n\n{1}", count, warningText));
            return count;
        }

        protected int DoDeassign()
        {
            int count = 0;
            IEnumerable<int> surveySids = new int[] { };
            IEnumerable<int> personSids = new int[] { };

            switch (Direction)
            {
                case AssignDirection.None:
                    break;
                case AssignDirection.UserToSurvey:
                    surveySids = from survey in surveysTree.CheckedSurveys select survey.Id;
                    personSids = from person in SelectedUsers select person.Id;
                    break;
                case AssignDirection.SurveyToUser:
                    surveySids = from survey in SelectedSurveys select survey.Id;
                    personSids = from person in personsTree.SelectedPersons select person.SID;
                    break;                
                default:
                    throw new NotSupportedException();
            }

            if (personSids.Count() != 0)
            {
                foreach (int surveySid in surveySids)
                {
                    try
                    {
                        using (var transaction = new DatabaseTransactionScope("Supervisor.DoDeassign", DeadlockPriority.Supervisor))
                        {
                            count += AssignmentWithEventLoggingPerformer.DeassignResourcesFromSurvey(surveySid, personSids);

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Context.AddError(ex);
                    }
                }
            }

            ShowClientMessage(string.Format("Total deassigned operations: {0}", count));
            return count;
        }

        protected void ResetAll()
        {
            Direction = AssignDirection.None;
            SelectedUsers = null;
            SelectedSurveys = null;
            RegisterStartupScript("ShowDiv('UsersGrid');");

            //UsersGrid.Grid.DisplayLayout.ColHeadersVisibleDefault = ShowMarginInfo.No;
            UsersGrid.BindData();

            UsersGrid.ClearSelectedKeys();
            SurveysGrid.ClearSelectedKeys();

            lbGridName.Text = Strings.InterviewersSurveys;

            surveysTree.RefreshData();
            personsTree.RefreshData();
        }

        /// <summary>
        /// Assigns persons to surveys.
        /// </summary>
        /// <param name="surveySids">Survey SIDs to assign.</param>
        /// <param name="personSids">Person SIDs to assign.</param>
        /// <returns>Count of successful assignments.</returns>
        private int AddMultipleAssignments(IEnumerable<int> surveySids, IEnumerable<int> personSids)
        {
            int count = 0;
            foreach (int surveySid in surveySids)
            {
                try
                {
                    using (var transaction = new DatabaseTransactionScope("AddMultipleAssignments", DeadlockPriority.Supervisor))
                    {
                        count += AssignmentWithEventLoggingPerformer.AssignResourcesToSurvey(surveySid, personSids);
                        
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Context.AddError(ex);
                }
            }

            return count;
        }

        #endregion
       
    }
}

