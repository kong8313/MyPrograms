<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.master" CodeBehind="CallCentersList.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.CallCenters.CallCentersList" %>
<%@ Register TagPrefix="Controls" TagName="CallCentersList" Src="~/CallCenters/Controls/CallCentersList.ascx" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <controls:CallCentersList runat="server" ID="_callCenters" />
</asp:Content>
