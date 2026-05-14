<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true"
    CodeBehind="BreakTypesList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.BreakTypesList" %>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="false" SortedColumnName="Name" GridName="<%$CPResource:BreakTypes%>"
        PrimaryKeyColumn="Id" EnablePaging="true" PageSize="50" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" HintText="<%$CPResource:BreakType_GridHint%>">
        <Commands>
            <controls:OverlayCommand Key="Add" Title="<%$CPResource:AddBreakType%>" Caption="<%$CPResource:AddBreakType%>" SelectMode="No" Image="plus" RefreshOwner="True" Width="450" Height="270" DialogMode="Create" Url="Resources/BreakTypeProperties.aspx" />
            <controls:OverlayCommand Key="Edit" Title="<%$CPResource:EditBreakType%>" Caption="<%$CPResource:EditBreakType%>" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="450" Height="270" DialogMode="ViewEdit" IDName="Id" IDColumnName="Id" Url="Resources/BreakTypeProperties.aspx" />
            <controls:Command Key="Delete" Caption="<%$CPResource:DeleteBreakType%>" SelectMode="MultiRow" OnServerClick="DeleteBreakTypes" Image="delete" Confirmation="cnfr_DelBreakTypes" />
        </Commands>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="Add" />
            <controls:ToolbarCommandButton Key="Edit" />
            <controls:ToolbarCommandButton Key="Delete" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Add" />
            <controls:DataMenuItem Key="Edit" />
            <controls:DataMenuItem Key="Delete" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn DataFieldName="Id" HeaderText="Id" SearchColumnType="Number" Key="Id" Width="75px" />
            <controls:GeneralGridColumn DataFieldName="Name" HeaderText="<%$CPResource:Name%>" SearchColumnType="Text" Key="Name" Width="200" />
            <controls:GeneralGridColumn DataFieldName="Description" HeaderText="<%$CPResource:Description%>" SearchColumnType="Text" Key="Description" Width="100%" />
            <controls:GeneralGridColumn DataFieldName="Type" HeaderText="<%$CPResource:BreakTypeType%>" SearchColumnType="TextDropDown" Key="Type" Width="90" />
            <controls:GeneralGridColumn DataFieldName="YellowThreshold" HeaderText="<%$CPResource:BreakTypeYellowAlertHeader%>" SearchColumnType="Number" Key="YellowThreshold" Width="85" />
            <controls:GeneralGridColumn DataFieldName="RedThreshold" HeaderText="<%$CPResource:BreakTypeRedAlertHeader%>" SearchColumnType="Number" Key="RedThreshold" Width="70" />
        </Columns>
    </controls:Grid>
</asp:Content>
