<%@ Page language="c#" MasterPageFile="~/MasterPages/Main.Master" Codebehind="CustomTimezoneAdd.aspx.cs" AutoEventWireup="True" 
Inherits="Confirmit.CATI.Supervisor.CustomTimezone.CustomTimezoneAdd" %>

<%@ Register TagPrefix="Controls" TagName="CustomTimezoneAddControl" Src="~/CustomTimezone/Controls/CustomTimezoneAddControl.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" Runat="Server">
	<Controls:CustomTimezoneAddControl id="CustomTimezoneAddControl" runat="server" />
</asp:Content>
