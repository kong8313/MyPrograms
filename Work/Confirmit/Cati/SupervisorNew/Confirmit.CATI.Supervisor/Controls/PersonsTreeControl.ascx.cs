using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI;
using Confirmit.CATI.Supervisor.Core.Persons;
using System.Collections.Generic;
using Infragistics.Web.UI.NavigationControls;

namespace Confirmit.CATI.Supervisor.Controls
{
	public partial class PersonsTreeControl : BaseWUC
	{
		#region Members
		private const string GroupPath = "0";
		private const string PersonPath = "1";

		private List<int> m_ExpandedItems = new List<int>();
		private List<int> m_CheckedItems = new List<int>();

		private List<PersonInfoItem> m_CheckedPersons;
		private List<DataTreeNode> m_CheckedNodes;
        
        #endregion

		#region Properties
		/// <summary>
		/// Id of the root group for the tree (persons root by default).
		/// </summary>
		public int RootId
		{
			get
			{
                if (ViewState["RootId"] == null)
                {                   
                    ViewState["RootId"] = PersonManager.GetCatiRootID();
                }
				return (int)ViewState["RootId"];
			}
			set
			{
				ViewState["RootId"] = value;
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
		/// Gets list of checked persons (including duplicates, according to perfomance improvement).
		/// </summary>
		public List<PersonInfoItem> CheckedPersons
		{
			get
			{
				if (m_CheckedPersons == null)
				{
					m_CheckedPersons = new List<PersonInfoItem>();

					foreach (DataTreeNode node in CheckedNodes)
					{
						//If it's person, add it.
						if (node.DataPath == PersonPath)
						{
							int id = int.Parse(node.Key);
							string name = node.Text;
							var item = new PersonInfoItem(id, name);

							m_CheckedPersons.Add(item);
						}
						//If it's group and its children are not loaded to the tree, get them from database.
                        //or if filter is applied all person from the selected group have to be selected.
						else if (node.Nodes.Count < 1 || Filter != String.Empty)
						{
                            AddChildrenRecursive(int.Parse(node.Key));
						}
					}
				}
				return m_CheckedPersons;
			}
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

		/// <summary>
		/// This method gets child persons for not loaded branches.
		/// </summary>
		/// <remarks>
		/// Filter must not be applied for childs if parent group has been selected.
		/// </remarks>
		private void AddChildrenRecursive(int parentSid)
		{
			var persons = PersonManager.GetPersonsLevel(parentSid, string.Empty);

            m_CheckedPersons.AddRange(persons.Where(person => !m_CheckedPersons.Contains(person)));
            
			var groups = PersonManager.GetPersonGroupsLevel(parentSid, string.Empty);

			foreach (var group in groups)
			{
				AddChildrenRecursive(group.Id);
			}
		}
        
        private void CheckAll()
        {
            tree.CheckAllNodes();            
        }

		#endregion

		#region Lifecycle

        protected void Page_Init(object sender, EventArgs e)
        {   
            //must be set on Page_Init stage
            tree.NodePopulate += tree_NodePopulate;
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

		/// <summary>
		/// Finds first occurence of the specified person.
		/// </summary>
		protected void btnFindFirst_Click(object sender, EventArgs e)
		{
			FindNode(tbxFind.Text.Trim(), false);
		}

		/// <summary>
		/// Finds next occurence of the specified person.
		/// </summary>
		protected void btnFindNext_Click(object sender, EventArgs e)
		{
            FindNode(tbxFind.Text.Trim(), (tree.SelectedNodes.Count > 0));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Tree initialization.
		/// Here we add single node to a tree (root persons group).
		/// </summary>
		private void InitTree()
		{
			//Clear all "cached" lists.
			m_CheckedPersons = null;
			m_CheckedNodes = null;
			IsInFindMode = false;
			//Clears tree.
			tree.Nodes.Clear();
			TotalNodesCount = 0;
			//Add root group.

		    var node = new DataTreeNode {Key = RootId.ToString(), DataPath = GroupPath, Text = Strings.Users,  Expanded = true};
            
		    tree.Nodes.Add(node);
			TotalNodesCount++;

			LoadChildren(node);			
		}

		/// <summary>
		/// Loads children of node.
		/// </summary>
		/// <param name="parent">Parent node.</param>
		private void LoadChildren(DataTreeNode parent)
		{
			//Get children.
			List<PersonGroupInfoItem> childGroups = PersonManager.GetPersonGroupsLevel(int.Parse(parent.Key), Filter);
            List<PersonInfoItem> childPersons = PersonManager.GetPersonsLevel(int.Parse(parent.Key), Filter);

			foreach (PersonGroupInfoItem group in childGroups)
			{
				if (ExcludeEmptyGroups && group.Count < 1)
					continue;

			    var node = new DataTreeNode
			                   {
			                       Key = group.Id.ToString(),
			                       DataPath = GroupPath,			                       
			                       Text = String.Format("{0} ({1})", group.Name, group.Count),
			                       ToolTip = String.Format(Strings.MembersCount, group.Count),
			                       IsEmptyParent = (group.Count > 0),
                                   ImageUrl = tree.NodeSettings.ParentNodeImageUrl
			                   };

			    parent.Nodes.Add(node);
				TotalNodesCount++;
				node.CheckState = parent.CheckState;
			}
			foreach (PersonInfoItem person in childPersons)
			{
				var node = new DataTreeNode();
				node.Key = person.Id.ToString();
				node.DataPath = PersonPath;
				node.Text = person.Name;
				parent.Nodes.Add(node);
				TotalNodesCount++;
                node.CheckState = parent.CheckState;
			}
		}

		/// <summary>
		/// Refreshes data.
		/// </summary>
		private void RefreshData()
		{
			SaveChecked(); //Save checked state of nodes to restore after data refresh.
			SaveExpanded(); //Save expanded state of nodes to restore after data refresh.
			InitTree(); //Clear and initialize tree.
			RestoreExpanded(); //Save expanded states and loads expanded branches.
			RestoreChecked(); //Restore expanded states.		    
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
		/// Saves checked states of tree nodes (to restore after data refresh).
		/// </summary>
		private void SaveChecked()
		{
			m_CheckedItems.Clear();
            foreach (DataTreeNode node in CheckedNodes)
                m_CheckedItems.Add(int.Parse(node.Key));
		}

		/// <summary>
		/// Restores checked states of tree nodes after data refresh.
		/// </summary>
		private void RestoreChecked()
		{
			foreach (DataTreeNode node in tree.Nodes)
			{
				RestoreCheckedRecursive(node);
			}
		}

		/// <summary>
		/// Restores checked states of child nodes recursive.
		/// </summary>
		private void RestoreCheckedRecursive(DataTreeNode parent)
		{
			if (m_CheckedItems.Contains(int.Parse(parent.Key)))
			{
				parent.CheckState = CheckBoxState.Checked;
			}
            foreach (DataTreeNode child in parent.Nodes)
			{
				RestoreCheckedRecursive(child);
			}
		}

		/// <summary>
		/// Finds node with specified text.
		/// </summary>
		private void FindNode(string name, bool findNext)
		{           
            //If tree is empty, then no processing is needed.
            if (tree.Nodes[0] == null || tree.Nodes[0].Nodes[0] == null)
                return;            
            
            //If search was never performed before, hide groups.
            if (!IsInFindMode)
                HideSubgroups();

            //Set start node according to find mode (first or next).
            DataTreeNode node;
            if (findNext)
            {                
                if (tree.SelectedNodes.Count>0 && tree.SelectedNodes[0].Level == 1)
                    node = tree.SelectedNodes[0].NextNode;
                else
                    node = tree.Nodes[0].Nodes[0];
            }
            else
            {
                node = tree.Nodes[0].Nodes[0];
            }

            tree.UnselectAllNodes();

            //Find.
            while (node != null)
            {
                if (node.DataPath != GroupPath && node.Text.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    node.Selected = true;
                    //tree.ActiveNode = node;
                    //tree.ScrollNodeIntoView(node);                                 
                    return;
                }
                node = node.NextNode;
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
			tbxFind.Attributes.Add("onkeydown", String.Format(format, btnFindFirst.ClientID));
		}

		#endregion
	}
}