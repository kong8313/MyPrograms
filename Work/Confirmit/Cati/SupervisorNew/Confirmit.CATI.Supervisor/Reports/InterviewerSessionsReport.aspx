<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="InterviewerSessionsReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.InterviewerSessionsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_Grid" runat="server" HasMultySelectionCheckBox="false" IncludeGridName="True"
        HideSelectedColumn="true" PrimaryKeyColumn="PersonName" GridName="<%$CPResource:InterviewerBreakDetails%>"
        PageSize="50" SortedColumnName="StartTime" SortIndicator="Descending" ShowFullToolbarBorders="False"
        TopToolbarLayout="DoubleMenu">
        <LeftToolbarItems>
            <controls:XpMenuItem ID="btnPersons" runat="server"
                Text="Interviewers..." TextAndImage="true" ImageName="persons">
            </controls:XpMenuItem>
        </LeftToolbarItems>
        <Columns>
            <controls:GeneralGridColumn Key="PersonName" DataFieldName="PersonName" Width="100%"
                SearchColumnName="PersonName" SearchColumnType="Text" HeaderText="<%$CPResource:Interviewer%>" />
            <controls:GeneralGridColumn Key="StartTime" DataFieldName="StartTime" SearchColumnName="StartTime"
                Width="170" SearchColumnType="PredefinedDatePeriod" HeaderText="<%$CPResource:StartDateLocalTZ%>" />
            <controls:GeneralGridColumn Key="FinishTime" DataFieldName="FinishTime" SearchColumnName="FinishTime"
                Width="170" SearchColumnType="PredefinedDatePeriod" HeaderText="<%$CPResource:FinishDateLocalTZ%>" />
            <controls:GeneralGridColumn Key="Duration" DataFieldName="Duration" Width="130"
                SearchColumnName="Duration" SearchColumnType="Number" HeaderText="<%$CPResource:InterviewerSessionsReport_DurationHeader %>" />
            <controls:GeneralGridColumn Key="Event" DataFieldName="Event" Width="200" EnableSorting="False"
                SearchColumnType="DropDown" HeaderText="<%$CPResource:EventBreakReportColumn%>" />
        </Columns>
    </controls:Grid>    
</asp:Content>
