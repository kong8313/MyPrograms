<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SrvInfo.Assignment.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvInfo_Assignment" %>
<div class="tab-content">
<controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="SID_Calls">
    <Commands>
        <Controls:OverlayCommand Key="New" Caption="New" Width="820" Height="640" Top="50" URL="Surveys/AddOrReplaceSurveyPersonAssignment.aspx" Title="AddAssignment" SelectMode="No" Image="plus" RefreshOwner="True" />
        <Controls:OverlayCommand Key="Replace" Caption="Replace" Width="820" Height="640" Top="50" URL="Surveys/AddOrReplaceSurveyPersonAssignment.aspx?ReplaceAssignment=true" Title="ReplaceAssignment" SelectMode="No" Image="sync" RefreshOwner="True" />
        <controls:Command Key="Deassign" Caption="DeassignUsersAndGroups" SelectMode="MultiRow"
                          OnServerClick="DeassignUsers" Image="cancel" Confirmation="cnfr_deassign" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="New" />
        <controls:ToolbarCommandButton Key="Replace" />
        <controls:ToolbarCommandButton Key="Deassign" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="New" />
        <controls:DataMenuItem Key="Deassign" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn DataFieldName="SID" Key="SID" HeaderText="<%$CPResource:ID%>"
            SearchColumnType="Number" Width="100px" />
        <controls:GeneralGridColumn Key="Name" DataFieldName="Name" SearchColumnType="Text"
            HeaderText="<%$CPResource:Name%>" />
        <controls:GeneralGridColumn Key="IsGroup" DataFieldName="IsGroup" SearchColumnType="DropDown"
            HeaderText="<%$CPResource:PersonType%>" Width="100px" />
        <controls:GeneralGridColumn Key="AssignedCallsCount" DataFieldName="AssignedCallsCount"
            SearchColumnType="Number" HeaderText="<%$CPResource:Count%>" Width="100px" />
        <controls:UnboundGeneralGridColumn Key="SID_Calls" Hidden="True">
        </controls:UnboundGeneralGridColumn>
    </Columns>
</controls:Grid>
    </div>