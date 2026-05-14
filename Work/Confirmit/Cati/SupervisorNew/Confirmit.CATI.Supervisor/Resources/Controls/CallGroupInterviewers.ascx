<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallGroupInterviewers.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.Controls.CallGroupInterviewers" %>
<%@ Register TagPrefix="controls" TagName="Grid" Src="~/Controls/GeneralGrid.ascx" %>
<controls:Grid ID="grid" runat="server" PrimaryKeyColumn="SID">
     <Commands>
        <Controls:OverlayCommand Key="New" Caption="New" Width="720" Height="640" Top="100" URL="Resources/AddCallGroupInterviewerAssignment.aspx" Title="AddAssignment" SelectMode="No" Image="plus" RefreshOwner="True" />
        <controls:Command Key="Deassign" Caption="DeassignInterviewers" SelectMode="MultiRow" OnServerClick="DeassignInterviewers" Image="block" Confirmation="cnfr_deassign" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="New" />        
        <controls:ToolbarCommandButton Key="Deassign" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="New" />
        <controls:DataMenuItem Key="Deassign" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn Key="SID" HeaderText="<%$CPResource:ID%>" SearchColumnType="Number" DataFieldName="SID" Width="50px" Hidden="True" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="Name" SearchColumnType="Text"
            DataFieldName="Name" Width="200px" SortIndicator="Ascending" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:Description%>" Key="Description" SearchColumnType="Text"
            DataFieldName="Description" Width="100%" />        
    </Columns>
</controls:Grid>
