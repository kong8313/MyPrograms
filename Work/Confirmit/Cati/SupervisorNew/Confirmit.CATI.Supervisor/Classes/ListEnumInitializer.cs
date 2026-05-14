using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Classes
{
    internal class ListEnumInitializer
    {
        public static  void FillListControlWithEnumValues<TEnum>(ListControl list)
        {
            foreach (Enum item in Enum.GetValues(typeof(TEnum)))
            {
                string value = Convert.ToInt32(item).ToString();
                string text = Core.Common.ResourceWrapper.Instance.GetString(item.Description());
                list.Items.Add(new ListItem(text, value));
            }
        }
    }
}