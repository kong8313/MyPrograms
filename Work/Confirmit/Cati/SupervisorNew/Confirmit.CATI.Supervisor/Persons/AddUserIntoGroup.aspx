<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master"
	Codebehind="AddUserIntoGroup.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Persons.AddUserIntoGroup" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
	<controls:Dialog runat="server" ID="dialogControl" HideHeader="True" EnableViewState="true" Mode="Modal">
		<okbutton onclick="OKButtonClick" Text="Add" />
		<content>
				<Controls:Grid id="userListGrid" runat="server" PrimaryKeyColumn="PersonSID">
					<Columns>
						<Controls:GeneralGridColumn HeaderText="ID" Key="PersonSID" DataFieldName="PersonSID" SearchColumnType="Number" Width="100px"/>
						<Controls:GeneralGridColumn HeaderText="Login" Key="PersonName" DataFieldName="PersonName" SearchColumnType="Text"  />
					</Columns>
				</Controls:Grid>    
			</content>
	</controls:Dialog>
</asp:Content>