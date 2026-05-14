<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="DdiNumberProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DdiNumberProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <input type="hidden" runat="server" id="selectedSurveyId" />
    <script type="text/javascript">
        function selectSurvey() {
            var settings = { height: "700px", width: "650px", top: "100px" };
            top.overlay.show('<%=Strings.SelectSurvey %>', "CallManagement/Controls/SelectSurvey.aspx", null, settings, null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;
                if (args.data) {
                    document.getElementById("<%=selectedSurveyId.ClientID %>").value = args.data;
                    Common.updatePanel('<%=ClientID %>');
                }
            });
        }

        function showMessageAndCloseFrame(message) {
            alert(message);
            top.overlay.closeLast(true);
        }
    </script>
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" CausesValidation="True" runat="server" />
        <Content>
            <main class="content-panel">
                <table width="100%" height="100%" class="settings-table">
                    <tr>
                        <td>
                            <asp:Label ID="lblTelephoneNumber" Text="<%$CPResource:DDINumber%>" runat="server" />
                            <% if (IsNew)
                            { %>
                            <controls:TextFieldValidator ID="telephoneValidator" ControlToValidate="tbTelephoneNumber"
                                IsRequired="true" FieldRequredErrorMessage="Err_EmptyTelephoneNumber" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" ValidInputExpression="^[0-9]{1,255}$" />
                            <% } %>
                        </td>
                        <td>
                            <% if (IsNew)
                            { %>
                            <controls:TextBox ID="tbTelephoneNumber" runat="server" Width="100%" />
                            <% }
                            else
                            { %>
                            <asp:Label ID="lblTelephoneNumberValue" Text="Dialer" runat="server" />
                            <% } %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDialer" Text="Dialer" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlDialers" runat="server" Width="100%"></controls:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:Survey%>" />
                        </td>
                        <td>
                            <controls:ImageButton ID="btnSurvey" Text="<%$CPResource:SelectDotDot%>" runat="server" IsSubmit="false" OnClientClick="selectSurvey();" ImageName="assignment_turned_in" />
                            <asp:Label ID="lblSurveyName" runat="server" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
