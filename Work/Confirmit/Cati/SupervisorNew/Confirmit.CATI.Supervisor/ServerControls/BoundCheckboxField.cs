using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infragistics.Web.UI;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class BoundCheckBoxField : Infragistics.Web.UI.GridControls.BoundCheckBoxField, ICollectionObject
    {
        public BoundCheckBoxField()
        {
            this.CheckBox.CheckedImageUrl = "checked.svg";
            this.CheckBox.UncheckedImageUrl = "unchecked.svg";
        }

        string ICollectionObject.GetObjectType()
        {
            return GetType().AssemblyQualifiedName.Replace('.', '~');
        }
    }
}