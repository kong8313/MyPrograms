<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShiftOutlook.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Script.ShiftOutlook.ShiftOutlook" %>
<script type="text/javascript">

    var debounce = (func, delay) => {
        var inDebounce;
        return function () {
            const context = this;
            const args = arguments;
            clearTimeout(inDebounce);
            inDebounce = setTimeout(() => func.apply(context, args), delay);
        }
    };

    Y.on("load", function () {
        Y.on("windowresize", debounce(resizeScript, 100));
    });

    function resizeScript() {
        var webDayView = Y.one("#" + "<%=WebdayView1.ClientID%>");
        var webDayViewScrollableDiv = Y.one("#" + "<%=WebdayView1.ClientID%>" + "_divScroll");
        var shiftsTable = webDayViewScrollableDiv.get("childNodes");

        var webDayViewHeight = webDayView.get('parentNode').get("offsetHeight") - webDayView.get("offsetTop");

        webDayView.setStyle("height", webDayViewHeight + "px");

        if (shiftsTable._nodes[1]) {
            // 16 px for ActivityEdge row
            shiftsTable._nodes[1].style.height = "calc(100% - 16px)";
            shiftsTable._nodes[1].style.width = "100%";
        }
    }

</script>
<div id="divWebDayView" class="web-day-view <%=CssClass %>">
    <table style="width: 100%; height: 14px; border: 1px; margin-bottom: 10px;">
        <tr style="height: 12px; border: 1px">
            <td style="width: 15px; background: LightSteelBlue">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td style="padding: 0px 15px 0px 5px;" nowrap>shift default
            </td>
            <td style="width: 1px; background: LightGreen">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td style="padding: 0px 15px 0px 5px;" nowrap>shift overridden
            </td>
            <td style="width: 15px; background: LightYellow">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td style="padding: 0px 15px 0px 5px;" nowrap>exclusion default
            </td>
            <td style="width: 15px; background: Bisque">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td style="padding: 0px 15px 0px 5px;" nowrap>exclusion overridden
            </td>
            <td style="width: 100%"></td>
        </tr>
    </table>
    <ig_sched:WebScheduleInfo ID="WebScheduleInfo1" runat="server" EnableReminders="false"
        EnableRecurringActivities="false" TimeDisplayFormat="Default" EnableProgressIndicator="False"
        EnableSmartCallbacks="False" EnableTheming="False" FirstDayOfWeek="Monday" AllowAllDayEvents="False">
    </ig_sched:WebScheduleInfo>
    <controls:CatiWebDayView ID="WebdayView1" Width="100%" Font-Size="XX-Small" CssClass="web-day-view__ig-view"
        EnableActivityMoving="false" EnableActivityResizing="false" EnableAutoActivityDialog="false"
        WebScheduleInfoID="WebScheduleInfo1" TimeSlotInterval="SixtyMinutes" TimeDisplayFormat="Time24Hour"
        runat="server" VisibleDays="7" NavigationAnimation="None" BackColor="White" EnableTheming="False"
        AppointmentTooltipFormatString="<SUBJECT><NEW_LINE><DESCRIPTION>" EnableAppStyling="True">
        <CaptionHeaderStyle Height="10px">
        </CaptionHeaderStyle>
        <ActivityEdgeFreeStyle BackColor="LightGreen">
        </ActivityEdgeFreeStyle>
        <ActivityEdgeTentativeStyle BackColor="Bisque">
        </ActivityEdgeTentativeStyle>
        <NextButtonImage AlternateText="next week" />
        <ActivityEdgeOutofOfficeStyle BackColor="LightYellow">
        </ActivityEdgeOutofOfficeStyle>
        <ActivityEdgeBusyStyle BackColor="LightSteelBlue">
        </ActivityEdgeBusyStyle>
        <PrevButtonImage AlternateText="prev week" />
        <AllDayEventAreaStyle Height="0px">
        </AllDayEventAreaStyle>
        <AppointmentStyle Width="40px">
        </AppointmentStyle>
        <WorkingTimeSlotStyle BackColor="White">
        </WorkingTimeSlotStyle>
        <NonWorkingTimeSlotStyle BackColor="White">
        </NonWorkingTimeSlotStyle>
        <TodayHeaderStyle BackColor="White">
        </TodayHeaderStyle>
    </controls:CatiWebDayView>
</div>
