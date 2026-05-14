using System;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class CallCentersList : CallCenterBaseForm
    {
        public override string TopTitle
        {
            get
            {                
                return Strings.CallCenters;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}