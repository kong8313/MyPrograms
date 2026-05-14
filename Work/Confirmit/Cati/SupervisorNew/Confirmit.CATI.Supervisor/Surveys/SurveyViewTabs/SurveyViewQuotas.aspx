<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyViewQuotas.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs.SurveyViewQuotas" %>

<%@ Register TagPrefix="Controls" TagName="SrvInfoQuotas" Src="~/Surveys/Controls/SrvInfoQuotas.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">
        function callManagement(fields, values, surveyId) {
            var pathToCallManagement = '<%=BaseRelativePath("CallManagement/CallManagement.aspx?mode=Floating")%>&ID=' + surveyId;
            top.GetWM().focusWindow(pathToCallManagement);

            PageMethods.CallManagement(fields, values, surveyId, function(queryString) {
                if (queryString === 'success') {
                    top.GetWM().openWindow(
                        pathToCallManagement,
                        "<%=GetResString("CallManagement") %>",
                        "width=1024px, height=630px,location=no,toolbar=no, menubar=no,status=no,resizable=yes,scrollbars=yes",
                        true
                    );
                } else {
                    alert(queryString);
                }
            });
        }

    </script>
    <Controls:SrvInfoQuotas runat="server" ID="SrvInfoQuotas" />
</asp:Content>
