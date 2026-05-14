<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="InboundCallHistoryReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.InboundCallHistoryReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_Grid" runat="server" HasMultySelectionCheckBox="false" HideSelectedColumn="true"
        PrimaryKeyColumn="ID" PageSize="50" SortedColumnName="EventDate" SortIndicator="Descending" ShowFullToolbarBorders="False">
        <Columns>
            <controls:GeneralGridColumn Key="ID" DataFieldName="ID" Hidden="true" />
            <controls:GeneralGridColumn Key="EventDate" DataFieldName="EventDate" SearchColumnName="EventDate" Width="150"
                SearchColumnType="PredefinedDatePeriod" HeaderText="<%$CPResource:DateInLocalTZ%>" />
            <controls:GeneralGridColumn Key="ProjectID" DataFieldName="ProjectID" SearchColumnName="ProjectID"
                SearchColumnType="Text" HeaderText="<%$CPResource:ProjectId%>" />
            <controls:GeneralGridColumn Key="ProjectName" DataFieldName="ProjectName" SearchColumnName="ProjectName"
                SearchColumnType="Text" HeaderText="<%$CPResource:ProjectName%>" />
            <controls:GeneralGridColumn Key="InboundNumber" DataFieldName="InboundNumber"
                SearchColumnName="InboundNumber" SearchColumnType="Text" HeaderText="<%$CPResource:InboundTelNumber%>" />
            <controls:GeneralGridColumn Key="RespondentNumber" DataFieldName="RespondentNumber" SearchColumnName="RespondentNumber"
                SearchColumnType="Text" HeaderText="<%$CPResource:RespondendTelNumber%>" />
            <controls:GeneralGridColumn Key="InterviewId" DataFieldName="InterviewId"
                SearchColumnName="InterviewId" SearchColumnType="Text" HeaderText="<%$CPResource:InterviewId%>" />
            <controls:GeneralGridColumn Key="OperationTitle" DataFieldName="OperationTitle"
                SearchColumnName="OperationType" SearchColumnType="DropDown" EnableSorting="False" HeaderText="<%$CPResource:InboundOperation%>" />

        </Columns>
    </controls:Grid>
</asp:Content>
