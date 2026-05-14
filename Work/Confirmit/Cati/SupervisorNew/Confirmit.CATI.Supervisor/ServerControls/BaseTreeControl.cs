using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Web.UI;
using Infragistics.Web.UI.NavigationControls;
using Confirmit.CATI.Supervisor.Classes;
using System.Web.Script.Serialization;
using Confirmit.CATI.Supervisor.Controls;
using Infragistics.Web.UI.Framework;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class BaseTreeControl : WebDataTree
    {
        private bool _EnableAutoChildrenChecking = true;
        protected HiddenField _ClientDataContainer = new HiddenField();
        protected Button _doubleClickEventSenderButton = new Button();
        protected Button _nodeDroppedClickEventSenderButton = new Button();

        public string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        public bool UseCheckBoxes
        {
            get { return CheckBoxMode == CheckBoxMode.BiState; }
            set
            {
                if (value)
                {
                    CheckBoxMode = CheckBoxMode.BiState;
                }
            }
        }

        public bool SupportDoubleClick
        {
            get;
            set;
        }

        public bool EnableAutoChildrenChecking
        {
            get { return _EnableAutoChildrenChecking; }
            set { _EnableAutoChildrenChecking = value; }
        }

        public new IEnumerable<DataTreeNode> AllNodes
        {
            get
            {
                return base.AllNodes.Cast<DataTreeNode>();
            }
        }

        public bool AreAllNodesChecked
        {
            get
            {
                return AllNodes.All(x => x.CheckState == CheckBoxState.Checked);
            }
        }

        public BaseTreeControl()
        {
            _doubleClickEventSenderButton.Style.Add("display", "none");
            _doubleClickEventSenderButton.Click += DoubleClickEventSenderButtonClick;

            _nodeDroppedClickEventSenderButton.Style.Add("display", "none");
            _nodeDroppedClickEventSenderButton.Click += DroppedClickEventSenderButtonClick;

        }

        protected override RunBot CreateRunBot()
        {
            return (RunBot) new CatiDataTreeBot((IControlMain) this);
        }

        public new event EventHandler<NodeDroppedEventArgs> NodeDropped;

        public event EventHandler<NodeDoubleClickEventArgs> NodeDoubleClick;

        public void CheckAllNodes()
        {
            foreach (var node in AllNodes)
            {
                node.CheckState = CheckBoxState.Checked;
            }
        }

        public void UnselectAllNodes()
        {
            foreach (var node in SelectedNodes)
            {
                node.Selected = false;
            }
        }



        protected override RendererBase CreateRenderer()
        {
            if (this.EnableClientRendering && !this.DesignMode)
                return (RendererBase)new DataTreeClientRenderer();
            return (RendererBase)new CatiDataTreeRenderer();
        }

        protected void DoubleClickEventSenderButtonClick(object sender, EventArgs e)
        {
            if (NodeDoubleClick != null)
            {
                var valuesDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(_ClientDataContainer.Value);

                var eventArgs = new NodeDoubleClickEventArgs(valuesDictionary["NodePath"], valuesDictionary["NodeKey"]);

                NodeDoubleClick(this, eventArgs);
            }
        }

        protected void DroppedClickEventSenderButtonClick(object sender, EventArgs e)
        {
            if (NodeDropped != null)
            {
                var valuesDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(_ClientDataContainer.Value);

                var eventArgs = new NodeDroppedEventArgs(valuesDictionary["NodePath"], valuesDictionary["NodeKey"]);

                NodeDropped(this, eventArgs);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClientEvents.Initialize = ClientControllerName + ".ScrollToSelected";

            if (UseCheckBoxes)
            {
                base.EnableAutoChecking = EnableAutoChildrenChecking;
                ClientEvents.SelectionChanged = ClientControllerName + ".SelectionChanged";
                ClientEvents.NodeClick = ClientControllerName + ".NodeClick";
                ClientEvents.NodePopulated = ClientControllerName + ".NodePopulated";
            }

            if (SupportDoubleClick)
            {
                NodeEditing.Enabled = true;
                NodeEditing.EnableOnDoubleClick = true;
                base.ClientEvents.NodeEditingEntering = ClientControllerName + ".NodeEditingEntering";
            }

            if (DragDropSettings.EnableDragDrop)
            {
                ClientEvents.NodeDropping = ClientControllerName + ".NodeDropping";
            }

            Controls.Add(_ClientDataContainer);
            Controls.Add(_doubleClickEventSenderButton);
            Controls.Add(_nodeDroppedClickEventSenderButton);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PageHelper.RegisterClientLibrary("client/TreeControl.js");

            Page.ClientScript.RegisterStartupScript(GetType(),
                                                    ClientControllerName,
                                                    string.Format("var {0} = new BaseTreeControl({1});", ClientControllerName, GetClientSettings()), true);
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);

            _ClientDataContainer.RenderControl(output);
            _doubleClickEventSenderButton.RenderControl(output);
            _nodeDroppedClickEventSenderButton.RenderControl(output);
        }

        private object GetClientSettings()
        {
            var settings = new
            {
                ClientDataContainerFieldId = _ClientDataContainer.ClientID,
                NodeDoubleClickEventSenderButtonId = _doubleClickEventSenderButton.ClientID,
                NodeDroppedClickEventSenderButtonId = _nodeDroppedClickEventSenderButton.ClientID
            };

            return new JavaScriptSerializer().Serialize(settings);
        }
    }

    public class CatiDataTreeBot : DataTreeBot
    {
        public CatiDataTreeBot(IControlMain iControlMain) : base(iControlMain)
        {
        }

        protected override void SaveClientProperties()
        {
            base.SaveClientProperties();
            if (this.DataTree.CheckBoxMode != CheckBoxMode.Off)
            {
                this.WriteClientOnlyProperty("uncheckedImageURL", base.StyleBot.ResolveImageUrl("checkbox_off.svg"));
                this.WriteClientOnlyProperty("checkedImageURL", base.StyleBot.ResolveImageUrl("checkbox_on.svg"));
                this.WriteClientOnlyProperty("partialImageURL",
                    base.StyleBot.ResolveImageUrl("ig_checkbox_partial.gif"));
            }
        }
    }

    public class CatiDataTreeRenderer : DataTreeRenderer
    {
        //protected override void RenderNodeExpandImage(DataTreeNode node, HtmlTextWriter writer)
        //{
        //    int nodeImageIndex = this.GetNodeImageIndex(node);
        //    if (!this._dataTree.EnableExpandImages && !this._dataTree.EnableConnectorLines)
        //        return;
        //    string str1 = this.StyleBot.ResolveImageUrl(this._imageManager[nodeImageIndex]);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Src, str1);
        //    string str2 = "";
        //    if ((node.Nodes.Cast<DataTreeNode>().Any(x => x.Visible) || node.IsEmptyParent) && this._dataTree.EnableExpandImages)
        //    {
        //        if (node.Expanded && !string.IsNullOrEmpty(this._dataTree.CollapseImageToolTip))
        //            str2 = string.Format(this._dataTree.CollapseImageToolTip, (object)node.Text);
        //        else if (!node.Expanded && !string.IsNullOrEmpty(this._dataTree.ExpandImageToolTip))
        //            str2 = string.Format(this._dataTree.ExpandImageToolTip, (object)node.Text);
        //    }
        //    writer.AddAttribute(HtmlTextWriterAttribute.Alt, str2);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Title, str2);
        //    if (!this.DesignMode)
        //        writer.AddAttribute(HtmlTextWriterAttribute.Class, nodeImageIndex.ToString());
        //    writer.RenderBeginTag(HtmlTextWriterTag.Img);
        //    writer.RenderEndTag();
        //}


        protected override void Initialize(HtmlTextWriter writer)
        {
            this._dataTree = this.Control as WebDataTree;
           
            List<string> stringList = new List<string>();
                stringList.Add("unchecked.svg");
                stringList.Add("checked.svg");
                stringList.Add("ig_checkbox_partial.gif");
            if (this._dataTree.EnableConnectorLines)
                this._imageManager = new ImageManager(this.StyleBot.ImageDirectoryResolved(), _images1, stringList);
            else
                this._imageManager = new ImageManager(this.StyleBot.ImageDirectoryResolved(), _images2, stringList);
        }

        protected override void RenderNodeCheckBox(DataTreeNode node, HtmlTextWriter writer)
        {
            using (var stringWriter = new StringWriter())
            using (var htmlWriter = new HtmlTextWriter(stringWriter))
            {
                base.RenderNodeCheckBox(node, htmlWriter);

                var sWriter = htmlWriter.InnerWriter as StringWriter;
                var content = sWriter.ToString();
                content = content.Replace("ig_checkbox_off.gif", "checkbox_off.svg");
                content = content.Replace("ig_checkbox_on.gif", "checkbox_on.svg");
                writer.Write(content);
            }
        }



        private string[] _images1 = new string[17]
        {
            "igdt_Blank.svg",
            "igdt_Empty.svg",
            "igdt_First.svg",
            "igdt_FirstPlus.svg",
            "igdt_FirstMinus.svg",
            "igdt_Middle.svg",
            "igdt_MiddlePlus.svg",
            "igdt_MiddleMinus.svg",
            "igdt_Last.svg",
            "igdt_LastPlus.svg",
            "igdt_LastMinus.svg",
            "igdt_Plus.svg",
            "igdt_Minus.svg",
            "igdt_Single.svg",
            "igdt_SinglePlus.svg",
            "igdt_SingleMinus.svg",
            "igdt_Line.svg"
        };
        private string[] _images2 = new string[3]
        {
            "igdt_Leaf.gif",
            "expand_more.svg",
            "expand_less.svg"
        };
    }
}