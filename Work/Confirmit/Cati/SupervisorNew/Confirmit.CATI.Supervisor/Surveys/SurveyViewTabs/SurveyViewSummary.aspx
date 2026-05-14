<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewSummary.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewSummary" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoSummary" Src="~/Surveys/Controls/SrvInfo.Summary.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoSummary runat="server" ID="Summary" />
</asp:Content>
