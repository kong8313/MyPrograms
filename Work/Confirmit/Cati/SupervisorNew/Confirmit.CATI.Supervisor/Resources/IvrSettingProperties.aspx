<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="IvrSettingProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.IvrSettingProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OkButtonClick" CausesValidation="True" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblLanguageDescription" Text="<%$CPResource:LanguageDescription%>" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlLanguages" runat="server" Width="100%"></controls:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWrongInputAudioUrl" Text="<%$CPResource:WrongInputAudioUrl%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbWrongInputAudioUrl" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWrongInputText" Text="<%$CPResource:WrongInputText%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbWrongInputText" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWrongInputExitAudioUrl" Text="<%$CPResource:WrongInputExitAudioUrl%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbWrongInputExitAudioUrl" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWrongInputExitText" Text="<%$CPResource:WrongInputExitText%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbWrongInputExitText" runat="server" Width="100%" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
