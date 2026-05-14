<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewDialerSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewDialerSettings" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoDialerSetting" Src="~/Surveys/Controls/SrvInfo.DialerSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoDialerSetting runat="server" ID="SrvInfoDialerSetting" />
</asp:Content>
