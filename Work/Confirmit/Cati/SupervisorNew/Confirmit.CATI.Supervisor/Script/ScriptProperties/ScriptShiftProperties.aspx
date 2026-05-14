<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptShiftProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptShiftProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <asp:HiddenField ID="hfConfimOverride" runat="server" />
    <asp:HiddenField ID="hfConfimNewNotDefault" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="hfTemplateType" runat="server" />
    <asp:HiddenField ID="hfRowId" runat="server" />
    <asp:HiddenField ID="hfHasRespondentTimeZone" runat="server" />
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClientClick="if(!ValidateChanges()) return false;" OnClick="OKButtonClick" />

        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <%=GetResString("Shift Type:")%>
                        </td>
                        <td colspan="3">
                            <controls:DropDownList columnKey="ShiftTypeName" ID="ddlShiftType" runat="server" Width="100%" />
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <%=Strings.StartDay%>
                        </td>
                        <td>
                            <asp:PlaceHolder runat="server" ID="phShiftsStartDay">
                                <controls:DayOfWeekDropDownList columnKey="StartDay" ID="ddlStartDay" runat="server"
                                    Width="100%" onChange="synchronizeDays();" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="phExclusiveStartDay">
                                <controls:DatePicker DropButton-Style-Height="14px" ID="wdteStartDate"
                                    runat="server" Width="100%" Height="18px" AllowNull="false" EnableViewState="false">
                                    <ClientSideEvents ValueChanged="synchronizeDates"></ClientSideEvents>
                                </controls:DatePicker>
                            </asp:PlaceHolder>
                        </td>
                        <td>
                            <%=Strings.ShiftTime%>
                        </td>
                        <td>
                            <controls:TextBox ID="tbStartTime" columnKey="StartTime" runat="server" Width="60px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.FinishDay%>
                        </td>
                        <td>
                            <asp:PlaceHolder runat="server" ID="phShiftsEndDay">
                                <controls:DayOfWeekDropDownList columnKey="EndDay" ID="ddlEndDay" runat="server"
                                    Width="100%" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="phExclusiveEndDay">
                                <controls:DatePicker DropButton-Style-Height="14px" ID="wdteEndDate"
                                    runat="server" Width="100%" Height="18px" AllowNull="false" EnableViewState="false">
                                </controls:DatePicker>
                            </asp:PlaceHolder>
                        </td>
                        <td>
                            <%=Strings.ShiftTime%>
                        </td>
                        <td>
                            <controls:TextBox ID="tbEndTime" columnkey="EndTime" runat="server" Width="60px" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
    <script id="shiftScript" language="javascript" type="text/javascript">

        /*Validation for entered time*/
        function ValidateChanges() {

            var TemplateType = { Shifts: 0, Exclusions: 1 };

            var hfRowId = document.getElementById("<%=this.hfRowId.ClientID%>");
            var hfConfimOverride = document.getElementById("<%=this.hfConfimOverride.ClientID%>");
            var hfConfimNewNotDefault = document.getElementById("<%=this.hfConfimNewNotDefault.ClientID%>");
            var tbStartTime = document.getElementById("<%=this.tbStartTime.ClientID%>");
            var tbEndTime = document.getElementById("<%=this.tbEndTime.ClientID%>");
            var ddlShiftType = document.getElementById("<%=this.ddlShiftType.ClientID%>");

            var hfHasRespondentTimeZone = document.getElementById("<%=this.hfHasRespondentTimeZone.ClientID%>");
            var templateType = $get("<%=hfTemplateType.ClientID%>").value;

            if (hfRowId.value && hfRowId.value != "null") {

                if (hfConfimOverride.value != "" && hfHasRespondentTimeZone.value == "true") {
                    if ((templateType == TemplateType.Shifts) || templateType == TemplateType.Exclusions) {
                        if (!confirm(hfConfimOverride.value)) return false;
                    }
                }
            }
            else {
                if (hfConfimNewNotDefault.value != "") {
                    if (!confirm(hfConfimNewNotDefault.value)) return false;
                }
            }

            if (ddlShiftType.selectedIndex < 0) {
                alert('<%=GetResString("Shift type should be selected")%>');
                ddlShiftType.focus();
                return false;
            }

            if (!CheckTime(tbStartTime.value)) {
                alert('<%=GetResString("Start time is incorrect format")%>');
                tbStartTime.focus();
                return false;
            }

            if (!CheckTime(tbEndTime.value)) {
                alert('<%=GetResString("End time is incorrect format")%>');
                tbEndTime.focus();
                return false;
            }

            if (templateType == TemplateType.Shifts) {

                /*for start/end day controls for shifts*/
                var ddlStartDay = document.getElementById("<%=this.ddlStartDay.ClientID%>");
                var ddlEndDay = document.getElementById("<%=this.ddlEndDay.ClientID%>");

                if (!CheckDuration(ddlStartDay.value, ddlEndDay.value, tbStartTime.value, tbEndTime.value)) {
                    alert('<%=Strings.ShiftEndEqualsStart%>');
                    tbEndTime.focus();
                    return false;
                }
                if (!IsStartOfWeek(ddlEndDay.selectedIndex, tbEndTime.value)) {
                    if (ddlEndDay.selectedIndex < ddlStartDay.selectedIndex) {
                        alert('<%=Strings.EndDayLessStartDay%>');
                    tbEndTime.focus();
                    return false;
                }
                else if (ddlEndDay.selectedIndex == ddlStartDay.selectedIndex) {
                    var startTime = parseInt(tbStartTime.value.split(":")[0], 10) * 60 + parseInt(tbStartTime.value.split(":")[1], 10);
                    var endTime = parseInt(tbEndTime.value.split(":")[0], 10) * 60 + parseInt(tbEndTime.value.split(":")[1], 10);

                    if (startTime > endTime) {
                        alert('<%=Strings.EndTimeLessStartTime%>');
                            tbEndTime.focus();
                            return false;
                        }
                    }
                }
            } else if (templateType == TemplateType.Exclusions) {

                /*for start/end day controls for exclusions*/
                var startDateChooser = $IG.WebTextEditor.find("<%=this.wdteStartDate.ClientID%>");
                var endDateChooser = $IG.WebTextEditor.find("<%=this.wdteEndDate.ClientID%>");

                var startDate = startDateChooser.get_value();
                var endDate = endDateChooser.get_value();

                if (startDate == null) {
                    alert('<%=Strings.StartDateIsNotSpecified%>');
                    startDateChooser.focus();
                    return false;
                }

                if (endDate == null) {
                    alert('<%=Strings.EndDateIsNotSpecified%>');
                    endDateChooser.focus();
                    return false;
                }

                var startMilliseconds = Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate(), startDate.getHours(), startDate.getMinutes(), startDate.getSeconds(), startDate.getMilliseconds());
                var endMilliseconds = Date.UTC(endDate.getFullYear(), endDate.getMonth(), endDate.getDate(), endDate.getHours(), endDate.getMinutes(), endDate.getSeconds(), endDate.getMilliseconds());
                if (endMilliseconds < startMilliseconds) {
                    alert('<%=Strings.EndDayLessStartDay%>');
                tbEndTime.focus();
                return false;
            }
            else {
                if (endMilliseconds == startMilliseconds) {
                    var startTime = parseInt(tbStartTime.value.split(":")[0], 10) * 60 + parseInt(tbStartTime.value.split(":")[1], 10);
                    var endTime = parseInt(tbEndTime.value.split(":")[0], 10) * 60 + parseInt(tbEndTime.value.split(":")[1], 10);

                    if (startTime > endTime) {
                        alert('<%=Strings.EndTimeLessStartTime%>');
                            tbEndTime.focus();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        function CheckTime(value) {
            var regEx = new RegExp("^([0-9]|[0-1][0-9]|[2][0-3])[:][0-5][0-9]$");
            if (regEx.test(value.trim())) {
                return true;
            }
            return false;
        }

        function CheckDuration(startDay, endDay, startTime, endTime) {
            if (startDay == endDay && startTime == endTime) {
                return false;
            }
            return true;
        }

        function IsStartOfWeek(day, time) {
            return day == 0 && (time == '0:00' || time == '00:00');
        }

        /* Sets selected end day equal to currently selected start day of shift.*/
        function synchronizeDays() {
            var ddlStartDay = document.getElementById("<%=ddlStartDay.ClientID%>");
            var ddlEndDay = document.getElementById("<%=ddlEndDay.ClientID%>");
            ddlEndDay.selectedIndex = ddlStartDay.selectedIndex;
        }

        function synchronizeDates() {
            var startDateChooser = $IG.WebTextEditor.find("<%=wdteStartDate.ClientID%>");
            var endDateChooser = $IG.WebTextEditor.find("<%=wdteEndDate.ClientID%>");

            if (startDateChooser != null && endDateChooser != null) {
                endDateChooser.set_text(startDateChooser.get_text());
            }
        }

    </script>

</asp:Content>
