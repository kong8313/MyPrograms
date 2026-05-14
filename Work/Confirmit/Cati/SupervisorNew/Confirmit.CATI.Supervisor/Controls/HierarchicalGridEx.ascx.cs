using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Configuration;
using Confirmit.CATI.Supervisor.ServerControls;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Confirmit.CATI.Supervisor.Controls
{
    #region HierarchicalRowState - state of the hierarchical row (collapsed or expanded).
    /// <summary>
    /// State of the hierarchical row (collapsed or expanded).
    /// </summary>
    public enum HierarchicalRowState
    {
        Collapsed,
        Expanded
    }
    #endregion

    #region HierarchicalRowStateCollection - collection of the row states for the hierarchical grid.
    /// <summary>
    /// Collection of the row states for the hierarchical grid.
    /// </summary>
    [Serializable]
    public class HierarchicalRowStateCollection
    {
        private Dictionary<int, HierarchicalRowState> m_States;

        protected Dictionary<int, HierarchicalRowState> States
        {
            get
            {
                if (m_States == null)
                {
                    m_States = new Dictionary<int, HierarchicalRowState>();
                }
                return m_States;
            }
        }

        /// <summary>
        /// Gets or sets the state for the row of passed index.
        /// Note: if setting new state for some row, we  need to update grid's data bindings (RefreshData() method).
        /// </summary>
        /// <param name="index">Row index (rowIndex).</param>
        public HierarchicalRowState this[int index]
        {
            get
            {
                return States.ContainsKey(index) ? States[index] : HierarchicalRowState.Collapsed;
            }
            set
            {
                States[index] = value;
            }
        }

        /// <summary>
        /// "Clears" all states (i.e. set them to collapsed state for each row).
        /// </summary>
        public void Clear()
        {
            States.Clear();
        }
    }
    #endregion

    [ParseChildren(true)]
    [PersistChildren(false)]
    public partial class HierarchicalGridEx : System.Web.UI.UserControl
    {
        /// <summary>
        /// Occurs when grid needs to get data.
        /// </summary>
        public event GetPageDelegate GetPage;
        /// <summary>
        /// Occurs after data is bound to the grid.
        /// </summary>
        public event EventHandler DataBound;
        /// <summary>
        /// Occurs after the row is created in grid.
        /// </summary>
        public event GridViewRowEventHandler RowCreated;
        /// <summary>
        /// Occurs after data is bound to the row.
        /// </summary>
        public event GridViewRowEventHandler RowDataBound;

        public event GridViewRowEventHandler RowHeaderDataBound;
        /// <summary>
        /// Occurs before hierarchical row (subrow) is created.
        /// Here we can change hierarchical row template.
        /// Occurs only if row is expanded.
        /// </summary>
        public event GridViewRowEventHandler HierarchicalRowPreCreated;
        /// <summary>
        /// Occurs after hierarchical row (subrow) is created.
        /// Here we can access controls inside template.
        /// Occurs only if row is expanded.
        /// </summary>
        public event GridViewRowEventHandler HierarchicalRowCreated;
        /// <summary>
        /// Occurs after data is bound to hierarchical row (subrow).
        /// Here we can access controls inside template.
        /// Occurs only if row is expanded.
        /// </summary>
        public event GridViewRowEventHandler HierarchicalRowDataBound;
        /// <summary>
        /// Occurs when command is performed for grid row.
        /// </summary>
        public event GridViewCommandEventHandler RowCommand;

        public event EventHandler SelectedIndexChanged;

        private ITemplate m_HierarchicalRowTemplate;

        /// <summary>
        ///Gets or sets a value indicating whether bound fields are automatically created
        /// for each field in the data source.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public bool AutoGenerateColumns
        {
            get
            {
                return innerGrid.AutoGenerateColumns;
            }

            set
            {
                innerGrid.AutoGenerateColumns = value;
            }
        }

        /// <summary>
        /// Allows manipulating columns in runtime.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataControlFieldCollection Columns
        {
            get
            {
                return innerGrid.Columns;
            }
        }

        /// <summary>
        /// Gets DataKeys array.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataKeyArray DataKeys
        {
            get
            {
                return innerGrid.DataKeys;
            }
        }

        /// <summary>
        /// Gets DataKeyNmaes array.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] DataKeyNames
        {
            get
            {
                return innerGrid.DataKeyNames;
            }
            set
            {
                innerGrid.DataKeyNames = value;
            }
        }

        /// <summary>
        /// Gets or sets css class for the grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string CssClass
        {
            get
            {
                return innerGrid.CssClass;
            }
            set
            {
                innerGrid.CssClass = value;
            }
        }

        /// <summary>
        /// Enable or disable sorting for the grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public bool AllowSorting
        {
            get
            {
                return innerGrid.AllowSorting;
            }
            set
            {
                innerGrid.AllowSorting = value;
            }
        }

        /// <summary>
        /// Gets or sets grid lines for the grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public GridLines GridLines
        {
            get
            {
                return innerGrid.GridLines;
            }
            set
            {
                innerGrid.GridLines = value;
            }
        }

        /// <summary>
        /// Allows manipulating with grid header style.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle HeaderStyle
        {
            get
            {
                return innerGrid.HeaderStyle;
            }
        }

        /// <summary>
        /// Allows manipulating with grid row style.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle RowStyle
        {
            get
            {
                return innerGrid.RowStyle;
            }
        }
        /// <summary>
        /// Allows manipulating with grid alternating row style.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle AlternatingRowStyle
        {
            get
            {
                return innerGrid.AlternatingRowStyle;
            }
        }

        /// <summary>
        /// Allows manipulating with grid selected row style.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle SelectedRowStyle
        {
            get
            {
                return innerGrid.SelectedRowStyle;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public GridViewRowCollection Rows
        {
            get
            {
                return innerGrid.Rows;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public int SelectedIndex
        {
            get
            {
                return innerGrid.SelectedIndex;
            }
            set
            {
                innerGrid.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Hierarchical row template, which is shown after toggle button is pressed.
        /// </summary>
        [DefaultValue((string)null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(GridViewRow))]
        [Browsable(false)]
        public virtual ITemplate HierarchicalRowTemplate
        {
            get
            {
                return this.m_HierarchicalRowTemplate;
            }
            set
            {
                this.m_HierarchicalRowTemplate = value;
            }
        }

        /// <summary>
        /// Gets collection of the hierarchical row states for the grid (collapsed or expanded).
        /// </summary>
        [Browsable(false)]
        public HierarchicalRowStateCollection HierarchicalRowStates
        {
            get
            {
                HierarchicalRowStateCollection states = (HierarchicalRowStateCollection)ViewState["HierarchicalRowStates"];
                if (states == null)
                {
                    states = new HierarchicalRowStateCollection();
                    ViewState["HierarchicalRowStates"] = states;
                }
                return states;
            }
        }

        /// <summary>
        /// Image url for collapsed image button.
        /// </summary>
        [Browsable(true)]
        public string HierarchicalCollapsedImageUrl
        {
            get
            {
                object o = ViewState["HierarchicalCollapsedImageUrl"];
                return o != null ? (string)o : "~/svgimages/expand_more.svg";
            }
            set
            {
                ViewState["HierarchicalCollapsedImageUrl"] = value;
            }
        }

        /// <summary>
        /// Image url for expanded image button.
        /// </summary>
        [Browsable(true)]
        public string HierarchicalExpandedImageUrl
        {
            get
            {
                object o = ViewState["HierarchicalExpandedImageUrl"];
                return o != null ? (string)o : "~/svgimages/expand_less.svg";
            }
            set
            {
                ViewState["HierarchicalExpandedImageUrl"] = value;
            }
        }

        /// <summary>
        /// Defines if column with toggle buttons needs to be hidden.
        /// </summary>
        [Browsable(true)]
        public bool HideToggleColumn
        {
            get
            {
                object o = ViewState["HideToggleColumn"];
                return o != null ? (bool)o : false;
            }
            set
            {
                ViewState["HideToggleColumn"] = value;
            }
        }

        private bool _renderHierarchicalRows = true;
        [Browsable(true)]
        public bool RenderHierarchicalRows
        {
            get { return _renderHierarchicalRows; }
            set { _renderHierarchicalRows = value; }

        }

        /// <summary>
        /// Sort expression.
        /// </summary>
        public string SortExpression
        {
            get
            {
                object o = ViewState["SortExpression"];
                return o != null ? (string)o : String.Empty;
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }
        /// <summary>
        /// Sort order.
        /// </summary>
        public bool SortOrderAsc
        {
            get
            {
                object o = ViewState["SortAsc"];
                return o != null ? (bool)o : true;
            }
            set
            {
                ViewState["SortAsc"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!IsPostBack)
            {
                BindData();
            }
            innerGrid.Columns[0].Visible = !HideToggleColumn;
        }

        /// <summary>
        /// Refreshes data bound to the grid.
        /// </summary>
        public void RefreshData()
        {
            BindData();
        }

        /// <summary>
        /// Binds data to the grid.
        /// </summary>
        protected void BindData()
        {
            if (GetPage != null)
            {
                int totalCount = 0;
                innerGrid.DataSource = GetPage(out totalCount);
                innerGrid.DataBind();
            }

            // Raise event DataBound.
            OnDataBound(EventArgs.Empty);
        }

        /// <summary>
        /// Raises event DataBound.
        /// </summary>
        protected virtual void OnDataBound(EventArgs args)
        {
            if (DataBound != null)
            {
                DataBound(this, args);
            }
        }

        protected void OnSelectedChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Processes each row is created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            // Process headers.
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (!String.IsNullOrEmpty(SortExpression))
                {
                    // Find sorted column and add sorting image to the header.
                    for (int i = 0; i < innerGrid.Columns.Count; i++)
                    {
                        if (innerGrid.Columns[i].SortExpression == SortExpression)
                        {
                            var imSort = new SvgImage
                            {
                                ID = "imSort",
                                ImageName = SortOrderAsc
                                    ? "SortAsc"
                                    : "SortDesc"
                            };
                            e.Row.Cells[i].Controls.Add(imSort);
                        }
                    }
                }
            }
            // Process rows.
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (RowCreated != null)
                {
                    RowCreated(this, e);
                }

                if (RenderHierarchicalRows)
                {
                    // Create hierarchical row.
                    OnHierarchicalRowCreated(sender, e);
                }

                if (SelectedIndexChanged != null)
                {
                    e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(innerGrid, "Select$" + e.Row.RowIndex);
                }
            }
        }

        /// <summary>
        /// Creates hierarchical row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnHierarchicalRowCreated(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            // Add a cell and render a literal inside it to add new additional row.
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.Style[HtmlTextWriterStyle.Display] = "none";
            HierarchicalRowState state = HierarchicalRowStates[e.Row.RowIndex];
            cell.Controls.Add(
                new LiteralControl(
                    String.Format("</td></tr><tr style=\"display:{0};\"><td><td colspan=\"{1}\">",
                    state == HierarchicalRowState.Collapsed ? "none" : "table-row",
                    innerGrid.Columns.Count - 1// + 1
                    )
                    ));

            // Load subrow only if row is expanded.
            if (state == HierarchicalRowState.Expanded)
            {
                // Raise HierarchicalRowPreCreated event.
                if (HierarchicalRowPreCreated != null)
                {
                    HierarchicalRowPreCreated(sender, e);
                }

                // Instantiate subrow template.
                if (m_HierarchicalRowTemplate != null)
                {
                    m_HierarchicalRowTemplate.InstantiateIn(cell);
                }

                // Raise HierarchicalRowCreated event.
                if (HierarchicalRowCreated != null)
                {
                    HierarchicalRowCreated(sender, e);
                }
            }
        }

        /// <summary>
        /// Processes each row is bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (RowHeaderDataBound != null)
                {
                    RowHeaderDataBound(this, e);
                }
            }

            // Process rows.
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HierarchicalRowState toggleState = HierarchicalRowStates[e.Row.RowIndex];

                //// Set up toggle image button.
                //var ibToggle = (ServerControls.ImageButton)e.Row.FindControl("ibToggle");
                //ibToggle.CommandArgument = e.Row.RowIndex.ToString();
                //ibToggle.ImageName = toggleState == HierarchicalRowState.Collapsed
                //    ? "expand_more"
                //    : "expand_less";

                var ibToggle = (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("ibToggle");
                ibToggle.CommandArgument = e.Row.RowIndex.ToString();
                ibToggle.ImageUrl = toggleState == HierarchicalRowState.Collapsed
                    ? HierarchicalCollapsedImageUrl
                    : HierarchicalExpandedImageUrl;

                // Raise RowDataBound event.
                if (RowDataBound != null)
                {
                    RowDataBound(this, e);
                }

                // If row is in expanded state, bind hierarchical row (subrow).
                if (toggleState == HierarchicalRowState.Expanded)
                {
                    OnHierarchicalRowDataBound(sender, e);
                }
            }
        }

        /// <summary>
        /// Raises event of data bound for the hierarchical row (subrow).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnHierarchicalRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (m_HierarchicalRowTemplate != null && HierarchicalRowDataBound != null)
            {
                HierarchicalRowDataBound(this, e);
            }
        }

        /// <summary>
        /// Occurs when command is performed on innerGrid.
        /// Raises row command event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Process toggle image button click (i.e. expand or collapse row).
            if (e.CommandName == "toggle")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                HierarchicalRowState newState = HierarchicalRowStates[rowIndex] == HierarchicalRowState.Collapsed
                        ? HierarchicalRowState.Expanded
                        : HierarchicalRowState.Collapsed;
                HierarchicalRowStates[rowIndex] = newState;

                // Re-bind data.
                BindData();
            }

            // Raise RowCommand event.
            if (RowCommand != null)
            {
                RowCommand(this, e);
            }
        }

        /// <summary>
        /// Sorting event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            SortOrderAsc = (e.SortExpression == SortExpression)
                ? !SortOrderAsc
                : true;
            SortExpression = e.SortExpression;
            //Close all hierarchical rows (subrows).
            HierarchicalRowStates.Clear();
            // Bind data.
            BindData();
        }
    }
}