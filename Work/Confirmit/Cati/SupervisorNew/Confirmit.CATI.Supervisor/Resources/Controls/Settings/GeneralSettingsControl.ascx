<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralSettingsControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.Settings.GeneralSettingsControl" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Panel ID="Panel1" runat="server" DefaultButton="btnDefault" CssClass="tab-content">
    <controls:GeneralToolbar runat="server" ID="toolbar" LeftLabel="<%$CPResource:GeneralSiteSettingsHint%>">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" Text="<%$CPResource:Save%>" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <section class="content-panel general-settings">
        <div class="content-panel__scroll-pane">
            <div class="hidden">
                <asp:Button ID="btnDefault" runat="server" />
            </div>

            <%--<controls:Hint ID="Hint1" Text="<%$CPResource:GeneralSiteSettingsHint%>" runat="server" />--%>
            <table class="settings-table settings-table--default-columns settings-table--fixed-labels-300px">
                <tr>
                    <td nowrap="nowrap">
                        <%=Strings.FcdBehavior%>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:DropDownList runat="server" ID="ddlFcdBehaviorType"></controls:DropDownList>
                                </td>
                                <td>
                                    <controls:HelpTextViewer ID="hvFcdBehavior" runat="server" HelpTextId="FcdBehaviorHelpText"
                                        TitleTextId="FcdBehavior" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td nowrap="nowrap">
                        <%=Strings.DefaultCallDeliveryMode%>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:DropDownList runat="server" ID="ddlDefaultCallDeliveryMode"></controls:DropDownList>
                                </td>
                                <td>
                                    <controls:HelpTextViewer ID="hvDefaultCallDeliveryMode" runat="server" HelpTextId="DefaultCallDeliveryModeHelpText"
                                        TitleTextId="DefaultCalDeliveryMode" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td nowrap="nowrap">
                        <%=Strings.SupervisorEmailAddress%>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:TextBox ID="tbEmail" runat="server" AutoPostBack="false" CssClass="mail-textbox" />
                                </td>
                                <td>
                                    <controls:HelpTextViewer ID="hvtbEmail" runat="server" HelpTextId="SupervisorEmailAddressHelpText"
                                        TitleTextId="SupervisorEmailAddress" />
                                </td>
                                <td nowrap="nowrap">
                                    <asp:CustomValidator ID="cvEmail" ControlToValidate="tbEmail" Display="Dynamic" CssClass="validation-error"
                                        OnServerValidate="ValidateEmails" runat="server" ErrorMessage="<%$CPResource:EmailInvalidFormatMessage%>" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                
                 <tr>
                    <td nowrap="nowrap">
                        <%=Strings.IncludeOpenEndReviewTimeInInterviewDurations%>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:CheckBox ID="cbIncludeOpenEndReviewTimeInInterviewDurations" runat="server" AutoPostBack="false"  />
                                </td>
                                <td>
                                    <controls:HelpTextViewer ID="hvtbIncludeOpenEndReviewTimeInInterviewDurations" runat="server" HelpTextId="IncludeOpenEndReviewTimeInInterviewDurationsHelpText"
                                        TitleTextId="IncludeOpenEndReviewTimeInInterviewDurations" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <!-- Scheduled email reports are below -->
            <div class="settings-group-title">
                <%=Strings.DailyScheduledEmailReports%>
            </div>
            <div>
                <controls:Hint ID="Hint2" Text="<%$CPResource:DailyScheduledEmailReportsHint%>" runat="server" />
            </div>

            <table class="settings-table settings-table--default-columns  settings-table--fixed-labels-300px" style="width: auto;">
                <tr>
                    <td>
                        <controls:CheckBox ID="cbCallHistoryReportCheckBox" Text="Call history" runat="server" AutoPostBack="false"
                            onclick="OnReportEnabled(this)" />
                    </td>
                    <td>
                        <controls:DropDownList runat="server" ID="ddlCallHistoryReportHour" WrapperCssClass="dropdown-hours"></controls:DropDownList>

                    </td>
                    <td id="tdCallHistoryReportRecepients">
                        <controls:TextBox ID="CallHistoryReportRecepientsTextBox" runat="server" AutoPostBack="false" CssClass="mail-textbox" />
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="CallHistoryReportHelpViewer" runat="server" HelpTextId="CallHistoryReportHelpText"
                            TitleTextId="CallHistoryReportHelpTitle" />

                    </td>
                    <td class="settings-table__label-with-indent">
                        <controls:CheckBox ID="cbIncludeReplicatedVariables" runat="server" Text="<%$CPResource:IncludeReplicatedVariables%>" AutoPostBack="false"
                                           Checked="false" onclick="OnIncludeReplicatedVariables(this)" />
                    </td>
                    <td>
                        <controls:TextBox ID="ReplicatedVariablesTextBox" runat="server" AutoPostBack="False" />
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="IncludeReplicatedVariablesHelpViewer" runat="server" HelpTextId="IncludeReplicatedVariablesHelpText"
                                                 TitleTextId="IncludeReplicatedVariablesHelpTitle" />
                    </td>
                </tr>
                <tr class="settings-table__row--no-paddings">
                    <td colspan="2"></td>
                    <td>
                        <asp:CustomValidator ID="cvCallHistoryReport" ControlToValidate="CallHistoryReportRecepientsTextBox" Display="Dynamic" CssClass="validation-error"
                                             ValidateEmptyText="True" OnServerValidate="ValidateEmailsDoNotAllowEmptyEmailList" runat="server" ErrorMessage="<%$CPResource:EmailInvalidFormatMessage%>" />
                    </td>
                    <td colspan="2"></td>
                    <td colspan="2">
                        <asp:CustomValidator ID="cvReplicatedVariables" ControlToValidate="ReplicatedVariablesTextBox" Display="Dynamic" CssClass="validation-error"
                                                         ValidateEmptyText="True" OnServerValidate="ValidateVariablesDoNotAllowEmptyVariables" runat="server" ErrorMessage="<%$CPResource:ReplicatedVariablesInvalidFormatMessage%>" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <controls:CheckBox ID="cbSurveyOverviewReportCheckBox" Text="Survey overview" runat="server" AutoPostBack="false"
                            onclick="OnReportEnabled(this)" />
                    </td>
                    <td>
                        <controls:DropDownList runat="server" ID="ddlSurveyOverviewReportHour" WrapperCssClass="dropdown-hours"></controls:DropDownList>

                    </td>
                    <td id="tdSurveyOverviewReportRecepients">
                        <div nowrap="nowrap">
                            <controls:TextBox ID="SurveyOverviewReportRecepientsTextBox" runat="server" AutoPostBack="false" CssClass="mail-textbox" />
                        </div>
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="SurveyOverviewReportHelpViewer" runat="server" HelpTextId="SurveyOverviewReportHelpText"
                            TitleTextId="SurveyOverviewReportHelpTitle" />
                    </td>
                </tr>
                <tr class="settings-table__row--no-paddings">
                    <td colspan="2"></td>
                    <td>
                        <asp:CustomValidator ID="cvSurveyOverviewReport" ControlToValidate="SurveyOverviewReportRecepientsTextBox" Display="Dynamic" CssClass="validation-error"
                                             ValidateEmptyText="True" OnServerValidate="ValidateEmailsDoNotAllowEmptyEmailList" runat="server" ErrorMessage="<%$CPResource:EmailInvalidFormatMessage%>" />
                    </td>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td>
                        <controls:CheckBox ID="cbSurveyProductivityReportCheckBox" Text="Survey productivity" runat="server" AutoPostBack="false"
                            onclick="OnReportEnabled(this)" />
                    </td>
                    <td>
                        <controls:DropDownList runat="server" ID="ddlSurveyProductivityReportHour" WrapperCssClass="dropdown-hours"></controls:DropDownList>
                    </td>
                    <td id="tdSurveyProductivityReportRecepients">
                        <controls:TextBox ID="SurveyProductivityReportRecepientsTextBox" runat="server" AutoPostBack="false" CssClass="mail-textbox" />
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="SurveyProductivityReportHelpViewer" runat="server" HelpTextId="SurveyProductivityReportHelpText"
                            TitleTextId="SurveyProductivityReportHelpTitle" />
                    </td>
                    <td nowrap="nowrap">
                        
                    </td>
                </tr>
                <tr class="settings-table__row--no-paddings">
                    <td colspan="2"></td>
                    <td>
                        <asp:CustomValidator ID="cvSurveyProductivityReport" ControlToValidate="SurveyProductivityReportRecepientsTextBox" Display="Dynamic" CssClass="validation-error"
                                             ValidateEmptyText="True" OnServerValidate="ValidateEmailsDoNotAllowEmptyEmailList" runat="server" ErrorMessage="<%$CPResource:EmailInvalidFormatMessage%>" />
                    </td>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td>
                        <controls:CheckBox ID="cbInterviewerProductivityReportCheckBox" Text="Interviewer productivity" runat="server" AutoPostBack="false"
                            onclick="OnReportEnabled(this)" />
                    </td>
                    <td>
                        <controls:DropDownList runat="server" ID="ddlInterviewerProductivityReportHour" WrapperCssClass="dropdown-hours"></controls:DropDownList>

                    </td>
                    <td id="tdInterviewerProductivityReportRecepients">
                        <controls:TextBox ID="InterviewerProductivityReportRecepientsTextBox" runat="server" AutoPostBack="false" CssClass="mail-textbox" />
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="InterviewerProductivityReportHelpViewer" runat="server"
                            HelpTextId="InterviewerProductivityReportHelpText" TitleTextId="InterviewerProductivityReportHelpTitle" />
                    </td>
                    <td id="productivityReportTemplateLabel" class="settings-table__label-with-indent">
                        <%=Strings.InterviewerProductivityReportAppliedTemplate %>
                    </td>
                    <td>
                        <controls:DropDownList ID="ddlCustomizationTemplate" runat="server" /></td>
                    <td>
                        <controls:HelpTextViewer ID="InterviewerProductivityReportTemplateHelpViewer" runat="server"
                                                 HelpTextId="InterviewerProductivityReportTemplateSelectionHelp" TitleTextId="InterviewerProductivityReportAppliedTemplate" />
                    </td>
                </tr>
                <tr class="settings-table__row--no-paddings">
                    <td colspan="2"></td>
                    <td>
                        <asp:CustomValidator ID="cvInterviewerProductivityReport" ControlToValidate="InterviewerProductivityReportRecepientsTextBox" Display="Dynamic" CssClass="validation-error"
                                             ValidateEmptyText="True" OnServerValidate="ValidateEmailsDoNotAllowEmptyEmailList" runat="server" ErrorMessage="<%$CPResource:EmailInvalidFormatMessage%>" />
                    </td>
                    <td colspan="2"></td>
                    <td colspan="2">
                        
                    </td>
                </tr>
            </table>

            <controls:Hint ID="Hint3" Text="<%$CPResource:RoutineMaintenanceSettingsHint%>" runat="server" />

            <table class="settings-table settings-table--default-columns  settings-table--fixed-labels-300px" style="width: auto;">
                <tr>
                    <td><%= Strings.RoutineMaintenanceDailyShiftLabel %></td>
                    <td>
                        <controls:DateTimeEditor ID="RoutineMaintenanceDailyShiftTime" runat="server" HorizontalAlign="Left"
                            EditModeFormat="HH:mm:ss" Nullable="false" MinimumNumberOfValidFields="3">
                            <Buttons SpinButtonsDisplay="OnRight">
                            </Buttons>
                        </controls:DateTimeEditor>
                    </td>
                    <td>
                        <controls:HelpTextViewer ID="RoutineMaintenanceDailyShiftTimeHelpViewer" runat="server"
                            HelpTextId="RoutineMaintenanceDailyShiftTimeHelpText" TitleTextId="RoutineMaintenanceDailyShiftTimeHelpTitle" />
                    </td>
                    <td nowrap="nowrap">&nbsp;</td>
                </tr>
                <tr>
                    <td><%= Strings.RoutineMaintenanceWeeklyShiftDayLabel %></td>
                    <td>
                        <controls:DropDownList ID="RoutineMaintenanceWeeklyShiftDayList" runat="server" /></td>
                    <td>
                        <controls:HelpTextViewer ID="RoutineMaintenanceWeeklyShiftDayHelpViewer" runat="server"
                            HelpTextId="RoutineMaintenanceWeeklyShiftDayHelpText" TitleTextId="RoutineMaintenanceWeeklyShiftDayHelpTitle" />
                    </td>
                    <td nowrap="nowrap">
                        <asp:CustomValidator ID="RoutineMaintenanceWeeklyShiftDayValidator" ControlToValidate="RoutineMaintenanceWeeklyShiftDayList" Display="Dynamic" CssClass="validation-error"
                            ValidateEmptyText="True" OnServerValidate="ValidateRoutineMaintenanceWeeklyShiftDay" runat="server" ErrorMessage="<%$CPResource:RoutineMaintenanceWeeklyShiftDayInvalidFormatMessage%>" />
                    </td>
                </tr>
                <tr>
                    <td><%= Strings.RoutineMaintenanceMonthlyShiftWeekLabel %></td>
                    <td>
                        <controls:TextBox ID="RoutineMaintenanceMonthlyShiftWeek" runat="server" AutoPostBack="false" /></td>
                    <td>
                        <controls:HelpTextViewer ID="RoutineMaintenanceMonthlyShiftWeekHelpViewer" runat="server"
                            HelpTextId="RoutineMaintenanceMonthlyShiftWeekHelpText" TitleTextId="RoutineMaintenanceMonthlyShiftWeekHelpTitle" />
                    </td>
                    <td nowrap="nowrap">
                        <asp:CustomValidator ID="RoutineMaintenanceMonthlyShiftWeekValidator" ControlToValidate="RoutineMaintenanceMonthlyShiftWeek" Display="Dynamic" CssClass="validation-error"
                            ValidateEmptyText="True" OnServerValidate="ValidateRoutineMaintenanceMonthlyShiftWeek" runat="server" ErrorMessage="<%$CPResource:RoutineMaintenanceMonthlyShiftWeekInvalidFormatMessage%>" />
                    </td>
                </tr>
                <tr>
                    <td><%= Strings.RoutineMaintenanceDurationLabel %></td>
                    <td>
                        <controls:DateTimeEditor ID="RoutineMaintenanceDuration" runat="server" HorizontalAlign="Left"
                            EditModeFormat="HH:mm:ss" Nullable="false" MinimumNumberOfValidFields="3">
                            <Buttons SpinButtonsDisplay="OnRight">
                            </Buttons>
                        </controls:DateTimeEditor>

                    </td>
                    <td>
                        <controls:HelpTextViewer ID="RoutineMaintenanceDurationHelpViewer" runat="server"
                            HelpTextId="RoutineMaintenanceDurationHelpText" TitleTextId="RoutineMaintenanceDurationHelpTitle" />
                    </td>
                    <td nowrap="nowrap">&nbsp;</td>
                </tr>
            </table>

            <controls:Hint ID="HintTimeZonesBalancing" Text="<%$CPResource:TimezonesBalancingSettingsHint%>" runat="server" />

            <table class="settings-table settings-table--default-columns settings-table--fixed-labels-300px">
                <tr>
                    <td nowrap="nowrap">
                        <%=Strings.EndOfShiftThreshold%>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <controls:DropDownList runat="server" ID="EndOfShiftThreshold"></controls:DropDownList>
                                </td>
                                <td>
                                    <controls:HelpTextViewer ID="HelpTextEndOfShiftThreshold" runat="server" HelpTextId="EndOfShiftThresholdHelpText"
                                        TitleTextId="EndOfShiftThreshold" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </section>
