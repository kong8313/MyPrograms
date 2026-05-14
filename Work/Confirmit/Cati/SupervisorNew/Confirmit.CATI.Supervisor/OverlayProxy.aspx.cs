using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor
{
    public partial class OverlayProxy : BaseForm
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowPostbackProcessingAnimation = false;
        }
    }
}
