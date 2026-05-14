<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallCenterSwitch.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.CallCenterSwitch" %>

<asp:UpdatePanel ID="updatePanel" runat="server"  UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel runat="server" Style="height: 100%; padding: 6px" class="XpButton" ID="pnlInfo">
            <div style="text-align: left">
                <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="White" Text="<%$CPResource:CallCenterSwitch_User%>" />
                <asp:Label ID="lbUserName" runat="server" ForeColor="White" />
            </div>
            <div style="text-align: left; margin-top: 3px">
                <asp:Label ID="Label2" runat="server" Font-Bold="True" ForeColor="White" Text="Center: " />
                <asp:Label ID="lbUserCallCenter" runat="server" ForeColor="White" />
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<script>

    var CallCenterSwitchController = function (settings) {
        this.settings = settings;
        top.Y.on('updateUserSettings', updateSettings);

        this.switchCallCenter = function () {
            var overlaySettings = { height: "112px", width: "350px", top: "150px", calledWindow: window };
            top.overlay.show("User Settings", "CallCenters/SwitchCallCenter.aspx", null, overlaySettings, null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;

                updateSettings();
                top.wm.closeAllWindows();
                top.closeAndClearInfoFrame();
                Common.refreshListFrame();
            });
        };
        
        function updateSettings() {
            Common.updatePanel("<%=updatePanel.ClientID%>");
        }
    };
    
</script>
