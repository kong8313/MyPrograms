using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI;
using Confirmit.CATI.Supervisor.Core.Persons;
using System.Collections.Generic;
using Infragistics.Web.UI.NavigationControls;

namespace Confirmit.CATI.Supervisor.Controls
{  
	public partial class PersonsTreeWithAssignments : BaseWUC
	{
		#region Members

		private const string GroupPath = "0";
		private const string PersonPath = "1";

		private readonly List<int> m_ExpandedItems = new List<int>();

		private List<DataTreeNode> m_CheckedNodes;
        
        #endregion

		#region Properties
	
        /// <summary>
        /// SID of survey, tree is filtered by
        /// </summary>        
        public int SurveySID
        {
            get
            {
                return ViewState["SurveySID"] != null ? (int)ViewState["SurveySID"] : -1;
            }
            set
            {
                ViewState["SurveySID"] = value;
            }
        }

       /// <summary>
		/// Shows or hides checkboxes.
		/// </summary>
		public bool UseCheckBoxes
		{
			get { return tree.UseCheckBoxes; }
			set { tree.UseCheckBoxes = value; }
		}

        public string TreeClientId
        {
            get { return tree.ClientID; }
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
		/// Shows or hides person name filter.
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
			get { return ViewState["Filter"] != null ? (string)ViewState["Filter"] : String.Empty; }
			set { ViewState["Filter"] = value; }
		}

		/// <summary>
		/// Indicates, should empty groups be excluded from tree or not.
		/// </summary>
		public bool ExcludeEmptyGroups
		{
			get { return ViewState["ExcludeEmptyGroups"] != null ? (bool)ViewState["ExcludeEmptyGroups"] : false; }
			set { ViewState["ExcludeEmptyGroups"] = value; }
		}

		/// <summary>
		/// Gets or sets total number of nodes loaded into the tree.
		/// </summary>
		public int TotalNodesCount
		{
			get
			{
				return ViewState["TotalNodesCount"] != null ? (int)ViewState["TotalNodesCount"] : 0;
			}
			set
			{
				ViewState["TotalNodesCount"] = value;
			}
		}

		/// <summary>
		/// Indicates, is control in find mode or not.
		/// </summary>
		private bool IsInFindMode
		{
			get { return ViewState["IsInFindMode"] != null ? (bool)ViewState["IsInFindMode"] : false; }
			set { ViewState["IsInFindMode"] = value; }
		}

		/// <summary>
		/// Indicates if all nodes in tree are selected (as quick as possible).
		/// </summary>
		public bool AreAllNodesChecked
		{
			get
			{
				return (TotalNodesCount == CheckedNodes.Count);
			}
		}

		/// <summary>
		/// Gets checked nodes (here we just "cache" tree.CheckedNodes in class member to optimize perfomance).
		/// </summary>
		private List<DataTreeNode> CheckedNodes
		{
			get
			{
				if (m_CheckedNodes == null)
					m_CheckedNodes = tree.CheckedNodes;
				return m_CheckedNodes;
			}
		}

        /// <summary>
        /// List of persons, selected in the tree. It contains only selected
        /// items and doesn't contain sub-items of selected groups.
        /// </summary>
        public IEnumerable<PersonGroupInfo> SelectedPersons
        {
            get
            {
                var persons = new List<PersonGroupInfo>();

                foreach (var node in tree.CheckedNodes)
                {
                    int id;
                    if (int.TryParse(node.Key, out id))
                    {
                        persons.Add(new PersonGroupInfo(node.DataPath == GroupPath, id, node.Text));
                    }
                }
                return persons;
            }
        }

        /// <summary>
        /// return all persons for specified node (recursive)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public PersonGroupInfo GetPersonsByNode(string nodeKey, string nodePath)
        {
            int id;
            int.TryParse(nodeKey, out id);
            return new PersonGroupInfo(nodePath == GroupPath, id, "");
        }

        /// <summary>
        /// Indicates if all nodes should be checked by default.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public bool AllCheckedByDefault
        {
            get { return (bool)(ViewState["AllCheckedByDefault"] ?? false); }
            set { ViewState["AllCheckedByDefault"] = value; }
        }

	    private void CheckAll()
        {
            tree.CheckAllNodes();            
        }

		#endregion

        #region Events

        //public event NodeDroppedEventHandler NodeDropped;

        /// <summary>
        /// Occurs when tree node is double clicked.
        /// </summary>
        public event EventHandler<NodeDoubleClickEventArgs> NodeDoubleClick;

        public event EventHandler<NodeDroppedEventArgs> NodeDropped
        {
            add { tree.NodeDropped += value; }
            remove { tree.NodeDropped -= value; }
        }

        #endregion

		#region Lifecycle

        protected void Page_Init(object sender, EventArgs e)
        {   
            //must be set on Page_Init stage
            tree.NodePopulate += tree_NodePopulate;
            tree.NodeDoubleClick += tree_NodeDoubleClick;
        }        

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                InitTree();

