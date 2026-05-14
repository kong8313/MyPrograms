<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="GroupUserList.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.Persons.Controls.GroupUserList" %>
<controls:Grid ID="userListGrid" runat="server" PrimaryKeyColumn="ID" GridName="PersonsList">
	<commands>
		<Controls:OverlayCommand Key="Add" IDColumnName="ID" IDName="ID" Title="AddInterviewers" Caption="AddInterviewers" URL="Persons/AddUserIntoGroup.aspx" Width="650" Height="400" Top="100" Image="plus" SelectMode="No" DialogMode="Create" OnServerClick="OnInterviewersAdded" />
		<Controls:Command Key="Remove" SelectMode="MultiRow" Caption="RemoveInterviewers" OnServerClick="RemoveUser" Image="delete" Confirmation="cnfr_RemoveUsers"/>
		<Controls:Command Key="Save" Caption="Save" Image="save" OnServerClick="SaveClick" />
	</commands>
	<toolbaritems>
		<Controls:ToolbarCommandButton Key="Add"/>
		<Controls:ToolbarCommandButton Key="Remove"/>
		<Controls:ToolbarCommandButton Key="Save" ID="btnSave" runat="server"/>
	</toolbaritems>
	<DataMenuItems>
		<Controls:DataMenuItem Key="Add"/>
		<Controls:DataMenuItem Key="Remove"/>
	</DataMenuItems>
	<Columns>
		<controls:GeneralGridColumn HeaderText="ID" Key="ID" DataFieldName="Id"
			SearchColumnType="Number" Width="100" />
		<controls:GeneralGridColumn HeaderText="Login" Key="Login" DataFieldName="Name"
			SearchColumnType="Text" Width="150" />
		<controls:GeneralGridColumn HeaderText="Description" Key="Description"
			SearchColumnType="Text" DataFieldName="Description" />
	</Columns>
</controls:Grid>
