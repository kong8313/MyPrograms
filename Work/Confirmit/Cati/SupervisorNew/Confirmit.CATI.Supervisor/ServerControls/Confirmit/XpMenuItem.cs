using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.ServerControls.Confirmit
{

    /// <summary>
    /// Type of a menu item.
    /// </summary>
    public enum XpMenuItemType
    {
        Button,
        ToggleButton,
        DropButton,
        Separator,
        ListButton,
        Generic
    }

    /// <summary>
    /// Item of XpMenu class.
    /// </summary>
    [ParseChildren(true, "Controls")]
    public class XpMenuItem : WebControl, IPostBackEventHandler, INamingContainer, IButtonControl
    {
        private DropDownList _dropList;
        private string _tempId; // fxcop
        object _cachedSelectable;

        /// <summary>
        /// Child controls.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public override ControlCollection Controls => base.Controls;

        public bool ToggleButtonPressed
        {
            get
            {
                if (ButtonType != XpMenuItemType.ToggleButton)
                    throw new InvalidOperationException("Item type is not ToggleButton");
                return (bool)(ViewState["ToggleButtonPressed"] ?? false);
            }
            set
            {
                if (ButtonType != XpMenuItemType.ToggleButton)
                    throw new InvalidOperationException("Item type is not ToggleButton");
                ViewState["ToggleButtonPressed"] = value;
            }
        }

        /// <summary>
        /// ID of the text control.
        /// </summary>
        public string TextId { set; get; }

        public bool IsSubmit { get; set; } = true;
        /// <summary>
        /// Text of a menu item.
        /// </summary>
        public string Text { set; get; }

        public virtual string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the client-side script that executes when control's Change event is raised.
        /// </summary>
        public string OnChange { set; get; }

        /// <summary>
        /// Gets or sets the client-side script that executes when control's Click event is raised.
        /// </summary>
        public virtual string OnClientClick { get; set; }

        /// <summary>
        /// Gets or sets the client-side script that executes when control's GetSubItems event is raised.
        /// </summary>
        public string OnGetSubItems
        {
            set => OnClientClick = value;
            get => OnClientClick;
        }

        /// <summary>
        /// Button type.
        /// </summary>
        public XpMenuItemType ButtonType { set; get; } = XpMenuItemType.Button;

        /// <summary>
        /// Defines if both text and image should be shown in a menu item.
        /// </summary>
        public bool TextAndImage { set; get; }

        /// <summary>
        /// If this property is set to false, clicking the element will not cause it to recieve focus
        /// </summary>
        public bool Selectable
        {
            get
            {
                if (_cachedSelectable == null)
                {
                    if (Parent != null && Parent is XpMenu)
                        return ((XpMenu)Parent).Selectable;
                    return true;
                }
                else
                    return (bool)_cachedSelectable;
            }
            set => _cachedSelectable = value;
        }

        /// <summary>
        /// ListButton items collection.
        /// </summary>
        public ListItemCollection Items
        {
            get
            {
                if (ButtonType != XpMenuItemType.ListButton)
                    throw new ApplicationException("Base xp menuitem items collection only valid for listbutton");
                EnsureChildControls();
                return _dropList.Items;
            }
        }

        /// <summary>
        /// ListButton selected item.
        /// </summary>
        public ListItem SelectedItem
        {
            get
            {
                if (ButtonType != XpMenuItemType.ListButton)
                    throw new ApplicationException("Base xp menuitem items collection only valid for listbutton");
                EnsureChildControls();
                return _dropList.SelectedItem;
            }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public XpMenuItem()
            : base("div")
        {
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (ButtonType != XpMenuItemType.Generic)
                throw new ApplicationException("Base xpmenuitem only children when buttontype is generic");
            else
                base.AddParsedSubObject(obj);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public XpMenuItem(string id, XpMenuItemType buttonType, string textId, string imageName, string onClientClick, bool textAndImage)
            : base("div")
        {
            if (id != null)
                _tempId = id;
            ButtonType = buttonType;
            TextId = textId;
            ImageName = imageName;
            OnClientClick = onClientClick;
            TextAndImage = textAndImage;
        }

        protected override void AddAttributesToRender(HtmlTextWriter w)
        {
            string defaultClass = ButtonType == XpMenuItemType.ToggleButton && ToggleButtonPressed ? "XpButtonPressed" : "XpButton";
            string className = defaultClass;

            if (ButtonType == XpMenuItemType.Button
                || ButtonType == XpMenuItemType.DropButton
                || ButtonType == XpMenuItemType.ToggleButton)
            {
                if (!string.IsNullOrEmpty(ToolTip))
                    w.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);
                else if (!string.IsNullOrEmpty(Text))
                    w.AddAttribute(HtmlTextWriterAttribute.Title, Text);

                if (!string.IsNullOrEmpty(CssClass))
                    className += " " + CssClass;

                w.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

                if (ButtonType == XpMenuItemType.ListButton)
                    w.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

                if (ButtonType == XpMenuItemType.ToggleButton)
                    w.AddAttribute("isToggle", "true");
                else if (ButtonType == XpMenuItemType.DropButton)
                    w.AddAttribute("isDropItem", "true");

                if (ButtonType == XpMenuItemType.DropButton)
                {
                    w.AddStyleAttribute("padding", "0px 0px 0px 3px");
                    if (OnGetSubItems == null)
                        throw new ApplicationException("DropButton requires that OnGetSubItems is set");
                    w.AddAttribute("GetSubItemsFunction", OnGetSubItems);
                }
                else
                {
                    string onclick = string.Empty;
                    if (!Enabled)
                        onclick = "return;";
                    if (Attributes["onclick"] != null)
                        onclick += Attributes["onclick"];

                    if (!string.IsNullOrEmpty(onclick) && !onclick.EndsWith(";", true, System.Globalization.CultureInfo.InvariantCulture))
                        onclick += ";";

                    if (!string.IsNullOrEmpty(OnClientClick))
                        onclick += OnClientClick;
                    else
                    {
                        if (!IsSubmit)
                        {
                            onclick = "return false;";
                        }
                        else
                        {
                            PostBackOptions options = new PostBackOptions(this, string.Empty, PostBackUrl, AutoPostBack, false,
                                false, true, CausesValidation, ValidationGroup);
                            onclick += Page.ClientScript.GetPostBackEventReference(options);
                        }
                    }
                    w.AddAttribute(HtmlTextWriterAttribute.Onclick, onclick);

                    if (!Enabled)
                    {

                        w.AddAttribute("disabled", "true");
                        className += " XpButtonDisabled";
                    }
                }
                w.AddAttribute(HtmlTextWriterAttribute.Class, className);
            }
            else if (ButtonType == XpMenuItemType.Generic)
            {
                w.AddAttribute(HtmlTextWriterAttribute.Class, "XpGeneric");
            }

            if (Width != Unit.Empty)
                w.AddStyleAttribute("width", Width.ToString());

            // common attributes for all types
            if (!Selectable)
                w.AddAttribute("unselectable", "on");
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (ButtonType == XpMenuItemType.ToggleButton)
            {
                ToggleButtonPressed = !ToggleButtonPressed;
            }

            if (string.IsNullOrEmpty(eventArgument))
            {
                if (Click != null)
                {
                    if (CausesValidation)
                        Page.Validate();
                    Click(this, EventArgs.Empty);
                }
            }
            else
            {
                if (Command != null)
                {
                    if (CausesValidation)
                        Page.Validate();
                    Command(this, new CommandEventArgs(eventArgument, null));
                }
            }


        }

        protected override void RenderContents(HtmlTextWriter w)
        {
            var imageContent = "";

            if (!string.IsNullOrEmpty(ImageName))
            {
                var tooltip = !string.IsNullOrEmpty(ToolTip) ? ToolTip : Text;
                imageContent = $"<button type='button' class='comd-button {(TextAndImage ? "comd-button--icon-with-text" : "comd-button--icon")}' data-button='{ImageName}'>{new ImageProvider().GetSvg(ImageName, tooltip)}{(TextAndImage && !string.IsNullOrEmpty(Text) ? $"<span>{Text}</span>" : "")}</button>";
            }

            if (ButtonType == XpMenuItemType.Separator)
            {
                w.WriteLine("&nbsp;");
            }
            else if (ButtonType == XpMenuItemType.DropButton)
            {
                // TODO: Selectable???

                var arrowImage = "<img" + (Selectable ? "" : " unselectable=\"on\"") + " src=\"" +
                                 Page.Request.ApplicationPath +
                                 "/small_down_black_arrow.gif\" border=\"0\" align=\"absmiddle\" />";

                w.WriteLine("<div" + (Selectable ? "" : " unselectable=\"on\"") + " cellpadding=\"0\" cellspacing=\"0\">");
                if (!string.IsNullOrEmpty(imageContent))
                {
                    w.WriteLine($"<div {(Selectable ? "" : " unselectable='on'")}>{imageContent}</div>");
                }
                if ((TextAndImage || string.IsNullOrEmpty(imageContent)) && Text != null)
                    w.WriteLine("<div" + (Selectable ? "" : " unselectable=\"on\"") + " style=\"white-space:nowrap;padding-right:1px;color:" + ((XpMenu)Parent).TextColor + ";\">" + Text + "</div>");
                w.WriteLine("<div" + (Selectable ? "" : " unselectable=\"on\"") + ">" + arrowImage + "</div>");

                w.WriteLine("</div>");
            }
            else if (ButtonType == XpMenuItemType.Button || ButtonType == XpMenuItemType.ToggleButton)
            {
                if (!string.IsNullOrEmpty(ImageName))
                {
                    // TODO: svg tooltip and title
                    string alt = string.Empty;
                    if (!string.IsNullOrEmpty(ToolTip))
                        alt = ToolTip;
                    else if (!string.IsNullOrEmpty(Text))
                        alt = Text;

                    w.WriteLine(imageContent);
                }
                else
                    RenderText(w);
            }
            else if (ButtonType == XpMenuItemType.ListButton)
            {
                EnsureChildControls();
                _dropList.Attributes["onchange"] = OnChange;
                _dropList.RenderControl(w);
            }
            else if (ButtonType == XpMenuItemType.Generic)
                base.RenderChildren(w);
        }

        private void RenderText(HtmlTextWriter w)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                w.WriteLine("<span>" + Text + "</span>");
            }
        }

        protected override void CreateChildControls()
        {
            if (ButtonType == XpMenuItemType.ListButton)
            {
                _dropList = new DropDownList();
                _dropList.ID = "list";
                _dropList.Attributes["style"] = "font-size:10px;font-family:verdana;";
                Controls.Add(_dropList);
            }
        }


        /// <summary>
        /// Occurs when selected index of a ListButton is changed.
        /// </summary>
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                if (ButtonType != XpMenuItemType.ListButton)
                    throw new ApplicationException("Base xp menuitem items collection only valid for listbutton");
                _dropList.SelectedIndexChanged += value;
            }
            remove
            {
                _dropList.SelectedIndexChanged -= value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (_tempId != null && _tempId.Length > 0) // set from constructor
                ID = _tempId;

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!string.IsNullOrEmpty(TextId) && string.IsNullOrEmpty(Text))
                Text = BaseForm.GetResString(TextId);

            // so that the script will be on top of the page
            Page.ClientScript.GetPostBackEventReference(this, String.Empty);
            if (ButtonType == XpMenuItemType.ListButton && !Selectable)
                _dropList.Attributes["unselectable"] = "on";
        }

        /// <summary>
        /// Occurs when the button control is clicked.
        /// </summary>
        public event CommandEventHandler Command;

        /// <summary>
        /// Occurs when the button control is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        ///Gets or sets an optional argument that is propagated to the Command event.
        /// </summary>
        public string CommandArgument { get; set; }

        /// <summary>
        /// Gets or sets the command name that is propagated to the Command event.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets the URL of the Web page to post to from the current page when the button control is clicked.
        /// </summary>
        public string PostBackUrl { get; set; }

        /// <summary>
        /// Gets or sets the name for the group of controls for which the button control 
        /// causes validation when it posts back to the server.
        /// </summary>
        public string ValidationGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether clicking the button causes page validation to occur.
        /// </summary>
        public bool CausesValidation { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that indicates whether the control will automatically post back to the server in response to a user action.
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                return AutoPostBack1;
            }
            set
            {
                if (value && ButtonType != XpMenuItemType.ListButton)
                    throw new ApplicationException("BASE_XP_MENUITEM_ITEMS_COLLECTION_ONLY_VALID_FOR_LISTBUTTON");
                EnsureChildControls();
                //dropList.AutoPostBack = value;
                AutoPostBack1 = value;
            }
        }

        public bool AutoPostBack1 { get => AutoPostBack2; set => AutoPostBack2 = value; }
        public bool AutoPostBack2 { get; set; }
    }
}
