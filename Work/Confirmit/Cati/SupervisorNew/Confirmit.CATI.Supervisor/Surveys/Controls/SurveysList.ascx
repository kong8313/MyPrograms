<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SurveysList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SurveysList" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Import Namespace="ConfirmitDialerInterface" %>

<script type="text/javascript">
    function openSurveyInfoFrame() {
        Y.on("domready", function () {
            openAndSetInfoFrame('<%=BaseRelativePath("Surveys/SurveyView.aspx")%>?Source=Combo&ID=<%=SurveySID.ToString() %>&SurveyPropertiesTab=<%=SurveyPropertiesTab %>');
        });
    }

    function checkForPredictiveSurvey(gridController) {

        var row = gridController.GetSelectedRow();

        if (row) {

            var dialMode = row.get_cellByColumnKey('DialMode').get_value();

            if (dialMode != '<%=(int)DialingMode.Predictive%>') {

                gridController.hideContextMenu();

                alert('<%=Strings.CallsSentToDialerDistributionIsAvailableForPredictiveSurveyOnly%>');

                return false;
            }
        }

        return true;
    }

    Common.onGlobalEvent("FilterSurveysBySurveyEvent", function (projectId) {
        var gridController = <%=m_grid.ClientControllerName%>;
        gridController.clearSearchFields();
        gridController.setValueToSearchField("Name", projectId);
        gridController.refresh();
    });

    function showSynchronizeRespondentsDialog() {
        var gridController = <%=m_grid.ClientControllerName%>;
        var row = gridController.GetSelectedRow();

        if (row) {
            var projectId = row.get_cellByColumnKey('Name').get_value();
            top.showSynchronizeRespondentsDialog(projectId);
        }
    }
</script>

<controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="SID" 
    SortedColumnName="SID" SortIndicator="Descending" OnDblClickCommand="View" IncludeGridName="True" GridName="Surveys list"
    ShowFullToolbarBorders="False">
    <commands>
		<Controls:Command Key="Open" Caption="Open" OnServerClick="Open" Image="survey_open"/>
		<Controls:Command Key="Close" Caption="Close" OnServerClick="Close" Confirmation="DoYouWantCloseSurvey" Image="survey_close"/>
		<Controls:Command Key="Shutdown" Caption="Shutdown" OnServerClick="Shutdown" Confirmation="DoYouWantShudownSurvey" Image="survey_shutdown"/>		
		<Controls:OverlayCommand Key="ExportQuotaStatusReport" DialogMode="ViewEdit" SelectMode="SingleRow" IDColumnName="SID" IDName="ID"  Title="ExportQuotaStatusReport" Caption="ExportQuotaStatusReport" Width="540" Height="420" URL="Surveys/ExportQuotaStatusReport.aspx"  Image="export_quota_status_report"/>		
		<%--<Controls:ViewCommand Key="SurveysAssignments" Caption="SurveysAssignments" WindowResCaptionName="SurveysAssignments" IDColumnName="SID" IDName="ID"  Width="1024" Height="610" URL="Surveys/SurveysAssignments.aspx" FloatingMode="true" SelectMode="No" Image="check"/>--%>
		<Controls:OverlayCommand Key="AddAssignment" Caption="AddAssignment" IDColumnName="SID" IDName="ID" Width="820" Height="640" Top="50" URL="Surveys/AddOrReplaceSurveyPersonAssignment.aspx" Title="AddAssignment" SelectMode="SingleRow" Image="assignment" RefreshInfoFrame="true"  />
		<Controls:OverlayCommand Key="ReplaceAssignment" Caption="ReplaceAssignment" IDColumnName="SID" IDName="ID" Width="820" Height="640" Top="50" URL="Surveys/AddOrReplaceSurveyPersonAssignment.aspx?ReplaceAssignment=true" Title="ReplaceAssignment" SelectMode="SingleRow" Image="assignment_replace"/>
		<Controls:ViewCommand Key="CallManagement" Caption="CallManagement" IDColumnName="SID" IDName="ID" Width="1024" Height="630" URL="CallManagement/CallManagement.aspx" FloatingMode="true" SelectMode="SingleRow" Image="call_management" />
		
        <Controls:OverlayCommand Key="CallHistoryExport" DialogMode="ViewEdit" Caption="CallHistoryExport" SelectMode="MultiRow" Title="ExportCallHistoryData" IDColumnName="SID" IDName="IDS" Width="750" Height="670" Top="100" URL="Surveys/ExportCallHistoryData.aspx" Image="call_history" />
        <Controls:ViewCommand Key="CallsSentToDialerDistribution" Caption="CallsSentToDialerDistribution" SelectMode="SingleRow" ValidateFunctionName="checkForPredictiveSurvey"  WindowResCaptionName="CallsSentToDialerDistribution" IDColumnName="SID" IDName="ID" Width="1300" Height="768" URL="Surveys/CallsSentToDialerDistribution.aspx" FloatingMode="true" Image="distribution"/>
        <Controls:ViewCommand Key="CallsPromotionHistory" Caption="CallsPromotionHistory" SelectMode="SingleRow" WindowResCaptionName="CallsPromotionHistory" IDColumnName="SID" IDName="ID" Width="800" Height="500" URL="Surveys/CallsPromotionHistory.aspx" FloatingMode="true" Image="history"/>
        
        <Controls:ViewCommand Key="SurveyOverview" Caption="SurveyOverview" WindowResCaptionName="SurveyOverview" IDColumnName="SID" IDName="ID" Width="1200" Height="800" URL="Reports/SurveyOverviewReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="ProductivityReport" Caption="ProductivityReport" WindowResCaptionName="ProductivityReport" IDColumnName="SID" IDName="ID" Width="1200" Height="800" URL="Reports/ProductivityReport.aspx?OpenSource=CP" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="SampleStatusSummaryReport" Caption="SampleStatusSummaryReport" WindowResCaptionName="SampleStatusSummaryReport" IDColumnName="SID" IDName="ID"  Width="1200" Height="650" URL="Reports/SampleStatusSummaryReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="SampleStatusSummaryByQuestionReport" Caption="SampleStatusSummaryByQuestionReport" WindowResCaptionName="SampleStatusSummaryByQuestionReport" IDColumnName="SID" IDName="ID"  Width="1200" Height="650" URL="Reports/SampleStatusSummaryByQuestionReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="InterviewerProductivityReport" Caption="InterviewerProductivityReport" WindowResCaptionName="InterviewerProductivityReport" IDColumnName="SID" IDName="ID" Width="1200" Height="630" URL="Reports/CatiProductivityReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="AttemptsByDispositionReport" Caption="AttemptsByDispositionReport" WindowResCaptionName="AttemptsByDispositionReport" IDColumnName="SID" IDName="ID" Width="1200" Height="630" URL="Reports/AttemptsByDispositionReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="NumberOfAttemptsReport" Caption="NumberOfAttemptsReport" WindowResCaptionName="NumberOfAttemptsReport" IDColumnName="SID" IDName="ID" Width="1200" Height="630" URL="Reports/NumberOfAttemptsReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="CallAttemptsReport" Caption="CallAttemptsReport" WindowResCaptionName="CallAttemptsReport" IDColumnName="SID" IDName="ID" Width="1200" Height="650" URL="Reports/CallAttemptsReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
		<Controls:ViewCommand Key="SampleUtilisationReport" Caption="SampleUtilisationReport" WindowResCaptionName="SampleUtilisationReport" IDColumnName="SID" IDName="ID" Width="1200" Height="650" URL="Reports/SampleUtilisationReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
        <Controls:ViewCommand Key="InboundCallSummaryReport" Caption="InboundCallSummaryReport" WindowResCaptionName="InboundCallSummaryReport" IDColumnName="SID" IDName="ID" Width="1200" Height="650" URL="Reports/InboundCallSummaryReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
        <Controls:ViewCommand Key="QuotaProgressReport" Caption="QuotaProgressReport" WindowResCaptionName="QuotaProgressReport" IDColumnName="SID" IDName="ID" Width="1200" Height="650" URL="Reports/QuotaProgressReport.aspx" FloatingMode="true" SelectMode="SingleRow"/>
        
		<Controls:ViewCommand Key="View" Caption="View" URL="Surveys/SurveyView.aspx" IDColumnName="SID" IDName="ID" Image="view" />
		<Controls:OverlayCommand Key="SendMessage" DialogMode="ViewEdit" SelectMode="MultiRow" Title="SendMessage" Caption="SendMessage" IDName="IDS" IDColumnName="SID" InlineParams="MessageRecipientType=Survey" Width="560" Height = "390" URL="Messaging/SendMessageView.aspx" Image="send"/>
        
        <Controls:Command Key="SynchronizeRespondents" Caption="SynchronizeRespondents" Image="synchronize_respondents" OnClientClick="showSynchronizeRespondentsDialog()"/>
        </commands>
    <toolbaritems>
        <Controls:ToolbarCommandButton Key="View"/>
        <Controls:ToolbarCommandButton Key="CallManagement"/> 
        <Controls:ToolbarCommandButton Key="CallHistoryExport"/>
        <asp:Button runat="server" ID="ReviewerOpen" Text="<%$CPResource:GoToReviewer%>" class="open-new-button open-new-icon"/>
    </toolbaritems>

    <DataMenuItems>
        <controls:DataMenuItem Key="Open"/>
        <controls:DataMenuItem Key="Close"/>
        <controls:DataMenuItem Key="Shutdown"/>
        
        <controls:DataMenuItem IsSeparator="true" />
        <Controls:DataMenuItem Key="View"/>
		<Controls:DataMenuItem Key="AddAssignment"/>
		<Controls:DataMenuItem Key="ReplaceAssignment"/>
		<%--<Controls:DataMenuItem Key="SurveysAssignments"/>--%>
        <Controls:DataMenuItem Key="CallManagement"/>
        
        <controls:DataMenuItem IsSeparator="true" />
        <Controls:DataMenuItem Key="CallHistoryExport"/>
        <Controls:DataMenuItem Key="ExportQuotaStatusReport"/>

        <controls:DataMenuItem IsSeparator="true" />
		<Controls:DataMenuItem Key="CallsSentToDialerDistribution"/>
        <Controls:DataMenuItem Key="CallsPromotionHistory"/>
        
        <controls:DataMenuItem IsSeparator="true" />
        <Controls:DataMenuItem Key="SendMessage"/>
        <controls:DataMenuItem IsSeparator="True"/>
        <Controls:DataMenuItem Text="Reports" ImageUrl="reports">
            <Items>
                <Controls:DataMenuItem Key="SurveyOverview" />
        	    <Controls:DataMenuItem Key="ProductivityReport" />
        	    <Controls:DataMenuItem Key="SampleStatusSummaryReport" />
                <Controls:DataMenuItem Key="SampleStatusSummaryByQuestionReport" />
        	    <controls:DataMenuItem Key="CallAttemptsReport" />
        	    <Controls:DataMenuItem Key="InterviewerProductivityReport" />
        	    <Controls:DataMenuItem Key="AttemptsByDispositionReport" />
        	    <Controls:DataMenuItem Key="NumberOfAttemptsReport" />
                <Controls:DataMenuItem Key="SampleUtilisationReport" />
                <Controls:DataMenuItem Key="InboundCallSummaryReport" />
                <Controls:DataMenuItem Key="QuotaProgressReport" />
            </Items>
        </Controls:DataMenuItem>
        <controls:DataMenuItem IsSeparator="true" />
        <Controls:DataMenuItem Key="SynchronizeRespondents"/>
    </DataMenuItems>
        
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" Key="SID" Width="60" Hidden="True" />

        <controls:GeneralGridColumn Key="Name" HeaderText="<%$CPResource:ProjectId%>" SearchColumnType="Text" Width="150px"/>       
        <controls:GeneralGridColumn Key="CampaignID" HeaderText="<%$CPResource:CampaignID%>" SearchColumnType="Text" Width="150px"/>       
        <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Description" SearchColumnType="Text" Width="100%"/>

        <controls:GeneralGridColumn HeaderText="<%$CPResource:SampleSize%>" Key="SampleSize" SearchColumnType="Number" Width="100" />
        <controls:GeneralGridColumn HeaderText="State" Key="State"  SearchColumnType="DropDown" Width="80" />
        <controls:GeneralGridColumn Key="DialMode"  Hidden="True"/>

    </Columns>
</controls:Grid>