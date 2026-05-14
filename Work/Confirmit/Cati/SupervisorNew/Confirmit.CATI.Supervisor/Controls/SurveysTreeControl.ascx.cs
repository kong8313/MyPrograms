using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Infragistics.Web.UI;
using Infragistics.Web.UI.NavigationControls;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class SurveysTreeControl : BaseWUC
    {
        #region Members

        private const string RootGroupKey = "root";
        private List<string> m_CheckedItems = new List<string>();
        private List<string> m_AllItems = new List<string>();

        #endregion
 
        #region Properties

        public event EventHandler DataBound;

        public bool AreAllNodesChecked
        {
            get
            {
                return tree.AreAllNodesChecked;
            }
        }

        public bool UseCheckBoxes
        {
            get { return tree.UseCheckBoxes; }
            set { tree.UseCheckBoxes = value; }
        }

        /// <summary>
        /// Height of the tree.
        /// </summary>
        public Unit Height
        {
            get { return tree.Height; }
            set { tree.Height = value; }
        }

        /// <summary>
        /// Shows or hides survey name filter.
        /// </summary>
        public bool ShowFilter
        {
            get { return phFilter.Visible; }
            set { phFilter.Visible = value; }
        }        

        /// <summary>
        /// Gets or sets filter string.
        /// It's necessary to store it in ViewState and change only if "apply/reset filter" buttons are clicked.
        /// </summary>
        public string Filter
        {
            get { return (string)ViewState["Filter"] ?? String.Empty; }
            set { ViewState["Filter"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data bind should be done on each postback.
        /// </summary>
        /// <value><c>true</c> if data bind should be done on each postback; otherwise, <c>false</c>.</value>
        public bool AutoBindOnPostback { get; set; }

        public bool OnlyOpenedSurveys { get; set; }

        public SurveyInfo SelectedSurvey
        {
            get
            {
                var nodes = tree.SelectedNodes;

                if (nodes != null && nodes.Count() > 0 && nodes[0].Key != RootGroupKey)
                {
                    return new SurveyInfo(int.Parse(nodes[0].Key));
                }

                return null;
            }
        }
               
        public List<SurveyInfoItem> CheckedSurveys
        {
            get
            {
                return (from DataTreeNode node in tree.Nodes[0].Nodes
                        where node.CheckState == CheckBoxState.Checked
                        select new SurveyInfoItem(int.Parse(node.Key), node.Text))
                    .ToList();
            }
        }

        #endregion

        #region Page life cycle

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitTree();
                tree.CheckAllNodes();
            }

            if (IsPostBack && AutoBindOnPostback)
            {
                RefreshData();
            }

            //If checkboxes are allowed, subscribe to server-side event NodeChecked.            
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // "Select only open" button does not make sense if we show only opened surveys in the tree
            btnCheckOpenSurveys.Visible = !OnlyOpenedSurveys && tree.UseCheckBoxes;

            RegisterClientScripts();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Applies filter to the tree.
        /// </summary>
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Filter = tbxFilter.Text.Trim();
            RefreshData();
        }

        /// <summary>
        /// Populates tree with unfiltered data.
        /// </summary>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Filter = String.Empty;
            tbxFilter.Text = String.Empty;
            RefreshData();
        }

        /// <summary>
        /// Finds first occurence of the specified survey.
        /// </summary>
        protected void btnFindFirst_Click(object sender, EventArgs e)
        {
            FindNode(tbxFind.Text.Trim(), false);
        }

        /// <summary>
        /// Finds next occurence of the specified survey.
        /// </summary>
        protected void btnFindNext_Click(object sender, EventArgs e)
        {
            FindNode(tbxFind.Text.Trim(), true);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkSortMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void chkSortMode_CheckedChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        /// <summary>
        /// Handles the Click event of the btnCheckOpenSurveys control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnCheckOpenSurveys_Click(object sender, EventArgs e)
        {
            SetCheckedSurveys(SurveyService.OpenedSurveys.Select(x => x.SID.GetValueOrDefault()), true);
        }

        #endregion

        #region Methods
       
        private void InitTree()
        {
            tree.Nodes.Clear();

            var root = new DataTreeNode
            {
                Key = RootGroupKey,
                Text = Strings.Surveys,
                Expanded = true,
                CheckState = m_CheckedItems.Contains(RootGroupKey) ? CheckBoxState.Checked : CheckBoxState.Unchecked
            };

            tree.Nodes.Add(root);

            List<SurveyInfoItem> surveys = GetAllSurveys();

            surveys = surveys.OrderBy(x => chkSortMode.Checked ? x.ConfirmitID : x.Name).ToList();

            foreach (SurveyInfoItem survey in surveys)
            {
                //Set survey node to checked if it was previously checked or it is a new node and a root node is checked.
                bool check = m_CheckedItems.Contains(survey.Id.ToString()) ||
                            (root.CheckState == CheckBoxState.Checked && m_AllItems.Contains(survey.Id.ToString()) == false);

                var node = new DataTreeNode
                {
                    Key = survey.Id.ToString(),
                    Text = String.Format("{0} ({1})", survey.Name, survey.ConfirmitID),
                    CheckState = check ? CheckBoxState.Checked : CheckBoxState.Unchecked
                };

                root.Nodes.Add(node);
            }

            if (DataBound != null)
            {
                DataBound(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the checked surveys in the tree by IDs. All other surveys will be unchecked.
        /// </summary>
        /// <param name="surveyIds">The survey IDs to check.</param>
        /// <param name="checkRootNode"></param>
        public void SetCheckedSurveys(IEnumerable<int> surveyIds, bool checkRootNode)
        {
            tree.Nodes[0].CheckState = checkRootNode ? CheckBoxState.Checked : CheckBoxState.Unchecked;

            foreach (DataTreeNode node in tree.Nodes[0].Nodes)
            {
                node.CheckState = surveyIds != null && surveyIds.Any(x => x.ToString() == node.Key) ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            }
        }

        private List<SurveyInfoItem> GetAllSurveys()
        {
            var surveys = SurveyManager.GetSurveys(User.Name, Filter);

            if (OnlyOpenedSurveys)
            {
                var openedSurveyIDs = SurveyService.OpenedSurveys.Select(x => x.SID);
                surveys = surveys.Where(x => openedSurveyIDs.Contains(x.Id)).ToList();
            }

            return surveys;
        }

        /// <summary>
        /// Refreshes data.
        /// </summary>
        private void RefreshData()
        {
            m_CheckedItems = tree.CheckedNodes.Select(x => x.Key).ToList();
            m_AllItems = tree.AllNodes.Select(x => x.Key).ToList();
            InitTree();
        }

        /// <summary>
        /// Finds node with specified text.
        /// </summary>
        private void FindNode(string name, bool findNext)
        {
            //Set start position for search.
            DataTreeNode node;

            if (findNext && (tree.SelectedNodes.Count > 0) && (tree.SelectedNodes[0].ParentNode != null))
                node = tree.SelectedNodes[0].NextNode;
            else
                node = tree.Nodes.FindNodeByKey(RootGroupKey).Nodes[0];

            tree.UnselectAllNodes();

            //Search.
            while (node != null)
            {
                if (node.Text.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    node.Selected = true;
                    // tree.ScrollNodeIntoView(node);
                    return;
                }
                node = node.NextNode;
            }
        }

        /// <summary>
        /// Register client scripts.
        /// </summary>
        private void RegisterClientScripts()
        {
            string format = "if(event.keyCode == 13) {{document.getElementById('{0}').click(); return false;}}";
            tbxFilter.Attributes.Add("onkeydown", String.Format(format, btnFilter.ClientID));
            tbxFind.Attributes.Add("onkeydown", String.Format(format, btnFindFirst.ClientID));            
        }

        public List<SurveyInfoItem> GetCheckedSurveysOrAll()
        {
            var checkedSurveys = CheckedSurveys;

            if (checkedSurveys.Count == 0)
            {
                checkedSurveys = GetAllSurveys();
            }

            return checkedSurveys;
        }

        public void SetSelectedSurvey(int surveyId)
        {
            var node = tree.Nodes[0].Nodes.OfType<DataTreeNode>().Where(x => x.Key == surveyId.ToString()).FirstOrDefault();

            if (node != null)
            {
                node.Selected = true;
                tree.ActiveNode = node;
            }
        }


        #endregion
    }
}