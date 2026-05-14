<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ConnectOrActivateDialer.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.ConnectOrActivateDialer" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="True">
        <OKButton OnClick="OKButtonClick" runat="server" Text="<%$CPResource:ConnectOrActivateDialer%>" />
        <Content>
            <main class="content-panel" style="height: auto;">
                <controls:Hint ID="DialerHint" Text="<%$CPResource:ConnectOrActivateHintText%>" runat="server" />
                <controls:Hint ID="DialerState" Text="<%$CPResource:DialerDisconnectedAndDeactivated%>" runat="server" Visible="False" />
                <controls:Hint ID="lblMessage" Text="<%$CPResource:DialerImpossibleOperation%>" runat="server" Visible="False" />
                <table class="settings-table">
                    <tr runat="server" id="ConnectAndActivateDialerRow">
                        <td>
                            <div class="flex-panel">
                                <controls:RadioButton Checked="True" Text=" " GroupName="dialer" ID="rbConnectAndActivateDialer" runat="server" />
                                <%=Strings.ConnectAndActivateDialer %>
                            </div>
                        </td>
                    </tr>
                    <tr runat="server" id="ConnectDialerRow">
                        <td>
                            <div class="flex-panel">
                                <controls:RadioButton GroupName="dialer" Text=" " ID="rbConnectDialer" runat="server" />
                                <%=Strings.ConnectDialer %>
                            </div>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
