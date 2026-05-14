<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="CallGroupProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.CallGroupProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="AddCallGroup" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--controls-100percent">
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:Name%>"></asp:Label>

                            <controls:TextFieldValidator ID="tfvPriorityGroupName" ControlToValidate="tbPriorityGroupName" IsRequired="true"
                                FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbPriorityGroupName" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="<%$CPResource:Description%>"></asp:Label>
                        </td>
                        <td>
                            <controls:TextBox ID="tbPriorityGroupDescription" runat="server" Width="100%" MaxLength="255"></controls:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.DesignStateGroup%>
                        </td>
                        <td>
                            <controls:DropDownList runat="server" ID="ddlStatesList" Width="100%" MaxLength="255"></controls:DropDownList>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
