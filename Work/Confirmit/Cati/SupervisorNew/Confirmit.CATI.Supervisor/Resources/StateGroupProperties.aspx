<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="StateGroupProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.StateGroupProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true" ResHeaderText="StateGroupProperties">
        <OKButton OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel">
                <table class="settings-table">
                    <tr>
                        <td>
                            <%=Strings.Name%>
                            <controls:TextFieldValidator ID="tfvStateGroupName" ControlToValidate="tbStateGroupName" IsRequired="true"
                                FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbStateGroupName" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
