<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SessionForReview.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.SessionForReview" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">

    <controls:Dialog ID="SessionNameDialog" runat="server" Mode="Modal" HideHeader="true">
        <Content>
            <main class="content-panel">
                <controls:Hint ID="NewSessionHint" Text="<%$CPResource:NewSessionHintMessage%>" runat="server" />
                <table class="settings-table settings-table--controls-100percent settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblSessionName" runat="server" Text="Session name" />
                        </td>
                        <td>
                            <controls:TextBox ID="SessionNameTextBox" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="RequiredSessionNameValidator" Display="Dynamic" ControlToValidate="SessionNameTextBox"
                                ForeColor="red" ErrorMessage="<%$CPResource:Err_SessionNameRequired%>" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
        <OKButton OnClick="OkButtonClick" runat="server" />
    </controls:Dialog>

    <controls:AntiForgery ID="AntiForgery" SessionName="ReviewSessionAntiForgery" runat="server" />

    <controls:Dialog ID="SessionUrlDialog" runat="server" Mode="Modal" HideHeader="true">
        <Content>
            <main class="content-panel">
            <controls:Hint ID="SessionUrlHint" Text="<%$CPResource:SessionUrlHintMessage%>" runat="server" />
            <controls:TextBox ID="SessionUrlTextBox" runat="server" Width="100%" ReadOnly="True" />
            </main>
        </Content>
        <OKButton OnClick="CloseButtonClick" runat="server" />
    </controls:Dialog>

</asp:Content>
