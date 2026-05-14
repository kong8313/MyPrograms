<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveysTreeWithAssignments.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.SurveysTreeWithAssignments" %>
<asp:PlaceHolder ID="phFilter" runat="server">
    <div class="flex-panel flex-panel-column">
        <div class="flex-panel flex-panel-row filter-controls--with-padding filter-controls--with-margin" style="flex-wrap: wrap;">
            <asp:Label ID="lblFilter" runat="server" Text="<%$CPResource:PersonFilter%>" />
            <controls:TextBox ID="tbxFilter" runat="server" CssClass="plain_textbox plain_textbox--fixed-200px" />
            <controls:ImageButton ID="btnFilter" runat="server" ResName="Apply" ImageName="search" OnClick="btnFilter_Click" />
            <controls:ImageButton ID="btnReset" runat="server" ResName="Reset" ImageName="reset" OnClick="btnReset_Click" />
        </div>
        <div>
            <controls:CheckBox ID="chkSortMode" runat="server" AutoPostBack="true" OnCheckedChanged="chkSortMode_CheckedChanged"
                Text="<%$CPResource:SortSurveysByProjectID%>" />
        </div>
    </div>
</asp:PlaceHolder>
<div>
    <controls:BaseTreeControl runat="server" ID="tree" UseCheckBoxes="true"
        EnableAjax="true" SelectionType="Single" Height="100%" Width="100%" Visible="true" SupportDoubleClick="true">
        <NodeSettings ParentNodeImageUrl="~/svgimages/survey.svg"
            LeafNodeImageUrl="~/svgimages/survey.svg" />
        <DragDropSettings EnableDragDrop="true" AllowDrop="true" EnableDropInsertion="false" EnableExpandOnDrop="false" DragDropMode="Copy" />
        <AutoPostBackFlags NodeDropped="On" />
    </controls:BaseTreeControl>
</div>
