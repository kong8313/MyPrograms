<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SurveyViewGeneral.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewGeneral" %>
<%@ Register TagPrefix="Controls" TagName="SrvInfoGeneral" Src="~/Surveys/Controls/SrvInfo.General.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SrvInfoGeneral runat="server" ID="General" />
</asp:Content>
