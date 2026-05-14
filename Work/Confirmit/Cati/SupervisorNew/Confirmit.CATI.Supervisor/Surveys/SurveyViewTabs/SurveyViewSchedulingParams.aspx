<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewSchedulingParams.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewSchedulingParams" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoSchedulingParams" Src="~/Surveys/Controls/SrvInfo.SchedulingParams.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
                            <Controls:SrvInfoSchedulingParams runat="server" ID="SchedulingParams" />

</asp:Content>
