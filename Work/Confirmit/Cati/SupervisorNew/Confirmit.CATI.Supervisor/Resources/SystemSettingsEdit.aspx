<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SystemSettingsEdit.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.SystemSettingsEdit" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">

    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="Save" Text="Save" />
        <Content>
            <main class="content-panel">
            <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                <tr>
                    <td>
                        <asp:Label Text="<%$CPResource:SystemName%>" runat="server" />
                    </td>
                    <td style="word-break: break-all;"><%#HttpUtility.HtmlEncode(DefaultCompanySetting.SystemName)%></td>
                </tr>

                <tr>
                    <td>
                        <asp:Label Text="<%$CPResource:Group%>" runat="server" />
                    </td>
                    <td><%#HttpUtility.HtmlEncode(DefaultCompanySetting.Group)%></td>
                </tr>

                <tr>
                    <td>
                        <asp:Label Text="<%$CPResource:DisplayName%>" runat="server" />
                    </td>
                    <td><%#HttpUtility.HtmlEncode(DefaultCompanySetting.DisplayName)%></td>
                </tr>

                <tr>
                    <td>
                        <asp:Label Text="<%$CPResource:Description%>" runat="server" />
                    </td>
                    <td><%#HttpUtility.HtmlEncode(DefaultCompanySetting.Description)%></td>
                </tr>

                <tr runat="server" id="trSettingValue">
                    <td>
                        <asp:Label Text="<%$CPResource:Value%>" runat="server" />
                    </td>
                    <td id="tdSettingValue" runat="server" />
                </tr>   
                
                <tr runat="server" id="trHint" class="">
                    <td colspan="2">
                        <controls:Hint ID="systemSettingHint" runat="server" />
                    </td>
                </tr>
            </table>
            </main>
        </Content>
    </controls:Dialog>

</asp:Content>
