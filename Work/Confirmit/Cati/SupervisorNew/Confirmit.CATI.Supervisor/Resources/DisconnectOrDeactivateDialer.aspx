<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="DisconnectOrDeactivateDialer.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DisconnectOrDeactivateDialer" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script>
        Y.on('domready', function () {
            Y.one('input[type="checkbox"]').on('change', function (e) {
                var checkbox = e.target;
                if (checkbox.get('checked')) {
                    var confirmed = confirm("<%= Strings.DialerAttentionNotReversibleAction%>");
                    if (confirmed === false) {
                        checkbox.set('checked', false);
                    }
                }
            });
        });
    </script>
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="True">
        <OKButton OnClick="OKButtonClick" Text="<%$CPResource:DisconnectOrDeactivateDialer%>" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="DialerHint" Text="<%$CPResource:DisconnectOrDeactivateHintText%>" runat="server" />
                <controls:Hint ID="DialerState" Text="<%$CPResource:DialerDisconnectedAndDeactivated%>" runat="server" Visible="False" />
                <controls:Hint ID="lblMessage" Text="<%$CPResource:DialerImpossibleOperation%>" runat="server" Visible="False" />
                <table class="settings-table">
                    <tr runat="server" id="DisconnectAndDeactivateDialerRow">
                        <td>
                            <div class="flex-panel">
                                <controls:RadioButton GroupName="dialer" Text=" " ID="rbDisconnectAndDeactivateDialer" Checked="true" runat="server" />
                                <%=Strings.DisconnectAndDeactivateDialer %>
                            </div>
                        </td>
                    </tr>
                    <tr runat="server" id="DeactivateDialerRow">
                        <td>
                            <div class="flex-panel">
                                <controls:RadioButton GroupName="dialer" Text=" " ID="rbDeactivateDialer" runat="server" />
                                <%=Strings.DeactivateDialer %>
                            </div>
                        </td>
                    </tr>
                    <tr runat="server" id="TerminateTasksLine">
                        <td>
                            <hr />
                        </td>
                    </tr>
                    <tr runat="server" id="TerminateTasksRow">
                        <td>
                            <div class="flex-panel">
                                <controls:CheckBox ID="cbTerminateTasks" runat="server" />
                                <asp:Label ID="lblTerminateTasks" Text="<%$CPResource:TerminateTasks%>" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
