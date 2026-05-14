using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls
{    
    public sealed class DayOfWeekDropDownList : DropDownList
    {       
        #region Constructors

        public DayOfWeekDropDownList()
        {            
            Items.Add(new ListItem(Strings.Monday,Strings.Monday));
            Items.Add(new ListItem(Strings.Tuesday,Strings.Tuesday));
            Items.Add(new ListItem(Strings.Wednesday,Strings.Wednesday));
            Items.Add(new ListItem(Strings.Thursday,Strings.Thursday));
            Items.Add(new ListItem(Strings.Friday,Strings.Friday));
            Items.Add(new ListItem(Strings.Saturday,Strings.Saturday));
            Items.Add(new ListItem(Strings.Sunday,Strings.Sunday));
            
        }

        #endregion        

    }
}
