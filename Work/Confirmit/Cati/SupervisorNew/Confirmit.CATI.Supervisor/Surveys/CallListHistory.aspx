<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="CallListHistory.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Surveys.CallListHistory" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style type="text/css">
        tbody tr.IsLinkedInterview {
            color: Orange;
        }
    </style>
    <controls:Dialog runat="server" ID="dialog" EnableViewState="true" HideHeader="True" HideButtons="True" Mode="Frame">
        <Content>
            <controls:Grid ID="grid" HintText="<%$CPResource:CallAttemptsHistoryHint %>" runat="server" PrimaryKeyColumn="CallHistoryId" HideSelectedColumn="true" EnablePaging="false" EnableSorting="False">
                <Commands>
                    <controls:Command Key="LinkedInterviews" Caption="LinkedInterviews" SelectMode="SingleRow" Image="record_voice" OnClientClick="ShowLinkedInterviews()" />
                    <controls:BaseOverlayCommand Key="Edit" Caption="Edit" SelectMode="SingleRow" Image="edit" OnClientClick="ShowCallHistoryProperties()" />
                    <controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" Confirmation="cnfr_DeleteCallAttempt" OnServerClick="Delete" />
                </Commands>
                <ToolbarItems>
                    <controls:ToolbarCommandButton Key="Edit" runat="server" />
                    <controls:ToolbarCommandButton Key="Delete" runat="server" />
                    <controls:ToolbarCommandButton Key="LinkedInterviews" ID="ToolbarCommandButton2" runat="server" />
                </ToolbarItems>
                <DataMenuItems>
                    <controls:DataMenuItem Key="Edit" Text="Edit" />
                    <controls:DataMenuItem Key="Delete" Text="Delete" />
                </DataMenuItems>

                <Columns>
                    <controls:GeneralGridColumn
                        HeaderText="CallHistoryId"
                        Key="CallHistoryId"
                        DataFieldName="CallHistoryId"
                        Width="100"
                        Hidden="true" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:Order%>"
                        Key="SurveyId"
                        DataFieldName="SurveyId"
                        Width="100"
                        Hidden="true" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:CallAttemptNumber%>"
                        Key="CallAttemptNumber"
                        DataFieldName="CallAttemptNumber"
                        Width="25" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:StartTime%>"
                        Key="StartTime"
                        DataFieldName="StartTime"
                        Width="126" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:EndTime%>"
                        Key="EndTime"
                        DataFieldName="EndTime"
                        Width="126" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:ExtendedStatusCode%>"
                        Key="ITS"
                        DataFieldName="ITS"
                        Width="135"
                        Hidden="true" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:ExtendedStatus%>"
                        Key="TransientState"
                        DataFieldName="TransientState"
                        Width="100" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:Person%>"
                        Key="Person"
                        DataFieldName="Person"
                        Width="90" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:TelNumber%>"
                        Key="TelNumber"
                        DataFieldName="TelNumber"
                        Width="120" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:RespondentName%>"
                        Key="Respondent"
                        DataFieldName="Respondent"
                        Width="100" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:TimeZoneID%>"
                        Key="TimeZoneID"
                        DataFieldName="TimeZoneId"
                        Width="100"
                        Hidden="true" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:Timezone%>"
                        Key="TimeZone"
                        DataFieldName="TimeZone"
                        Width="100" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:InterviewTime%>"
                        Key="InterviewTime"
                        DataFieldName="InterviewTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="90" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:ReviewTime%>"
                        Key="ReviewTime"
                        DataFieldName="ReviewTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="90" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:WaitTime%>"
                        Key="WaitingTime"
                        DataFieldName="WaitingTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="70" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:PreviewTime%>"
                        Key="PreviewTime"
                        DataFieldName="PreviewTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="70" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:ConnectedTime%>"
                        Key="ConnectedTime"
                        DataFieldName="ConnectedTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="70" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:WrapTime%>"
                        Key="WrapTime"
                        DataFieldName="WrapTime"
                        DataFormatString="{0:HH:mm:ss}"
                        Width="70" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:AppointmentTime%>"
                        Key="TimeToCalc"
                        DataFieldName="TimeToCall"
                        Width="126" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:AppointmentExpTime%>"
                        Key="TimeToExpire"
                        DataFieldName="TimeToExpire"
                        Width="140" />
                    <controls:GeneralGridColumn
                        HeaderText="<%$CPResource:CallCenter%>"
                        Key="CallCenterName"
                        DataFieldName="CallCenterName"
                        Width="100" />
                    <controls:GeneralGridColumn
                        HeaderText=""
                        Key="LinkedInterviewSessionId"
                        DataFieldName="LinkedInterviewSessionId"
                        Width="100"
                        Hidden="True" />
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
    <script type="text/javascript">

        function ShowLinkedInterviews(gridController) {

            var linkedInterviewSessionId = parseInt(gridController.GetSelectedRow().get_cellByColumnKey('LinkedInterviewSessionId').get_value());
            var surveyId = "<%=SurveyId%>";
            var interviewId = "<%=InterviewId%>";

            if (linkedInterviewSessionId === 0) {
                alert("Not a linked interview");
                return;
            }

            var settings = { height: 420 + "px", width: 680 + "px" };
            var params = {
                LinkedInterviewSessionId: linkedInterviewSessionId,
                SurveyId: surveyId,
                InterviewId: interviewId
            };

            overlay.show("<%=Strings.LinkedInterviews%>", "Surveys/LinkedInterviewChain.aspx", params, settings, null);
        }

        function ShowCallHistoryProperties(gridController) {

            var callHistoryId = parseInt(gridController.GetSelectedRow().get_cellByColumnKey('CallHistoryId').get_value());

            if (callHistoryId === 0) {
                alert("<%=Strings.ProhibitedToEditCallHistory%>");
                return;
            }

            var settings = { height: 150 + "px", width: 530 + "px" };
            var params = {
                CallHistoryId: callHistoryId
            };

            overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;

                Common.updatePanel('');
            });

            overlay.show("<%=Strings.EditCallHistory%>", "Surveys/CallHistoryProperties.aspx", params, settings, null);
        }
    </script>

</asp:Content>
