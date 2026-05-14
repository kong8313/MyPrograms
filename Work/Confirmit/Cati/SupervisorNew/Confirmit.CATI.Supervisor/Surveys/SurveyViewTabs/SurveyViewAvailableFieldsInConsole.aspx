<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewAvailableFieldsInConsole.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewAvailableFieldsInConsole" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoAvailableFieldsInConsole" Src="~/Surveys/Controls/SrvInfoAvailableFieldsInConsole.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoAvailableFieldsInConsole runat="server" ID="AvailableFieldsInConsole" />
</asp:Content>
