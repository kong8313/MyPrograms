<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InterviewerConsoleSettingsControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.Settings.InterviewerConsoleSettingsControl" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Panel ID="Panel1" runat="server" DefaultButton="btnDefault" CssClass="tab-content">
    <controls:GeneralToolbar runat="server" ID="toolbar" LeftLabel="<%$CPResource:InterviewerConsoleToolbarButtonsHintText%>">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" Text="<%$CPResource:Save%>" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <section class="content-panel">
        <script>
            function showCompanyLogoUrlDialog() {
                top.showCompanyLogoUrlDialog();
            }
            function redirectToPerformanceMetrics() {
                top.redirectToPerformanceMetrics();
            }
        </script>
        <div class="content-panel__scroll-pane">
            <div class="hidden">
                <asp:Button ID="btnDefault" runat="server" />
            </div>

            <%--<controls:Hint runat="server" Text="<%$CPResource:InterviewerConsoleToolbarButtonsHintText%>" />--%>
            <asp:Label runat="server" CssClass="settings-group-title"><%= Strings.EnableDisableStandardInterviewerToolbarFunctions %></asp:Label>
            <div class="flex-panel flex-panel-row flex-panel-row--align-top">
                <table class="settings-table settings-table--default-columns  settings-table--fixed-labels-400px settings-table--auto-width">
                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnablePreviousPageToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnablePreviousPage" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnablePreviousPageHelpText" runat="server"
                                            HelpTextId="EnablePreviousPageToolbarButtonHelpText" TitleTextId="EnablePreviousPageToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableNextPageToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableNextPage" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableNextPageHelpText" runat="server"
                                            HelpTextId="EnableNextPageToolbarButtonHelpText" TitleTextId="EnableNextPageToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableAppointmentToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableAppointment" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableAppointmentHelpText" runat="server"
                                            HelpTextId="EnableAppointmentToolbarButtonHelpText" TitleTextId="EnableAppointmentToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableRedoToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableRedo" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="RedoHelpText" runat="server"
                                            HelpTextId="EnableRedoToolbarButtonHelpText" TitleTextId="EnableRedoToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableFastForwardToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableFastForward" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="FastForwardHelpText" runat="server"
                                            HelpTextId="EnableFastForwardToolbarButtonHelpText" TitleTextId="EnableFastForwardToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableCheckSpellingToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableCheckSpelling" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="CheckSpellingHelpText" runat="server"
                                            HelpTextId="EnableCheckSpellingToolbarButtonHelpText" TitleTextId="EnableCheckSpellingToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableLogoutAfterFinishToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableLogoutAfterFinish" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableLogoutAfterFinishHelp" runat="server"
                                            HelpTextId="EnableLogoutAfterFinishToolbarButtonHelpText" TitleTextId="EnableLogoutAfterFinishToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <table class="settings-table settings-table--default-columns  settings-table--fixed-labels-400px settings-table--auto-width">
                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableTerminateToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableTerminate" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableTerminateHelpText" runat="server"
                                            HelpTextId="EnableTerminateToolbarButtonHelpText" TitleTextId="EnableTerminateToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableTakeBreakToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableTakeBreak" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableTakeBreakHelpText" runat="server"
                                            HelpTextId="EnableTakeBreakToolbarButtonHelpText" TitleTextId="EnableTakeBreakToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableChangeTaskChoiceToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableChangeTaskChoice" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="ChangeTaskChoiceHelpText" runat="server"
                                            HelpTextId="EnableChangeTaskChoiceToolbarButtonHelpText" TitleTextId="EnableChangeTaskChoiceToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableMessageFormToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableMessageForm" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="MessageFormHelpText" runat="server"
                                            HelpTextId="EnableMessageFormToolbarButtonHelpText" TitleTextId="EnableMessageFormToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableAppointmensListToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableAppointmensList" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="AppointmensListHelpText" runat="server"
                                            HelpTextId="EnableAppointmensListToolbarButtonHelpText" TitleTextId="EnableAppointmensListToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td nowrap="nowrap"><%= Strings.EnableRefreshToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableRefresh" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="RefreshHelpText" runat="server"
                                            HelpTextId="EnableRefreshToolbarButtonHelpText" TitleTextId="EnableRefreshToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Label runat="server" CssClass="settings-group-title" Style="margin-top: 20px;"><%= Strings.EnableDisableDialerRelatedToolbarFunctions %></asp:Label>
            <div class="flex-panel flex-panel-row flex-panel-row--align-top">
                <table class="settings-table settings-table--default-columns settings-table--fixed-labels-400px  settings-table--auto-width">
                    <tr id="rowEnableRedialToolbarButtonSettings" runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableRedialToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableRedial" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="RedialHelpText" runat="server"
                                            HelpTextId="EnableRedialToolbarButtonHelpText" TitleTextId="EnableRedialListToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableHangUpToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableHangUp" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="HangUpHelpText" runat="server"
                                            HelpTextId="EnableHangUpToolbarButtonHelpText" TitleTextId="EnableHangUpToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr id="rowEnableSoftphoneIntegration" runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableSoftphoneIntegrationToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableSoftphoneIntegration" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="SoftphoneIntegrationHelpText" runat="server"
                                            HelpTextId="EnableSoftphoneIntegrationToolbarHelpText" TitleTextId="EnableSoftphoneIntegrationToolbarButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <table class="settings-table settings-table--default-columns  settings-table--fixed-labels-400px  settings-table--auto-width">
                    <tr id="rowEnableInternalCallTransfer" runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableInternalCallTransferToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableInternalCallTransfer" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="InternalCallTransferHelpText" runat="server"
                                            HelpTextId="EnableInternalCallTransferToolbarButtonHelpText" TitleTextId="EnableInternalCallTransferToolbarButtonHelpTitle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="rowEnableExternalCallTransfer" runat="server">
                        <td nowrap="nowrap"><%= Strings.EnableExternalCallTransferToolbarButtonLabel %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableExternalCallTransfer" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="ExternalCallTransferHelpText" runat="server"
                                            HelpTextId="EnableExternalCallTransferToolbarButtonHelpText" TitleTextId="EnableExternalCallTransferToolbarButtonHelpTitle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="settings-group non-first-group">
                <asp:Label runat="server" CssClass="settings-group-title"><%= Strings.GeneralInterviewerConsoleSettings %></asp:Label>
                <table class="settings-table settings-table--default-columns settings-table--auto-width">
                     <tr id="rowEnableAppointmentTimeZoneAdjustment" runat="server">
                        <td><%= Strings.EnableAppointmentTimeZoneAdjustment %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableAppointmentTimeZoneAdjustment" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableAppointmentTimeZoneAdjustmentHelpText" runat="server"
                                            HelpTextId="EnableAppointmentTimeZoneAdjustmentHelpText" TitleTextId="EnableAppointmentTimeZoneAdjustment" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="rowEnableRedialNewNumber" runat="server">
                        <td><%= Strings.EnableRedialNewNumber %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableRedialNewNumber" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableRedialNewNumberHelpText" runat="server"
                                            HelpTextId="EnableRedialNewNumberHelpText" TitleTextId="EnableRedialNewNumber" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.EnableAppointmentsOutsidePermittedShiftTimes %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableAppointmentsOutsidePermittedShiftTimes" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableAppointmentsOutsidePermittedShiftTimesHelpText" runat="server"
                                            HelpTextId="EnableAppointmentsOutsidePermittedShiftTimesHelpText" TitleTextId="EnableAppointmentsOutsidePermittedShiftTimes" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.EnableAbilityToCancelDial %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableAbilityToCancelDial" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableAbilityToCancelDialHelpText" runat="server"
                                            HelpTextId="EnableAbilityToCancelDialHelpText" TitleTextId="EnableAbilityToCancelDial" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.EnableLogoutFromErrorAndWaitingScreen %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableLogoutFromErrorAndWaitingScreen" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableLogoutFromErrorAndWaitingScreenHelpText" runat="server"
                                            HelpTextId="EnableLogoutFromErrorAndWaitingScreenHelpText" TitleTextId="EnableLogoutFromErrorAndWaitingScreen" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.EnableTwoWayMessaging %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableTwoWayMessaging" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableTwoWayMessagingHelpText" runat="server"
                                            HelpTextId="EnableTwoWayMessagingHelpText" TitleTextId="EnableTwoWayMessaging" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
		    <td><%= Strings.EnableAutomaticScrolling %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cdEnableAutomaticScrolling" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableAutomaticScrollingHelpText" runat="server"
                                            HelpTextId="EnableAutomaticScrollingHelpText" TitleTextId="EnableAutomaticScrolling" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.OrderInterviewsByPriority %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbOrderInterviewsByPriority" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="OrderInterviewsByPriorityHelpText" runat="server"
                                            HelpTextId="OrderInterviewsByPriorityHelpText" TitleTextId="OrderInterviewsByPriority" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.EnableInterviewsRandomization %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbEnableInterviewsRandomization" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="EnableInterviewsRandomizationHelpText" runat="server"
                                            HelpTextId="EnableInterviewsRandomizationHelpText" TitleTextId="EnableInterviewsRandomization" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.ManualCallsInsideShiftOnly %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbManualCallsInsideShiftOnly" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="ManualCallsInsideShiftOnlyHelpText" runat="server"
                                            HelpTextId="ManualCallsInsideShiftOnlyHelpText" TitleTextId="ManualCallsInsideShiftOnly" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td><%= Strings.ManualDialTypeSelection %></td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <controls:CheckBox ID="cbManualDialTypeSelection" runat="server" AutoPostBack="false" /></td>
                                    <td>
                                        <controls:HelpTextViewer ID="ManualDialTypeSelectionHelpText" runat="server"
                                                                 HelpTextId="ManualDialTypeSelectionHelpText" TitleTextId="ManualDialTypeSelection" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <span ID="spanCompanyLogoUrl" class="company-url-link" onclick="showCompanyLogoUrlDialog();"><%= Strings.ChangeCompanyLogo %></span>
                                    </td>
                                    <td>
                                        <controls:HelpTextViewer ID="CompanyLogoUrlHelpText" runat="server"
                                                                 HelpTextId="CompanyLogoUrlHelpText" TitleTextId="CompanyLogoUrl" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="rowEnablePerformanceMetricsLink" runat="server">
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <span ID="spanPerformanceMetricsLink" class="performance-metrics-url-link" onclick="redirectToPerformanceMetrics();"><%= Strings.PerformanceMetrics %></span>
                                    </td>
                                    <td>
                                        <controls:HelpTextViewer ID="PerformanceMetricsLinkHelpText" runat="server"
                                                                 HelpTextId="PerformanceMetricsLinkHelpText" TitleTextId="PerformanceMetricsLink" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </section>
</asp:Panel>
