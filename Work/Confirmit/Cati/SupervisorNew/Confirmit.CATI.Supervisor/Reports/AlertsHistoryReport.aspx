<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AlertsHistoryReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.AlertsHistoryReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_Grid" runat="server" HasMultySelectionCheckBox="false"
                   HideSelectedColumn="true" PrimaryKeyColumn="SubmissionTime"
                   PageSize="50" SortedColumnName="EventDate" SortIndicator="Descending" ShowFullToolbarBorders="False">
        <ToolbarItems>
            <controls:XpMenuItem ID="btnSurveys" runat="server" Text="Surveys..." TextAndImage="true" ImageName="assignment_turned_in">
            </controls:XpMenuItem>
            <controls:XpMenuItem ID="btnPersons" runat="server" ButtonType="Button" Text="Interviewers..." TextAndImage="true" ImageName="persons">
            </controls:XpMenuItem>
        </ToolbarItems>
        <Columns>
            <controls:GeneralGridColumn Key="SubmissionTime" DataFieldName="SubmissionTime"
                                        SearchColumnName="SubmissionTime" Width="160" SearchColumnType="PredefinedDatePeriod"
                                        HeaderText="<%$CPResource:DateInLocalTZ%>" />
            <controls:GeneralGridColumn Key="ProjectId" DataFieldName="ProjectId" SearchColumnName="ProjectId"
                                        Width="100" SearchColumnType="Text" HeaderText="<%$CPResource:ProjectId%>" />
            <controls:GeneralGridColumn Key="SurveyName" DataFieldName="SurveyName" Width="50%"
                                        SearchColumnName="SurveyName" SearchColumnType="Text" HeaderText="<%$CPResource:ProjectName%>" />
            <controls:GeneralGridColumn Key="PersonName" DataFieldName="PersonName" Width="50%"
                                        SearchColumnName="PersonName" SearchColumnType="Text" HeaderText="<%$CPResource:Interviewer%>" />
            <controls:GeneralGridColumn Key="InterviewId" DataFieldName="InterviewId" Width="90"
                                        SearchColumnName="InterviewId" SearchColumnType="Number" HeaderText="<%$CPResource:InterviewId%>" />
            <controls:GeneralGridColumn Key="AlertType" DataFieldName="AlertType" Width="100"
                                        SearchColumnName="AlertType" SearchColumnType="TextDropDown" HeaderText="Type" />
            <controls:GeneralGridColumn Key="Alert" DataFieldName="Alert" Width="95" SearchColumnName="Alert"
                                        SearchColumnType="TextDropDown" HeaderText="<%$CPResource:Threshold%>" />
            <controls:GeneralGridColumn Key="AnswerDuration" DataFieldName="AnswerDuration"
                                        Width="105" SearchColumnName="AnswerDuration" SearchColumnType="Number"
                                        HeaderText="Duration (sec)" />
            <controls:GeneralGridColumn Key="QuestionId" DataFieldName="QuestionId" Width="75"
                                        SearchColumnName="QuestionId" SearchColumnType="Text" HeaderText="<%$CPResource:Question%>" />
            <controls:GeneralGridColumn Key="InterviewState" DataFieldName="InterviewState"
                                        Width="110" SearchColumnName="InterviewState" SearchColumnType="TextDropDown"
                                        HeaderText="<%$CPResource:InterviewStateName%>" />
        </Columns>
    </controls:Grid>    
</asp:Content>
