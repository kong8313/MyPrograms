<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewFilters.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewFilters" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoFilters" Src="~/Surveys/Controls/SrvInfo.Filters.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoFilters runat="server" ID="Filters" />
</asp:Content>
