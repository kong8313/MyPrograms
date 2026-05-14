using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Infragistics.Web.UI;
using Infragistics.Web.UI.EditorControls;
using Infragistics.Web.UI.Framework;
using Infragistics.Web.UI.LayoutControls;
using Infragistics.Web.UI.NavigationControls;
using StringWriter = System.IO.StringWriter;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class NumericEdit : WebNumericEditor
    {
        public NumericEdit()
        {
            DataMode = NumericDataMode.Int;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CssClass = "settings-value-numeric";
            var imageProvider = new ImageProvider();
            Buttons.LowerSpinButton.Text = $"<div class='svg-absolute-wrapper'>{imageProvider.GetSvg("arrow_drop_down")}</div>";
            Buttons.UpperSpinButton.Text = $"<div class='svg-absolute-wrapper'>{imageProvider.GetSvg("arrow_drop_up")}</div>";
        }
    }

    public class DataMenu : WebDataMenu
    {
        public DataMenu()
        {
            SubMenuOpeningDelay = 300;
            IsContextMenu = true;
            EnableScrolling = true;

            GroupSettings.AnimationType = AnimationType.OpacityAnimation;
            GroupSettings.AnimationDuration = 0;

            ItemTemplate = new ContextMenuItemTemplate();
        }

        protected override RendererBase CreateRenderer()
        {
            return (RendererBase)new DataMenuRendererCati(this);
        }
    }

    public class DataMenuRendererCati : DataMenuRenderer
    {
        private readonly WebDataMenu _menu;

        public DataMenuRendererCati(DataMenu menu)
        {
            _menu = menu;
        }

        public override void RenderItem(Infragistics.Web.UI.NavigationControls.DataMenuItem item, DataMenuItemCollection items, HtmlTextWriter writer, bool isHorizontal)
        {
            if (string.IsNullOrEmpty(item.ImageUrl))
            {
                base.RenderItem(item, items, writer, isHorizontal);
                return;
            }

            using (var stringWriter = new StringWriter())
            using (var htmlWriter = new HtmlTextWriter(stringWriter))
            {
                base.RenderItem(item, items, htmlWriter, isHorizontal);

                var sWriter = htmlWriter.InnerWriter as StringWriter;
                var content = sWriter.ToString();
                    var ip = new ImageProvider().GetSvg(item.ImageUrl, item.Text);
                    content = Regex.Replace(content, @"<img.+?>", ip,
                    RegexOptions.IgnoreCase);
                writer.Write(content);
            }
        }
    }

    public class ContextMenuItemTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            var hyperLink = new HyperLink();
            hyperLink.DataBinding += hyperLink_DataBinding;

            var image = new Image();
            image.DataBinding += image_DataBinding;
            hyperLink.Controls.Add(image);
            var span = new Label();
            span.DataBinding += span_DataBinding;
            hyperLink.Controls.Add(span);
            container.Controls.Add(hyperLink);
        }

        void span_DataBinding(object sender, EventArgs e)
        {
            var span = (Label)sender;
            var namingContainer = (TemplateContainer)span.NamingContainer;
            var item = ((DataMenuItem)namingContainer.Item);

            span.Text = item.Text;
            if (item.Items.Count > 1)
                span.Text += "<div class='ParentMenuItem'></div>";
        }

        void image_DataBinding(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var namingContainer = (TemplateContainer)image.NamingContainer;
            var item = ((DataMenuItem)namingContainer.Item);

            image.CssClass = "igdm_MenuItemVerticalIcon";
            image.ImageUrl = item.ImageUrl;
            image.Visible = !String.IsNullOrEmpty(item.ImageUrl);
        }

        void hyperLink_DataBinding(object sender, EventArgs e)
        {
            var link = (HyperLink)sender;
            var namingContainer = (TemplateContainer)link.NamingContainer;
            var item = ((DataMenuItem)namingContainer.Item);

            link.CssClass = item.ParentItem == null ? "igdm_MenuItemVerticalRootLink" : "igdm_MenuItemVerticalLink";
            link.Attributes["onmousedown"] = item.NavigateUrl;
        }
    }
    public class DataMenuItem : Infragistics.Web.UI.NavigationControls.DataMenuItem
    {
        public DataMenuItem()
        {
            ImageUrl = "empty";
        }

        public string TextId { set { Text = BaseForm.GetResString(value); } }
    }

    public class ExplorerBar : WebExplorerBar
    {
    }

    public class ExplorerBarGroup : Infragistics.Web.UI.NavigationControls.ExplorerBarGroup
    {
    }

    public class ExplorerBarItem : Infragistics.Web.UI.NavigationControls.ExplorerBarItem
    {
    }

    public class ExplorerBinding : ExplorerBarItemBinding
    {

    }

    public class ExplorerItemTemplate : ItemTemplate
    {

    }

    public class DateTimeEditor : WebDateTimeEditor
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var imageProvider = new ImageProvider();
            Buttons.LowerSpinButton.Text = $"<div class='svg-absolute-wrapper'>{imageProvider.GetSvg("arrow_drop_down")}</div>";
            Buttons.UpperSpinButton.Text = $"<div class='svg-absolute-wrapper'>{imageProvider.GetSvg("arrow_drop_up")}</div>";
        }
    }

    public class DatePicker : WebDatePicker
    {
        protected override void OnInit(EventArgs e)
        {
            Page = (Page)HttpContext.Current.Handler;
            base.OnInit(e);
        }
    }

    public class Tabs : WebTab
    {
        public IEnumerable<TabItem> TabItems
        {
            get { return Tabs.OfType<TabItem>(); }
        }

        public ContentTabItem GetTabByKey(string tabKey)
        {
            return Tabs.FindTabFromKey(tabKey);
        }

        public void SelectTabByKey(string tabKey)
        {
            ContentTabItem tab = Tabs.FindTabFromKey(tabKey);

            if (tab != null && !tab.Hidden)
            {
                SelectedIndex = Tabs.IndexOf(tab);
            }
        }

        protected bool OriginalSelectedIndexChangedIsEmpty
        {
            get { return (bool)(ViewState["OriginalSelectedIndexChangedIsEmpty"] ?? false); }
            set { ViewState["OriginalSelectedIndexChangedIsEmpty"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!Page.IsPostBack)
            {
                OriginalSelectedIndexChangedIsEmpty = string.IsNullOrEmpty(ClientEvents.SelectedIndexChanged);
            }

            if (OriginalSelectedIndexChangedIsEmpty)
            {
                var script = string.Format("function {0}(sender,args){{Common.fireGlobalEvent('TabsSelectedIndexChanging');}}", ClientID + "_SelectedIndexChanged");
                ((BaseForm)Page).RegisterScriptBlock(script);
            }
            else if (!OriginalSelectedIndexChangedIsEmpty)
            {
                var script = string.Format(
                        "function {0}(sender,args){{Common.fireGlobalEvent('TabsSelectedIndexChanging'); {1}(sender,args)}}",
                        ClientID + "_SelectedIndexChanged", ClientEvents.SelectedIndexChanged);
                ((BaseForm)Page).RegisterScriptBlock(script);
            }

            ClientEvents.SelectedIndexChanged = ClientID + "_SelectedIndexChanged";

            foreach (TabItem tabItem in TabItems)
            {
                tabItem.ScrollBars = ContentOverflow.Hidden;

                if (!string.IsNullOrEmpty(tabItem.ContentUrl))
                {
                    if (tabItem.ContentUrl.Contains("?"))
                    {
                        tabItem.ContentUrl += HttpContext.Current.Request.Url.Query.Replace('?', '&');
                    }
                    else
                    {
                        tabItem.ContentUrl += HttpContext.Current.Request.Url.Query;
                    }
                }
            }
        }
    }

    public class TabItem : ContentTabItem
    {
        private string _textId;

        public string TextId
        {
            get { return _textId; }
            set
            {
                _textId = value;

                if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(_textId))
                {
                    Text = BaseForm.GetResString(_textId);
                }
            }
        }

        public string Title { get; set; }
    }
}