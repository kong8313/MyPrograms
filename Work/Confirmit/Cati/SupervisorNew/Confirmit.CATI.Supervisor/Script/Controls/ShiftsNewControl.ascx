<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShiftsNewControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Script.Controls.ShiftsNewControl" %>
<%@ Register TagPrefix="Controls" TagName="ShiftOutlook" Src="~/Script/ShiftOutlook/ShiftOutlook.ascx" %>
<script>
    function toggleViews() {
        var checkbox = Y.one('#mode-changer')._node;
        checkbox.checked = !checkbox.checked;
        Y.one('.gridHolder').toggleClass('hidden');
        Y.one('.bottom-status-bar').toggleClass('hidden');
        Y.one('#divWebDayView').ancestor().toggleClass('hidden');
        Y.one('#<%=viewToggleValue.ClientID%>').set("value", checkbox.checked.toString());
    }
</script>
<controls:UpdatePanel ID="updatePanel" runat="server" style="height: 100%">
    <ContentTemplate>
        <div style="height: 100%">
            <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="true" HideResetButton="True" TopToolbarLayout="DoubleMenu" CssClass="general-grid-control--row-30px"
                PrimaryKeyColumn="Id" EnablePaging="False" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" HasMultySelectionCheckBox="False" HideRefreshButton="True" EnableSorting="False" SortIndicator="Ascending" SortedColumnKey="Id">

                <Commands>
                    <controls:OverlayCommand Key="New" Caption="New" Title="New Shift" SelectMode="No" Image="plus" RefreshOwner="True" Width="470" Height="280" DialogMode="Create" Url="Script/ScriptProperties/ScriptShiftProperties.aspx" OnServerClick="OnChange" />
                    <controls:OverlayCommand Key="Edit" Caption="Edit" Title="Edit" SelectMode="SingleRow" Image="settings" RefreshOwner="True" Width="470" Height="280" DialogMode="ViewEdit" IDName="ShiftId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptShiftProperties.aspx" OnServerClick="OnChange" />
                    <controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" OnServerClick="Delete" Confirmation="cnfr_ShiftDelete" />
                    <controls:Command Key="SetDefault" Caption="Set Default" SelectMode="SingleRow" Image="set_default" OnServerClick="SetDefault" />
                    <controls:Command Key="Launch" Caption="SaveAndLaunch" Image="play_circle" OnServerClick="ScheduleLaunchHandler" />
                    <controls:Command Key="Save" Caption="Save" Image="save" OnServerClick="ScheduleSaveHandler" />
                    <controls:Command Key="Export" Caption="Export" Image="export" OnClientClick="ShiftsExportClick()" />
                </Commands>

                <DataMenuItems>
                    <controls:DataMenuItem Key="New" />
                    <controls:DataMenuItem Key="Edit" />
                    <controls:DataMenuItem Key="Delete" />
                    <controls:DataMenuItem Key="SetDefault" />
                </DataMenuItems>
                <LeftToolbarItems>
                    <asp:Panel runat="server" CssClass="flex-panel flex-panel-row">
                        <div class="toolbar-item">
                            <asp:Label ID="lbToggleView" Text="Toggle view" runat="server" />
                            <div class="comd-button-toggle" onclick="toggleViews()">
                                <input type="checkbox" id="mode-changer" class="comd-button-toggle__checkbox" />
                                <label
                                    class="comd-button-toggle__label"
                                    data-checked-value="On"
                                    data-unchecked-value="Off">
                                </label>
                            </div>
                        </div>
                        <div class="toolbar-item">
                            <asp:Label ID="lblShowShifts" Text="Display:" runat="server" />
                            <controls:DropDownList ID="ddlShowShifts" runat="server" Width="90px" AutoPostBack="true">
                                <asp:ListItem Text="Shifts" Selected="True" />
                                <asp:ListItem Text="Exclusions" Selected="False" />
                                <asp:ListItem Text="Both" Selected="False" />
                            </controls:DropDownList>
                        </div>
                        <div class="toolbar-item toolbar-item--no-padding">
                            <asp:Label ID="lblTimeZone" Text="Current Timezone:" runat="server" />
                            <controls:DropDownList ID="ddlUsedTimeZones" runat="server" EnableViewState="true" Width="250px"
                                AutoPostBack="true" />
                        </div>
                    </asp:Panel>
                    <controls:XpMenuItem runat="server" ImageName="plus" ID="bttnAddTimezone" Text="Add ..." IsSubmit="False" />
                </LeftToolbarItems>
                <ToolbarItems>
                    <controls:ToolbarCommandButton Key="New" />
                    <controls:ToolbarCommandButton Key="Edit" />
                    <controls:ToolbarCommandButton Key="Delete" />
                    <controls:ToolbarCommandButton Key="SetDefault" />
                    <controls:XpMenuItem runat="server" ButtonType="Separator" />
                    <controls:ToolbarCommandButton Key="Export" />
                    <controls:ToolbarCommandButton Key="Launch" />
                    <controls:ToolbarCommandButton Key="Save" runat="server" ID="btnSave" />
                </ToolbarItems>

                <Columns>

                    <controls:GeneralGridColumn Key="Id" DataFieldName="Id" Header-Text="<%$CPResource:ID%>" Width="35" />
                    <controls:GeneralGridColumn Key="ShiftTypeId" DataFieldName="ShiftTypeId" Hidden="true" />
                    <controls:UnboundGeneralGridColumn Key="ShiftTypeName" Header-Text="<%$CPResource:ShiftType%>" Width="140" />
                    <controls:GeneralGridColumn Key="ShiftStatus" DataFieldName="ShiftStatus" Header-Text="<%$CPResource:ShiftStatus%>" Width="140" />
                    <controls:GeneralGridColumn Key="StartDayName" DataType="System.String" DataFieldName="StartDayName" Header-Text="<%$CPResource:StartDay%>" Width="120" />
                    <controls:GeneralGridColumn Key="StartTimeToString" DataFieldName="StartTimeToString" Header-Text="<%$CPResource:StartTime%>" Width="120" />
                    <controls:GeneralGridColumn Key="EndDayName" DataFieldName="EndDayName" Header-Text="<%$CPResource:FinishDay%>" Width="120" />
                    <controls:GeneralGridColumn Key="EndTimeToString" DataFieldName="EndTimeToString" Header-Text="<%$CPResource:FinishTime%>" Width="100%" />
                    <controls:GeneralGridColumn Key="HasRespondentTimeZone" DataFieldName="HasRespondentTimeZone" Hidden="true" />

                </Columns>
                <alternativecontrols>
                    <controls:ShiftOutlook ID="shiftOutlook" runat="server" Visible="true" EnableViewState="false" />
                </alternativecontrols>
            </controls:Grid>
        </div>
        <div style="display: none">
            <asp:Button ID="btnExport" runat="Server" OnClick="ScheduleExport" />
            <input type="hidden" runat="server" ID="viewToggleValue" />
        </div>
    </ContentTemplate>
