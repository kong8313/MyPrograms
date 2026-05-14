using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class ScriptInfo_General: BaseWUC
    {
        /// <summary>
        /// Gets or sets the script id.
        /// </summary>
        /// <value>The script id.</value>
        public int ScriptId
        {
            get { return ViewState["ScriptId"] == null ? 0 : (int)ViewState["ScriptId"]; }
            set { ViewState["ScriptId"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the script.
        /// </summary>
        /// <value>The name of the script.</value>
        public string ScriptName
        {
            get { return tbScriptName.Text; }
            set { tbScriptName.Text = value; }
        }

        public int? SelectedStateGroupId
        {
            get { return ddlStatesList.SelectedItem != null ? Int32.Parse(ddlStatesList.SelectedItem.Value) : (int?)null; }
            set
            {
                if (value.HasValue)
                {
                    var groupItem = (from ListItem item in ddlStatesList.Items
                                     let id = Int32.Parse(item.Value)
                                     where id == value
                                     select item).FirstOrDefault();

                    if (groupItem != null)
                    {
                        groupItem.Selected = true;
                    }
                }
                else
                {
                    ddlStatesList.SelectedIndex = 0;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if( IsPostBack == false)
            {
                ddlStatesList.Items.Clear();
                ddlStatesList.Items.AddRange(StateGroupRepository.GetAll().OrderBy(group => group.ID).Select(group => new ListItem(group.Name, group.ID.ToString())).ToArray());
            }
        }

        /// <summary>
        /// Checks the script validity.
        /// </summary>
        public void Validate()
        {
            if (String.IsNullOrEmpty(ScriptName))
            {
                throw new UserMessageException(Strings.Err_EmptyName);
            }
        }
    }
}