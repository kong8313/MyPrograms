<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="SrvInfo.General.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvPropertiesGeneral" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
<div class="tab-content">
    <div class="frame-dialog-header">
        <div class="frame-dialog-header__text">
            <asp:Label runat="server" ID="lbPageInfo"></asp:Label>
        </div>
        <div class="frame-dialog-header__controls flex-panel">
            <controls:GeneralToolbar ID="toolbar" runat="server">
                <controls:XpMenuItem ID="btnSave" runat="server" ImageName="save" TextId="Save" OnClick="SaveHandler" />
            </controls:GeneralToolbar>
            <asp:Button runat="server" ID="btnFilterTasks" Text="<%$CPResource:GoToTasks%>" class="open-new-button"/>
        </div>
    </div>
    <div class="tab-content__wrapper flex-panel" style="flex-wrap: wrap;">
        <table class="settings-table settings-table--no-min-width" style="width: auto; margin-right: 30px;">
            <tr runat="server" id="m_trName">
                <td class="settings-table__label">
                    <%=Strings.ProjectId%>
                </td>
                <td align="left" class="settings-table__value">
                    <div class="settings-table__goto">
                        <asp:Label ID="SrvName" runat="server" CssClass="plain_label"></asp:Label>
                        <asp:LinkButton runat="server" ID="lbtnFilterByProjectId" Text="<%$CPResource:FilterBySurveyId%>" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.SurveyProperties_ProjectName%>
                </td>
                <td align="left" class="settings-table__value">
                    <asp:Label ID="SrvDescription" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.ExtendedStatuses%>
                </td>
                <td class="settings-table__value">
                    <div class="settings-table__goto">
                        <controls:DropDownList runat="server" Width="100%" ID="lbITSDefGroup" />
                        <asp:LinkButton runat="server" ID="lbtnJumpToExtendedStatuse" Text="<%$CPResource:GoToGroup%>" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.SurveyTarget%>
                </td>
                <td class="settings-table__value">
                    <div class="settings-table__with-help">
                        <controls:TextBox ID="txtTarget" runat="server" CssClass="plain_textbox"></controls:TextBox>
                        <div class="divInline">
                            <controls:HelpTextViewer ID="HelpTextSurveyTarget" runat="server" HelpTextId="SuveyTargetHelpText" TitleTextId="SuveyTargetTitleId" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr runat="server">
                <td class="settings-table__label">
                    <%=Strings.Size%>
                </td>
                <td runat="server" id="txtSrvSize" class="settings-table__value"></td>
            </tr>
            <tr runat="server" id="m_trState">
                <td class="settings-table__label">
                    <%=Strings.State%>
                </td>
                <td runat="server" id="txtSrvState" class="settings-table__value"></td>
            </tr>
            <tr runat="server">
                <td class="settings-table__label">
                    <%=Strings.Scheduling%>
                </td>
                <td runat="server" class="settings-table__value">
                    <div class="settings-table__goto">
                        <controls:DropDownList ID="ddlSchedulingScript" runat="server" Width="100%" />
                        <asp:LinkButton runat="server" ID="lbtnJumpToScheduling" Text="<%$CPResource:GoToScheduling%>" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.SurveyProperties_DiallingMode%>
                </td>
                <td class="settings-table__value">
                    <asp:Label ID="lbDiallingMode" runat="server"></asp:Label>
                    <div class="divInline" style="margin-left: 6px">
                        <asp:Label ID="lbCallGroupsWarning" Font-Bold="True" ForeColor="Red" runat="server" Text="<%$CPResource:Warning%>" ToolTip="<%$CPResource:CallGroupsCannotBeUsedWithPredictiveDialling%>" />
                    </div>
                </td>
            </tr>
        </table>
        <table class="settings-table settings-table--no-min-width" style="width: auto;">
            <tr>
                <td class="settings-table__label">
                    <%=Strings.OpenendReview%>
                </td>
                <td runat="server" id="txtOpenendReview" class="settings-table__value"></td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.SupportTelBlacklist%>
                </td>
                <td runat="server" id="txtSupportTelBlacklist" class="settings-table__value"></td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.InterviewVoiceRecording%>
                </td>
                <td runat="server" id="txtInterviewVoiceRecording" class="settings-table__value"></td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.InterviewScreenRecording%>
                </td>
                <td runat="server" id="txtInterviewScreenRecording" class="settings-table__value"></td>
            </tr>
            <tr>
                <td class="settings-table__label">
                    <%=Strings.CallDeliveryMode%>
                </td>
                <td runat="server" class="settings-table__value">
                    <controls:DropDownList ID="ddlCallDeliveryMode" runat="server" WrapperCssClass="dropdown-control--wide">
                    </controls:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trInternalTransfer">
                <td class="settings-table__label">
                    <%=Strings.InternalCallTransfer%>
                </td>
                <td runat="server" class="settings-table__value">
                    <controls:DropDownList ID="ddlInternalTransfer" runat="server" WrapperCssClass="dropdown-control--wide">
                    </controls:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trExternalTransfer">
                <td class="settings-table__label">
                    <%=Strings.ExternalCallTransfer%>
                </td>
                <td runat="server" class="settings-table__value">
                    <controls:DropDownList ID="ddlExternalTransfer" runat="server" WrapperCssClass="dropdown-control--wide">
                    </controls:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trCallGroups">
                <td class="settings-table__label">
                    <%=Strings.CallGroups%>
                </td>
                <td class="settings-table__value">
                    <div class="settings-table__with-help">
                        <controls:DropDownList ID="ddlCallGroupsMode" runat="server" WrapperCssClass="dropdown-control--wide">
                            <asp:ListItem Text="<%$CPResource:Enabled%>" Value="1" />
                            <asp:ListItem Text="<%$CPResource:Disabled%>" Value="0" />
                        </controls:DropDownList>
                        <div class="divInline">
                            <controls:HelpTextViewer ID="hvCallGroups" runat="server" HelpTextId="SuveyViewCallGroupsHelpText" TitleTextId="CallGroups" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trQuotaForBalancing">
                <td class="settings-table__label">
                    <%=Strings.QuotaForBalancing%>
                </td>
                <td class="settings-table__value">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtnSetupBalancingParameters" Text="<%$CPResource:Configure%>"/>
                                <asp:Label ID="txtQuotaForBalancing" runat="server" EnableViewState="false" />
                            </td>
                            <td style="padding-left: 5px;">
                                <input type="image" src="~/images/icon-delete.gif" id="btnClearBalancedQuota" runat="server" style="cursor: pointer;" title="<%$CPResource:ClearBalancedQuota%>" onclick="clearBalancedQuota(); return false;" />
                                <input type="hidden" id="isBalancedQuotaCleared" runat="server" value="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr runat="server" id="trQuotaForClustering">
                <td class="settings-table__label">
                    <%=Strings.QuotaForClustering%>
                </td>
                <td class="settings-table__value">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtnSetupClusteringParameters" Text="<%$CPResource:Configure%>" />
                                <asp:Label ID="txtQuotaForClustering" runat="server" EnableViewState="false" />
                            </td>
                            <td style="padding-left: 5px;">
                                <input type="image" src="~/images/icon-delete.gif" id="btnClearClusteredQuota" runat="server" style="cursor: pointer;" title="<%$CPResource:ClearClusteredQuota%>" onclick="clearClusteredQuota(); return false;" />
                                <input type="hidden" id="isClusteredQuotaCleared" runat="server" value="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr runat="server" id="trInboundBehavior">
                <td class="settings-table__label">
                    <%=Strings.InboundBehaviorLabel%>
                </td>
                <td runat="server" class="settings-table__value">
                    <div class="settings-table__with-help">
                        <controls:DropDownList ID="ddlInboundBehaviorType" runat="server" WrapperCssClass="dropdown-control--wide">
                            <asp:ListItem Text="<%$CPResource:FindOnlyCliType%>" Value="0" />
                            <asp:ListItem Text="<%$CPResource:FindAndCreateCliType%>" Value="1" />
                            <asp:ListItem Text="<%$CPResource:CreateOnlyCliType%>" Value="2" />
                        </controls:DropDownList>
                        <div class="divInline">
                            <controls:HelpTextViewer ID="HelpTextInboundBehavior" runat="server" HelpTextId="InboundBehaviorHelpText" TitleTextId="InboundBehaviorTitleId" CustomWidth="500" />
                        </div>
                    </div>
                </td>      
            </tr>
        </table>
    </div>
