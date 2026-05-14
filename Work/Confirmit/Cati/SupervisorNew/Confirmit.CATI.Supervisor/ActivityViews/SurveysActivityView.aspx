<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveysActivityView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.SurveysActivityView" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<%@ Register TagPrefix="controls" TagName="HierarchicalGridEx" Src="~/Controls/HierarchicalGridEx.ascx" %>
<%@ Register TagPrefix="controls" TagName="StatusBreakdown" Src="~/ActivityViews/Controls/StatusBreakdown.ascx" %>
<%@ Register TagPrefix="controls" TagName="SurveyAlertsList" Src="~/ActivityViews/Controls/SurveyAlertsList.ascx" %>
<%@ Register TagPrefix="controls" TagName="StatusAlertsList" Src="~/ActivityViews/Controls/StatusAlertsList.ascx" %>
<%@ Register Src="~/ActivityViews/Controls/ActivityStatusBar.ascx" TagName="ActivityStatusBar" TagPrefix="controls" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">
        function showColumnSettingsDialog() {
            var settings = { height: "500px", width: "400px", top: "50px" };
            var pathToSettings = '<%=BaseRelativePath("ActivityViews/Controls/ActivityColumnSettings.aspx?SettingsFor=" + CustomizableViews.SurveyActivityView) %>';

            top.overlay.show("<%=Strings.ColumnSettings%>",
                pathToSettings,
                null,
                settings,
                null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;
                Common.updatePanel('<%=updatePanel.ClientID %>');

            });
        };

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
            ActivityViews.subscribeForContextMenu('SurveyActivityView');
        });
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
                            IsSubmit="False" AutoPostBack="False"
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
                    <controls:XpMenu ID="XpMenu2" runat="server">
                        <controls:XpMenuItem ID="btnRefresh" runat="server" AutoPostBack="false" ButtonType="Button"
                            ImageName="refresh" OnClientClick="Common.updatePanel(statusPanelId);" Text="<%$CPResource:Refresh%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem runat="server" ButtonType="Generic">
                            <controls:DropDownList ID="ddlRefresh" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRefresh_SelectedIndexChanged" Style="width: 100px;">
                                <asp:ListItem Text="<%$CPResource:RefreshRate_None%>" Value="0">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_15sec%>" Value="15000">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_30sec%>" Value="30000">
                                </asp:ListItem>
                                <asp:ListItem Text="<%$CPResource:RefreshRate_45sec%>" Value="45000">
                                </asp:ListItem>
                                <asp:ListItem Selected="true" Text="<%$CPResource:RefreshRate_1min%>" Value="60000">
                                </asp:ListItem>
                            </controls:DropDownList>
                        </controls:XpMenuItem>

                    </controls:XpMenu>
                </div>
                <div class="activity-view-toolbar__right">
                    <asp:UpdatePanel ID="updatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                        <ContentTemplate>
                            <controls:XpMenu ID="XpMenu1" runat="server">
                                <controls:XpMenuItem runat="server" ButtonType="Generic">
                                    <controls:CheckBox Text="CATI interviews only"
                                        ID="cbCatiInterviews" Checked="true" runat="server" Font-Bold="false" AutoPostBack="true" OnCheckedChanged="cbCatiInterviews_CheckedChanged"
                                        TextAlign="Right" />
                                </controls:XpMenuItem>
                                <controls:XpMenuItem runat="server" ButtonType="Generic">
                                    <controls:CheckBox Text="<%$CPResource:ActiveSurveysOnly%>"
                                        ID="cbActiveSurveys" Checked="true" runat="server" Font-Bold="false" AutoPostBack="true" OnCheckedChanged="cbActiveSurveys_OnCheckedChanged"
                                        TextAlign="Right" />
                                </controls:XpMenuItem>
                                <controls:XpMenuItem ID="btnSurveys" runat="server" ButtonType="ToggleButton" ImageName="assignment_turned_in"
                                    OnClientClick="return false" Text="<%$CPResource:Surveys%>">
                                </controls:XpMenuItem>
                            </controls:XpMenu>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnAlerts" runat="server" ButtonType="Button" ImageName="alert_filled"
                            OnClientClick="return false" Text="<%$CPResource:SurveyAlerts%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnStatusAlerts" runat="server" ButtonType="Button" ImageName="alert_outlined"
                            OnClientClick="return false" Text="<%$CPResource:StatusAlerts%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnColumnSettings" runat="server" ButtonType="Button" ImageName="settings" IsSubmit="False" AutoPostBack="false"
                            OnClientClick="showColumnSettingsDialog();" Text="Column Settings">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnExport" runat="server" ButtonType="Button" ImageName="export" IsSubmit="False"
                            OnClientClick="ActivityViews.exportView(hiddenExportId);" Text="<%$CPResource:Export%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
            </div>
        </div>
        <div class="activityViewBody flex-panel--all-awailable-space">
            <!-- Main grid view -->
            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                    <asp:AsyncPostBackTrigger ControlID="surveyAlertsList" EventName="AlertsChanged" />
                    <asp:AsyncPostBackTrigger ControlID="statusAlertsList" EventName="AlertsChanged" />
                </Triggers>
                <ContentTemplate>
                    <controls:ScrollableDiv runat="server">
                        <controls:HierarchicalGridEx ID="gridSurveys" runat="server" OnHierarchicalRowDataBound="gridSurveys_HierarchicalRowDataBound" OnRowCreated="gridSurveys_OnRowCreated"
                            OnRowDataBound="gridSurveys_OnRowDataBound" GridLines="Both" DataKeyNames="SID, Id" OnSelectedIndexChanged="gridSurveys_OnSelectedIndexChanged">
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
                                        <controls:ImageButton ID="ibSendMessage" runat="server" ToolTip="<%$CPResource:SendMessage%>"
                                            CommandName="SendMessage" CausesValidation="false" ImageName="send" CssClass="comd-button--small"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Id" HeaderText="<%$CPResource:ProjectId%>" SortExpression="Id" />
                                <asp:BoundField DataField="Name" ItemStyle-CssClass="hierarchical-grid__column-project-name" HeaderText="<%$CPResource:hdr_Name%>" SortExpression="Name" />
                            </Columns>
                            <HierarchicalRowTemplate>
                                <controls:StatusBreakdown ID="breakdown" runat="server" />
                            </HierarchicalRowTemplate>
                        </controls:HierarchicalGridEx>
                    </controls:ScrollableDiv>
                    <div id="hiddenDiv" style="display: none">
                        <asp:Button ID="btnHiddenExport" runat="server" OnClick="btnExport_Click" />
                    </div>
                    <controls:DataMenu runat="server" ID="gridContextMenu" EnableViewState="False">
                    </controls:DataMenu>
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
                    <controls:ActivityStatusBar ID="statusBar" runat="server"></controls:ActivityStatusBar>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>
    <!-- Survey alerts -->
    <asp:Panel ID="pnlAlerts" runat="server" CssClass="popup-extender-container">
        <controls:SurveyAlertsList ID="surveyAlertsList" runat="server" AutoBindOnPostback="true"
            OnAlertsChanged="surveyAlertsList_AlertsChanged" />
    </asp:Panel>
    <controls:PopupExtender InitializeOnPostback="False" ID="peAlerts" MasterID="btnAlerts"
        SlaveID="pnlAlerts" runat="server" />
    <!-- Status alerts -->
    <asp:Panel ID="pnlStatusAlerts" runat="server" CssClass="popup-extender-container">
        <controls:StatusAlertsList ID="statusAlertsList" runat="server" AutoBindOnPostback="true"
            OnAlertsChanged="statusAlertsList_AlertsChanged" />
    </asp:Panel>
    <controls:PopupExtender ID="peStatusAlerts" MasterID="btnStatusAlerts" SlaveID="pnlStatusAlerts" runat="server" InitializeOnPostback="False" />
    <asp:Timer ID="timer" runat="server" OnTick="timer_Tick" Enabled="true" Interval="60000" />
</asp:Content>
