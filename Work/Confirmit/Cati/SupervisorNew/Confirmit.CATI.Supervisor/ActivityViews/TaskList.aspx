<%@ Page AutoEventWireup="true" CodeBehind="TaskList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.TaskList"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<%@ Register Src="~/ActivityViews/Controls/ActivityStatusBar.ascx" TagName="ActivityStatusBar"
    TagPrefix="controls" %>
<%@ Register TagPrefix="controls" TagName="SurveyAlertsList" Src="~/ActivityViews/Controls/SurveyAlertsList.ascx" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">

    <script type="text/javascript" language="javascript">                

        function CF(confirmation) {
            if (confirm(confirmation)) {
                return true;
            }
            else {
                return false;
            }
        }

        function stopMonitoring(event, postBackFunction) {
            event.stopPropagation();
            if (CF("<%=Strings.cf_StopMonitoring%>")) {
                postBackFunction();
            };
        }

        /*send message function*/
        function sm(event, personId) {
            event.stopPropagation();
            messageSender.sendMessage('MessageRecipientType=Interviewer&DisableOffline=true&IDS=' + personId);
        }

        function isMonitoringInBrowserStarted(monitoringWindow) {
            return monitoringWindow && monitoringWindow.opener && !monitoringWindow.opener.closed;
        }

        function closeMonitoringWindowIfNeeded(personId) {
            var monitoringWindow = window["catiMonitoring_" + personId];
            if (isMonitoringInBrowserStarted(monitoringWindow)) {
                monitoringWindow.close();
            }
        }

        function switchMonitoring(event, sessionKey, postBackFunction) {
            event.stopPropagation();
            var settings = { height: "150px", width: "390px", top: "150px" };

            top.overlay.show("<%=Strings.Monitoring%>", "ActivityViews/TelephoneNumberDialog.aspx?SessionKey=" + sessionKey, null, settings, null);

            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;

                postBackFunction();

            });
        }

        function tt(event, personId) {
            event.stopPropagation();
            var settings = { height: "350px", width: "500px", top: "100px" };

            top.overlay.show("<%=Strings.TerminateTask%>", "ActivityViews/TerminateTask.aspx?PersonId=" + personId, null, settings, null);

            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;
                Common.updatePanel(statusPanelId);

                closeMonitoringWindowIfNeeded(personId);
            });
        }

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
            ActivityViews.subscribeForContextMenu('TaskList');
        });

        function showSelectAutomaticSurveyDialog(personId, title, width, height) {

            var settings = { height: height + "px", width: width + "px" };
            top.overlay.show(title, "Persons/ChangeAutomaticSurvey.aspx?IsGroup=false&ObjectSid=" + personId, null, settings, null);
        }

    </script>
    <main class="activity-view-panel">
        <div class="activityViewHeader">
            <div class="activity-view-header">
                <div class="activity-view-header__title">
                    <h2><%=Title %></h2>
                </div>
                <div class="activity-view-header__actions">
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnToolBarHelp" runat="server" ButtonType="Button" ImageName="help"
                            Text="<%$CPResource:Help%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnClose" runat="server" ButtonType="Button" ImageName="close"
                            OnClientClick="window.close()" Text="<%$CPResource:Close%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
            </div>
            <div class="activity-view-toolbar">
                <div class="activity-view-toolbar__left">
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnRefresh" runat="server" AutoPostBack="false" ButtonType="Button"
                            ImageName="refresh" OnClientClick="Common.updatePanel(statusPanelId);" Text="<%$CPResource:Refresh%>" />
                        <controls:XpMenuItem runat="server" ButtonType="Generic">
                            <controls:DropDownList ID="ddlRefresh" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRefresh_SelectedIndexChanged">
                                <asp:ListItem Text="<%$CPResource:RefreshRate_None%>" Value="0">
                                </asp:ListItem>
                                <asp:ListItem Selected="true" Text="<%$CPResource:RefreshRate_15sec%>" Value="15000">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_30sec%>" Value="30000">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_45sec%>" Value="45000">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_1min%>" Value="60000">
                                </asp:ListItem>
                            </controls:DropDownList>
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
                <div class="activity-view-toolbar__right">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" class="flex-panel flex-panel-row">
                        <ContentTemplate>
                            <div class="cati-controls-menu cati-controls-menu--justify">
                                <div class="flex-panel flex-panel-row">
                                    <asp:Label ID="lblDialType" runat="server" CssClass="plain_label" Style="padding-right: 10px;"
                                        Text="<%$CPResource:DialTypeName%>"></asp:Label>
                                    <controls:DialTypeDropDownList ID="ddlDialType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDialType_SelectedIndexChanged" AddAllOption="True" />
                                </div>
                                <controls:CheckBox Text="Show only alerts" ID="cbShowOnlyAlerts"
                                    Checked="false" runat="server" AutoPostBack="true" />
                                <controls:CheckBox Text="<%$CPResource:AlertsOnTop%>" ID="cbAlertsOnTop"
                                    Checked="true" runat="server" AutoPostBack="true" />
                                <controls:CheckBox Text="<%$CPResource:TlAllCallCenters%>" ID="cbAllCallCenters"
                                    Checked="false" runat="server" AutoPostBack="true" Visible="false"/>
                                <asp:UpdatePanel ID="updatePanel2" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" RenderMode="Inline">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <controls:CheckBox Text="<%$CPResource:IvrAgentCheckboxText%>" ID="cbIvrAgent"
                                            Checked="False" runat="server" AutoPostBack="true" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <controls:XpMenu ID="menu" runat="server">
                                <controls:XpMenuItem ID="btnSurveys" runat="server" ButtonType="ToggleButton" ImageName="assignment_turned_in"
                                    Text="<%$CPResource:Surveys%>" />
                                <controls:XpMenuItem ID="btnInterviewers" runat="server" ButtonType="ToggleButton" ImageName="persons"
                                    Text="<%$CPResource:Interviewers%>" />
                                <controls:XpMenuItem ID="btnAlerts" runat="server" ButtonType="ToggleButton" ImageName="alert_outlined"
                                    OnClientClick="return false;" Text="<%$CPResource:Alerts%>" />
                            </controls:XpMenu>
                            <%--When an UpdatePanel control is not inside another UpdatePanel control,
                                    the panel is updated according to the settings of the UpdateMode and ChildrenAsTriggers properties,
                                    together with the collection of triggers. When an UpdatePanel control is inside another UpdatePanel control,
                                    the child panel is automatically updated when the parent panel is updated.--%>
                            <controls:DataMenu runat="server" ID="gridContextMenu" EnableViewState="False">
                            </controls:DataMenu>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnExport" runat="server" ButtonType="Button" ImageName="export"
                            OnClientClick="ActivityViews.exportView(hiddenExportId);" Text="<%$CPResource:Export%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
            </div>
        </div>
        <div class="activityViewBody flex-panel--all-awailable-space">
            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AlertsList" EventName="AlertsChanged" />
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <controls:ScrollableDiv ID="ScrollableDiv1" runat="server">
                        <controls:HierarchicalGridEx GridLines="Both" ID="m_grid" runat="server"
                            DataKeyNames="PersonSID,IsMonitored,ProjectId,MonitoringSessionID,InterviewerName,IsWebConsole"
                            HideToggleColumn="true" OnRowDataBound="gridSurveys_OnRowDataBound" RenderHierarchicalRows="false">
                            <HeaderStyle CssClass="header" Wrap="false" />
                            <RowStyle CssClass="row" />
                            <SelectedRowStyle CssClass="tableRowSelectedCell" />
                            <AlternatingRowStyle CssClass="altrow" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <controls:SvgImage ID="imgAlert" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <controls:ClickablePanel runat="server" ID="m" ClientIDMode="Static" OnCommand="switchMonitoring" CssClass="monitoring-state" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Panel runat="server" ID="sm" ClientIDMode="Static" CssClass="sm" ToolTip="Send Message" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="InterviewID" HeaderText="<%$CPResource:ID%>" SortExpression="InterviewID" />
                                <asp:BoundField DataField="SurveySID" SortExpression="SurveySID" Visible="false" />
                                <asp:BoundField DataField="PersonSID" SortExpression="PersonSID" Visible="false" />
                                <asp:BoundField DataField="ProjectId" HeaderText="<%$CPResource:ProjectId%>" SortExpression="ProjectId" />
                                <asp:BoundField DataField="ProjectName" ItemStyle-CssClass="hierarchical-grid__column-project-name" HeaderText="<%$CPResource:ProjectName%>"
                                    SortExpression="ProjectName" />
                                <asp:BoundField DataField="InterviewerName" HeaderText="<%$CPResource:Interviewer%>"
                                    SortExpression="InterviewerName" />
                                 <asp:BoundField DataField="CallCenterName" HeaderText="<%$CPResource:CallCenter%>"
                                    SortExpression="CallCenterName" Visible="false" />
                                <asp:BoundField DataField="State" HeaderText="<%$CPResource:Question%>" SortExpression="State" />
                                <asp:BoundField DataField="SecondsElapsed" HeaderText="<%$CPResource:LastSubmissionSeconds%>"
                                    SortExpression="SecondsElapsed" />
                                <asp:TemplateField HeaderText="<%$CPResource:Duration%>" SortExpression="InterviewDurationInSeconds">
                                    <ItemTemplate>
                                        <asp:Label ID="InterviewDurationInSeconds" runat="server" ClientIDMode="Static" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TimeCallDelivered" DataFormatString="{0:T}" HeaderText="<%$CPResource:Delivered%>" SortExpression="TimeCallDelivered" />
                                <asp:TemplateField HeaderText="<%$CPResource:OpenEndReviewInSeconds%>" SortExpression="OpenEndReviewInSeconds">
                                    <ItemTemplate>
                                        <asp:Label ID="OpenEndReviewInSeconds" runat="server" ClientIDMode="Static" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TimezoneName" HeaderText="<%$CPResource:Timezone%>" SortExpression="TimezoneName" />
                                <asp:BoundField DataField="StatusLogout" HeaderText="<%$CPResource:Login%>" SortExpression="StatusLogout" />
                                <asp:BoundField DataField="InterviewState" HeaderText="<%$CPResource:State%>" SortExpression="InterviewState" />
                                <asp:BoundField DataField="LastKeepAliveTime" DataFormatString="{0:T}" HeaderText="<%$CPResource:KeepAlive%>"
                                    SortExpression="LastKeepAliveTime" />
                                <asp:BoundField DataField="LoggedInToDialer" HeaderText="<%$CPResource:Dialer%>"
                                    SortExpression="LoggedInToDialer" />
                                <asp:BoundField DataField="DiallingMode" HeaderText="<%$CPResource:DiallingMode%>"
                                    SortExpression="DiallingMode" />
                                <asp:BoundField DataField="ProblemState" HeaderText="<%$CPResource:Problem%>" SortExpression="ProblemState" />
                                <asp:BoundField DataField="IsMonitored" HeaderText="IsMonitored" SortExpression="IsMonitored"
                                    Visible="false" />
                                <asp:BoundField DataField="MonitoringSessionID" HeaderText="MonitoringSessionID"
                                    SortExpression="MonitoringSessionID" Visible="false" />
                                <asp:BoundField DataField="IsWebConsole" HeaderText="IsWebConsole"
                                    SortExpression="IsWebConsole" Visible="false" />
                                <asp:BoundField DataField="CallOutcome" HeaderText="<%$CPResource:InterviewerStatus%>"
                                    SortExpression="CallOutcome" Visible="false" />
                                <asp:BoundField DataField="StationIdentifier" HeaderText="<%$CPResource:StationIdentifier%>"
                                    SortExpression="StationIdentifier" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Panel runat="server" ID="tt" ClientIDMode="Static" CssClass="tt" ToolTip="Force interviewer to be logged out" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </controls:HierarchicalGridEx>
                    </controls:ScrollableDiv>
                    <div id="hiddenDiv" style="display: none">
                        <asp:Button ID="btnHiddenExport" runat="server" OnClick="btnExport_Click" />
                    </div>
                    <div id="start-live-monitoring" style="display: none"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="activityViewFooter">
            <asp:UpdatePanel ID="statusBarUpdatePanel" runat="server" ChildrenAsTriggers="true"
                UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <controls:ActivityStatusBar ID="statusBar" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>
    <!-- Alerts -->
    <asp:Panel ID="pnlAlerts" runat="server" CssClass="popup-extender-container">
        <controls:SurveyAlertsList AutoBindOnPostback="true" ID="AlertsList" runat="server"
            OnAlertsChanged="AlertsList_AlertsChanged" />
    </asp:Panel>
    <controls:PopupExtender InitializeOnPostback="True" ID="peAlerts" MasterID="btnAlerts"
        SlaveID="pnlAlerts" runat="server" />
    <asp:Timer ID="timer" runat="server" Enabled="true" Interval="15000" OnTick="timer_Tick">
    </asp:Timer>
</asp:Content>
