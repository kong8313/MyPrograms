<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewAssignment.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewAssignment" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoAssignment" Src="~/Surveys/Controls/SrvInfo.Assignment.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoAssignment runat="server" ID="Assignment" />
</asp:Content>
