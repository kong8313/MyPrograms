<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="TelephoneBlacklist.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.TelephoneBlacklist" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="false" SortedColumnName="Id" GridName="<%$CPResource:TelephoneBlacklist%>"
        PrimaryKeyColumn="Id" EnablePaging="true" PageSize="50" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" >
        <Commands>
            <Controls:OverlayCommand Key="Add" Title="Add Blacklist Number" Caption="Add" SelectMode="No" Image="plus" RefreshOwner="True" Width="800" Height="150" DialogMode="Create" Url="Resources/BlacklistNumberProperties.aspx" />
            <Controls:OverlayCommand Key="Edit" Title="Edit Blacklist Number" Caption="Edit" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="800" Height="150" DialogMode="ViewEdit" IDName="TelephoneNumber" IDColumnName="TelephoneNumber" Url="Resources/BlacklistNumberProperties.aspx" />
            <Controls:Command Key="Export" Caption="Export Blacklist" SelectMode="No" OnServerClick="ExportTelephoneBlacklist" Image="export"/>
		    <Controls:Command Key="Delete" Caption="Delete" SelectMode="MultiRow" OnServerClick="DeleteNumbersFormBlacklist" Image="delete" Confirmation="cnfr_DelTelNumber"/>
            <Controls:Command Key="DeleteAll" Caption="Delete all" SelectMode="No" OnServerClick="DeleteEntireTelephoneBlacklist" Image="delete_forever" Confirmation="cnfr_DeleteEntireTelephoneBlacklist" PromptAcceptCode="1234"/>
		</Commands>
        <ToolbarItems>  
            <controls:ToolbarCommandButton Key="Add"  />
			<controls:ToolbarCommandButton Key="Edit" />
			<controls:ToolbarCommandButton Key="Delete" />
            <controls:ToolbarCommandButton Key="DeleteAll" />
            <controls:ToolbarCommandButton Key="Export" />
            <Controls:XpMenuItem runat="server" ImageName = "publish" ToolTip="Import" OnClientClick="return false;" ID="btnImport"/>
	    </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Add" />
	        <controls:DataMenuItem Key="Edit" />
	        <controls:DataMenuItem Key="Delete" />
            <controls:DataMenuItem IsSeparator="True"/>
            <controls:DataMenuItem Key="Export" />
		</DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn DataFieldName="Id" HeaderText="Id" Key="Id" Hidden="true" />
            <controls:GeneralGridColumn DataFieldName="DisplayPattern" HeaderText="<%$CPResource:TelNumber%>" SearchColumnName="TelephoneNumber" SearchColumnType="Text" Key="TelephoneNumber" Width="30%"  />
            <controls:GeneralGridColumn DataFieldName="Timestamp" HeaderText="<%$CPResource:Timestamp%>" SearchColumnName="Timestamp" SearchColumnType="DateTime" Key="Timestamp" Width="20%"  />
            <controls:GeneralGridColumn DataFieldName="Comment" HeaderText="<%$CPResource:Comments%>" SearchColumnName="Comment" SearchColumnType="Text" Key="Comment" Width="50%"  />
        </Columns>
    </controls:Grid>
    <controls:PopupExtender ID="peImport" runat="server" MasterID="btnImport" SlaveID="pnlFileLoad"/>
    <asp:Panel ID="pnlFileLoad" runat="server">
        <table cellpadding="0" cellspacing="5" width="280px">
            <tr>
                <td align="left">
                    <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="<%$CPResource:SelectFileForImport%>">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:FileUpload CssClass="plain_textbox" ID="FileLoad" runat="server" Width="100%" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <controls:Button ID="btnFileLoad" runat="server" IsSubmit="true" OnClick="ImportNumbersToBlacklist"
                        Text="Load" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
