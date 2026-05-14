<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BreakTypeProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.BreakTypeProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" CausesValidation="True" runat="server" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--controls-100percent">
                    <tr>
                        <td>
                            <asp:Label ID="lblName" Text="<%$CPResource:Name%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbName" runat="server" Width="100%" MaxLength="25" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDescription" Text="<%$CPResource:Description%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbDescription" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:BreakTypeType%>" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlType" runat="server" Width="100%"></controls:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:BreakTypeYellowAlert%>" />
                        </td>
                        <td class="settings-table__value">
                            <div class="settings-table__with-help">
                                <controls:NumericEdit ID="neYellowThreshold" MinValue="1" MaxValue="1440" Nullable="true"  runat="server" Width="100%" />
                                <div class="divInline">
                                    <controls:HelpTextViewer ID="hvYellowThreshold" runat="server" CustomWidth="400" HelpTextId="BreakTypeYellowAlertHelpText" TitleTextId="BreakTypeYellowAlertHeader" />
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:BreakTypeRedAlert%>" />
                        </td>
                        <td class="settings-table__value">
                            <div class="settings-table__with-help">
                                <controls:NumericEdit ID="neRedThreshold" MinValue="1" MaxValue="1440" Nullable="true"  runat="server" Width="100%" />
                                <div class="divInline">
                                    <controls:HelpTextViewer ID="hvRedThreshold" runat="server" CustomWidth="400" HelpTextId="BreakTypeRedAlertHelpText" TitleTextId="BreakTypeRedAlertHeader" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
