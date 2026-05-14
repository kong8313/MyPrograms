<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AllQuotasBoard.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.Quota.AllQuotasBoard" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Import Namespace="ConfirmitDialerInterface" %>
<%@ Register TagPrefix="controls" TagName="GridToolbar" Src="~/Controls/GeneralToolbar.ascx" %>

<input type="hidden" runat="server" id="selectedSurveyId" />
<main id="<%=ClientID %>" class="flex-panel flex-panel-column" style="height: 100%;">
    <div id="trTopTitle" runat="server" class="general-grid-control__header-title" visible="False">
        <asp:Label runat="server" ID="topTitle" Text="All Quotas" />
    </div>
    <controls:GridToolbar runat="server" ID="topToolbar" ToolbarLayout="DoubleMenu">
        <LeftMenuItems>
            <controls:XpMenuItem runat="server" ButtonType="Generic" CssClass="flex-panel flex-panel-row">
                <div class="toolbar-item">
                    <asp:Label runat="server" ID="lblQuotas" Width="80" Style="text-align: right; padding-right: 5px"></asp:Label>
                    <controls:DropDownList runat="server" ID="ddlQuotas" Width="140" AutoPostBack="true" MaintainSelectedItemDuringDataBind="True" />
                </div>
                <div class="toolbar-item">
                    <asp:Label runat="server" ID="lblExtraCounter" Text="<%$CPResource:CallCounts%>" />
                    <controls:DropDownList runat="server" ID="ddlExtraCounter" Width="180" onchange="OnExtraCounterTypeChanged(true)"
                        onkeypress="onExtraCounterKeyPress()" EnableViewState="true" AutoPostBack="false">
                        <asp:ListItem Text="None" Value="0" Selected="true" />
                        <asp:ListItem Text="<%$CPResource:DailyQuotaCounters%>" Value="4" />
                        <asp:ListItem Text="<%$CPResource:ScheduledCalls%>" Value="1" />
                        <asp:ListItem Text="<%$CPResource:ScheduledCallsWithSpecificStatuses%>" Value="2" />
                        <asp:ListItem Text="<%$CPResource:InterviewsWithSpecificStatuses%>" Value="3" />
                    </controls:DropDownList>
                </div>
                <asp:Panel runat="server" class="toolbar-item">
                    <controls:ImageButton runat="server" ID="btnITSAllQ" Text="Select statuses" IsSubmit="false" ImageName="filter_1" style="display: none" />
                    <controls:CheckBox runat="server" ID="cbIncludeDisabledCalls" Text="<%$CPResource:ExtraCounterIncludeDisabledCalls%>" style="display: none; margin-left: 15px;" />
                </asp:Panel>
            </controls:XpMenuItem>
            <controls:XpMenuItem runat="server" ButtonType="Button" ID="btnSelectSurvey" Text="<%$CPResource:Surveys%>" TextAndImage="True" ImageName="assignment_turned_in" Visible="False" OnClientClick="selectSurvey();" />
        </LeftMenuItems>
        <RightMenuItems>
            <controls:XpMenuItem runat="server" ButtonType="Button" OnClientClick="Common.updatePanel('<%=ClientID %>')" Text="Refresh" TextAndImage="False" ImageName="refresh" />
            <controls:XpMenuItem runat="server" ButtonType="Separator"></controls:XpMenuItem>
            <controls:XpMenuItem runat="server" ButtonType="Button" ImageName="reset" OnClientClick="unselectAllCells();" ToolTip="<%$CPResource:Reset%>" />
            <controls:XpMenuItem runat="server" ButtonType="Button" ImageName="edit" OnClientClick="changeLimit();" ToolTip="<%$CPResource:ChangeLimit%>" />
            <controls:XpMenuItem runat="server" ButtonType="Button" Text="<%$CPResource:Properties%>" OnClientClick="showProperties();" TextAndImage="False" ImageName="settings" />
            <controls:XpMenuItem
                runat="server"
                ID="btnOpenQuota"
                ImageName="open_in_new"
                OnClientClick="OpenSelectedQuotaInNewWindow();"
                ToolTip="<%$CPResource:OpenQuotaInNewWindow%>"
                TextAndImage="False">
            </controls:XpMenuItem>
        </RightMenuItems>
    </controls:GridToolbar>
    <div runat="server" id="gridHolder" visible="true" class="content-panel flex-panel--all-awailable-space content-panel__scroll-pane all-quotas-panel">
    </div>
    <div class="XpMenu clearfix bottom-status-bar" id="statusBarDiv">
        <div id="labelDiv" class="total-records-count" runat="server">
            <asp:Label ID="lblRecordCount" runat="server" Text="" CssClass="boldLabel" />
        </div>
        <div runat="server" class="total-records-count-extra" id="extraInfoDiv">
            <asp:Label runat="server" ID="lblExtraInfo" CssClass="boldLabel" />
        </div>
    </div>

    <div class="flex-panel flex-panel-column">
        <controls:ItsSelect ID="itsSelectAllQ" runat="server" Height="295px" IsSubmit="true"  AutoPostBack="false" PopupExtenderMasterID="btnITSAllQ" />
    </div>

    <asp:HiddenField runat="server" ID="schCallsIts" Value="" />
    <asp:HiddenField runat="server" ID="intIts" Value="" />
    <asp:HiddenField runat="server" ID="dailyCntIts" Value="" />
    <asp:HiddenField runat="server" ID="currentCounterType" Value="" />
