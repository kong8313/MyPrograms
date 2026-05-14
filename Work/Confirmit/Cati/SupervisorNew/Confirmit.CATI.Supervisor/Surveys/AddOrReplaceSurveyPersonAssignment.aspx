<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    Codebehind="AddOrReplaceSurveyPersonAssignment.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.AddOrReplaceSurveyPersonAssignment" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" Title="AssignPersonSurvey">
        <okbutton onclick="OKButtonClick"/>
        <content>
        <Controls:PersonsAndGroupsList ID="userList" runat="server" HideSelectedColumn="false" DialTypeVisible="True">
        </Controls:PersonsAndGroupsList>
        </content>
    </controls:Dialog>
</asp:Content>
