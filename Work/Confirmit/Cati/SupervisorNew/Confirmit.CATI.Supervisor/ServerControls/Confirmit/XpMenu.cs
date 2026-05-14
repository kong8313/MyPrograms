using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls.Confirmit
{
    /// <summary>
    /// Container for XpMenuItems.
    /// </summary>
    [ParseChildren(true, "MenuItems")]
    public class XpMenu : WebControl, INamingContainer
    {
        private XpMenuItemCollection menuItems;
        private string _textColor = "black";

        /// <summary>
        /// Child controls.
        /// </summary>
        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }

        /// <summary>
        /// If this property is set to false, clicking the element will not cause it to recieve focus
        /// </summary>
        public bool Selectable
        {
            set { ViewState["Selectable"] = value; }
            get
            {
                object s = ViewState["Selectable"];
                return (s == null ? true : (bool)s);
            }
        }

        /// <summary>
        /// Menu items collection.
        /// </summary>
        public XpMenuItemCollection MenuItems
        {
            get
            {
                EnsureChildControls();
                if (menuItems == null)
                    menuItems = new XpMenuItemCollection(this);
                return menuItems;
            }
        }

        /// <summary>
        /// Text color.
        /// </summary>
        public string TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public XpMenu()
        {
        }

        /// <summary>
        /// Add separator to menu items collection.
        /// </summary>
        public void AddSeparator()
        {
            XpMenuItem item = new XpMenuItem();
            item.ButtonType = XpMenuItemType.Separator;
            MenuItems.Add(item);
        }

        protected override void Render(HtmlTextWriter w)
        {
            var cssClass = string.IsNullOrEmpty(CssClass) ? "cati-controls-menu" : CssClass;
            w.WriteLine("<div class=\"" + cssClass + "\" onmouseover=\"if(typeof(Menu_MouseOver)=='function'){Menu_MouseOver(event);}\" id=\"" + ClientID + "\" "
                + (Selectable ? "" : " unselectable=\"on\" onselectstart=\"return false;\" ondrag=\"return false;\"")
                + " cellpadding=\"0\" cellspacing=\"0\">");
            foreach (XpMenuItem item in MenuItems)
            {
                if (item.ButtonType == XpMenuItemType.Generic && item.Controls.Count > 0 && item.Controls.Cast<Control>().All(x => !x.Visible))
                {
                    continue;
                }

                item.RenderControl(w);
            }
            w.WriteLine("</div>");
        }
    }
}