</asp:Panel>

<script language="javascript" type="text/javascript">

    function OnIncludeReplicatedVariables(checkBox) {
        OnReportEnabled(checkBox);
    }

    function OnReportEnabled(checkBox) {
        var checkboxNode = Y.one(checkBox);
        var row = checkboxNode.ancestor("tr");
        row.all("input[type='text'], select").set("disabled", !checkBox.checked);

        if (checkBox.id === "<%=cbCallHistoryReportCheckBox.ClientID%>" || checkBox.id === "<%=cbIncludeReplicatedVariables.ClientID%>") {
            checkBox = document.getElementById("<%=cbCallHistoryReportCheckBox.ClientID%>");
            document.getElementById("<%=CallHistoryReportRecepientsTextBox.ClientID%>").disabled = !checkBox.checked;
            document.getElementById("<%=cbIncludeReplicatedVariables.ClientID%>").disabled = !checkBox.checked;
            document.getElementById("<%=ReplicatedVariablesTextBox.ClientID%>").disabled = 
                !document.getElementById("<%=cbIncludeReplicatedVariables.ClientID%>").checked || !checkBox.checked;
        }

        if (checkBox.id === "<%=cbInterviewerProductivityReportCheckBox.ClientID%>") {
            if (checkBox.checked) {
                document.getElementById("productivityReportTemplateLabel").style.opacity = 1;
                document.getElementById("<%=ddlCustomizationTemplate.ClientID%>").disabled = false;
            } else {
                document.getElementById("productivityReportTemplateLabel").style.opacity = 0.3;
                document.getElementById("<%=ddlCustomizationTemplate.ClientID%>").disabled = true;
            }
        }
    }

    Y.on('load', function () {
        OnReportEnabled(document.getElementById("<%=cbCallHistoryReportCheckBox.ClientID%>"));
        OnReportEnabled(document.getElementById("<%=cbSurveyProductivityReportCheckBox.ClientID%>"));
        OnReportEnabled(document.getElementById("<%=cbSurveyOverviewReportCheckBox.ClientID%>"));
        OnReportEnabled(document.getElementById("<%=cbInterviewerProductivityReportCheckBox.ClientID%>"));
        YUI().use('event-valuechange', function (Y) {

            var oldFcdValue = Y.one('#<%=ddlFcdBehaviorType.ClientID%>').get('value');

            Y.one('#<%=ddlFcdBehaviorType.ClientID%>').on('change', function (e) {
                if (oldFcdValue != Y.one('#<%=ddlFcdBehaviorType.ClientID%>').get('value')) {
                        if (!confirm('<%=Strings.ConfirmChangingOfFcdBehavior%>')) {
                            Y.one('#<%=ddlFcdBehaviorType.ClientID%>').set('value', oldFcdValue);
                        }
                    }

                });
        });

    });

</script>

<style type="text/css">
    .general-settings .dropdown-control {
        width: 290px;
    }

    .general-settings .dropdown-control.dropdown-hours {
        width: 70px;
    }

    .general-settings .plain_textbox {
        width: 290px;
    }
</style>