</controls:UpdatePanel>
<controls:PopupExtender ID="peAddTimeZone" MasterID="bttnAddTimezone" SlaveID="pnlAddTimeZone"
    AutoHide="false" runat="server" />
<asp:Panel ID="pnlAddTimeZone" runat="server" ScrollBars="auto" CssClass="popup-extender-container">
    <div class="popup-selector">
        <div class="popup-selector__content">
            <table cellspacing="5" cellpadding="0" width="450px">
                <tr align="left">
                    <td nowrap width="450px">
                        <asp:Label ID="lblAvailableTimeZones" Text="<%$CPResource:Available Timezones:%>"
                            runat="server" Font-Bold="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <controls:ListBox ID="ddlAvailableTimeZones" runat="server" Width="450px" SelectionMode="Multiple"
                            Height="100px" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="popup-selector__controls">
            <controls:Button ID="btnCancel" Text="Cancel" runat="server" OnClientClick="hidePopup();" CssClass="plain_button button-cancel"
                IsSubmit="false" />
            <controls:Button ID="btnAdd" Text="Add" runat="server" OnClick="AddNewTimeZone" IsSubmit="true" />

        </div>
    </div>
</asp:Panel>
<asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>
<script language="javascript">
    Common.onGlobalEvent("ScriptViewScheduleParametersChanged", function () {
        Common.updatePanel("<%=updatePanel.ClientID%>");
    });
    Common.onGlobalEvent("ScriptViewScheduleShiftTypeChanged", function () {
        Common.updatePanel("<%=updatePanel.ClientID%>");
    });

    //Needed for do synchronius postback
    function ShiftsExportClick() {
        var btnExport = document.getElementById("<%=btnExport.ClientID%>");
        if (btnExport) {
            btnExport.click();
        }
    }
</script>
