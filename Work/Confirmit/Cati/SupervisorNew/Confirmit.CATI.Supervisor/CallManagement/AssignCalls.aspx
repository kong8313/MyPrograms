<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
	CodeBehind="AssignCalls.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.AssignCall" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
	<controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal">
		<OKButton OnClick="SaveButtonClick" ResName="Assign" />
		<Content>
            <controls:PersonsAndGroupsList ID="personsAndGroupsList" ListName="GroupsAndPersons" DialTypeVisible="False" runat="server" />
		</Content>
	</controls:Dialog>
</asp:Content>
