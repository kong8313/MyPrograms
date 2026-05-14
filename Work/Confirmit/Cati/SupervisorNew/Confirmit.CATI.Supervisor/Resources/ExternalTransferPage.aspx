<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ExternalTransferPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.ExternalTransferPage" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
   <controls:Grid ID="grid" runat="server" HideSelectedColumn="false" SortedColumnName="ID" GridName="<%$CPResource:ExternalTransfer%>"
                   PrimaryKeyColumn="ID" EnablePaging="true" PageSize="50" ShowFullToolbarBorders="False" OnDblClickCommand="Properties" >
        <Commands>
            <Controls:OverlayCommand Key="Add" Title="<%$CPResource:Add%>" Caption="<%$CPResource:Add%>" SelectMode="No" Image="plus" RefreshOwner="True" Width="850" Height="620" DialogMode="Create" Url="Resources/ExternalTransferProperties.aspx" Top="80" />
            <Controls:OverlayCommand Key="Properties" Title="<%$CPResource:Edit%>" Caption="<%$CPResource:Properties%>" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="850" Height="620" DialogMode="ViewEdit" IDName="ID" IDColumnName="ID" Url="Resources/ExternalTransferProperties.aspx"  Top="80" />
            <Controls:Command Key="Delete" Caption="<%$CPResource:Delete%>" SelectMode="MultiRow" OnServerClick="Delete" Image="delete" Confirmation="cnfr_DelExternalTransferNumber"/>
        </Commands>
        <ToolbarItems>  
            <controls:ToolbarCommandButton Key="Add" />
            <controls:ToolbarCommandButton Key="Properties" />
            <controls:ToolbarCommandButton Key="Delete" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Add" />
            <controls:DataMenuItem Key="Properties" />
            <controls:DataMenuItem Key="Delete" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn DataFieldName="ID" Key="ID" Hidden="True"/>
            <controls:GeneralGridColumn DataFieldName="TelephoneNumber" HeaderText="<%$CPResource:TelNumber%>" SearchColumnType="Text" Key="TelephoneNumber" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="Description" HeaderText="<%$CPResource:Description%>" SearchColumnType="Text" Key="SurveyId" Width="300%"  />
            <controls:GeneralGridColumn DataFieldName="Hidden" HeaderText="<%$CPResource:Hidden%>" SearchColumnName="Hidden" SearchColumnType="DropDown" Key="Hidden" Width="80"  />
            <controls:GeneralGridColumn DataFieldName="Count" HeaderText="<%$CPResource:AssignedSurveysCount%>" SearchColumnType="Number" Key="Count" Width="130"  />
        </Columns>
    </controls:Grid>
</asp:Content>
