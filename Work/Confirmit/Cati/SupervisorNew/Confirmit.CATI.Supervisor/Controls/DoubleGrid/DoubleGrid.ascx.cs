using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Controls.DoubleGrid
{
    public partial class DoubleGrid : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Button AddButton
        {
            get { return btnAdd; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Button RemoveButton
        {
            get { return btnRemove; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Button RemoveAllButton
        {
            get { return btnRemoveAll; }
        }
        
        public void HideRemoveAllButton()
        {
            btnRemoveAll.Visible = false;
            labelRemoveAll.Visible = false;
        }

        protected void Page_Init()
        {
            if (LeftGridContent != null)
                LeftGridContent.InstantiateIn(phLeftGrid);
            
            if (RightGridContent != null)
                RightGridContent.InstantiateIn(phRightGrid);
            
            btnRemoveAll.ToolTip = Strings.ClearSelection;
        }

        [TemplateContainer(typeof(GeneralGrid)), PersistenceMode(PersistenceMode.InnerProperty), TemplateInstance(TemplateInstance.Single)]
        public ITemplate LeftGridContent { get; set; }

        [TemplateContainer(typeof(GeneralGrid)), PersistenceMode(PersistenceMode.InnerProperty), TemplateInstance(TemplateInstance.Single)]
        public ITemplate RightGridContent { get; set; }        
    }
}