<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTimeRangeSelect.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.DateTimeRangeSelect" %>
<%@ Register TagPrefix="PageControls" TagName="DateTimeEdit" Src="~/Controls/DateTimeEdit.ascx" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<div class="flex-panel flex-panel-row date-time-range-select">
    <controls:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged" />
    <controls:ImageButton ID="btnDateTimeRangeSelect" Text="Range..." runat="server" IsSubmit="false" ImageName="date_range" />
</div>

<asp:Panel ID="pnlDTRS" runat="server" CssClass="popupExtenderBordedPanel">
    <style type="text/css">
        .datetime-range__dates-wrapper {
            padding:5px 15px;
            border-bottom: 1px solid rgba(18, 24, 33, 0.12);
        }
    </style>

    <div class="datetime-range__dates-wrapper" >
        <table>
            <tr>
                <td nowrap style="text-align: left">
                    <b>
                        <%=Strings.FromDateTime%>
					    &nbsp;</b>
                </td>
                <td>
                    <PageControls:DateTimeEdit ID="dteStart" runat="server" />
                </td>
            </tr>
            <tr>
                <td nowrap style="text-align: left">
                    <b>
                        <%=Strings.ToDateTime%>
					    &nbsp;</b>
                </td>
                <td>
                    <PageControls:DateTimeEdit ID="dteEnd" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div style="text-align: right; margin: 5px;">
        <controls:Button ID="bttnOK" runat="server" ResName="Dlg_Ok" IsSubmit="true"
            OnClientClick="return okHandler();" OnClick="bttnOK_Click" />
    </div>
</asp:Panel>
<controls:PopupExtender ID="pexDTRS" MasterID="btnDateTimeRangeSelect" SlaveID="pnlDTRS" runat="server" />
<script type="text/javascript">
    function okHandler() {
        var result = checkInterval();
        if (result) {
            hidePopup();
            document.getElementById('<%=ddlFilter.ClientID%>').value = 0;
        }
        else {
            alert('<%=Strings.EndTimeLessStartTime %>');
        }
        return result;
    }

    // Checks that time interval is valid (not negative).
    // Returns true is interval is valid. Otherwise - false.
    function checkInterval() {
        var startDate = <%=dteStart.ClientControllerName%>.getDate();
        var endDate = <%=dteEnd.ClientControllerName%>.getDate();
        var startTime = <%=dteStart.ClientControllerName%>.getTime();
        var endTime = <%=dteEnd.ClientControllerName%>.getTime();

        if (startDate > endDate || (startDate.toString() == endDate.toString() && startTime >= endTime)) {
            return false;
        }

        return true;
    }
</script>
