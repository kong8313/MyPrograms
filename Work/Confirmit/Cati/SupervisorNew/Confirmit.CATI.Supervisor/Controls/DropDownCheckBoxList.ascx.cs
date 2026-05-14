using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Confirmit.CATI.Supervisor.Controls
{
    /// <summary>
    /// Simple class which describe a single item of checkboxlist.
    /// </summary>
    [Serializable]
    public partial class CheckBoxListItem : Object
    {
        private string m_Value;
        private bool m_Selected;
        private string m_Text;

        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public bool Selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
    }

    public partial class DropDownCheckBoxList : System.Web.UI.UserControl, IPostBackEventHandler
    {

        public event EventHandler SelectionChanged;

        #region Public properties

        /// <summary>
        /// Gets or sets position where dropdown appears.
        /// </summary>
        public PopupControlPopupPosition DropDownPosition
        {
            get { return pcExtender.Position; }
            set { pcExtender.Position = value; }
        }

        /// <summary>
        /// Gets or sets behaviour of control when user clicks "OK" button.
        /// </summary>
        public bool AutoPostback
        {
            get { return ViewState["AutoPostback"] == null ? true : (bool)ViewState["AutoPostback"]; }
            set { ViewState["AutoPostback"] = value; }
        }

        /// <summary>
        /// Gets or sets data source for checkboxlist.
        /// </summary>
        public object DataSource
        {
            get { return cblList.DataSource; }
            set { cblList.DataSource = value; }
        }

        /// <summary>
        /// Binds a data source to the checkboxlist and additionally saves listitems state into ViewState property.
        /// </summary>
        public new void DataBind()
        {
            cblList.DataBind();
            List<CheckBoxListItem> lst = new List<CheckBoxListItem>();
            for (int i = 0; i < cblList.Items.Count; i++)
            {
                CheckBoxListItem lstItem = new CheckBoxListItem();
                lstItem.Selected = cblList.Items[i].Selected;
                lstItem.Value = cblList.Items[i].Value;
                lstItem.Text = cblList.Items[i].Text;
                lst.Add(lstItem);
            }
            ViewState["SelectionArray"] = lst;
        }

        /// <summary>
        /// Gets or sets the field of the data source that provides the value of each list item.
        /// </summary>
        public string DataValueField
        {
            get { return cblList.DataValueField; }
            set { cblList.DataValueField = value; }
        }

        /// <summary>
        /// Gets or sets the field of the data source that provides the text content of the list items.
        /// </summary>
        public string DataTextField
        {
            get { return cblList.DataTextField; }
            set { cblList.DataTextField = value; }
        }

        /// <summary>
        /// Gets or sets the width of the textbox.
        /// </summary>
        public Unit TextWidth
        {
            get { return tbText.Width; }
            set { tbText.Width = value; }
        }

        /// <summary>
        /// Gets or sets the height of the dropdown panel
        /// </summary>
        public Unit DropDownPanelHeight
        {
            get { return ViewState["DropDownPanelHeight"] == null ? Unit.Empty : (Unit)ViewState["DropDownPanelHeight"]; }
            set { ViewState["DropDownPanelHeight"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the dropdown panel
        /// </summary>
        public Unit DropDownPanelWidth
        {
            get { return panel.Width; }
            set { panel.Width = value; }
        }

        /// <summary>
        /// Gets or sets the number of columns to display in checkboxlist.
        /// </summary>
        public int RepeatColumns
        {
            get { return ViewState["RepeatColumns"] == null ? 1 : (int)ViewState["RepeatColumns"]; }
            set { ViewState["RepeatColumns"] = value; }
        }

        /// <summary>
        /// Gets or sets the direction to repeat items in checkboxlist.
        /// </summary>
        public System.Web.UI.WebControls.RepeatDirection RepeatDirection
        {
            get { return ViewState["RepeatDirection"] == null ? System.Web.UI.WebControls.RepeatDirection.Vertical : (System.Web.UI.WebControls.RepeatDirection)ViewState["RepeatDirection"]; }
            set { ViewState["RepeatDirection"] = value; }
        }

        /// <summary>
        /// Gets the values selected in the checkboxlist
        /// </summary>
        public List<CheckBoxListItem> SelectionArray
        {
            get { return (System.Collections.Generic.List<CheckBoxListItem>)ViewState["SelectionArray"]; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AutoPostback)
                pcExtender.CommitScript = Page.ClientScript.GetPostBackEventReference(this, "Click");
        }

        /// <summary>
        /// Here we simply set some checkboxlist's properties.
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            cblList.RepeatColumns = RepeatColumns;
            cblList.RepeatDirection = RepeatDirection;
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "Click")
                if (SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
        }

        public void btnConfirm_Click(object sender, EventArgs args)
        {
            string str = "";
            List<CheckBoxListItem> lst = new List<CheckBoxListItem>();
            for (int i = 0; i < cblList.Items.Count; i++)
            {
                if (cblList.Items[i].Selected)
                    str += (cblList.Items[i].Text + ";");
                CheckBoxListItem lstItem = new CheckBoxListItem();
                lstItem.Selected = cblList.Items[i].Selected;
                lstItem.Value = cblList.Items[i].Value;
                lstItem.Text = cblList.Items[i].Text;
                lst.Add(lstItem);
            }
            ViewState["SelectionArray"] = lst;
            tbText.Text = str;
            pcExtender.Commit(str);
        }

        public void btnCancel_Click(object sender, EventArgs args)
        {
            pcExtender.Cancel();
        }

        #endregion
    }
}