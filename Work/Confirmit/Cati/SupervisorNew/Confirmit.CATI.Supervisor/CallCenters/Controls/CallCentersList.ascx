<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallCentersList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.CallCenters.Controls.CallCentersList" %>
<style>
    .DefaultCallCenter
    {
        font-weight: bold;
    }
</style>
<script>
    function refreshCallCenterInfo() {
        top.refreshCallCenterInfo();
    }
</script>
<controls:Grid ID="_callCenters" runat="server" HideSelectedColumn="true" PrimaryKeyColumn="ID" GridName="Call Centers"
    ShowFullToolbarBorders="False" EnablePaging="false" OnDblClickCommand="Edit">
    <Commands>
        <controls:OverlayCommand Key="New" Caption="New" Title="NewCallCenter" Url="CallCenters/CallCenterProperties.aspx"
            Top="250" Height="270" Width="560" DialogMode="Create" SelectMode="No" Image="plus" RefreshOwner="True"/>
        <controls:OverlayCommand Key="Edit" Caption="Properties" Url="CallCenters/CallCenterProperties.aspx"
            Image="edit" DialogMode="ViewEdit" IDName="ID" SelectMode="SingleRow" RefreshOwner = "True"
            IDColumnName="ID" Title="CallCenterProperties" Height="270" Width="560" Top="240"
            />
        <Controls:Command Key="SetDefault" Caption="SetDefaultCallCenter" OnServerClick="SetDefault" SelectMode="SingleRow" Image="assignment_add" />
        <controls:OverlayCommand Key="Delete" Caption="Delete" SelectMode="SingleRow" Url="CallCenters/DeleteCallCenter.aspx"
            Image="delete" IDColumnName="ID" Title="DeleteCallCenter" Height="320"
            IDName="ID" Width="620" Top="250" RefreshOwner="True" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="New" />
        <controls:ToolbarCommandButton Key="Edit" />
        <controls:ToolbarCommandButton Key="Delete" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="New" />
        <controls:DataMenuItem Key="Edit" />
        <controls:DataMenuItem Key="SetDefault" />
        <controls:DataMenuItem Key="Delete" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="ID" DataFieldName="ID"
            Width="80" SearchColumnType="Number" MinValue="1" MaxValue="255" />
        <controls:GeneralGridColumn Header-Text="<%$CPResource:CallCenterName%>" Key="Name"
            Width="200px" SearchColumnType="Text" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:CallCenterDescription%>" Key="Description"
            DataFieldName="Description" Width="100%" SearchColumnType="Text" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:DialerIDs%>" Key="DialerIds"
            DataFieldName="DialerIdsText" Width="100px" SearchColumnType="Text" />
        <controls:UnboundGeneralGridColumn Header-Text="Current call center" Key="IsCurrent" Width="120px" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:HidePiiShort%>" Key="HidePii" DataFieldName="HidePii"
                                    Width="80" SearchColumnType="DropDown" />
    </Columns>
</controls:Grid>
