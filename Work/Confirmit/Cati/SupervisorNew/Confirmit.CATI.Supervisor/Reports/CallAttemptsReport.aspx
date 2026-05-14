<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="CallAttemptsReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.CallAttemptsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_Grid" runat="server" HasMultySelectionCheckBox="false" HideSelectedColumn="true"
        PrimaryKeyColumn="ID" PageSize="50" SortedColumnName="EventDate" SortIndicator="Descending" ShowFullToolbarBorders="False">
        <toolbaritems>
            <controls:ToolbarCommandButton Key="Edit" ID="btnEdit" runat="server" />
            <controls:ToolbarCommandButton Key="Delete" ID="btnDelete" runat="server" />
            <controls:ToolbarCommandButton Key="IncludeDisposedByDialerAttempts" ButtonType="ToggleButton"  ID="btnIncludeDisposedByDialerAttempts" runat="server"/>
        </toolbaritems>
        <Commands>
            <controls:OverlayCommand Key="Edit" Title="<%$CPResource:EditCallHistory%>" Caption="<%$CPResource:Edit%>" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="530" Height="150" DialogMode="ViewEdit" IDName="CallHistoryId" IDColumnName="ID" Url="Surveys/CallHistoryProperties.aspx" />
            <controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" Confirmation="cnfr_DeleteCallAttempt" OnServerClick="Delete" />
            <controls:Command Key="IncludeDisposedByDialerAttempts" Caption="IncludeAttemptsDisposedByDialer" Image="phone_plus" />
        </Commands>
        <DataMenuItems>
            <controls:DataMenuItem Key="Edit" Text="Edit" />
            <controls:DataMenuItem Key="Delete" Text="Delete" />
        </DataMenuItems>

        <Columns>
            <controls:GeneralGridColumn Key="ID" DataFieldName="ID" Hidden="true" />
            <controls:GeneralGridColumn Key="EventDate" DataFieldName="EventDate" SearchColumnName="EventDate" Width="150"
                SearchColumnType="PredefinedDatePeriod" HeaderText="<%$CPResource:DateInLocalTZ%>" />
            <controls:GeneralGridColumn Key="ProjectID" DataFieldName="ProjectID" SearchColumnName="ProjectID"
                SearchColumnType="Text" HeaderText="<%$CPResource:ProjectId%>" />
            <controls:GeneralGridColumn Key="ProjectName" DataFieldName="ProjectName" SearchColumnName="ProjectName"
                SearchColumnType="Text" HeaderText="<%$CPResource:ProjectName%>" />
            <controls:GeneralGridColumn Key="InterviewerName" DataFieldName="InterviewerName"
                SearchColumnName="InterviewerName" SearchColumnType="Text" HeaderText="<%$CPResource:Interviewer%>" />
            <controls:GeneralGridColumn Key="InterviewID" DataFieldName="InterviewID" SearchColumnName="InterviewID"
                SearchColumnType="Number" HeaderText="<%$CPResource:InterviewId%>" />
            <controls:GeneralGridColumn Key="TelephoneNumber" DataFieldName="TelephoneNumber"
                SearchColumnName="TelephoneNumber" SearchColumnType="Text" HeaderText="<%$CPResource:Phone%>" />
            <controls:GeneralGridColumn Key="ExtendedStatusName" DataFieldName="ExtendedStatusName"
                SearchColumnName="ExtendedStatus" SearchColumnType="DropDown"
                HeaderText="<%$CPResource:ExtendedStatus%>" />
            <controls:GeneralGridColumn Key="WaitingTime" DataFieldName="WaitingTimeString" Width="50"
                HeaderText="<%$CPResource:Waiting%>" Header-Tooltip="<%$CPResource:WaitingTimeTooltip%>" />
            <controls:GeneralGridColumn Key="CallDuration" DataFieldName="CallDurationString" Width="53"
                HeaderText="<%$CPResource:Duration%>" Header-Tooltip="<%$CPResource:DurationTooltip%>" />
            <controls:GeneralGridColumn Key="DisplayTime" DataFieldName="DisplayTimeString" Width="45"
                HeaderText="<%$CPResource:Display%>" Header-Tooltip="<%$CPResource:DisplayTimeTooltip%>" />
            <controls:GeneralGridColumn Key="PreviewTime" DataFieldName="PreviewTimeString" Width="50"
                HeaderText="<%$CPResource:Preview%>" Header-Tooltip="<%$CPResource:PreviewTimeTooltip%>" />
            <controls:GeneralGridColumn Key="ConnectedTime" DataFieldName="ConnectedTimeString" Width="63"
                HeaderText="<%$CPResource:Connected%>" Header-Tooltip="<%$CPResource:ConnectedTimeTooltip%>" />
            <controls:GeneralGridColumn Key="WrapTime" DataFieldName="WrapTimeString" Width="50"
                HeaderText="<%$CPResource:Wrap%>" Header-Tooltip="<%$CPResource:WrapTimeTooltip%>" />
            <controls:GeneralGridColumn Key="OpenEndReviewDuration" DataFieldName="ReviewTimeString" Width="50"
                            HeaderText="<%$CPResource:Review%>" Header-Tooltip="<%$CPResource:ReviewTimeTooltip%>" />
        </Columns>
    </controls:Grid>
</asp:Content>
