<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ChangeAutomaticSurvey.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Persons.ChangeAutomaticSurvey" %>

<%@ Register TagPrefix="controls" TagName="SelectSurvey" Src="~/Persons/Controls/SelectSurvey.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" ResHeaderText="SelectSurveysToAssign">
        <OKButton OnClick="SelectButtonClick" Text="Select" />
        <Content>
            <controls:SelectSurvey ID="m_SurveyList" runat="server" />
        </Content>
    </controls:Dialog>
</asp:Content>