</div>
<script type="text/javascript">

    Y.on("load", function () {
        var targetEditBox = document.getElementById("<%=txtTarget.ClientID%>");
        targetEditBox.blur();
    });

    function jumpToShedule(itemId) {
        top.catiGoTo.jumpToSchedulingScript(itemId);
    }

    function jumpToExtendedStatus(itemId) {
        top.catiGoTo.jumpToExtendedStatus(itemId);
    }

    function showQuotaBalancingParametersDialog(surveyId, title) {
        var settings = { height: "400px", width: "720px" };

        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            var returnValue = args.data;
            if (returnValue) {
                setBalancedQuota(returnValue, false);
            } else {
                clearBalancedQuota();
            }
        });

        top.overlay.show(title, "Surveys/QuotaBalancingParameters.aspx?SurveyId=" + surveyId, null, settings, null);
    }

    function clearBalancedQuota() {
        setBalancedQuota('', true);
    }

    function setBalancedQuota(quotaName, isQuotaCleared) {
        var btnClearBalancedQuota = document.getElementById("<%=btnClearBalancedQuota.ClientID%>");
        var selectedQuota = document.getElementById('<%=txtQuotaForBalancing.ClientID%>');

        var hidden = document.getElementById('<%=isBalancedQuotaCleared.ClientID%>');

        if (isQuotaCleared) {
            selectedQuota.innerHTML = "";
            btnClearBalancedQuota.style.visibility = "hidden";
            hidden.value = "true";
            StateChecker.MarkAsChanged();
        }
        else {
            selectedQuota.innerHTML = quotaName;
            btnClearBalancedQuota.style.visibility = "visible";
            hidden.value = "false";
        }
    }

    function showQuotaClusteringParametersDialog(surveyId, title) {
        var settings = { height: "420px", width: "470px" };

        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            var returnValue = args.data;
            if (returnValue) {
                setClusteredQuota(returnValue, false);
            }
        });

        top.overlay.show(title, "Surveys/QuotaClusteringParameters.aspx?SurveyId=" + surveyId, null, settings, null);
    }

    function clearClusteredQuota() {
        setClusteredQuota('', true);
    }

    function setClusteredQuota(quotaName, isQuotaCleared) {
        var btnClearClusteredQuota = document.getElementById("<%=btnClearClusteredQuota.ClientID%>");
        var selectedQuota = document.getElementById('<%=txtQuotaForClustering.ClientID%>');

        var hidden = document.getElementById('<%=isClusteredQuotaCleared.ClientID%>');

        if (isQuotaCleared) {
            selectedQuota.innerHTML = "";
            btnClearClusteredQuota.style.visibility = "hidden";
            hidden.value = "true";
        }
        else {
            selectedQuota.innerHTML = quotaName;
            btnClearClusteredQuota.style.visibility = "visible";
            hidden.value = "false";
        }
    }

    function filterSurveys(projectId) {
        Common.fireGlobalEvent('FilterSurveysBySurveyEvent', projectId);
    }

</script>
