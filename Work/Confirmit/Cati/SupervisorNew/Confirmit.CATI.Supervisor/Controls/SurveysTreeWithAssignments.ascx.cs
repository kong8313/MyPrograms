using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI;
using Infragistics.Web.UI.NavigationControls;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class SurveysTreeWithAssignments : BaseWUC
    {
        #region Members

        private const string RootGroupKey = "root";

        #endregion

        #region Properties 
        
        public bool UseCheckBoxes
        {
            get { return tree.UseCheckBoxes; }
            set { tree.UseCheckBoxes = value; }
        }

        public bool AreAllNodesChecked
        {
            get
            {
                return tree.AreAllNodesChecked;
            }
        }

        public string TreeClientId
        {
            get { return tree.ClientID; }            
        }
        
        public Unit Height
        {
            get { return tree.Height; }
            set { tree.Height = value; }
        }

        public bool ShowFilter
        {
            get { return phFilter.Visible; }
            set { phFilter.Visible = value; }
        }

        public string Filter
        {
            get { return (string)ViewState["Filter"] ?? String.Empty; }
            set { ViewState["Filter"] = value; }
        }

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

        public List<SurveyInfo> CheckedSurveys
        {
            get
            {
                return (from DataTreeNode node in tree.Nodes[0].Nodes
                        where node.CheckState == CheckBoxState.Checked
                        select new SurveyInfo(int.Parse(node.Key)))
                    .ToList();
            }
        }

        #endregion

        #region Events

        public event EventHandler<NodeDoubleClickEventArgs> NodeDoubleClick;

        public event EventHandler<NodeDroppedEventArgs> NodeDropped
        {
            add { tree.NodeDropped += value; }
            remove { tree.NodeDropped -= value; }
        }

        #endregion

        #region Page life cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitTree();             
            }

            tree.NodeDoubleClick += tree_NodeDoubleClick;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
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
            FindNode(tbxFilter.Text.Trim(), false);
        }

        /// <summary>
        /// Finds next occurence of the specified survey.
        /// </summary>
        protected void btnFindNext_Click(object sender, EventArgs e)
        {
            FindNode(tbxFilter.Text.Trim(), true);
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
            SetCheckedSurveys(SurveyService.OpenedSurveys.Select(x => x.SID.GetValueOrDefault()).ToArray(), true);
        }

        void tree_NodeDoubleClick(object sender, NodeDoubleClickEventArgs e)
        {
            if (NodeDoubleClick != null)
            {
                NodeDoubleClick(sender, e);
            }
        }

        #endregion

        #region Methods

        public void RefreshData()
        {
            InitTree();
        }        

        public List<SurveyInfo> GetSurveysByNode(string nodeKey)
        {
            var surveys = new List<SurveyInfo>();

            if (nodeKey == RootGroupKey)
            {
                surveys.AddRange(SurveyManager.GetSurveyList(User.Name, String.Empty));
            }
            else
            {
                var survey = new SurveyInfo(int.Parse(nodeKey));
                surveys.Add(survey);
            }

            return surveys;
        }

        public void SetCheckedSurveys(int[] surveyIds, bool checkRootNode)
        {
            tree.Nodes[0].CheckState = checkRootNode ? CheckBoxState.Checked : CheckBoxState.Unchecked;

            foreach (DataTreeNode node in tree.Nodes[0].Nodes)
            {
                node.CheckState = surveyIds != null && surveyIds.Any(x => x.ToString(CultureInfo.InvariantCulture) == node.Key) ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            }
        }

        public void UnselectAllSurveys()
        {
            tree.UnselectAllNodes();
        }

        public void CheckSelectedNodes()
        {
            foreach (var node in tree.SelectedNodes)
            {
                node.CheckState = CheckBoxState.Checked;
            }
        }        

        private void InitTree()
        {
            tree.Nodes.Clear();            

            var root = new DataTreeNode
            {
                Key = RootGroupKey,
                Text = Strings.AllSurveys,
                Expanded = true                
            };

            tree.Nodes.Add(root);

            LoadSurveys(root);            
        }        

        private void LoadSurveys(DataTreeNode root)
        {
            var surveys = SurveyManager.GetSurveyList(User.Name, Filter);

            surveys = surveys.OrderBy(x => chkSortMode.Checked ? x.ConfirmitID : x.Name).ToList();

            foreach (var surveyInfo in surveys)
            {                
                var ndSurvey = new DataTreeNode
                                   {
                                       Key = surveyInfo.Id.ToString(CultureInfo.InvariantCulture),                                       
                                       Text =
                                           string.Format("{0} ({1}) ({2})", surveyInfo.Name.Replace("\\", " "),
                                                         surveyInfo.ConfirmitID, surveyInfo.AssignedPersonCount),
                                       ToolTip =
                                           string.Format(Strings.TotalAssignedUsers, surveyInfo.AssignedPersonCount)
                                   };


                root.Nodes.Add(ndSurvey);
            }
        }        
             
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
     
        private void RegisterClientScripts()
        {
            string format = "if(event.keyCode == 13) {{document.getElementById('{0}').click(); return false;}}";
            tbxFilter.Attributes.Add("onkeydown", String.Format(format, btnFilter.ClientID));
        }     

        #endregion        
    }
}