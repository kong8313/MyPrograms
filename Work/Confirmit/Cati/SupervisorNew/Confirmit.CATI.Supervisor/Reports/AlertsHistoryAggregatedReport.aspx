<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AlertsHistoryAggregatedReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.AlertsHistoryAggregatedReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="m_Grid" runat="server" IncludeGridName="false" HasMultySelectionCheckBox="false"
        HideSelectedColumn="true" PrimaryKeyColumn="InterviewerId"
        SortedColumnName="EventDate" SortIndicator="Descending" TopToolbarLayout="DoubleMenu"
        ShowFullToolbarBorders="False">
        <LeftToolbarItems>
            
            <controls:XpMenuItem runat="server" ButtonType="Generic">
                <asp:Label runat="server" ID="lblAlertsTitle" Style="margin-left: 5" Text="Alert:"
                    Width="40" />
                <controls:DropDownList runat="server" ID="ddlThreshold" Width="120">
                    <asp:ListItem Text="All" Value="0" />
                    <asp:ListItem Text="Last submission" Value="1" />
                    <asp:ListItem Text="Quick answer" Value="2" />
                </controls:DropDownList>
            </controls:XpMenuItem>
            <controls:XpMenuItem runat="server" ButtonType="Separator" Width="10" />
            <controls:XpMenuItem runat="server" ButtonType="Generic">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblDates" runat="server" Text="Date range:" Width="75" />
                        </td>
                        <td>
                            <asp:Panel runat="server" ID="Panel2">
                                <controls:DateTimeRangeSelect ID="dtrsDates" runat="server" AutoPostBack="false" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </controls:XpMenuItem>
            <asp:Panel runat="server" ID="Panel3" CssClass="flex-panel flex-panel-row">
                <asp:Label runat="server" ID="lblInterviewStateTitle" Text="Interview state:"
                    Width="100" />
                <controls:DropDownList runat="server" ID="ddlInterviewState" style="width: 120px;">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Interviewing" Value="4" />
                    <asp:ListItem Text="Openend Review" Value="5" />
                </controls:DropDownList>
            </asp:Panel>
        </LeftToolbarItems>
        <ToolbarItems>
            <controls:XpMenuItem ID="btnSurveys" runat="server" ButtonType="ToggleButton" Text="Surveys..." ImageName="assignment_turned_in">
            </controls:XpMenuItem>
            <controls:XpMenuItem ID="btnPersons" runat="server" ButtonType="ToggleButton" Text="Interviewers..." ImageName="persons">
            </controls:XpMenuItem>
        </ToolbarItems>
        <Columns>
            <controls:GeneralGridColumn Key="InterviewerId" DataFieldName="InterviewerId" Hidden="true"
                HeaderText="<%$CPResource:InterviewerId%>" />
            <controls:GeneralGridColumn Key="InterviewerName" DataFieldName="InterviewerName"
                SearchColumnName="InterviewerName" SearchColumnType="Text" HeaderText="Interviewer"
                Width="100%" />
            <controls:GeneralGridColumn Key="RedCount" DataFieldName="RedCount" SearchColumnName="RedCount"
                SearchColumnType="Number" HeaderText="Red Count" Width="100" />
            <controls:GeneralGridColumn Key="AmberCount" DataFieldName="AmberCount" SearchColumnName="AmberCount"
                SearchColumnType="Number" HeaderText="Warning Count" Width="100" />
            <controls:GeneralGridColumn Key="TotalCount" DataFieldName="TotalCount" SearchColumnName="TotalCount"
                SearchColumnType="Number" HeaderText="Total Count" Width="100" />
        </Columns>
    </controls:Grid>
</asp:Content>
