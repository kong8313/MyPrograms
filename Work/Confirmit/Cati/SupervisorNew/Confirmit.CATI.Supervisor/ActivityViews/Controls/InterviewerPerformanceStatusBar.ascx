<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InterviewerPerformanceStatusBar.ascx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.InterviewerPerformanceStatusBar" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<div class="ActivityListStatusBar">
    <asp:Panel runat="server" ID="pnlSystemInfo" class="flex-panel flex-panel-row">
        <div id="activity-progress-placeholder"></div>
        <div class="activity-progress-label activity-progress-interviewers">
            <div runat="server" id="totalInterviewsExceededWarningMessage" >
                <controls:SvgImage Visible="False" id="totalInterviewsExceededWarning" runat="server" ImageName="warning" />
            </div>
            <span><%=Strings.TotalInterviewersWorkedToday%></span>
            <asp:Label ID="lblTotalInterviewersWorkedToday" runat="server" />
        </div>
        <div class="activity-progress-label">
            <span><%=Strings.LoggedInterviewers%></span>
            <asp:Label ID="lblLoggedInterviewers" runat="server" />
        </div>
    </asp:Panel>
    <div>
        <asp:Label ID="lblTime" runat="server" />
    </div>
</div>
