<%@ Page Language="c#" MasterPageFile="../MasterPages/RightFrameWithInfo.master"
    CodeBehind="SurveysList.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveysList" %>

<%@ Register TagPrefix="Controls" TagName="SurveysList" Src="~/Surveys/Controls/SurveysList.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="RightFrameContent" runat="Server">
    <Controls:SurveysList runat="server" ID="SrvList" />
</asp:Content>
