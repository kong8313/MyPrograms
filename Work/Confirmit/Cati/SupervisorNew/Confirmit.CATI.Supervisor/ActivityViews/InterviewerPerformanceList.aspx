<%@ Page AutoEventWireup="true" CodeBehind="InterviewerPerformanceList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.InterviewerPerformanceList"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<%@ Register Src="~/ActivityViews/Controls/InterviewerPerformanceStatusBar.ascx"
    TagName="InterviewerPerformanceStatusBar" TagPrefix="controls" %>

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
                        <controls:XpMenuItem ID="btnRefresh" runat="server" OnClick="RefreshData" ButtonType="Button"
                            ImageName="refresh" Text="<%$CPResource:Refresh%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
                <div class="activity-view-toolbar__right">
                    <div class="cati-controls-menu cati-controls-menu--justify">
                        <controls:CheckBox ID="cbLoggedInterviewersOnly" runat="server"
                            Checked="True" AutoPostBack="true" Text="<%$CPResource:LoggedInterviewersOnly%>"
                            ToolTip="<%$CPResource:LoggedInterviewersOnly%>" OnCheckedChanged="RefreshData" />

                        <controls:CheckBox ID="cbFilterBySurveys" runat="server"
                            Checked="False" AutoPostBack="true" Text="<%$CPResource:BreakdownBySurveys%>"
                            ToolTip="<%$CPResource:BreakdownBySurveys%>" OnCheckedChanged="CbBreakdownBySurveysChangeHandler" />

                        <controls:CheckBox ID="cbFilterByActiveSurveysOnly" runat="server"
                            Checked="False" AutoPostBack="true" Text="<%$CPResource:ActiveSurveysOnly%>"
                            ToolTip="<%$CPResource:ActiveSurveysOnly%>" OnCheckedChanged="RefreshData" />
                        
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                        <ContentTemplate>
                            <controls:XpMenu ID="menu" runat="server">
                                <controls:XpMenuItem ID="btnInterviewers" runat="server" ButtonType="ToggleButton"
                                                     ImageName="persons" Text="<%$CPResource:Interviewers%>" />
                                <controls:XpMenuItem ID="btnSurveys" runat="server" ButtonType="ToggleButton" ImageName="assignment_turned_in"
                                                     OnClientClick="return false" Text="<%$CPResource:Surveys%>">
                                </controls:XpMenuItem>
                            </controls:XpMenu>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <controls:XpMenu ID="XpMenu1" runat="server">
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
                    <asp:AsyncPostBackTrigger ControlID="XpMenu1" />
                    <asp:AsyncPostBackTrigger ControlID="cbLoggedInterviewersOnly" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <controls:ScrollableDiv runat="server">
                        <controls:HierarchicalGridEx GridLines="Both" ID="m_grid" runat="server"
                            HideToggleColumn="true">
                            <HeaderStyle CssClass="header" Wrap="false" />
                            <RowStyle CssClass="row" Wrap="false" />
                            <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                            <Columns>
                                <asp:TemplateField ItemStyle-Width="12" HeaderStyle-Width="12" ControlStyle-Width="12" />
                                <asp:BoundField DataField="InterviewerId" SortExpression="InterviewerId" Visible="false" />
                                <asp:BoundField DataField="InterviewerName" HeaderText="<%$CPResource:Interviewer%>"
                                    SortExpression="InterviewerName" />
                                <asp:BoundField DataField="ProjectId" HeaderText="<%$CPResource:ProjectId%>"
                                    SortExpression="ProjectId" Visible="false" />
                                <asp:BoundField DataField="ProjectName" HeaderText="<%$CPResource:SurveyName%>"
                                    SortExpression="ProjectName" Visible="false" />
                                <asp:BoundField DataField="InterviewingTime" SortExpression="InterviewingTime" HeaderText="<%$CPResource:InterviewingTime%>" />
                                <asp:BoundField DataField="TotalInterviewCount" HeaderText="<%$CPResource:Interviews%>"
                                    SortExpression="TotalInterviewCount" />
                                <asp:BoundField DataField="CompletedInterviewCount" HeaderText="<%$CPResource:Completes%>"
                                    SortExpression="CompletedInterviewCount" />
                                <asp:BoundField DataField="CompletedInLastHourCount" HeaderText="<%$CPResource:InterviewerPerformanceList_StrikeRateLastHour%>"
                                    SortExpression="CompletedInLastHourCount" />
                                <asp:BoundField DataField="StrikeRateAverage" HeaderText="<%$CPResource:InterviewerPerformanceList_StrikeRateAverage%>"
                                    SortExpression="StrikeRateAverage" />
                            </Columns>
                        </controls:HierarchicalGridEx>
                    </controls:ScrollableDiv>
                    <div id="hiddenDiv" style="display: none">
                        <asp:Button ID="btnHiddenExport" runat="server" OnClick="btnExport_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="activityViewFooter">
            <asp:UpdatePanel ID="statusBarUpdatePanel" runat="server" ChildrenAsTriggers="true"
                UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="XpMenu1" />
                    <asp:AsyncPostBackTrigger ControlID="cbLoggedInterviewersOnly" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <controls:InterviewerPerformanceStatusBar ID="statusBar" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>

    <asp:Timer ID="timer" runat="server" Enabled="True" Interval="60000" OnTick="RefreshData">
    </asp:Timer>
</asp:Content>
