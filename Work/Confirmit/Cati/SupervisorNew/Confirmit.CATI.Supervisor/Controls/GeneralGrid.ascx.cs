using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls.Grid;
using Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using Infragistics.Web.UI;
using Infragistics.Web.UI.GridControls;
using Infragistics.Web.UI.NavigationControls;
using DataMenuItem = Confirmit.CATI.Supervisor.ServerControls.DataMenuItem;
using InitializeRowEventHandler = Infragistics.Web.UI.GridControls.InitializeRowEventHandler;
using SortDirection = System.Web.UI.WebControls.SortDirection;

namespace Confirmit.CATI.Supervisor.Controls
{
    public delegate object GetPageDelegate(out int TotalCount);

    public partial class GeneralGrid : GridBaseControl, IPostBackEventHandler
    {
        private const string _selectedColumnKey = "Selected";
        private const string _emptyColumnKey = "EmptyColumn";

        [Serializable]
        public class Paging
        {
            private int _pageIndex = 1;

            public int PageIndex
            {
                get { return _pageIndex; }
                set { _pageIndex = value < 1 ? 1 : value; }
            }

            private int _pageSize = 100;
            public int PageSize
            {
                get { return _pageSize; }
                set { _pageSize = value; }
            }

            public int PageCount { get; set; }

            public void SetDataSourceSize(int totalCount)
            {
                PageCount = (int)Math.Ceiling((double)totalCount / PageSize);
            }
        }

        #region Fields

        private GridColumnCollection m_Columns = new GridColumnCollection();
        private Dictionary<string, Command> m_Commands = new Dictionary<string, Command>();
        private Hashtable m_disabledCommands = new Hashtable();
        private string m_GridName = "";
        private string m_dblClickCommand = "";
        private HashSet<string> _hiddenCommands = new HashSet<string>();
        private List<MenuItem> m_menuItems = new List<MenuItem>();
        private List<Control> m_toolbarItems = new List<Control>();
        private string m_PrimaryKeyColumn;
        private bool m_Bounded = false;
        private bool m_ClearSearchControlsState = false;
        private string customSearchParametersSessionKey;
        
        private SearchParametersProvider _searchParametersProvider = new SearchParametersProvider();
        private ISupervisorSettings _supervisorSettings;
        
        [StoreInViewState]
        protected Paging _paging = new Paging();

        #endregion

        #region Properties

        /// <summary>
        /// Flag allows to induce columns initialization on "Init" stage.
        /// </summary>
        /// <remarks>
        /// There is a problem with adding a new row using templates. 
        /// To workaround this problem column initialization must take place on Init stage.
        /// </remarks>
        public bool InitializeColumnsOnInitStage
        {
            get;
            set;
        }

        public bool PreserveSelectedState { get; set; }

        public string CssClass { get; set; } = "";

        /// <summary>
        /// List of Command objects associated with current grid
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Command> Commands
        {
            set
            {
                m_Commands = value.ToDictionary(x => x.Key);
            }
            get
            {
                return m_Commands.Values.ToList();
            }
        }

        public bool ClearSearchControlsState { get { return m_ClearSearchControlsState; } }

        public ControlCollection Templates { get { return dataGrid.Templates; } }

        /// <summary>
        /// Name of the command (from commands' list) that fire when user doubleclicks on some row
        /// </summary>
        public string OnDblClickCommand
        {
            get { return m_dblClickCommand; }
            set { m_dblClickCommand = value; }
        }

        public bool AutoGenerateColumns
        {
            get { return dataGrid.AutoGenerateColumns; }
            set { dataGrid.AutoGenerateColumns = value; }
        }

        public ToolbarLayout TopToolbarLayout
        {
            get { return topToolbar.ToolbarLayout; }
            set { topToolbar.ToolbarLayout = value; }
        }

        /// <summary>
        /// Message to display in grid when there is no data available.
        /// If this property is not fill empty grid is shown.
        /// </summary>
        public string NoDataMessage { get; set; }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataMenuItemCollection DataMenuItems
        {
            get { return gridContextMenu.Items; }
        }

