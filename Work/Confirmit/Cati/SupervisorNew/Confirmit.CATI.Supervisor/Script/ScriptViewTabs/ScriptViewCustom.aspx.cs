using System;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Script.ScriptViewTabs
{
    public partial class ScriptViewCustom : BaseForm
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}