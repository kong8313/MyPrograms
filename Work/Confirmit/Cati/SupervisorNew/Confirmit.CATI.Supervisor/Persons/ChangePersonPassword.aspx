<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangePersonPassword.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.ChangePersonPassword" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="ChangePassword" ResName="Save" />
        <Content>
            <main class="content-panel">
                <div class="settings-table__row">
                    <div class="settings-table__label">
                        <asp:Label ID="lblNewPassword" runat="server" Text="<%$CPResource:NewPassword%>"></asp:Label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbxChange" Display="Dynamic" Text="*" ErrorMessage="<%$CPResource:Err_PasswordIsEmpty%>" />
                    </div>
                    <div class="settings-table__value">

                        <controls:TextBox ID="tbxChange" TextMode="Password" runat="server" />

                    </div>
                </div>
                <div class="settings-table__row">
                    <div class="settings-table__label">
                        <asp:Label runat="server" Text="<%$CPResource:ConfirmPassword%>"></asp:Label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbxConfirmChange" Display="Dynamic" Text="*" ErrorMessage="<%$CPResource:Err_PasswordIsEmpty%>" />
                    </div>
                    <div class="settings-table__value">
                        <controls:TextBox ID="tbxConfirmChange" TextMode="Password" runat="server" />
                    </div>
                </div>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
