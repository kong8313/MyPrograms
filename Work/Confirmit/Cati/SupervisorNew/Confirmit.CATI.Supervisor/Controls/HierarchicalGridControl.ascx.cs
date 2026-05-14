using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using Confirmit.CATI.Telephony;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Collections.Generic;
using System.Text;

using Infragistics.Web.UI.NavigationControls;

using DataMenuItem = Confirmit.CATI.Supervisor.ServerControls.DataMenuItem;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class HierarchicalGridControl : GridBaseControl, IPostBackEventHandler
    {
        private bool _bounded = false;
        private readonly DataMenuItemCollection _mDataMenuItems = new DataMenuItemCollection();
        private readonly List<Control> m_toolbarItems = new List<Control>();
        private readonly List<Control> _leftToolbarItems = new List<Control>();
        private readonly Dictionary<string, Command> m_Commands = new Dictionary<string, Command>();        
        private readonly Dictionary<string, bool> m_disabledCommands = new Dictionary<string, bool>();

        public string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        /// <summary>
        /// Gets or sets grid's name (shown in toolbar).
        /// </summary>
        public string LeftLabel { get; set; }


        public string GridId
        {
            get
            {
                return m_grid.ID;
            }
        }

        public string GridClientId
        {
            get
            {                
                return m_grid.ClientID;
            }
        }

        public string ToolBarName
        {
            get
            {
                return topToolbar.ClientID;
            }
        }

        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute  )]
        public string DataKeyFields
        {
            get { return m_grid.DataKeyFields; }
            set { m_grid.DataKeyFields = value; }
        }
              
        /// <summary>
        /// Items of the grid context menu.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataMenuItemCollection DataMenuItems
        {
            get
            {
                return _mDataMenuItems;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public GridFieldCollection Columns
        {
            get
            {
                return m_grid.Columns;
            }
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
        /// Gets or sets arraylist of command objects associated with current grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Command> Commands
        {
            get
            {
                return new List<Command>(m_Commands.Values);
            }
            set
            {
                foreach (Command c in value)
                {
                    m_Commands[c.Key] = c;
                }
            }
        }

        public ToolbarLayout TopToolbarLayout
        {
            get { return topToolbar.ToolbarLayout; }
            set { topToolbar.ToolbarLayout = value; }
        }

        /// <summary>
        /// Returns command by name.
        /// </summary>
        public Command GetCommand( string key )
        {
            return m_Commands[key];
        }        

        /// <summary>
        /// Delegate for retrieving datasource.
        /// </summary>
        public GetPageDelegate GetPage;        

        /// <summary>
        /// Collection of bands of the grid.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]        
        public BandCollection Bands
        {
            get
            {
                return m_grid.Bands;
            }
        }

        [DefaultValue(null)]        
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]        
        public Behaviors Behaviors
        {
            get { return m_grid.Behaviors; }
        }       

        public string HighlightedKey
        {
            get
            {
                return hHighlighted.Value;
            }
            private set
            {
                hHighlighted.Value = value;
            }
        }

        public string[] ExpandedRowsKeys
        {
            get
            {
                if (hExpandedRows.Value.Length > 0)
                {
                    return hExpandedRows.Value.Split(',');
                }

                return new string[0];
            }
        }

        public GridRecord SelectedRow
        {
            get
            {                
                return GetHighlightedRow(m_grid.GridView);                
            }
        }
        
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]  
        public ContainerGridClientEvents ClientEvents
        {
            get
            {
                return m_grid.ClientEvents;
            }
        }

        public string OnDblClickCommand { get; set; }

        public override string ClientGetCurrentRow()
        {
            return ClientControllerName + ".getSelectedRow()";
        }

        public override string ClientGetSelectedRows()
        {
            throw new NotSupportedException();
        }

        public event InitializeRowEventHandler InitializeRow
        {
            add
            {
                m_grid.InitializeRow += value;

            }
            remove
            {
                m_grid.InitializeRow -= value;

            }
        }

        public event ContainerRowCancelEventHandler RowIslandsPopulating
        {
            add
            {
                m_grid.RowIslandsPopulating += value;

            }
            remove
            {
                m_grid.RowIslandsPopulating -= value;

            }
        }

        public event RowAddingHandler RowAdding
        {
            add
            {
                m_grid.RowAdding += value;

            }
            remove
            {
                m_grid.RowAdding -= value;

            }
        }

        public event RowUpdatingHandler RowUpdating
        {
            add
            {
                m_grid.RowUpdating += value;

            }
            remove
            {
                m_grid.RowUpdating -= value;

            }
        }

        public void InitDataSource()
        {            
            if (GetPage != null)
            {
                int tc;
                object pageRecords = GetPage(out tc);
                m_grid.DataSource = pageRecords;         
            }
        }

        public void BindData()
        {         
            if (_bounded)
            {
                System.Diagnostics.Trace.TraceWarning("Double data bind is called for the grid {0}", UniqueID);
            }

            if (GetPage != null)
            {
                int tc;                
                object pageRecords = GetPage( out tc );                
                m_grid.DataSource = pageRecords;                                
                m_grid.DataBind();                
                _bounded = true;
            }            
        }        

        public void RefreshData()
        {            
            //m_grid.RefreshBehaviors();
            //m_grid.Rows.Clear();
            BindData();            
        }     

        public void EnableCommand( string commandKey )
        {
            DisableCommand( commandKey, false );
        }

        public void DisableCommand( string commandKey )
        {
            DisableCommand( commandKey, true );
        }

        public void DisableCommand( string commandKey, bool enabled )
        {
            if (enabled)
            {
				if (!m_disabledCommands.ContainsKey(commandKey))
				{
					m_disabledCommands.Add( commandKey, true );
				}
            }
            else
            {
                if (m_disabledCommands.ContainsKey( commandKey ))
                {
                    m_disabledCommands.Remove( commandKey );
                }
            }

        }

        /// <summary>
        /// Menu initialization for each band.
        /// NB: Each band MUST have unique value as 'Key' property!!
        /// </summary>
        private void InitMenus()
        {            
            var clientScript = new StringBuilder();
            
            //Create context menu for the most parent grid.
            if (DataMenuItems.Count > 0)
            {
                var topGridMenu = new DataMenu { ID = "DefaultContextMenu_" + m_grid.ClientID };

                Controls.Add( topGridMenu );

                InitContextMenu(topGridMenu, DataMenuItems);

                clientScript.AppendFormat("{0}.AddMenu('{1}', '{2}');", ClientControllerName, "default", topGridMenu.ClientID);
            }

            CreateMenuForBands(m_grid.Bands, clientScript);

            Page.ClientScript.RegisterStartupScript(Page.GetType(), ClientControllerName, clientScript.ToString(), true);

            m_grid.ClientEvents.ContextMenu = string.Format("{0}.ShowContextMenu", ClientControllerName);         
        }

        private void InitContextMenu(DataMenu menu, DataMenuItemCollection items )
        {
            menu.Items.Clear();
            foreach (var item in items.OfType<DataMenuItem>())
            {
                menu.Items.Add(item);
                if (m_Commands.ContainsKey(item.Key))
                {
                    var command = m_Commands[item.Key];

                    item.Text = GetResString(command.Caption);
                    item.NavigateUrl = "javascript:" + command.GetClientEventJavaScript(Page, this);
                    item.ImageUrl = !string.IsNullOrEmpty(command.Image)
                                        ? command.Image
                                        : "";
                    if (m_disabledCommands.ContainsKey(command.Key))
                    {
                        item.Enabled = false;
                    }
                }
                else
                {
                    item.ImageUrl = !string.IsNullOrEmpty(item.ImageUrl) ? BaseRelativePath("images/" + item.ImageUrl) : "";
                }
            }
        }

        private void CreateMenuForBands(BandCollection bandCollection,  StringBuilder clientScript)
        {
            if (bandCollection.Count > 0)
            {
                foreach (GridBand band in bandCollection)
                {
                    if (band.DataMenuItems.Count > 0)
                    {
                        var contextMenu = new DataMenu { ID = "ContextMenu_" + band.Key };

                        Controls.Add(contextMenu);

                        InitContextMenu(contextMenu, band.DataMenuItems);

                        clientScript.AppendFormat("{0}.AddMenu('{1}', '{2}');", ClientControllerName, band.Key,
                                                  contextMenu.ClientID);
                    }

                    CreateMenuForBands(band.Bands, clientScript);
                }
            }
        }

        private void InitToolbar()
        {
            AddToolbarControls(topToolbar.RightMenuItems, m_toolbarItems);
            AddToolbarControls(topToolbar.LeftMenuItems, _leftToolbarItems);            
        }

        private void AddToolbarControls(XpMenuItemCollection toolbarItemsCollection, IEnumerable<Control> toolbarItems)
        {
            toolbarItemsCollection.Clear();

            foreach (Control tbitem in toolbarItems)
            {
                var button = tbitem as ToolbarCommandButton;
                if (button != null)
                {
                    topToolbar.AddCommandButton(button, m_Commands[button.Key], m_disabledCommands.ContainsKey(button.Key), this, toolbarItemsCollection);
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
        
        private void DisableCommands()
        {
            foreach (var btn in DataMenuItems.OfType<DataMenuItem>())
            {
                btn.Visible = !m_disabledCommands.ContainsKey( btn.Key );
            }

            foreach (var btn in topToolbar.RightMenuItems.OfType<ToolbarCommandButton>())
            {
                btn.Enabled = !m_disabledCommands.ContainsKey(btn.Key);
            }
        }

        /// <summary>
        /// Returns row key. 
        /// Takes into account keys of parent rows.
        /// </summary>        
        ///<remarks>
        /// Note that this method must be matched to the client side 'GetRowKey' method.
        /// </remarks>
        private string GetRowKey(ContainerGridRecord currentRow)
        {
            string key = String.Empty;
            var row = currentRow;
            do
            {
                if (row.DataKey.Any())
                {
                    var currentRowKey = row.DataKey[0].ToString();

                    key = (key != "") ? currentRowKey + "_" + key : currentRowKey;                    

                    row = row.Owner.ControlMain.ParentRow;
                }

            } while (row != null );

            return key;
        }       

        private GridRecord GetHighlightedRow(ContainerGrid grid)
        {
            foreach (ContainerGridRecord gridRecord in grid.Rows)
            {
                var rowKey = GetRowKey(gridRecord);

                if (rowKey == HighlightedKey)
                {
                    return gridRecord;
                }

                if (gridRecord.RowIslands.Count == 0) continue;

                var result = GetHighlightedRow(gridRecord.RowIslands[0]);

                if (result != null) { return result; }
            }

            return null;
        }

        public void HighlightRow(string rowKey, ContainerGridRecord parentRow = null)
        {
            HighlightedKey = (parentRow != null) ? GetRowKey(parentRow) + rowKey : rowKey;
        }

        private void RestoreHighlightedRowAndExpandedStates(ContainerGrid grid)
        {
            foreach (ContainerGridRecord gridRecord in grid.Rows)
            {
                var rowKey = GetRowKey(gridRecord);

                if (rowKey == HighlightedKey)
                {
                    grid.Behaviors.Selection.SelectedRows.Clear();
                    grid.Behaviors.Selection.SelectedRows.Add(gridRecord);
                    gridRecord.ExpandAnscestors();
                }

                if (ExpandedRowsKeys.Contains(rowKey))
                {
                    gridRecord.Expanded = true;
                }

                if (gridRecord.RowIslands.Count > 0 && gridRecord.RowIslands[0].Rows.Count > 0)
                {
                    //gridRecord.Expanded = ExpandedRowsKeys.Contains(rowKey);

                    if (gridRecord.Expanded)
                    {
                        RestoreHighlightedRowAndExpandedStates(gridRecord.RowIslands[0]);
                    }
                }
            }
        }

        public string ItemCssClass { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {

            InitToolbar();

            ((IBand) m_grid).EnableEmptyRowIslands = false;
            m_grid.ItemCssClass = ItemCssClass;
            m_grid.ExpandButton.SetupDefaultImages("expand_more.svg", "expand_more.svg", "expand_more.svg");
            m_grid.CollapseButton.SetupDefaultImages("expand_less.svg", "expand_less.svg", "expand_less.svg");
            foreach (GridBand band in m_grid.Bands)
            {
                band.ExpandButton.SetupDefaultImages("expand_more.svg", "expand_more.svg", "expand_more.svg");
                band.CollapseButton.SetupDefaultImages("expand_less.svg", "expand_less.svg", "expand_less.svg");
            }
        }
      
        private void Page_Load( object sender, EventArgs e )
        {                        
            if (!string.IsNullOrEmpty(LeftLabel))
            {
                topToolbar.LeftLabel = LeftLabel;
            }

            foreach (var command in m_Commands.Values)
            {
                command.Owner = this;
            }
         
            InitMenus();            
        }

        private void Page_PreRender( object sender, EventArgs e )
        {            
            if (_bounded == false)
            {                                                
                BindData();
            }
            
            if (m_grid.Bands.Count == 0)
            {
                m_grid.ExpansionColumnCss = "ighg_NoExpansionColumn";
            }            
                        
            RestoreHighlightedRowAndExpandedStates(m_grid.GridView);

            DisableCommands();
          
            RegisterClientScripts();            
        }

        protected void InitializeRowHandler(object sender, RowEventArgs e)
        {
            var row = e.Row;
            row.CssClass = "igg_Row";
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.IndexOf("__command_") == 0)
            {
                var eh = m_Commands[eventArgument.Replace("__command_", "")].ServerClickEventHandler;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
            else
            {
                if (m_Commands.ContainsKey(eventArgument))
                {
                    var command = m_Commands[eventArgument];
                    if (command.ServerClickEventHandler != null)
                    {
                        var eh = command.ServerClickEventHandler;
                        eh(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void RegisterClientScripts()
        {
            m_grid.ClientEvents.Initialize = ClientControllerName + ".InitializeGridHandler";
            m_grid.ClientEvents.RowExpanded = ClientControllerName + ".RowExpandedHandler";
            m_grid.ClientEvents.RowCollapsed = ClientControllerName + ".RowCollapsedHandler";
            m_grid.ClientEvents.Click = ClientControllerName + ".onClick";

            if (String.IsNullOrEmpty(OnDblClickCommand) == false && m_Commands.ContainsKey(OnDblClickCommand))
            {
                m_grid.ClientEvents.DoubleClick = ClientControllerName + ".onDoubleClick";
            }

            Page.RegisterClientLibrary("Client/HierarchicalGridControl.js");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), 
                                                        ClientControllerName,
                                                        $"var {ClientControllerName} = new HierarchicalGridControl({GetClientSettings()});", 
                                                        true);
            
            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                        "rowSelectionChangedHandler",
                                                        $"function rowSelectionChangedHandler(sender, args){{ {ClientControllerName}.onRowSelectionChanged(sender, args); }}",
                                                        true);            
        }

        private object GetClientSettings()
        {
            var settings = new
            {                
                GridId = GridClientId,
                hHighlightedId = hHighlighted.ClientID,
                hExpandedRowsId = hExpandedRows.ClientID,
                DoubleClickCommand = m_Commands.ContainsKey(OnDblClickCommand) ? m_Commands[OnDblClickCommand].GetClientEventJavaScript(Page, this) : String.Empty,
            };

            return new JavaScriptSerializer().Serialize(settings);
        }
    }
}
