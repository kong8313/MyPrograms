<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    Codebehind="SelectAutomaticSurveyDialog.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.SelectAutomaticSurveyDialog" %>
<%@ Register TagPrefix="controls" TagName="SelectSurvey" Src="~/Persons/Controls/SelectSurvey.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" ResHeaderText="SelectSurveysToAssign">
        <okbutton onclick="OKButtonClick" Text="Select" />
        <content>
			<controls:SelectSurvey ID="m_SurveyList" runat="server" />
        </content>
    </controls:Dialog>
</asp:Content>
