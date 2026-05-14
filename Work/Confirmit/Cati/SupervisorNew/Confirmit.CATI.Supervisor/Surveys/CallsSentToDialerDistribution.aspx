<%@ Page AutoEventWireup="true" CodeBehind="CallsSentToDialerDistribution.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.CallsSentToDialerDistribution"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <script type="text/javascript" language="javascript">
        function cbSetDefaultTime_Click(isChecked) {
            var row = Y.one("#rowTimeSelection");
            row.setStyle("visibility", isChecked ? "hidden" : "visible");
        }

        function OnDateTimeEditorInit() {
            var checkbox = document.getElementById('<%=cbSetDefaultTime.ClientID%>');
            cbSetDefaultTime_Click(checkbox.checked);
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
                        <controls:XpMenuItem ID="btnClose" runat="server" ButtonType="Button" ImageName="close"
                            OnClientClick="window.close()" Text="<%$CPResource:Close%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
            </div>
            <div class="activity-view-toolbar">
                <div class="activity-view-toolbar__left">
                    <controls:XpMenu ID="XpMenu1" runat="server">
                        <controls:XpMenuItem ID="btnRefresh" runat="server" AutoPostBack="false" ButtonType="Button"
                            ImageName="refresh" OnClick="Menu_UpdateClick" Text="<%$CPResource:Refresh%>">
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnTimeSelection" runat="server" ButtonType="Button" ImageName="time"
                            OnClientClick="return false" Text="<%$CPResource:SelectTime%>">
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                    <asp:UpdatePanel ID="updatePanelSelectedTime" runat="server" ChildrenAsTriggers="true"
                        UpdateMode="Always">
                        <ContentTemplate>
                            <asp:Label ID="lblTitle" runat="server" Text="<%$CPResource:SelectedMode%>" />
                            <asp:Label ID="lblSelectedTime" runat="server"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div class="activity-view-toolbar__right">
                </div>
            </div>
        </div>
        <div class="activityViewBody flex-panel--all-awailable-space">
            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSetTime" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnCancelChooseSurveys" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div class="activityscrollablediv">
                        <section>
                            <h3 runat="server" id="DialersRequests"></h3>
                            <controls:HierarchicalGridEx GridLines="Both" AutoGenerateColumns="true" ID="m_grid"
                                RenderHierarchicalRows="false" runat="server" HideToggleColumn="true" OnRowHeaderDataBound="m_grid_RowHeaderDataBound">
                                <HeaderStyle CssClass="header" Wrap="false" />
                                <RowStyle CssClass="row" Wrap="false" />
                                <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                                <Columns>
                                </Columns>
                            </controls:HierarchicalGridEx>
                            <h4 runat="server" id="TotalCallsSentToDialer" class="notesText"></h4>
                        </section>

                        <section>
                            <h3 runat="server" id="CallsBreakdownInDialerCache"></h3>
                            <controls:HierarchicalGridEx GridLines="Both" AutoGenerateColumns="true" ID="m_gridDialerCalls"
                                RenderHierarchicalRows="false" runat="server" HideToggleColumn="true">
                                <HeaderStyle CssClass="header" Wrap="false" />
                                <RowStyle CssClass="row" Wrap="false" />
                                <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                                <Columns>
                                </Columns>
                            </controls:HierarchicalGridEx>
                            <h4 runat="server" id="TotalCallsInDialerCache" class="notesText"></h4>
                        </section>

                        <section>
                            <h3 runat="server" id="DispositionTable"></h3>
                            <controls:HierarchicalGridEx GridLines="Both" AutoGenerateColumns="true" ID="m_gridIts"
                                RenderHierarchicalRows="false" runat="server" HideToggleColumn="true">
                                <HeaderStyle CssClass="header" Wrap="false" />
                                <RowStyle CssClass="row" Wrap="false" />
                                <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                                <Columns>
                                </Columns>
                            </controls:HierarchicalGridEx>
                            <h4 runat="server" id="TotalProcessedCalls" class="notesText"></h4>
                        </section>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="activityViewFooter">
            <asp:UpdatePanel ID="statusBarUpdatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" class="ActivityListStatusBar">
                <ContentTemplate>
                    <asp:Label ID="lblTime" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>


    <asp:Panel ID="pnlTimeSelection" runat="server" CssClass="popup-extender-container" DefaultButton="btnSetTime">
        <asp:UpdatePanel ID="updateTimeSelection" runat="server" ChildrenAsTriggers="true"
            UpdateMode="Always">
            <ContentTemplate>
                <div class="popup-extender-panel">
                    <div class="popup-selector">
                        <div class="popup-selector__content">
                            <table class="settings-table settings-table--default-columns settings-table--fixed-labels-200px">
                                <tr>
                                    <td colspan="3">
                                        <h4><%=Strings.SpecifyTimeForDistributionPeriod%></h4>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%=Strings.DefaultTime%>
                                    </td>
                                    <td colspan="2">
                                        <controls:CheckBox ID="cbSetDefaultTime" runat="server" Checked="true" Text="Last 20 times"
                                            onclick="cbSetDefaultTime_Click(this.checked)" />
                                    </td>
                                </tr>
                                <tr id="rowTimeSelection">
                                    <td nowrap>
                                        <%=Strings.StartTime%>
                                    </td>
                                    <td>
                                        <controls:DropDownList ID="ddlDays" runat="server"  style="width: 100px;" />
                                    </td>
                                    <td style="width: 120px;">
                                        <controls:DateTimeEditor ID="dteTime" runat="server" HorizontalAlign="Center"
                                            EditModeFormat="H:mm:ss" Nullable="false" MinimumNumberOfValidFields="3">
                                            <Buttons SpinButtonsDisplay="OnRight">
                                            </Buttons>
                                            <ClientEvents Initialize="OnDateTimeEditorInit"></ClientEvents>
                                        </controls:DateTimeEditor>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="popup-selector__controls">
                            <controls:Button ID="btnCancelChooseSurveys" runat="server" OnClientClick="hidePopup();" CssClass="plain_button button-cancel"
                                             OnClick="btnCancelSetTime_Click" Text="<%$CPResource:Dlg_Cancel%>" />
                            <controls:Button ID="btnSetTime" runat="server" OnClick="btnSetTime_Click" Text="<%$CPResource:Dlg_Ok %>"
                                OnClientClick="hidePopup();" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <controls:PopupExtender InitializeOnPostback="False" ID="peTimeSelection" runat="server"
        MasterID="btnTimeSelection" SlaveID="pnlTimeSelection">
    </controls:PopupExtender>
</asp:Content>
