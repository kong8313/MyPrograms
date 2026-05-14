<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallGroupStatuses.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.Controls.CallGroupStatuses" %>
<%@ Register TagPrefix="controls" TagName="Grid" Src="~/Controls/GeneralGrid.ascx" %>
<controls:Grid ID="grid" runat="server" HideSelectedColumn="False" OnDblClickCommand="ChangePriority"
    PrimaryKeyColumn="Id" AutoGenerateColumns="false" ShowFullToolbarBorders="False">
	<commands>		
		<Controls:OverlayCommand Key="Add" RefreshInfoFrame="true" SelectMode="No" Caption="Add" Title="AddStatuses" URL="Resources/AddCallGroupStatuses.aspx" Height="520"  Width="480" Image="plus"/>				
        <controls:OverlayCommand Key="ChangePriority" DialogMode="ViewEdit" SelectMode="MultiRow"
            Title="ChangePriority" Caption="ChangePriority" IDName="IDS" IDColumnName="Id" Width="325" Top="200"
            Height="170" Url="Resources/CallGroupChangePriority.aspx" Image="swap_vert" RefreshOwner="True" />
		<controls:Command Key="Delete" SelectMode="MultiRow" Caption="Delete" OnServerClick="Delete" Confirmation="CallGroupView_ConfirmationDeleteStatus" Image="delete"/>
	</commands>
	<toolbaritems>
	    <controls:ToolbarCommandButton Key="Add" />
	    <Controls:ToolbarCommandButton Key="ChangePriority"/>	    
		<controls:ToolbarCommandButton Key="Delete" />
	</toolbaritems>
	<DataMenuItems>
	    <controls:DataMenuItem Key="Add" />
		<controls:DataMenuItem Key="ChangePriority" />		
		<controls:DataMenuItem Key="Delete" />
	</DataMenuItems>
	<Columns>
		<controls:GeneralGridColumn HeaderText="ID" Key="Id" DataFieldName="Id" Width="75px" SearchColumnType="Number"/>				
		<controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name" SearchColumnType="Text"
			Width="100%" />
        <controls:GeneralGridColumn HeaderText="Priority" Key="Priority" DataFieldName="Priority" Width="175px" SearchColumnType="Number"/>		
	</Columns>
</controls:Grid>
