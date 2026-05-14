using System;
using System.Globalization;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    public partial class StateTemplate : BaseWUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterClientLibrary("Resources/client/StateTemplate.js");

            // If no user languages specified in browser settings - CurrentCulture is read-only and we cannot change it directly.
            tbxPriority.Culture = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            tbxPriority.Culture.NumberFormat.NumberGroupSizes = new [] {0};

            tbxPriority.MaxValue = Int32.MaxValue;
            tbxPriority.Value = 1;

        }
    }
}