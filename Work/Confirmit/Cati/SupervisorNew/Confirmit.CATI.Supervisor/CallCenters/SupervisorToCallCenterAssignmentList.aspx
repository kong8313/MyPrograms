<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.master"
    CodeBehind="SupervisorToCallCenterAssignmentList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.SupervisorToCallCenterAssignmentList" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content"> 
    <controls:Grid ID="_assignmentsGrid" runat="server" EnablePaging="false" SortedColumnName="SupervisorName" GridName="Call Centers - Supervisors"
        OnDblClickCommand="Assign" PrimaryKeyColumn="SupervisorName">
        <Commands>
            <controls:OverlayCommand Key="Assign" Caption="AssignSupervisorToCallCenter" Url="CallCenters/SupervisorToCallCenterAssignment.aspx"
                Image="person" DialogMode="ViewEdit" IDName="ID" SelectMode="MultiRow"
                IDColumnName="SupervisorName" Title="AssignSupervisorToCallCenter" Height="400"
                Width="800" Top="250" RefreshOwner="True" />
        </Commands>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="Assign" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Assign" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ConfirmitSupervisorUserId%>"
                Key="SupervisorName" Width="400px" DataFieldName="SupervisorName" SearchColumnType="Text"
                SortIndicator="Ascending" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ConfirmitSupervisorFullName%>"
                Key="SupervisorFullName" Width="400px" DataFieldName="SupervisorFullName" SearchColumnType="Text"
                SortIndicator="Ascending" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:AssignedCallCenter%>" Key="CallCenterName"
                DataFieldName="CallCenterName" Width="100%" SearchColumnName="CallCenterId" SearchColumnType="DropDown" />
        </Columns>
    </controls:Grid>
</asp:Content>
