<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="States.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.States" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" style="height: 100%">
        <ContentTemplate>            
                <controls:Grid ID="grid" runat="server" EnableHeaderMenu="true" HideSelectedColumn="true" MakeMarginForExpanCollapseButton="True"
                    PrimaryKeyColumn="StateId" EnablePaging="false" EnableSorting="false" ShowFullToolbarBorders="False" OnDblClickCommand="Edit">
                    <Commands>
                        <controls:OverlayCommand Key="Edit" DialogMode="ViewEdit" SelectMode="MultiRow"
                            Title="Edit" Caption="Edit" Image="edit"
                            IDName="StateId" IDColumnName="StateId" Width="550" Height="280" Url="Resources/StateProperties.aspx" RefreshOwner="true" />
                        <Controls:Command Key="Export" Caption="Export" OnServerClick="ExportStateGroup" Image="export"/>
                    </Commands>
                    <ToolbarItems>
                        <controls:ToolbarCommandButton Key="Edit" />
                        <controls:ToolbarCommandButton Key="Export" />
                    </ToolbarItems>
                    <DataMenuItems>
                        <controls:DataMenuItem Key="Edit" />
                    </DataMenuItems>

                    <Columns>
                        <controls:GeneralGridColumn Header-Text="<%$CPResource:StateID%>" Key="StateId" DataFieldName="StateID" Width="120" />
                        <controls:GeneralGridColumn Header-Text="<%$CPResource:AaporCode%>" Key="AaporCode" DataFieldName="AaporCode" Width="70" />
                        <controls:GeneralGridColumn Header-Text="<%$CPResource:Name%>" Key="Name" DataFieldName="Name" Width="100%" />
                        <controls:GeneralGridColumn Header-Text="<%$CPResource:Priority%>" Key="Priority" DataFieldName="Priority" Width="80" />
                        <controls:BoundCheckBoxField Header-Text="<%$CPResource:DA%>" Key="DisallowActivation" DataFieldName="DA" Width="150" />
                        <controls:BoundCheckBoxField Header-Text="<%$CPResource:FcdAction%>" Key="FcdAction" DataFieldName="FcdAction" Width="200" />
                    </Columns>

                </controls:Grid>            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
