using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Controls
{
    /// <summary>
    /// Listbox side enum.
    /// </summary>
    public enum ListBoxSide
    {
        LeftListBox,
        RightListBox
    }

    /// <summary>
    /// DoubleList box control class.
    /// </summary>
    public partial class DoubleListBox : BaseWUC
    {
        
        #region Fields

        private List<String> m_LeftValues = new List<string>();
        private List<String> m_RightValues = new List<string>();

        #endregion

        #region Properties

        public int Rows
        {
            get
            {
                return Math.Max(leftList.Rows, rightList.Rows);
            }
            set 
            {
                leftList.Rows = value;
                rightList.Rows = value;
            }
        }

        /// <summary>
        /// Dictionary that stores DoubleListBox items.
        /// </summary>
        private Dictionary<int, string> Items               
        {
            get
            {
                if (ViewState["Items"] == null)
                    ViewState["Items"] = new Dictionary<int, string>();
                return (Dictionary<int, string>)ViewState["Items"];
            }
        }

        /// <summary>
        /// Read only property that gives left listbox values.
        /// </summary>
        public List<int> LeftValues
        {
            get
            {
                List<int> result = new List<int>();
                if (leftIDs.Value != "")
                {
                    string[] ids = leftIDs.Value.Substring(1).Split(';');
                    foreach (string val in ids )
                    {
                        result.Add(Int32.Parse(val));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Read only property that gives right listbox values.
        /// </summary>
        public List<int> RightValues
        {
            get
            {
                List<int> result = new List<int>();
                if (rightIDs.Value != "")
                {
                    string[] ids = rightIDs.Value.Substring(1).Split(';');
                    foreach (string val in ids)
                    {
                        result.Add(Int32.Parse(val));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Left listbox caption.
        /// </summary>
        public string LeftCaption
        {
            get
            {
                return leftCaption.Text;
            }
            set
            {
                leftCaption.Text = value;
            }
        }

        /// <summary>
        /// Right listbox caption.
        /// </summary>
        public string RightCaption
        {
            get
            {
                return rightCaption.Text;
            }
            set
            {
                rightCaption.Text = value;
            }
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Clears double list box control
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            rightIDs.Value = String.Empty;
            leftIDs.Value = String.Empty;
        }

        /// <summary>
        /// Adds record to DoubleListBox.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <param name="id">Item id.</param>
        /// <param name="isSelected">Is item selected. Set true if item schould be in right listbox. And set false if item schould be in left listbox.</param>
        public void AddRecord(string name, int id, ListBoxSide side)
        {
            if (!Items.ContainsKey(id))
            {
                Items.Add(id, name);
                if (side == ListBoxSide.RightListBox)
                    rightIDs.Value += ";" + id.ToString(); 
                else
                    leftIDs.Value += ";" + id.ToString();
            }
        }

        /// <summary>
        /// Binds data to DoubleListBox.
        /// </summary>
        protected new void DataBind()
        {
            leftList.Items.Clear();
            rightList.Items.Clear();
            foreach (int id in LeftValues)
            {
                ListItem li = new ListItem(Items[id], id.ToString());
                leftList.Items.Add(li);
            }
            foreach (int id in RightValues)
            {
                ListItem li = new ListItem(Items[id], id.ToString());
                rightList.Items.Add(li);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PageHelper.RegisterClientLibrary("client/DoubleListBox.js");
            var leftToRightScript = "moveElements('" + leftList.ClientID + "','" + leftIDs.ClientID + "','" + rightList.ClientID + "','" + rightIDs.ClientID + "')";
            var rightToLeftScript = "moveElements('" + rightList.ClientID + "','" + rightIDs.ClientID + "','" + leftList.ClientID + "','" + leftIDs.ClientID + "')";

            leftList.Attributes.Add("ondblclick", leftToRightScript);
            bttnRight.Attributes.Add("onclick", leftToRightScript);

            bttnLeft.Attributes.Add("onclick", rightToLeftScript);
            rightList.Attributes.Add("ondblclick", rightToLeftScript);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            DataBind();
        }

        #endregion
        
    }
}