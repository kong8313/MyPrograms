<%@ Page AutoEventWireup="true" CodeBehind="AppointmentList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.AppointmentList"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<%@ Register Src="~/ActivityViews/Controls/ActivityStatusBar.ascx" TagName="ActivityStatusBar"
    TagPrefix="controls" %>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <script language="javascript" type="text/javascript">
        function activateAppointment(surveySID, callID) {

            var settings = { height: "620px", width: "760px", top: "100px" };
            var params = { CallSelectionType: 0, IDS: callID, SurveyID: surveySID, CallState: 1 };

            top.overlay.show('<%=Strings.Activate %>', "CallManagement/ActivateCalls.aspx", params, settings, null);

            return top.overlay;
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
                            ImageName="refresh" OnClientClick="Common.updatePanel(statusPanelId);" Text="<%$CPResource:Refresh%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
                <div class="activity-view-toolbar__right">
                    <div class="cati-controls-menu cati-controls-menu--justify">
                        <controls:CheckBox ID="chkTimeMode" runat="server" AutoPostBack="true"
                            Text="<%$CPResource:RespondentTZ%>" ToolTip="<%$CPResource:ShowTimeInRespondentTZ%>" />

                        <div class="flex-panel flex-panel-row">
                            <asp:Label runat="server" Text="<%$CPResource:FilterLabel%>" Style="padding-right: 10px;"></asp:Label>
                            <controls:DropDownList ID="ddlExtendedStatus" runat="server" Width="140" AutoPostBack="true"
                                MaintainSelectedItemDuringDataBind="True">
                            </controls:DropDownList>
                        </div>

                        <controls:DropDownList ID="ddlTimeZones" runat="server" AutoPostBack="true" Width="250">
                        </controls:DropDownList>

                        <asp:UpdatePanel ID="timeUpdatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlTimeZones" EventName="SelectedIndexChanged" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:Label ID="lblTime" runat="server"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                        <ContentTemplate>
                            <controls:XpMenu ID="menu" runat="server">
                                <controls:XpMenuItem ID="btnSurveys" runat="server" ButtonType="ToggleButton" ImageName="assignment_turned_in"
                                    OnClientClick="return false" Text="<%$CPResource:Surveys%>">
                                </controls:XpMenuItem>
                            </controls:XpMenu>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnInterval" runat="server" ButtonType="Button" ImageName="compare_arrows"
                            OnClientClick="return false" Text="<%$CPResource:Intervals%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnAlert" runat="server" ButtonType="Button" ImageName="alert_outlined"
                            OnClientClick="return false" Text="<%$CPResource:Alerts%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnExport" runat="server" ButtonType="Button" ImageName="export"
                            OnClientClick="ActivityViews.exportView(hiddenExportId);" Text="<%$CPResource:Export%>">
                        </controls:XpMenuItem>

                    </controls:XpMenu>
                </div>
            </div>
        </div>
        <div class="activityViewBody flex-panel--all-awailable-space">
            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" class="flex-panel flex-panel-row">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAddAlert" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                    <asp:AsyncPostBackTrigger ControlID="chkTimeMode" EventName="CheckedChanged" />
                </Triggers>
                <ContentTemplate>
                    <!-- Counters grid -->
                    <div class="activityscrollablediv" style="width: 30%;">
                        <controls:HierarchicalGridEx ID="countsGrid" runat="server" GridLines="Both"
                            HideToggleColumn="true">
                            <HeaderStyle CssClass="header" Wrap="false" />
                            <RowStyle CssClass="row" Wrap="false" />
                            <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                            <Columns>
                                <asp:BoundField DataField="ProjectId" HeaderText="<%$CPResource:ProjectId%>" SortExpression="ProjectId" />
                                <asp:BoundField DataField="ProjectName" HeaderText="<%$CPResource:ProjectName%>"
                                    SortExpression="ProjectName" ItemStyle-CssClass="hierarchical-grid__column-project-name hierarchical-grid__column-project-name--narrow" />
                                <asp:BoundField DataField="ShortIntervalCount" HeaderText="<%$CPResource:Short%>"
                                    SortExpression="ShortIntervalCount" />
                                <asp:BoundField DataField="LongIntervalCount" HeaderText="<%$CPResource:Long%>" SortExpression="LongIntervalCount" />
                            </Columns>
                        </controls:HierarchicalGridEx>
                    </div>
                    <div class="activity-view-divider"></div>

                    <!-- Appointmets grid -->
                    <div class="activityscrollablediv" style="width: 70%;">
                        <controls:HierarchicalGridEx ID="m_grid" runat="server" DataKeyNames="InterviewID,SurveySID"
                            GridLines="Both" HideToggleColumn="true">
                            <HeaderStyle CssClass="header" Wrap="false" />
                            <RowStyle CssClass="row" Wrap="false" />
                            <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                            <Columns>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <controls:SvgImage ID="imgAlert" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <controls:ImageButton ID="ibActivate" runat="server" OnClientClick=""
                                            ToolTip="<%$CPResource:Activate%>" CausesValidation="false" ImageName="activate" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="InterviewID" HeaderText="<%$CPResource:InterviewId%>"
                                    SortExpression="InterviewID" />
                                <asp:BoundField DataField="SurveySID" SortExpression="SurveySID" Visible="false" />
                                <asp:BoundField DataField="ProjectID" HeaderText="<%$CPResource:ProjectId%>" SortExpression="ProjectID" />
                                <asp:BoundField DataField="ProjectName" HeaderText="<%$CPResource:ProjectName%>"
                                    SortExpression="ProjectName" ItemStyle-CssClass="hierarchical-grid__column-project-name" />
                                <asp:BoundField DataField="InterviewerName" HeaderText="<%$CPResource:InterviewerName%>"
                                    SortExpression="InterviewerName" />
                                <asp:BoundField DataField="AppointmentTime" DataFormatString="{0:g}" HeaderText="<%$CPResource:AppointmentTime%>"
                                    SortExpression="AppointmentTime" />
                                <asp:BoundField DataField="TimezoneName" HeaderText="<%$CPResource:Timezone%>" SortExpression="TimezoneName" />
                                <asp:BoundField DataField="ExtendedStatusName" HeaderText="<%$CPResource:ExtendedStatus%>" SortExpression="ExtendedStatusName" />
                            </Columns>
                        </controls:HierarchicalGridEx>
                    </div>
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
                    <asp:AsyncPostBackTrigger ControlID="timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <controls:ActivityStatusBar ID="statusBar" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>


    <!-- Intervals -->
    <asp:Panel ID="pnlInterval" runat="server" CssClass="popup-extender-container">
        <asp:UpdatePanel ID="updatePanelIntervals" runat="server" ChildrenAsTriggers="true" class="popup-extender-panel"
            UpdateMode="Always">
            <ContentTemplate>
                <div class="popup-selector">
                    <div class="popup-selector__content">
                        <h3>
                            <asp:Label ID="Label3" runat="server" Text="<%$CPResource:SelectIntervalsForAppCounters%>"></asp:Label></h3>
                        <div class="flex-panel flex-panel-row row-with-inputs">
                            <asp:Label ID="Label4" runat="server" Text="<%$CPResource:Short%>"></asp:Label>

                            <div class="flex-panel flex-panel-row">
                                <controls:NumericEdit ID="wneShort" runat="server" MinValue="1" MaxValue="23" Nullable="False"
                                    ValueText="1" Width="50">
                                    <Buttons SpinButtonsDisplay="OnRight">
                                    </Buttons>
                                </controls:NumericEdit>

                                <asp:Label ID="Label1" runat="server" Text="<%$CPResource:Hours%>"></asp:Label>
                            </div>
                        </div>
                        <div class="flex-panel flex-panel-row row-with-inputs">
                            <asp:Label ID="Label5" runat="server" Text="<%$CPResource:Long%>"></asp:Label>

                            <div class="flex-panel flex-panel-row">
                                <controls:NumericEdit ID="wneLong" runat="server" MaxValue="23" MinValue="1" Nullable="False"
                                    ValueText="1" Width="50">
                                    <Buttons SpinButtonsDisplay="OnRight">
                                    </Buttons>
                                </controls:NumericEdit>

                                <controls:DropDownList ID="ddlLong" runat="server">
                                    <asp:ListItem Selected="true" Text="<%$CPResource:Hours%>" Value="Hours">
                                    </asp:ListItem>
                                    <asp:ListItem Text="<%$CPResource:Days%>" Value="Days">
                                    </asp:ListItem>
                                </controls:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="popup-selector__controls">
                        <controls:Button ID="btnCancelIntervals" runat="server" IsSubmit="false" OnClientClick="hidePopup();" CssClass="plain_button button-cancel"
                            Text="<%$CPResource:Cancel%>" />
                        <controls:Button ID="btnSetIntervals" runat="server" OnClick="btnSetIntervals_Click"
                            OnClientClick="hidePopup();" Text="<%$CPResource:Dlg_Ok%>" />

                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </asp:Panel>
    <controls:PopupExtender ID="PopupExtender1" runat="server" MasterID="btnInterval" SlaveID="pnlInterval" InitializeOnPostback="False">
    </controls:PopupExtender>
    <!-- Alerts -->
    <asp:Panel ID="pnlAlert" runat="server" CssClass="popup-extender-container">
        <asp:UpdatePanel ID="updatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" class="popup-extender-panel">
            <ContentTemplate>
                <div class="popup-selector">
                    <div class="popup-selector__content">
                        <h3 class="h3--with-margin">
                            <asp:Label ID="lblField" runat="server" Text="<%$CPResource:AppointmentTime%>"></asp:Label>
                        </h3>
                        <div class="flex-panel flex-panel-row" style="justify-content: space-between;">
                            <asp:Label ID="lblWarning" runat="server" Text="<%$CPResource:AboutToBeDueIn%>" style="padding-right: 30px;"></asp:Label>
                            <div class="flex-panel flex-panel-row">
                                <controls:TextBox ID="tbxWarning" runat="server" Width="50">
                                </controls:TextBox>

                                <asp:Label ID="Label2" runat="server" Text="<%$CPResource:Minutes%>"></asp:Label>
                            </div>
                        </div>
                        <div class="flex-panel flex-panel-row" style="justify-content: space-between;">
                            <asp:Label ID="lblError" runat="server" Text="<%$CPResource:OverdueAfter%>"></asp:Label>

                            <div class="flex-panel flex-panel-row">
                                <controls:TextBox ID="tbxError" runat="server" Width="50">
                                </controls:TextBox>

                                <asp:Label ID="Label6" runat="server" Text="<%$CPResource:Minutes%>"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="popup-selector__controls">
                        <controls:Button ID="btnCancelAlert" runat="server" IsSubmit="false" OnClientClick="hidePopup();" CssClass="plain_button button-cancel"
                            Text="<%$CPResource:Cancel%>" />
                        <controls:Button ID="btnAddAlert" runat="server" OnClick="btnAddAlert_Click" OnClientClick="hidePopup();"
                            Text="<%$CPResource:Dlg_Ok%>" />

                    </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <controls:PopupExtender ID="peAddAlert" runat="server" MasterID="btnAlert" SlaveID="pnlAlert" InitializeOnPostback="False">
    </controls:PopupExtender>
    <asp:Timer ID="timer" runat="server" Enabled="true" Interval="60000" OnTick="timer_Tick">
    </asp:Timer>
</asp:Content>
