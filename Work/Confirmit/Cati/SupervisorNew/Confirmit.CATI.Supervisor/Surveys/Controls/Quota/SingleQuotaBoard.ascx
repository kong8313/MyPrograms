<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SingleQuotaBoard.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.Quota.SingleQuotaBoard" %>
<%@ Import Namespace="ConfirmitDialerInterface" %>

<style type="text/css">
    tbody tr td.DisabledColumn.quotas-cell-row-disabled {
        color: lightcoral;
    }

    tbody tr td.DisabledColumn {
        color: gray;
    }
</style>
<input type="hidden" runat="server" id="selectedFields" />
<input type="hidden" runat="server" id="selectedSurveyId" />
<controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="ID" GridNameWidth="0px" TopToolbarLayout="DoubleMenu"
    OnDblClickCommand="ChangeLimit" EnablePaging="false">
    <Commands>
        <controls:OverlayCommand Key="ChangeLimit" DialogMode="ViewEdit" SelectMode="MultiRow"
            Title="ChangeLimit" Caption="ChangeLimit" IDName="IDS" IDColumnName="ID" Width="345" Top="200"
            Height="140" Url="Surveys/ChangeLimit.aspx" Image="edit" RefreshOwner="True" />
        <controls:OverlayCommand Key="Activate" Caption="Activate" Title="Activate" RefreshOwner="True" DialogMode="ViewEdit" SelectMode="MultiRow" Width="760" Top="100"
            Height="620" Url="CallManagement/ActivateCalls.aspx" IDName="IDS" Image="activate" />
        <controls:OverlayCommand Key="ChangePriority" Caption="ChangePriority" Title="ChangePriority" RefreshOwner="True" DialogMode="ViewEdit" SelectMode="MultiRow" Width="325" Top="200"
            Height="170" Url="CallManagement/ChangePriority.aspx" Image="swap_vert" IDName="IDS" />
        <controls:Command Key="Enable" Caption="EnableCalls" Image="active_calls" OnServerClick="Enable"
            Confirmation="<%$CPResource:conf_EnableSelectedQuotaCells%>" />
        <controls:Command Key="CallManagement" Caption="CallManagement" SelectMode="SingleRow" Image="call" OnClientClick="openCallManagement();" />
        <controls:Command Key="Disable" Caption="DisableCalls" Image="cancel" OnServerClick="Disable"
            Confirmation="<%$CPResource:conf_DisableSelectedQuotaCells%>" />
        <controls:Command Key="Open" Caption="OpenCells" OnServerClick="Open"
            Confirmation="<%$CPResource:conf_OpenSelectedQuotaCells%>" Image="_lock" />
        <controls:Command Key="Close" Caption="CloseCells" OnServerClick="Close"
            Confirmation="<%$CPResource:conf_CloseSelectedQuotaCells%>" Image="unlock" />
        <controls:Command Key="SetBalancingCellPriorityDisabled" Caption="NoBalancing" OnServerClick="SetBalancingCellPriorityDisabled" />
        <controls:Command Key="SetBalancingCellPriorityLow" Caption="Low" OnServerClick="SetBalancingCellPriorityLow" />
        <controls:Command Key="SetBalancingCellPriorityMedium" Caption="Medium" OnServerClick="SetBalancingCellPriorityMedium" />
        <controls:Command Key="SetBalancingCellPriorityHigh" Caption="High" OnServerClick="SetBalancingCellPriorityHigh" />
        <controls:OverlayCommand Key="showStatusBreakdown" DialogMode="ViewEdit" SelectMode="SingleRow"
            Title="StatusBreakdown" Caption="StatusBreakdown" IDName="CellId" IDColumnName="ID" Width="420" Top="200"
            Height="260" Url="Surveys/StatusBreakdown.aspx" Image="view" />
        <controls:OverlayCommand Key="Properties" DialogMode="ViewEdit" SelectMode="No"
            Title="<%$CPResource:Properties%>" Caption="<%$CPResource:Properties%>" Width="650" Top="100"
            Height="500" Url="Surveys/Controls/Quota/QuotaProperties.aspx" Image="settings" RefreshOwner="True" />
    </Commands>
    <LeftToolbarItems>
        <asp:Panel runat="server" CssClass="flex-panel flex-panel-row">
            <div class="toolbar-item">
                <asp:Label runat="server" ID="lblQuotas" CssClass="toolbar-label"></asp:Label>

                <controls:DropDownList runat="server" ID="ddlQuotas" Width="140" AutoPostBack="true" MaintainSelectedItemDuringDataBind="True" OnSelectedIndexChanged="ddlQuotas_SelectedIndexChanged" />
            </div>
            <controls:ImageButton runat="server" ID="btnSelectFields" Text="<%$CPResource:QuotaActionFilter%>" IsSubmit="false" ImageName="filter_list" style="margin-right: 10px" />
            <div class="toolbar-item toolbar-item--no-padding">
                <asp:Label runat="server" ID="lblExtraCounter" Text="<%$CPResource:CallCounts%>" CssClass="toolbar-label" />
                <controls:DropDownList runat="server" ID="ddlExtraCounter" Width="180" onchange="OnExtraCounterTypeChanged(true)"
                    onkeypress="onExtraCounterKeyPress()" EnableViewState="true" AutoPostBack="false">
                    <asp:ListItem Text="None" Value="0" Selected="true" />
                    <asp:ListItem Text="<%$CPResource:DailyQuotaCounters%>" Value="4" />
                    <asp:ListItem Text="<%$CPResource:ScheduledCalls%>" Value="1" />
                    <asp:ListItem Text="<%$CPResource:ScheduledCallsWithSpecificStatuses%>" Value="2" />
                    <asp:ListItem Text="<%$CPResource:InterviewsWithSpecificStatuses%>" Value="3" />
                </controls:DropDownList>
            </div>
        </asp:Panel>
        <controls:XpMenuItem runat="server" ImageName="filter_1" ID="btnITS" Text="Select statuses" IsSubmit="False" style="display: none" />
        <controls:XpMenuItem runat="server" ButtonType="Generic">
            <controls:CheckBox runat="server" ID="cbIncludeDisabledCalls" Text="<%$CPResource:ExtraCounterIncludeDisabledCalls%>" style="display: none" />
        </controls:XpMenuItem>
        <controls:XpMenuItem runat="server" ButtonType="Button" ID="btnSelectSurvey" Text="<%$CPResource:Surveys%>" TextAndImage="True" ImageName="assignment_turned_in" Visible="False" OnClientClick="selectSurvey();">
        </controls:XpMenuItem>
    </LeftToolbarItems>
    <ToolbarItems>
        <asp:Label runat="server" ID="lblWarning" Style="text-align: right" Width="100%"
            Font-Bold="true" ForeColor="Red" Visible="true"></asp:Label>
        <controls:ToolbarStdBlock />
        <controls:ToolbarCommandButton Key="ChangeLimit" />
        <controls:XpMenuItem
            runat="server"
            ID="btnOpenQuota"
            ImageName="open_in_new"
            OnClientClick="OpenSelectedQuotaInNewWindow();"
            ToolTip="<%$CPResource:OpenQuotaInNewWindow%>"
            TextAndImage="False">
        </controls:XpMenuItem>
        <controls:XpMenuItem
            runat="server"
            ID="XpMenuItem1"
            ImageName="reports"
            OnClientClick="OpenQuotaProgressReportForSelectedQuotaInNewWindow();"
            ToolTip="<%$CPResource:OpenQuotaProgressReportForSelectedQuotaInNewWindow%>"
            TextAndImage="False">
        </controls:XpMenuItem>
        <controls:ToolbarCommandButton Key="Properties" runat="server" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="ChangeLimit" />
        <controls:DataMenuItem Key="Activate" />
        <controls:DataMenuItem Key="ChangePriority" />
        <controls:DataMenuItem Key="Enable" />
        <controls:DataMenuItem Key="Disable" />
        <controls:DataMenuItem Key="showStatusBreakdown" />
        <controls:DataMenuItem Key="CallManagement" />
        <controls:DataMenuItem Key="Open" />
        <controls:DataMenuItem Key="Close" />
        <controls:DataMenuItem Key="ChangeBalancingPriority" TextId="QuotaBalancingPriority">
            <Items>
                <controls:DataMenuItem Key="SetBalancingCellPriorityDisabled" />
                <controls:DataMenuItem Key="SetBalancingCellPriorityLow" />
                <controls:DataMenuItem Key="SetBalancingCellPriorityMedium" />
                <controls:DataMenuItem Key="SetBalancingCellPriorityHigh" />
            </Items>
        </controls:DataMenuItem>
    </DataMenuItems>
