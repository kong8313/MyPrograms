<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VariablesTreeControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.VariablesTreeControl" %>
<style>
    .igdt_NodeGroup
    {
        margin-top: 0px;
    }
    a.igdt_Node{
        text-decoration: none;
    }
</style>
<controls:BaseTreeControl runat="server" ID="tree" EnableAjax="false" SelectionType="Single"
    Height="100%" Width="100%" Visible="true">
    <NodeSettings ParentNodeImageUrl="~/images/small/icon_surveys_folder_closed.gif"
        LeafNodeImageUrl="~/images/small/icon_survey_folder_closed.gif" />
</controls:BaseTreeControl>
