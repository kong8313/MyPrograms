<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonitoringConsoleSettingsControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.Settings.MonitoringConsoleSettingsControl" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Panel ID="Panel1" runat="server" DefaultButton="btnDefault">
    <controls:GeneralToolbar runat="server" ID="toolbar" LeftLabel="<%$CPResource:MonitoringConsoleToolbarButtonsHintText%>">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" Text="<%$CPResource:Save%>" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <section class="content-panel">
        <div class="hidden">
            <asp:Button ID="btnDefault" runat="server" />
        </div>

        <div class="flex-panel flex-panel-row flex-panel-row--align-top">
            <table class="settings-table settings-table--default-columns">
                <tr id="coachingRow" runat="server">
                    <td nowrap="nowrap" width="500"><%= Strings.AllowMonitoringCoachingMode %></td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                   <controls:CheckBox ID="cdAllowMonitoringCoachingMode" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                   <controls:HelpTextViewer ID="AllowMonitoringCoachingModeHelpText" runat="server"
                                        HelpTextId="AllowMonitoringCoachingModeHelpText" TitleTextId="AllowMonitoringCoachingModeTitle" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="bargingRow" runat="server">
                    <td nowrap="nowrap" width="500"><%= Strings.AllowMonitoringBargingMode %></td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:CheckBox ID="cdAllowMonitoringBargingMode" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                   <controls:HelpTextViewer ID="AllowMonitoringBargingModeHelpText" runat="server"
                                        HelpTextId="AllowMonitoringBargingModeHelpText" TitleTextId="AllowMonitoringBargingModeTitle" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                
            </table>
        </div>
    </section>
</asp:Panel>
