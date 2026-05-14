<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonsTreeWithAssignments.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.PersonsTreeWithAssignments" %>

<asp:PlaceHolder ID="phFilter" runat="server">
    <div class="flex-panel flex-panel-row filter-controls--with-padding filter-controls--with-margin">
        <asp:Label ID="lblFilter" runat="server" Text="<%$CPResource:PersonFilter%>" />
        <controls:TextBox ID="tbxFilter" runat="server" CssClass="plain_textbox plain_textbox--fixed-200px"/>
        <controls:ImageButton ID="btnFilter" runat="server" ResName="Apply" ImageName="search" OnClick="btnFilter_Click" />
        <controls:ImageButton ID="btnReset" runat="server" ResName="Reset" ImageName="reset" OnClick="btnReset_Click" />
    </div>
</asp:PlaceHolder>
<div>
    <controls:BaseTreeControl runat="server" ID="tree"
        EnableAutoChildrenChecking="false" EnableAjax="false" SelectionType="Single"
        Height="100%" Width="100%" Visible="true" UseCheckBoxes="true" SupportDoubleClick="true">
        <NodeSettings ParentNodeImageUrl="~/svgimages/people.svg"
            LeafNodeImageUrl="~/svgimages/person.svg" />
        <DragDropSettings EnableDragDrop="true" AllowDrop="true" EnableDropInsertion="true" EnableExpandOnDrop="false" DragDropMode="Move" />
        <AutoPostBackFlags NodeDropped="On" />
    </controls:BaseTreeControl>
</div>