</controls:Grid>
<controls:PopupExtender ID="peFields" MasterID="btnSelectFields" SlaveID="pnlFields"
    runat="server" />
<asp:Panel ID="pnlFields" runat="server" Width="120px" Height="180px" CssClass="popup-extender-container">
    <controls:UpdatePanel runat="server" ID="updatePanelFieldsList" class="popup-extender-panel">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnFieldsSelected" />
        </Triggers>
        <ContentTemplate>
            <div class="popup-selector">
                <div class="popup-selector__content">
                    <controls:CheckBoxList ID="cbFields" runat="server" RepeatColumns="1" RepeatDirection="Vertical"
                        AutoPostBack="false" KeepOneChecked="true" ErrorMessageOnLastUncheck="<%$CPResource:ActionFilterNoneSelectedWarning%>" />
                </div>
                <div class="popup-selector__controls">
                    <controls:Button ID="btnFieldsSelected" runat="server" Text="<%$CPResource:Dlg_Ok%>"
                        IsSubmit="true" OnClientClick="hidePopup()" OnClick="SaveFields" />
                </div>
            </div>
        </ContentTemplate>
    </controls:UpdatePanel>
</asp:Panel>
<div class="flex-panel flex-panel-column">
    <controls:ItsSelect ID="itsSelect" runat="server" Height="295px" OnClick="RefreshGrid" />
