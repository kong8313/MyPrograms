<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityStatusBar.ascx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.ActivityStatusBar" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<div class="ActivityListStatusBar">
    <asp:Panel runat="server" ID="pnlSystemInfo" class="flex-panel flex-panel-row">
        <div id="activity-progress-placeholder"></div>
        <div id="divLoggedInterviewers" runat="server" class="activity-progress-label activity-progress-interviewers">
            <div runat="server" id="activityListExceededWarningMessage" >
                <controls:SvgImage Visible="False" id="activityListExceededWarning" runat="server" ImageName="warning" />
            </div>
            <span><%=Strings.LoggedInterviewers%></span>
            <asp:Label ID="lblLoggedInterviewers" runat="server" />
        </div>
        <div id="divLoggedIvrAgents" runat="server" class="activity-progress-label">
            <span id="spnLoggedIvrAgents" runat="server"><%=Strings.LoggedIVRAgents%></span>
            <asp:Label ID="lblLoggedIvrAgents" runat="server" />
        </div>
        <div id="divOpenSurveys" runat="server" class="activity-progress-label">
            <span><%=Strings.OpenSurveys%></span>
            <asp:Label ID="lblOpenSurveys" runat="server" />
        </div>
        <div id="divCalls" runat="server" class="activity-progress-label">
            <span><%=Strings.StrikeRate%></span>
            <asp:Label ID="lblCalls" runat="server" />
        </div>
    </asp:Panel>
    <div>
        <asp:Label ID="lblTime" runat="server" />
    </div>
</div>
