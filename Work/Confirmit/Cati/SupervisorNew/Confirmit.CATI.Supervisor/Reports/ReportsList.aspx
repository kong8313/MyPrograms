<%@ Page language="c#" MasterPageFile="~/MasterPages/Main.Master" Codebehind="ReportsList.aspx.cs" AutoEventWireup="true" Inherits="Confirmit.CATI.Supervisor.Reports.ReportsList" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" Runat="Server">
	<controls:Grid ID="gridReports" runat="server" EnableSorting="false"
        HideRefreshButton="true"
        HideSelectedColumn="true" ShowFullToolbarBorders="False" PrimaryKeyColumn="Name">
		<Columns>
			<controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name" Width=250/>
			<controls:GeneralGridColumn HeaderText="Report Type" Key="Type" DataFieldName="Type" Width=150/>
			<controls:GeneralGridColumn HeaderText="Description" Key="Description" DataFieldName="Description" Width=100%/>
		</Columns>
	</Controls:Grid>
</asp:Content>
