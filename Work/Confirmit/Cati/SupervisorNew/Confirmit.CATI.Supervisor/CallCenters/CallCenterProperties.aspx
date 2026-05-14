<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="CallCenterProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.CallCenterProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog runat="server" ID="_dialog" HideHeader="True" Mode="Modal">
        <OKButton OnClick="SaveButtonClick" Text="Save"/>
        <Content>
            <script>
                function refreshCallCenterInfo() {
                    top.refreshCallCenterInfo();
                }
            </script>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <colgroup>
                        <col style="min-width: 160px;">
                        <col style="width: 100%">
                    </colgroup>
                    <tr>
                        <td>
                            <%=Strings.Name%>
                            <controls:TextFieldValidator ID="tfvCallCenterName" ControlToValidate="tbCallCenterName"
                                IsRequired="true" FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbCallCenterName" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.Description%>
                            <controls:TextFieldValidator ID="tfvDescription" ControlToValidate="tbDescription"
                                IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbDescription" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.LocalTZ%>
                        </td>
                        <td>
                            <controls:DropDownList ID="_activeTimezones" runat="server" Width="100%" MaxLength="255"></controls:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="display: flex;">
                            <%= Strings.DialerIDs %>
                            <controls:HelpTextViewer ID="hvDialerIds" runat="server" HelpTextId="DialerIdsValidationHelpText"
                                TitleTextId="DialerIdsHelp" />
                            <controls:TextFieldValidator ID="tfvDialerIds" ControlToValidate="tbDialerIds"
                                IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue" ValidInputExpression="^[0-9 ]{0,255}$"
                                runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbDialerIds" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%= Strings.HidePii %>
                        </td>
                        <td>
                            <controls:CheckBox ID="cbHidePii" runat="server" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
