using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class DialerState : BaseWUC
    {
        public int MarginTop { get; set; }

        /// <summary>
        /// Hint text
        /// </summary>
        public string Text
        {
            get
            {
                return lblState.Text;
            }
            set
            {
                lblState.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StateContainer.Style.Add("margin-top", MarginTop + "px");
        }
    }
}