                if (AllCheckedByDefault)
                {
                    CheckAll();
                }
            }            
	    }        

		protected void Page_PreRender()
		{
			RegisterClientScripts();		    
		}

		#endregion

		#region Event handlers

        /// <summary>
        /// Loads immediate children of node.
        /// </summary>
        void tree_NodePopulate(object sender, DataTreeNodeEventArgs e)
        {
            LoadChildren(e.Node);
        }

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

        void tree_NodeDoubleClick(object sender, NodeDoubleClickEventArgs e)
        {
            if (NodeDoubleClick != null)
            {
                NodeDoubleClick(sender, e);
            }
        }

	    #endregion

		#region Methods

        /// <summary>
        /// Unselects all persons.
        /// </summary>
        public void UnselectAllPersons()
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
		/// <summary>
		/// Tree initialization.
		/// Here we add single node to a tree (root persons group).
		/// </summary>
		private void InitTree()
		{
			//Clear all "cached" lists.
			m_CheckedNodes = null;
			IsInFindMode = false;
			//Clears tree.
			tree.Nodes.Clear();
			TotalNodesCount = 0;
			//Add root group.

            var node = new DataTreeNode
            {
                Key = PersonManager.GetCatiRootID().ToString(),
                DataPath = GroupPath,
                Text = Strings.Users,
                Expanded = true
            };
            
		    tree.Nodes.Add(node);
			TotalNodesCount++;

			LoadChildren(node);			
		}

	    protected void LoadChildren( DataTreeNode parent_node)
        {
            int id = int.Parse(parent_node.Key);

            var personsAndGroupsList = PersonManager.GetPersonsHierarchyLevel(id, SurveySID, tbxFilter.Text);

            var children = personsAndGroupsList.OrderByDescending(c => c.IsGroup).ThenBy(c => c.Name).ToList();

            foreach (PersonGroupInfo person_info in children)
            {
                var node = new DataTreeNode();

                if (person_info.IsGroup)
                {
                    node.Key = person_info.SID.ToString();
                    node.DataPath = GroupPath;

                    node.Text = string.Format("{0} ({1})", person_info.Name.Replace("\\", " "), person_info.TotalAssignedSurvey);
                    node.ToolTip = string.Format(string.Format(Strings.TotalAssignedSurvey, person_info.TotalAssignedSurvey));

                    node.IsEmptyParent = (person_info.MembersCount > 0);
                    node.ImageUrl = tree.NodeSettings.ParentNodeImageUrl;

                    if (m_ExpandedItems.IndexOf(int.Parse(node.Key)) != -1)
                        LoadChildren(node);
                }
                else
                {
                    node.Text = string.Format("{0} ({1})",
                                    person_info.Name.Replace("\\", " "),
                                    person_info.TotalAssignedSurvey);
                    node.ToolTip = string.Format(string.Format(Strings.TotalAssignedSurvey, person_info.TotalAssignedSurvey));

                    node.Key = person_info.SID.ToString();
                    node.DataPath = PersonPath;                    
                }

                parent_node.Nodes.Add(node);

            }
        }

		/// <summary>
		/// Refreshes data.
		/// </summary>
		public void RefreshData()
		{			
			SaveExpanded(); //Save expanded state of nodes to restore after data refresh.
			InitTree(); //Clear and initialize tree.
			RestoreExpanded(); //Save expanded states and loads expanded branches.		
		}

		/// <summary>
		/// Saves expanded states of tree nodes (to restore after data refresh).
		/// </summary>
		private void SaveExpanded()
		{
			m_ExpandedItems.Clear();
			foreach (DataTreeNode node in tree.Nodes)
				SaveExpandedRecursive(node);
		}

		/// <summary>
		/// Restores expanded states of tree nodes after data refresh.
		/// </summary>
		private void RestoreExpanded()
		{
			foreach (DataTreeNode node in tree.Nodes)
				RestoreExpandedRecursive(node);
		}

		/// <summary>
		/// Saves expanded state of child nodes recursive.
		/// </summary>
		private void SaveExpandedRecursive(DataTreeNode parent)
		{
			if (parent.Expanded)
                m_ExpandedItems.Add(int.Parse(parent.Key));
			foreach (DataTreeNode child in parent.Nodes)
				SaveExpandedRecursive(child);
		}

		/// <summary>
		/// Restores expanded state of child nodes recursive.
		/// Loads expanded branches.
		/// </summary>
		private void RestoreExpandedRecursive(DataTreeNode parent)
		{
			if (m_ExpandedItems.Contains(int.Parse(parent.Key)))
			{
				parent.Expanded = true;
				if (parent.Nodes.Count < 1)
					LoadChildren(parent);
			}
			foreach (DataTreeNode child in parent.Nodes)
			{
				RestoreExpandedRecursive(child);
			}
		}
				
		/// <summary>
		/// Hides subgroups.
		/// </summary>
		private void HideSubgroups()
		{
			IsInFindMode = true;
			var nodes = tree.Nodes[0].Nodes;
			foreach (DataTreeNode node in nodes)
			{
				if (node.DataPath == GroupPath)
					node.Visible = false;
			}
		}

		/// <summary>
		/// Register client scripts.
		/// </summary>
		private void RegisterClientScripts()
		{
			string format = "if(event.keyCode == 13) {{document.getElementById('{0}').click(); return false;}}";
			tbxFilter.Attributes.Add("onkeydown", String.Format(format, btnFilter.ClientID));            
		}

		#endregion        
    }
}