        /// <summary>
        /// Items of the top toolbar
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Control> ToolbarItems
        {
            get { return m_toolbarItems; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Control> LeftToolbarItems
        {
            get { return _leftToolbarItems; }
        }

        /// <summary>
        /// Property gets collection of client side events of Infragistics grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public GridClientEvents ClientEvents
        {
            //TODO:! return type
            get { return dataGrid.ClientEvents; }
        }

        public string ColumnResizedClientEvent
        {
            get { return dataGrid.Behaviors.ColumnResizing.ColumnResizingClientEvents.ColumnResized; }
            set { dataGrid.Behaviors.ColumnResizing.ColumnResizingClientEvents.ColumnResized = value; }
        }

        /// <summary>
        /// Return client grid id, use this name for work with grid on client side
        /// </summary>
        /// because of this grid is container for infragGrid, id of Grid on client side will be as follow
        public string GridClientId
        {
            get
            {
                return dataGrid.ClientID;
            }
        }

        [DefaultValue(null)]
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Behaviors Behaviors
        {
            get { return dataGrid.Behaviors; }
        }

        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public string DataKeyFields
        {
            get { return dataGrid.DataKeyFields; }
            set { dataGrid.DataKeyFields = value; }
        }

        public event InitializeRowEventHandler InitializeRow
        {
            add { dataGrid.InitializeRow += value; }
            remove { dataGrid.InitializeRow -= value; }
        }

        /// <summary>
        /// Defines, whever or not refresh button is hidden (false by default)
        /// </summary>
        public bool HideRefreshButton { get; set; }

        public bool HideResetButton { get; set; }

        public bool HideSelectedColumn { get; set; }

        public bool ShowLastEmptyColumn { get; set; }

        public bool IncludeGridName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the inner grid with the data should be hidden.
        /// </summary>
        public bool HideContent
        {
            get
            {
                return !gridHolder.Visible;
            }
            set
            {
                gridHolder.Visible = !value;
            }
        }

        /// <summary>
        /// Gets/sets property indicated will grid have checkbox
        /// for selection all rows on page
        /// </summary>
        public bool HasMultySelectionCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether grid pager should be hidden.
        /// </summary>
        /// <value><c>true</c> if grid pager should be hidden; otherwise, <c>false</c>.</value>
        protected bool HidePager { get; set; }

        /// <summary>
        /// Sets a value indicating whether paging is enabled for the grid.
        /// </summary>
        /// <value><c>true</c> if paging is enabled for the grid; otherwise, <c>false</c>.</value>
        public bool EnablePaging
        {
            set
            {
                HidePager = !value;
                if (!value)
                {
                    PageSize = Int32.MaxValue;
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether paging is enabled for the grid.
        /// </summary>
        /// <value><c>true</c> if paging is enabled for the grid; otherwise, <c>false</c>.</value>
        public bool KeepSelection
        {
            get;
            set;
        }

        /// <summary>
        /// If true, keeps pages buttons always enabled
        /// Used to make grid works correctly when being inserted in WebAsyncRefreshPanel
        /// false, by default
        /// </summary>
        [StoreInViewState]
        public bool SimplifiedPagerMode;

        /// <summary>
        /// A flag indicating the visibility state of control during the last request execution.
        /// </summary>
        /// <remarks>Currently it is stored in ControlState, because ViewState may be not saved and loaded for invisible control</remarks>
        /// <seealso cref="http://stackoverflow.com/questions/3504254/asp-net-control-life-cycle-after-load-view-state/3504567#3504567"/>
        private bool _previouslyWasVisible;

        /// <summary>
        /// Gets a value indicating whether we should use search parameters stored in Session even if it is a postback.
        /// </summary>
        private bool ForceSearchParametersFromSession
        {
            get
            {
                // If previously control was hidden and now it is visible - values of searchable headers
                // are not contained in request parameters, so we need to get them from Session
                return !_previouslyWasVisible && Visible;
            }
        }

        public bool HideToolBar
        {
            get { return !topToolbarRow.Visible; }
            set { topToolbarRow.Visible = !value; }
        }

        public bool EnableSorting { get; set; }

        public GridColumnCollection Columns
        {
            get { return m_Columns; }
            set { m_Columns = value; }
        }

        public ControlDataField SelectionColumn
        {
            get { return dataGrid.Columns.FromKey(_selectedColumnKey); }
        }

        /// <summary>
        /// Placeholder to put controls which will be shown alongside with a grid
        /// Usually is used with some toggle control to display either grid or some alternative control
        /// Example: ShiftsNewControl
        /// </summary>
        public List<Control> AlternativeControls { get; set; } = new List<Control>();

        public int PageSize
        {
            get
            {
                return _paging.PageSize;
            }
            set
            {
                _paging.PageSize = value;
            }
        }

        public int PageIndex
        {
            set { _paging.PageIndex = value; }
            get { return _paging.PageIndex; }
        }

        /// <summary>
        /// Gets paging arguments.
        /// </summary>
        public PagingArgs PageArguments
        {
            get
            {
                return new PagingArgs(PageIndex, PageSize, SortedColumnKey, SortIndicatorAsc, SearchParameterCollection);
            }
        }

        /// <summary>
        /// Gets/sets sort direction for column 
        /// with name stored in SortedColumnName
        /// </summary>
        [StoreInViewState]
        public SortDirection SortIndicator;

        /// <summary>
        /// Gets/sets name of column that used for data sorting
        /// </summary>
        public string SortedColumnName
        {
            get
            {
                string columnName = (string)ViewState["SortedColumnName"];

                if (string.IsNullOrEmpty(columnName))
                {
                    if (dataGrid.Columns.OfType<ISortableField>().Any())
                    {
                        columnName = dataGrid.Columns.OfType<ISortableField>().First().DataFieldName;
                    }
                    else if (m_Columns.OfType<ISortableField>().Any())
                    {
                        columnName = m_Columns.OfType<ISortableField>().First().DataFieldName;
                    }
                    else
                    {
                        // If there is no valid columns to get column name.
                        columnName = string.Empty;
                    }
                }

                return columnName;
            }
            set
            {
                ViewState["SortedColumnName"] = value;
            }
        }

        /// <summary>
        /// Gets key for sorted column 
        /// </summary>
        public string SortedColumnKey
        {
            get
            {
                if (dataGrid.Columns.OfType<ISortableField>().Any(column => column.DataFieldName == SortedColumnName) == false)
                {
                    SortedColumnName = String.Empty; // Reset to default if column has been removed.
                }

                var gridColumn = dataGrid.Columns.OfType<ISortableField>().FirstOrDefault(column => column.DataFieldName == SortedColumnName);

                return gridColumn != null ? gridColumn.Key : "";
            }
        }

        /// <summary>
        /// Gets sort direction
        /// </summary>
        public bool SortIndicatorAsc
        {
            get
            {
                switch (SortIndicator)
                {
                    case SortDirection.Ascending:
                        return true;
                    case SortDirection.Descending:
                        return false;
                }

                return true;
            }
        }

        public string PrimaryKeyColumn
        {
            set { m_PrimaryKeyColumn = value; }
            get { return m_PrimaryKeyColumn; }
        }

        /// <summary>
        /// Gets the keys of the selected rows converted to the type specified by the type parameter.
        /// </summary>
        /// <typeparam name="T">Type to convert keys.</typeparam>
        /// <returns>The list of keys of the selected rows.</returns>
        public List<T> GetSelectedKeys<T>()
        {
            return SelectedKeys.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();
        }

        /// <summary>
        /// Gets the keys of the selected rows converted to integer.
        /// </summary>
        public List<int> SelectedKeysInt
        {
            get
            {
                return GetSelectedKeys<int>();
            }
        }

        /// <summary>
        /// Returns keys of selected rows. If there are no checked rows, returns key of highlighted row
        /// </summary>        
        public string[] SelectedKeys
        {
            get
            {
                if (string.IsNullOrEmpty(PrimaryKeyColumn))
                {
                    return new string[0];
                }

                if (Columns.FromKey(PrimaryKeyColumn) == null)
                {
                    throw new InternalErrorException(
                        string.Format("Column with Key = '{0}' not exists in grid '{1}'", PrimaryKeyColumn, ID));
                }

                if (CheckedKeys.Length > 0)
                {
                    return hSelected.Value.Split(',');
                }

                if (string.IsNullOrEmpty(hHighlighted.Value) == false)
                {
                    return HighlightedKey.CreateArray();
                }

                return new string[0];
            }

            set
            {
                hSelected.Value = String.Join(",", value);
            }
        }

        public string HighlightedKey
        {
            get
            {
                return hHighlighted.Value;
            }
        }

        /// <summary>
        /// Returns keys only of checked rows
        /// </summary>
        public string[] CheckedKeys
        {
            get
            {
                if (hSelected.Value.Length > 0)
                {
                    return hSelected.Value.Split(',');
                }

                return new string[0];
            }
        }

        public int TotalCount
        {
            get
            {
                return (int)(ViewState["TotalCount"] ?? 0);
            }
            set
            {
                ViewState["TotalCount"] = value;
                hTotalCount.Value = value.ToString();
            }
        }

        public string ExtraStatusBarText { get; set; }

        public bool DisableAutoBind { get; set; }

        /// <summary>
        /// If it is false - top toolbar won't have a top and left border. True by default. Should be set to false if grid is located near frame borders to avoid too thick borders.
        /// </summary>
        [Obsolete]
        public bool ShowFullToolbarBorders { get; set; }

        public bool IsBound
        {
            get { return !DisableAutoBind || ViewState["RefreshPressed"] != null; }
        }

        public string GridName
        {
            get { return m_GridName; }
            set { m_GridName = value; }
        }

        public string TopTitle { get; set; }

        /// <summary>
        /// Help links for title. Visible only if TopTitle is used
        /// </summary>
        public List<HelpLink> HelpLinks { get; set; }
        
        /// <summary>
        /// Gets/sets width for Grid name label
        /// </summary>
        /// <remarks>
        /// Actually used for correct formating of toolbar.         
        /// </remarks>
        public Unit GridNameWidth
        {
            get;
            set;
        }

        public RightToolbarButtonsConfiguration RightToolbarButtons { get; set; }

        /// <summary>
        /// Gets search parameter collection
        /// </summary>
        public SearchParameterCollection SearchParameterCollection
        {
            get
            {
                // If some filtered columns are hidden - we should skip corresponding search parameters, so return only parameters for visible columns.
                var visibleSearchColumnNames = m_Columns.Where(z => z.Hidden == false).OfType<ISearchableField>().Select(y => y.SearchColumnName);

                return new SearchParameterCollection(GetHeaderState().Where(x => visibleSearchColumnNames.Contains(x.ColumnName)));
            }
        }

        /// <summary>
        /// Hint text
        /// </summary>
        public string HintText
        {
            get
            {
                return gridHint.Text;
            }
            set
            {
                gridHint.Text = value;
            }
        }

        public HintType HintType
        {
            get => gridHint.HintType;
            set => gridHint.HintType = value;
        }

        public string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        public bool MakeMarginForExpanCollapseButton { get; set; } = false;

        public string ToolbarCssClass { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Event that fires up when grid wants to refresh it's datasource
        /// </summary>
        public GetPageDelegate GetPage;

        /// <summary>
        /// DatePicker controls that are used in searchable headers. We need to keep them here because we send their IDs to the client.
        /// </summary>
        private readonly List<Control> _dateHeaderControls = new List<Control>();

        private List<Control> _leftToolbarItems = new List<Control>();

        /// <summary>
        /// Event that fires up when user clicks on Refresh button on the grid
        /// </summary>
        public event EventHandler Refresh;

        /// <summary>
        /// Event that fires up when user clicks on Reset button on the grid
        /// </summary>
        public event EventHandler Reset;

        public event RowAddingHandler RowAdding
        {
            add
            {
                dataGrid.RowAdding += value;

            }
            remove
            {
                dataGrid.RowAdding -= value;

            }
        }

        public event RowUpdatingHandler RowUpdating
        {
            add
            {
                dataGrid.RowUpdating += value;

            }
            remove
            {
                dataGrid.RowUpdating -= value;

            }
        }

        #endregion

        #region Constructors

        public GeneralGrid()
        {
            EnableSorting = true;
            HasMultySelectionCheckBox = true;
            IncludeGridName = true;
            GridNameWidth = new Unit("50%");
            ShowFullToolbarBorders = true;
            NoDataMessage = "No items available";
            _supervisorSettings = ServiceLocator.Resolve<ISupervisorSettings>();
        }
        #endregion

        protected override object SaveControlState()
        {
            return new Pair(base.SaveControlState(), Visible);
        }

        protected override void LoadControlState(object savedState)
        {
            base.LoadControlState(((Pair)savedState).First);
            _previouslyWasVisible = (bool)((Pair)savedState).Second;
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.IndexOf("__sort") == 0)
            {
                SortData(null);
            }
            else if (eventArgument.IndexOf("__command_") == 0)
            {
                EventHandler eh =
                    m_Commands[eventArgument.Replace("__command_", "")].ServerClickEventHandler;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
            else //this block is needed for server event raising, after modal dialog is closed with 'OK' result
            {
                if (m_Commands.ContainsKey(eventArgument))
                {
                    Command command = m_Commands[eventArgument];
                    if (command.ServerClickEventHandler != null)
                    {
                        EventHandler eh = command.ServerClickEventHandler;
                        if (eh != null)
                            eh(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void SortData(SortDirection? direction)
        {
            var column = dataGrid.Columns.FromKey(hSortColumnKey.Value) as ISortableField;

            if (column != null)
            {
                if (column.DataFieldName == SortedColumnName)
                {
                    SortIndicator = (SortIndicator == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
                }
                else
                {
                    SortedColumnName = column.DataFieldName;
                    SortIndicator = SortDirection.Descending;
                }

                if (direction != null)
                {
                    SortIndicator = (direction.Value == SortDirection.Ascending) ? SortDirection.Ascending : SortDirection.Descending;
                }

                BindData();
            }
        }

        private void SetSortedIndicator()
        {
            foreach (var column in dataGrid.Columns.OfType<ISortableField>())
            {
                if (column.DataFieldName == SortedColumnName)
                {
                    column.SortIndicator = SortIndicator;
                }
                else
                {
                    column.SortIndicator = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns command by name
        /// </summary>
        /// <param name="key">The key.</param>
        public Command GetCommand(string key)
        {
            return m_Commands[key];
        }

        public void ClearSelectedKeys()
        {
            hSelected.Value = "";
        }

        public void DropBindedState()
        {
            ViewState["RefreshPressed"] = null;
            dataGrid.Rows.Clear();
            m_Bounded = false;
        }

        public void BindData()
        {
            if (m_Bounded)
            {
                System.Diagnostics.Trace.TraceWarning("Double data bind is called for the grid {0}", UniqueID);
            }

            m_Bounded = true;

            if (DisableAutoBind && ViewState["RefreshPressed"] == null)
            {
                return;
            }

            SetSortedIndicator();

            if (GetPage != null)
            {
                int tc = 0;
                object pageRecords = GetPage(out tc);

                TotalCount = tc;

                _paging.SetDataSourceSize(tc);

                if (_paging.PageIndex != 1 && _paging.PageIndex > _paging.PageCount)
                {
                    _paging.PageIndex = _paging.PageCount;
                    pageRecords = GetPage(out tc);
                }

                dataGrid.Rows.Clear();
                dataGrid.DataSource = pageRecords;
                dataGrid.DataBind();
            }
        }

        /// <summary>
        /// Binds new data to the grid
        /// </summary>
        public void RefreshData()
        {
            RefreshData(true);
        }

        /// <summary>
        /// Binds new data to the grid
        /// </summary>
        /// <param name="dropPaging">Move to the page N1</param>
        public void RefreshData(bool dropPaging)
        {
            if(!(_supervisorSettings.TablesPreserveSelectionState && PreserveSelectedState) || m_ClearSearchControlsState)
                ClearSelectedKeys();
            ViewState["RefreshPressed"] = true;
            if (dropPaging)
                PageIndex = 1;
            BindData();
        }

        /// <summary>
        /// Reinitializes the columns in the grid.
        /// </summary>
        public void RefreshColumns()
        {
            InitColumns();
        }

        /// <summary>
        /// Clears state of searching controls
        /// </summary>
        public void RefreshSearchControls()
        {
            m_ClearSearchControlsState = true;
        }

        public void DisableCommand(string command_key)
        {
            m_disabledCommands[command_key] = true;
        }

        public void EnableCommand(string command_key)
        {
            m_disabledCommands[command_key] = null;
        }

        public void EnableCommand(string command_key, bool enable)
        {
            if (enable)
                m_disabledCommands[command_key] = null;
            else
                m_disabledCommands[command_key] = true;
        }

        /// <summary>
        /// Set Toolbar item and context menu item for specified command to invisible 
        /// </summary>
        /// <param name="key">Command key</param>
        public void HideCommand(string key)
        {
            _hiddenCommands.Add(key);
        }

        /// <summary>
        /// Enable or disable commands depends of enabled param
        /// </summary>
        /// <param name="key">key of command setted in the grid</param>
        /// <param name="hide"> bool value </param>
        public void HideCommand(string key, bool hide)
        {
            if (hide)
            {
                _hiddenCommands.Add(key);
            }
            else
            {
                _hiddenCommands.Remove(key);
            }
        }

        public override string ClientGetCurrentRow()
        {
            return ClientControllerName + ".GetSelectedRow()";
        }

        public string ClientGetIsRowsSelected()
        {
            return String.Format("document.getElementById( '{0}' ).value != ''", hSelected.ClientID);
        }

        /// <summary>
        /// Returns string that can be used to get selected rows on client side
        /// </summary>        
        public override string ClientGetSelectedRows()
        {
            return String.Format("document.getElementById( '{0}' ).value", hSelected.ClientID);
        }

        /// <summary>
        /// Retutns decimal separator key code in order to user culture.
        /// </summary>
        private int GetDecimalSeparatorKeyCode()
        {
            var decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // key      code
            // ','      44
            // '.'      46
            return decimalSeparator == "," ? 44 : 46;
        }

        private void InitColumns()
        {
            // HACK: if we just call Columns.Clear() here - we have problems with header temlates. Removing columns one by one works fine :)
            for (int i = dataGrid.Columns.Count - 1; i >= 0; i--)
            {
                dataGrid.Columns.RemoveAt(i);
            }

            if (!HideSelectedColumn)
            {
                var selectedColumn = new TemplateDataField { Key = _selectedColumnKey, Width = Unit.Pixel(21) };
                selectedColumn.HeaderTemplate = new SelectionColumnHeaderTemplate(m_Columns.HasSearchColumn(), ClientControllerName);
                selectedColumn.ItemTemplate = new SelectionColumnTemplate();
                dataGrid.Columns.Add(selectedColumn);
            }


            foreach (var column in m_Columns)
            {
                dataGrid.Columns.Add(column);
                if (column is ISearchableField || column is ISortableField)
                {
                    AddHeaderTemplate(column);
                }
            }

            if (ShowLastEmptyColumn)
            {
                var emptyColumn = new TemplateDataField
                {
                    Key = _emptyColumnKey,
                    Width = Unit.Percentage(100),
                    Header = { Text = "" }
                };
                dataGrid.Columns.Add(emptyColumn);
            }
        }

        public void SetVisibleIndexForEmptyColumn(int visibleIndex)
        {
            dataGrid.Columns.FromKey(_emptyColumnKey).VisibleIndex = visibleIndex;
        }

        /// <summary>
        /// Gets column with added header template
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private void AddHeaderTemplate(GridField column)
        {
            //tc.CopyFrom(column);

            //tc.Header.Style.Height = m_TemplatedHeaderHeight;

            string defaultValue = null;
            SearchOperator defaultOperator = SearchOperator.Equal;

            if (column is ISearchableField)
            {
                defaultValue = (column as ISearchableField).SearchDefaultValue;
                defaultOperator = (column as ISearchableField).SearchDefaultOperator;

                if (m_ClearSearchControlsState == false)
                {
                    if (!IsPostBack || ForceSearchParametersFromSession)
                    {
                        var searchParameters = Session[GetSearchParametersSessionKey()] as SearchParameterCollection;
                        if (searchParameters != null)
                        {
                            // We have previously saved search parameters in session - it means that all default values
                            // for columns are saved in session and we do not need to get them from columns.
                            var searchParameter =
                                searchParameters.FirstOrDefault(x => x.ColumnName == (column as ISearchableField).SearchColumnName);
                            if (searchParameter != null)
                            {
                                defaultValue = searchParameter.Value.ToString();
                                defaultOperator = searchParameter.Operator;
                            }
                            else
                            {
                                // Search parameter for current column was empty - so we should clean default value
                                // and should not use default value from column as it may be cleaned by user.
                                defaultValue = null;
                                defaultOperator = SearchOperator.Equal;
                            }
                        }
                    }
                }
            }

            var templateId = column.Key + "Template";
            var settings = new HeaderTemplateSettings
            {
                GridClientController = ClientControllerName,
                SortColumnKeyProvider = () => SortedColumnKey,
                SortDirectionProvider = () => SortIndicator,
                HasSearchControls = m_Columns.HasSearchColumn(),
                IsSortable = column is ISortableField && (column as ISortableField).EnableSorting && EnableSorting
            };

            var headerTemplate = new FieldHeaderTemplateFactory().Create(column, settings, defaultValue, defaultOperator);
            dataGrid.Templates.Add(new ItemTemplate { TemplateID = templateId, Template = headerTemplate });
            
            if (headerTemplate is IRequiresPreInitialization)
            {
                ((IRequiresPreInitialization)headerTemplate).PreInitialize(menuPlaceholder);
            }
            column.Header.TemplateId = templateId;

        }

        private void WriteExtraBarText()
        {
            if (string.IsNullOrWhiteSpace(ExtraStatusBarText))
            {
                extraInfoDiv.Visible = false;
            }
            else
            {
                extraInfoDiv.Visible = true;
                lblExtraInfo.Text = HttpUtility.HtmlEncode(ExtraStatusBarText);
            }
        }

        private void WriteCount()
        {
            lblRecordCount.Text = HideSelectedColumn
                                      ? string.Format("Total : {0}", TotalCount)
                                      : string.Format("Total : {0}                     Selected : {1}",
                                                      TotalCount, CheckedKeys.Length);
        }

        private void InitToolbar()
        {
            topToolbar.MenuCssClass = ToolbarCssClass;

            int i = -1;
            ToolbarCommandButton btn;
            foreach (var tbitem in m_toolbarItems)
            {
                if (tbitem is ToolbarStdBlock)
                {
                    i = m_toolbarItems.IndexOf(tbitem);
                    break;
                }
            }
            if (i != -1)
            {
                m_toolbarItems.RemoveAt(i);
            }
            else
            {
                i = 0;
            }

            foreach (var item in m_toolbarItems)
            {
                if (item is XpMenuItem menuItem && menuItem.ButtonType == XpMenuItemType.Separator)
                {
                    continue;
                }

                if (item is ToolbarCommandButton toolbarButton)
                {
                    var command = m_Commands[toolbarButton.Key];
                    toolbarButton.Text = !string.IsNullOrEmpty(command.Caption) ? GetResString(command.Caption) : "";
                }
            }

            if (!HideRefreshButton)
            {
                btn = new ToolbarCommandButton { Key = "Refresh" };
                m_toolbarItems.Insert(i, btn);
                i++;
            }

            if (!HideResetButton)
            {
                bool doesResetMakeSense = !HideSelectedColumn || m_Columns.HasSearchColumn();

                if (doesResetMakeSense)
                {
                    btn = new ToolbarCommandButton { Key = "Reset" };
                    m_toolbarItems.Insert(i, btn);
                    i++;
                }
            }

            AddToolbarControls(topToolbar.RightMenuItems, m_toolbarItems);
            AddToolbarControls(topToolbar.LeftMenuItems, _leftToolbarItems);
        }

        private void UpdateRightToolbarOptionalButtons()
        {
            if (RightToolbarButtons != RightToolbarButtonsConfiguration.None)
            {
                topToolbar.RightMenuItems.Add(new XpMenuItem { ButtonType = XpMenuItemType.Separator });

                if (RightToolbarButtons == RightToolbarButtonsConfiguration.CloseWindow)
                {
                    var btn = new ToolbarCommandButton { Key = "CloseWindow" };
                    topToolbar.AddCommandButton(btn, m_Commands[btn.Key], m_disabledCommands[btn.Key] == null,
                                                this);
                }
            }
        }

        private void UpdateToolbarButtonsAccessibility()
        {
            UpdateButtonsAccessibility(topToolbar.RightMenuItems);
            UpdateButtonsAccessibility(topToolbar.LeftMenuItems);
        }

        private void UpdateButtonsAccessibility(XpMenuItemCollection toolbarItemsCollection)
        {
            foreach (var button in toolbarItemsCollection.OfType<ToolbarCommandButton>())
            {
                button.Enabled = (m_disabledCommands[button.Key] == null);
            }
        }

        private void AddToolbarControls(XpMenuItemCollection toolbarItemsCollection, IEnumerable<Control> toolbarItems)
        {
            toolbarItemsCollection.Clear();

            foreach (Control tbitem in toolbarItems)
            {
                var button = tbitem as ToolbarCommandButton;
                if (button != null)
                {
                    topToolbar.AddCommandButton(button, m_Commands[button.Key], m_disabledCommands[button.Key] == null, this, toolbarItemsCollection);
                    continue;
                }

                var item = tbitem as XpMenuItem;
                if (item != null)
                {
                    toolbarItemsCollection.Add(item);
                    continue;
                }

                var wc = tbitem as WebControl;
                if (wc != null)
                {
                    var genericItem = new XpMenuItem { ButtonType = XpMenuItemType.Generic };
                    genericItem.Controls.Add(wc);
                    toolbarItemsCollection.Add(genericItem);
                }
            }
        }

        private void SetupGridLabel()
        {
            if (IncludeGridName)
            {
                topToolbar.LeftLabel = string.IsNullOrEmpty(m_GridName)
                                           ? String.Empty
                                           : GetResString(m_GridName);
            }

            if (string.IsNullOrWhiteSpace(TopTitle))
            {
                trTopTitle.Visible = false;
            }
            else
            {
                topTitle.Text = HttpUtility.HtmlEncode(TopTitle);
            }

            if (HelpLinks == null || !HelpLinks.Any())
            {
                links.Visible = false;
            }
            else
            {
                foreach (var helpLink in HelpLinks)
                {
                    var link = new HyperLink();
                    link.NavigateUrl = helpLink.Url;
                    link.Target = "_blank";
                    link.Text = helpLink.Text + "<img src=\"../SvgImages/open_in_new.svg\" alt=\"icon\" class=\"general-grid-control__header-links__icon\" />";
                    links.Controls.Add(link);
                }
            }
        }

        private void HideCommands()
        {
            foreach (var btn in DataMenuItems.OfType<DataMenuItem>())
            {
                btn.Visible = !_hiddenCommands.Contains(btn.Key);
            }

            foreach (var btn in topToolbar.RightMenuItems.OfType<ToolbarCommandButton>())
            {
                btn.Visible = !_hiddenCommands.Contains(btn.Key);
            }

            foreach (var btn in topToolbar.LeftMenuItems.OfType<ToolbarCommandButton>())
            {
                btn.Visible = !_hiddenCommands.Contains(btn.Key);
            }
        }

        private int GetPageIndexInputWidth(int pageIndex)
        {
            return (pageIndex.ToString().Length + 1) * 7;
        }

        private void PreparePager()
        {
            if (!HidePager)
            {
                lblPageCount.Text = _paging.PageCount.ToString(CultureInfo.InvariantCulture);

                wnePageIndex.MaxValue = _paging.PageCount;
                wnePageIndex.ValueInt = _paging.PageIndex;
                wnePageIndex.Width = GetPageIndexInputWidth(_paging.PageIndex);

                if (_paging.PageCount <= 1)
                {
                    EnablePagerButtons(false, false, false, false, false);
                }
                else if (_paging.PageIndex == _paging.PageCount)
                {
                    EnablePagerButtons(true, true, false, false, true);
                }
                else if (_paging.PageIndex == 1)
                {
                    EnablePagerButtons(false, false, true, true, true);
                }
                else
                {
                    EnablePagerButtons(true, true, true, true, true);
                }

                if ((dataGrid.DataSource is ICollection) && (dataGrid.DataSource as ICollection).Count < _paging.PageSize)
                {
                    btnNextPage.Enabled = false;
                    btnBottomPage.Enabled = false;
                }
            }
            else
            {
                rightMenuDiv.Style["display"] = "none";
            }
        }

        private void EnablePagerButtons(bool first, bool prev, bool next, bool last, bool goTo)
        {
            btnTopPage.Enabled = first;
            btnPrevPage.Enabled = prev;
            btnNextPage.Enabled = next;
            btnBottomPage.Enabled = last;
        }

        /// <summary>
        /// Returns toolbar item by key.
        /// </summary>
        public ToolbarCommandButton GetToolbarItemByKey(string key)
        {
            return ToolbarItems.OfType<ToolbarCommandButton>().FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Returns post back reference that raised validation before post back
        /// </summary>
        private string GetPostBackEventReference(Control control, string argument)
        {
            string reference = Page.ClientScript.GetPostBackEventReference(
                                 new PostBackOptions(control, argument, "", false, false, true, true, true, ""));

            reference = Regex.Replace(reference, "\"", "'");

            return reference;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            foreach (Control c in AlternativeControls)
            {
                phAlternativeControls.Controls.Add(c);
            }

            InitToolbarDefaultButtons();
            InitToolbar();
            

            AutoGenerateColumns = false;

            PageHelper.RegisterClientLibrary("client/GeneralGrid.js");

            Page.RegisterRequiresControlState(this);

            dataGrid.InitializeRow += Grid_InitializeRow;

            if (InitializeColumnsOnInitStage)
                InitColumns();
        }

        private void InitToolbarDefaultButtons()
        {
            m_Commands.Add("Refresh", new Command("Refresh", "Refresh", "refresh", RefreshHandler));
            m_Commands.Add(
                "Reset",
                new Command(
                    "Reset",
                    "Reset",
                    "reset",
                    ResetHandler));

            var logoffCommand = new Command("Logoff", "LogoffButtonTooltip", string.Empty, "LogoffWithConfirmation();");
            m_Commands.Add("Logoff", logoffCommand);

            m_Commands.Add("CloseWindow", new Command("CloseWindow", "CloseWindow", "close", "window.top.close()"));
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SubscribeStatusbarEventHandlers();

            foreach (Command command in m_Commands.Values)
                command.Owner = this;

            if (InitializeColumnsOnInitStage == false)//if not initialized on Init stage
                InitColumns();

            if (String.IsNullOrEmpty(HintText))
            {
                trHint.Visible = false;
            }
        }

        private void SubscribeStatusbarEventHandlers()
        {
            btnTopPage.Click += delegate
            {
                _paging.PageIndex = 1;
                BindData();
            };

            btnBottomPage.Click += delegate
            {
                _paging.PageIndex = _paging.PageCount;
                BindData();
            };

            btnPrevPage.Click += delegate
            {
                _paging.PageIndex--;

                BindData();
            };

            btnNextPage.Click += delegate
            {
                _paging.PageIndex++;

                BindData();
            };

            wnePageIndex.EnterKeyPress += delegate
            {
                _paging.PageIndex = wnePageIndex.ValueInt;
                BindData();
            };
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (m_Bounded == false)
            {
                BindData();
            }

            SaveHeaderState();

            if (IsPostBack && !ForceSearchParametersFromSession)
            {
                LoadHeaderStateFromPostData();
            }

            Session[GetSearchParametersSessionKey()] = SearchParameterCollection;

            PreparePager();
            WriteCount();
            WriteExtraBarText();
            SetupGridLabel();

            RestoreHighlightedRow(dataGrid);

            Page.RegisterScriptBlock(string.Format("var {0} = new GeneralGrid({1});", ClientControllerName, GetClientSettings()), "Controller" + ClientID, GetType());

            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "DateSearchValues" + ClientID, ClientControllerName + ".BeforeSubmit();");
            dataGrid.ClientEvents.ContextMenu = ClientControllerName + ".onContextMenu";
            dataGrid.ClientEvents.MouseDown = ClientControllerName + ".onMouseDown";
            dataGrid.ClientEvents.Initialize = ClientControllerName + ".InitializeGrid";
            dataGrid.ClientEvents.Click = ClientControllerName + ".onClick";

            dataGrid.Behaviors.Selection.SelectionClientEvents.RowSelectionChanging = ClientControllerName + ".onRowSelectionChanging";
            dataGrid.Behaviors.Selection.SelectionClientEvents.RowSelectionChanged = ClientControllerName + ".onRowSelectionChanged";

            if (m_dblClickCommand != String.Empty && m_Commands.ContainsKey(m_dblClickCommand))
            {
                dataGrid.ClientEvents.DoubleClick = ClientControllerName + ".onDoubleClick";
            }

            InitContextMenu();
            HideCommands();
            UpdateToolbarButtonsAccessibility();
            UpdateRightToolbarOptionalButtons();
            topToolbar.MakeMarginForExpanCollapseButton = MakeMarginForExpanCollapseButton;
        }

        private void RestoreHighlightedRow(ServerControls.DataGrid grid)
        {
            if (!KeepSelection)
            {
                return;
            }

            GridRecord highlightedGridRecord = GetHighlightedGridRecord(grid);

            grid.Behaviors.Selection.SelectedRows.Clear();
            grid.Behaviors.Selection.SelectedRows.Add(highlightedGridRecord);
        }

        private GridRecord GetHighlightedGridRecord(ServerControls.DataGrid grid)
        {
            return grid.Rows.Cast<GridRecord>().FirstOrDefault(gridRecord => gridRecord.Items.FindItemByKey(PrimaryKeyColumn).Value.ToString() == HighlightedKey);
        }

        private void InitContextMenu()
        {
            foreach (var item in gridContextMenu.Allitems.OfType<DataMenuItem>())
            {
                if (m_Commands.ContainsKey(item.Key))
                {
                    var command = m_Commands[item.Key];
                    item.Text = GetResString(command.Caption);
                    item.ImageUrl = !string.IsNullOrEmpty(command.Image)
                                        ? command.Image
                                        : string.Empty;
                    if (m_disabledCommands[command.Key] != null)
                    {
                        item.Enabled = false;
                    }
                    else
                    {
                        item.NavigateUrl = "javascript:" + GetClientAction(command);
                    }
                }
                else
                {
                    item.ImageUrl = !string.IsNullOrEmpty(item.ImageUrl) ? item.ImageUrl : string.Empty;
                }
            }

            gridContextMenu.ClearTemplates();
        }

        private string GetClientAction(Command command)
        {
            return command.GetClientEventJavaScript(Page, this) + ";hideContextMenu('" + gridContextMenu.ClientID + "');";
        }

        private object GetClientSettings()
        {
            var settings = new
            {
                DateControlIds = _dateHeaderControls.Select(x => x.ClientID).ToArray(),
                DateValuesHiddenId = hDateValues.ClientID,
                ItemContextMenuId = gridContextMenu.ClientID,
                GridId = GridClientId,
                GridHolderId = gridHolder.ClientID,
                GeneralGridId = ClientID,
                KeepSelection = KeepSelection,
                RecordsCountLabelId = lblRecordCount.ClientID,
                HiddenSelectedId = hSelected.ClientID,
                PrimaryKeyColumn = PrimaryKeyColumn ?? m_Columns.First(x => !x.Hidden).Key,
                hSortColumnKeyId = hSortColumnKey.ClientID,
                hHighlightedId = hHighlighted.ClientID,
                SortPostBackReference = GetPostBackEventReference(this, "__sort"),
                SortingDisablecColumnKeys = m_Columns
                        .Where(x => !x.Hidden && (!(x is ISortableField) || !(x as ISortableField).EnableSorting))
                        .Select(x => x.Key).Union(_selectedColumnKey.CreateArray()).ToArray(),
                GridMinWidth = GetMinGridWidth(),
                EnableSorting = EnableSorting,
                RefreshCommand = GetCommand("Refresh").GetClientEventJavaScript(Page, this),
                DecimalSeparatorKeyCode = GetDecimalSeparatorKeyCode(),
                DoubleClickCommand = m_Commands.ContainsKey(m_dblClickCommand) ? m_Commands[m_dblClickCommand].GetClientEventJavaScript(Page, this) : String.Empty,
            };

            return new JavaScriptSerializer().Serialize(settings);
        }

        private int GetMinGridWidth()
        {
            var cols = dataGrid.Columns.OfType<GridField>().Where(x => !x.Hidden);

            return cols.OfType<IMinWidth>().Sum(x => x.MinWidth) +
                   cols.Where(x => !(x is IMinWidth) && x.Width.Type == UnitType.Pixel).Sum(y => (int)y.Width.Value);
        }

        #region Event handlers

        public void RefreshHandler(object sender, EventArgs args)
        {
            if (Refresh != null)
                Refresh(this, EventArgs.Empty);
            DropBindedState();
            RefreshData();
            DropBindedState();
        }
        public void ResetHandler(object sender, EventArgs args)
        {
            if (Reset != null)
            {
                Reset(this, EventArgs.Empty);
            }
            else if (Refresh != null)
                Refresh(this, EventArgs.Empty);

            RefreshSearchControls();
            DropBindedState();
            RefreshData();
            DropBindedState();
        }
        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (!HideSelectedColumn)
            {
                foreach (string s in CheckedKeys)
                {
                    var value = e.Row.Items.FindItemByKey(m_PrimaryKeyColumn).Value;
                    if (value != null && s == value.ToString())
                    {
                        var checkbox =
                            e.Row.Items.FindItemByKey(_selectedColumnKey).FindControl("cbxSelection") as
                            NotSubmitCheckBox;
                        checkbox.Checked = true;

                        break;
                    }
                }
            }

            if (e.Row.Items.Count > 0)
            {
                /* Workaround for bug #75618. 
                   IG renders comment containing row index for each row ( <!--[2]--> )
                   This comments usually goes after row tag, 
                   but for some reason if last td of the row is empty, IE11 interprets it as located inside tr  */

                var lastCell = e.Row.Items[e.Row.Items.Count - 1];

                if (String.IsNullOrEmpty(lastCell.Text))
                    lastCell.Text = "&nbsp;";
            }

        }

        private void sortAscItem_Click(object sender, EventArgs e)
        {
            SortData(SortDirection.Ascending);
        }

        private void sortDescItem_Click(object sender, EventArgs e)
        {
            SortData(SortDirection.Descending);
        }

        private void RefreshLink_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        #endregion

        #region Templated header methods

        /// <summary>
        /// Saves information about controls in the filtration bar into ViewState
        /// </summary>
        private void SaveHeaderState()
        {
            //TODO:!
            foreach (var field in dataGrid.Columns.OfType<GridField>().Where(x => x is ISearchableField))
            {
                if (field != null &&
                    field.Header != null) //Header can be not instansiated in current time
                {
                    Control controlValue =
                        field.Header.TemplateContainer.FindControl(TemplatedHeaderHelper.ValueControlName);
                    Control controlOperator =
                        field.Header.TemplateContainer.FindControl(TemplatedHeaderHelper.OperatorControlName);

                    var headerState = new ColumnHeaderState
                    {
                        ValueControlUniqueId = (controlValue != null) ? controlValue.UniqueID : String.Empty,
                        ValueControlClientId = (controlValue != null) ? controlValue.ClientID : String.Empty,
                        OperatorControlUniqueId =
                                (controlOperator != null) ? controlOperator.UniqueID : String.Empty,
                    };

                    if (controlValue != null)
                    {
                        ViewState[GetViewStateKeyForColumnHeaderState(field.Key)] = headerState;
                    }

                    if (m_Columns.OfType<ISearchableField>().Any(
                            x => x.Key == field.Key && x.SearchColumnType == SearchColumnType.DateTime))
                    {
                        _dateHeaderControls.Add(controlValue);
                    }
                }
            }
        }

        /// <summary>
        /// Restored state of controls in the filtration bar using post data.
        /// </summary>
        /// <remarks>
        /// Pay attention to following:
        /// UltraWebGrid changes identificator of Column.HeaderItem each time on DataBind.
        /// For example if there are two templated columns. 
        /// After first databind Column1.HeaderItem.Id will be "ctr01", Column2.HeaderItem.Id will be "ctr02"
        /// After second databind Column1.HeaderItem.Id will be "ctr03", Column2.HeaderItem.Id will be "ctr04"
        /// Because this ids are actually used while loading state of filtration toolbar
        /// DataBind method should be called "correct" count
        /// Apparently it is Infragisctics's grid feature (bug).
        /// </remarks>
        private void LoadHeaderStateFromPostData()
        {
            //TODO:!

            //We should not load data into controls to clear them
            if (m_ClearSearchControlsState)
            {
                return;
            }

            foreach (var field in dataGrid.Columns.OfType<GridField>().Where(x => x is ISearchableField))
            {
                if (field != null &&
                    field.Header != null) //Header can be not instansiated in current time
                {
                    ColumnHeaderState headerState =
                        ViewState[GetViewStateKeyForColumnHeaderState(field.Key)] as ColumnHeaderState;

                    if (headerState != null)
                    {
                        Control controlValue =
                            field.Header.TemplateContainer.FindControl(TemplatedHeaderHelper.ValueControlName);
                        if (controlValue != null &&
                            string.IsNullOrEmpty(headerState.ValueControlUniqueId) == false)
                        {
                            ((IPostBackDataHandler)controlValue).LoadPostData(headerState.ValueControlUniqueId,
                                                                               Request.Form);
                        }

                        Control ctrOperator =
                            field.Header.TemplateContainer.FindControl(TemplatedHeaderHelper.OperatorControlName);

                        if (controlValue != null &&
                            string.IsNullOrEmpty(headerState.OperatorControlUniqueId) == false)
                        {
                            ((IPostBackDataHandler)ctrOperator).LoadPostData(headerState.OperatorControlUniqueId,
                                                                              Request.Form);
                        }
                    }
                }
            }
        }

        private SearchParameterCollection GetHeaderState()
        {
            var collection = new SearchParameterCollection();

            if (m_ClearSearchControlsState == false)
            {
                if (!IsPostBack || ForceSearchParametersFromSession)
                {
                    var searchParameters = Session[GetSearchParametersSessionKey()] as SearchParameterCollection;
                    if (searchParameters != null && searchParameters.Any())
                    {
                        // If it is first page loading and we have search parameters stored in session - just return these parameters.
                        // We have to return cloned parameters because this collection may be changed outside of the grid.
                        return (SearchParameterCollection)searchParameters.Clone();
                    }
                }
            }

            foreach (var gridColumn in m_Columns.OfType<ISearchableField>())
            {
                // If it is the first page loading or "Reset" button is pressed - we use the default searching parameters,
                // in case of postback - we use parameters received from post data.
                SearchParameter parameter = IsPostBack && !m_ClearSearchControlsState
                                                ? GetSearchParameterFromRequest(gridColumn)
                                                : _searchParametersProvider.GetDefaultSearchParameter(gridColumn);

                if (parameter != null)
                {
                    collection.Add(parameter);
                }
            }

            return collection;
        }

        private SearchParameter GetSearchParameterFromRequest(ISearchableField gridColumn)
        {
            ColumnHeaderState headerState = ViewState[GetViewStateKeyForColumnHeaderState(gridColumn.Key)] as ColumnHeaderState;
            SearchParameter result = null;
            if (headerState != null)
            {
                result = _searchParametersProvider.GetSearchParameterFromRequest(gridColumn, headerState, hDateValues.Value);
            }

            return result;
        }

        /// <summary>
        /// Gets the key for the column header state to store it in the ViewState.
        /// It should be different from the actual column key because column key could match
        ///  some other keys that we use in GeneralGrid for the ViewState (e.g. TotalCount).
        /// </summary>
        private string GetViewStateKeyForColumnHeaderState(string columnKey)
        {
            return "__ColumnHeaderState_" + columnKey;
        }

        public void SetSearchParametersSessionKey(string sessionKey)
        {
            customSearchParametersSessionKey = sessionKey;
        }

        public string GetSearchParametersSessionKey()
        {
            return customSearchParametersSessionKey ?? string.Format("_GeneralGridSearchParameters_{0}_{1}_{2}", Page.GetType().FullName, ClientID, Request.Url.Query);
        }

        public void ClearSessionSearchParameters()
        {
            Session[GetSearchParametersSessionKey()] = null;
        }

        #endregion


    }
}