</main>

<script type="text/javascript">
    Y.on("load", quotaRowSelection);

    var pack = {};

    function showSelectedCellsInfo() {
        var line = '<%= Strings.AllQuotasBoardBottomMessage %>'.format(pack.totalQuotas, pack.totalCells, pack.selectedQuotas, pack.selectedCells);
        Y.one('#<%=lblRecordCount.ClientID %>').set('text', line);
    }

    function selectSingle(e) {
        var info = getInfo(e);

        if (e.get('checked')) {
            pack[info.name].push(info.id);
            pack.selectedCells++;

            if (pack[info.name].length === 1) {
                pack.selectedQuotas++;
                pack.quotas.push(info.name);;
            }
        } else {
            var index = pack[info.name].indexOf(info.id);
            pack[info.name].splice(index, 1);

            pack.selectedCells--;

            if (pack[info.name].length === 0) {
                pack.selectedQuotas--;
                index = pack.quotas.indexOf(info.name);
                pack.quotas.splice(index);
            }
        }

        showSelectedCellsInfo();
    }


    function selectAll(e) {
        var info = getInfo(e);
        var checked = e.get('checked');

        if (checked) {
            if (pack[info.name].length === 0) {
                pack.selectedQuotas++;
                pack.quotas.push(info.name);
            }
        } else {
            pack.selectedQuotas--;
            var index = pack.quotas.indexOf(info.name);
            pack.quotas.splice(index);
        }

        pack.selectedCells -= pack[info.name].length;
        pack[info.name] = [];
        var grid = Y.one("#<%= gridHolder.ClientID %>");

        grid.all('.selector-single[quota-name="' + info.name + '"] input').each(function (element) {
            if (checked) {
                var id = parseInt(element.get('parentNode').getAttribute('quota-row-id')).toString();
                pack[info.name].push(id);
                pack.selectedCells++;
            }

            element.set('checked', checked);
        });

        showSelectedCellsInfo();
    }

    function getInfo(checkbox) {
        var parent = checkbox.get('parentNode');
        var id = parent.getAttribute('quota-row-id');
        var name = parent.getAttribute('quota-name');

        return {
            id: id,
            name: name
        };
    }

    function quotaRowSelection() {
        var grid = Y.one("#<%= gridHolder.ClientID %>");

        pack.totalQuotas = 0;
        pack.totalCells = 0;
        pack.selectedQuotas = 0;
        pack.selectedCells = 0;

        pack.quotas = [];

        grid.all('.selector-all input').each(function (checkbox) {
            var info = getInfo(checkbox);

            checkbox.on('click', function () {
                selectAll(checkbox);
            });

            checkbox.set('checked', false);

            pack[info.name] = [];

            pack.totalQuotas++;
        });

        grid.all('.selector-single input').each(function (checkbox) {
            checkbox.on('click', function () {
                selectSingle(checkbox);
            });

            checkbox.set('checked', false);

            pack.totalCells++;
        });

        showSelectedCellsInfo();
    }

    function unselectAllCells() {
        var grid = Y.one("#<%= gridHolder.ClientID %>");

        for (var i = 0; i < pack.quotas.length; i++) {
            var quota = pack.quotas[i];

            grid.all('[quota-name="' + quota + '"] input').each(function (element) {
                element.set('checked', false);
            });

            pack[quota] = [];
        }

        pack.quotas = [];
        pack.selectedCells = 0;
        pack.selectedQuotas = 0;

        showSelectedCellsInfo();
    }

    var quotaOpenCount = 0;

    function OpenSelectedQuotaInNewWindow() {
        quotaOpenCount++;
        var surveyId = "<%= HttpContext.Current.Request.QueryString["ID"] %>";

        GetWM().openWindow(
            "SurveyViewQuotas.aspx?ID=" + surveyId + "&startAllQuotas=true&quota=true" + "&count=" + quotaOpenCount,
            "",
            "width=1200px, height=500px,location=no,toolbar=no, menubar=no,status=no,resizable=yes,scrollbars=yes"
        );
    };

    function showProperties() {
        var surveyId = "<%= Survey.SID %>";
        var settings = { height: "500px", width: "650px", top: "100px" };
        var params = { SurveyID: surveyId };

        top.overlay.show('<%= Strings.Properties %>', "Surveys/Controls/Quota/QuotaProperties.aspx", params, settings, null);
        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;
            Common.updatePanel('<%=ClientID %>');
        });
    }

    function selectSurvey() {
        var settings = { height: "700px", width: "650px", top: "100px" };

        top.overlay.show('<%= Strings.SelectSurvey %>', "Surveys/Controls/Quota/SelectSurvey.aspx", null, settings, null);
        top.overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            if (args.data) {
                document.getElementById("<%=selectedSurveyId.ClientID %>").value = args.data;
            }

            Common.updatePanel('<%=ClientID %>');
        });
    }

    function changeLimit() {
        var data = {
            ID: <%= Survey.SID %>,
            quotas: pack.quotas,
            selectedCells: pack.selectedCells,
            selectedQuotas: pack.selectedQuotas
        };

        for (var i = 0; i < data.quotas.length; i++) {
            var quota = data.quotas[i];
            data[quota] = pack[quota];
        }

        if (data.quotas.length === 0) {
            alert('<%= Strings.NoRowsSelected %>');
        } else {
            var settings = { height: "300px", width: "525px", top: "200px" };

            top.overlay.show('<%= Strings.ChangeLimit %>', "Surveys/ChangeLimitForMultipleQuotas.aspx", data, settings, null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true) {
                    return;
                } else {
                    if (typeof args.data !== 'undefined') {
                        alert(args.data);
                    }
                }

                Common.updatePanel('<%=ClientID %>');
            });
        }
    }

    if (document.getElementById("<%=schCallsIts.ClientID%>").value == '')
        document.getElementById("<%=schCallsIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.FreshSample%>"]').get("id");

    if (document.getElementById("<%=intIts.ClientID%>").value == '')
        document.getElementById("<%=intIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.FreshSample%>"]').get("id");

    if (document.getElementById("<%=dailyCntIts.ClientID%>").value == '')
        document.getElementById("<%=dailyCntIts.ClientID%>").value = "#" + Y.one('#itsList input[type=checkbox][value="<%=(int)CallOutcome.Completed%>"]').get("id");

    function switchItsList(value) {

        var currentCounterType = document.getElementById("<%=currentCounterType.ClientID%>").value;

        if (value > 1) {
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
            document.getElementById('<%= btnITSAllQ.ClientID %>').style.display = '';
            InitSelectAllButtonLabel();
        }
        else {
            document.getElementById('<%= btnITSAllQ.ClientID %>').style.display = 'none';
        }

        document.getElementById('<%=cbIncludeDisabledCalls.ClientID %>').parentElement.style.display = (value == 1 || value == 2) ? '' : 'none';
        if (needRefresh && (value == 0 || value == 1 || value == 4)) {
            Common.updatePanel('<%=ClientID %>');
        }
    }

    function onExtraCounterKeyPress() {
        if (event.keyCode == 13) // Enter pressed
        {
            Common.updatePanel('<%=ClientID %>');
        }
    }

    OnExtraCounterTypeChanged(false);

</script>