</div>
<asp:HiddenField runat="server" ID="schCallsIts" Value="" />
<asp:HiddenField runat="server" ID="intIts" Value="" />
<asp:HiddenField runat="server" ID="dailyCntIts" Value="" />
<asp:HiddenField runat="server" ID="currentCounterType" Value="" />

<script type="text/javascript">
    var quotaOpenCount = 0;

    if (document.getElementById("<%=schCallsIts.ClientID%>").value == '')
        document.getElementById("<%=schCallsIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.FreshSample%>"]').get("id");

    if (document.getElementById("<%=intIts.ClientID%>").value == '')
        document.getElementById("<%=intIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.FreshSample%>"]').get("id");

    if (document.getElementById("<%=dailyCntIts.ClientID%>").value == '')
        document.getElementById("<%=dailyCntIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.Completed%>"]').get("id");

    function OpenSelectedQuotaInNewWindow() {
        quotaOpenCount++;
        var surveyId = "<%= HttpContext.Current.Request.QueryString["ID"] %>";
        var quotaName = document.getElementById("<%= ddlQuotas.ClientID %>").value;

        GetWM().openWindow(
            "SurveyViewQuotas.aspx?ID=" + surveyId + "&startQuotaName=" + encodeURIComponent(quotaName) + "&quota=true" + "&count=" + quotaOpenCount,
            "",
            "width=1200px, height=500px,location=no,toolbar=no, menubar=no,status=no,resizable=yes,scrollbars=yes"
        );
    };

    function OpenQuotaProgressReportForSelectedQuotaInNewWindow() {
        var surveyId = "<%= HttpContext.Current.Request.QueryString["ID"] %>";
        var quotaName = document.getElementById("<%= ddlQuotas.ClientID %>").value;

        GetWM().openWindow(
            "<%=BaseRelativePath("Reports/QuotaProgressReport.aspx")%>" + "?ID=" + surveyId + "&QuotaName=" + encodeURIComponent(quotaName) + "&AutoBuildReport=true",
            "",
            "width=1024px, height=750px,location=no,toolbar=no, menubar=no,status=no,resizable=yes,scrollbars=yes"
        );
    };


    function switchItsList(value) {

        var currentCounterType = document.getElementById("<%=currentCounterType.ClientID%>").value;

        if (value > 1) {
            //var itsList = Y.all('#itsList input[type=checkbox]:checked').get("id");
            //for (var i = itsList.length; i--;) {
            //    itsList[i] = '#' + itsList[i];
            var itsList = [];
            Y.all('#itsList input[type=checkbox]').each(function (el) {
                if (el._node.checked) {
                    itsList.push('#' + el.get("id"));
                }
            });

            var itsString = itsList.join(",");
            if (currentCounterType == 2)
                document.getElementById("<%=schCallsIts.ClientID%>").value = itsString;
            else if (currentCounterType == 3)
                document.getElementById("<%=intIts.ClientID%>").value = itsString;
            else if (currentCounterType == 4)
                document.getElementById("<%=dailyCntIts.ClientID%>").value = itsString;

            Y.all('#itsList input[type=checkbox]').set('checked', false);
            if (value == 2)
                Y.all(document.getElementById("<%=schCallsIts.ClientID%>").value).set('checked', true);
            else if (value == 3)
                Y.all(document.getElementById("<%=intIts.ClientID%>").value).set('checked', true);
            else if (value == 4)
                Y.all(document.getElementById("<%=dailyCntIts.ClientID%>").value).set('checked', true);

        }
        document.getElementById("<%=currentCounterType.ClientID%>").value = value;
    }

    function OnExtraCounterTypeChanged(needRefresh) {
        var value = document.getElementById('<%=ddlExtraCounter.ClientID%>').value;

        if (needRefresh)
            switchItsList(value);

        if (value == 2 || value == 3 || value == 4) {
            document.getElementById('<%= btnITS.ClientID %>').style.display = '';
            InitSelectAllButtonLabel();
        }
        else {
            document.getElementById('<%= btnITS.ClientID %>').style.display = 'none';
        }

        document.getElementById('<%= cbIncludeDisabledCalls.ClientID %>').parentElement.style.display = (value == 1 || value == 2) ? '' : 'none';
        if (needRefresh && (value == 0 || value == 1 || value == 4)) {
            <%=m_grid.GetCommand("Refresh").GetClientEventJavaScript(Page, m_grid) %> 
        }
    }

    function onExtraCounterKeyPress() {
        if (event.keyCode == 13) // Enter pressed
        {
            <%=m_grid.GetCommand("Refresh").GetClientEventJavaScript(Page, m_grid) %> 
        }
    }
    OnExtraCounterTypeChanged(false);

    function getSelectedIds() {
        var s = <%=m_grid.ClientGetSelectedRows() %>;

        if (s == null || s == '') {
            var row = <%=m_grid.ClientGetCurrentRow() %>;

            if (row) {
                s = row.get_cellByColumnKey("ID").get_value();
            }
        }

        return s;
    }

    function openCallManagement() {
        var row = <%=m_grid.ClientGetCurrentRow() %>;
        var selectedFields = document.getElementById("<%=selectedFields.ClientID %>").value.split(',');
        var values = [];
        if (row) {
            for (var i = 0; i < selectedFields.length; i++) {
                values.push(row.get_cellByColumnKey(selectedFields[i] + "_precode").get_value());
            }
        }

        if (window.callManagement)
            window.callManagement(document.getElementById("<%=selectedFields.ClientID %>").value, values.join(','), <%=Survey.SID %>);
    }

    function showEnableDisableCallsDialog(title, operationId) {

        var settings = { height: "340px", width: "520px" };

        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            <%=m_grid.GetCommand("Refresh").GetClientEventJavaScript(Page, m_grid) %>

        });

        top.overlay.show(title, "AsyncOperations/AsyncOperationProgress.aspx?OperationId=" + operationId + "&OperationTitle=" + title + "&DialogTitle=" + title, null, settings, null);

    }

    function selectSurvey() {
        var settings = { height: "700px", width: "650px", top: "100px" };

        top.overlay.show('Select survey', "Surveys/Controls/Quota/SelectSurvey.aspx", null, settings, null);
        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            if (args.data) {
                document.getElementById("<%=selectedSurveyId.ClientID %>").value = args.data;
            }

            Common.updatePanel('<%=ClientID %>');
        });
    }
</script>
