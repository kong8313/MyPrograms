<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SampleStatusSummaryByQuestionReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.SampleStatusSummaryByQuestionReport"
    ClientIDMode="AutoID" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <div class="report-panel">
        <div class="report-panel__toolbar">
            <h2><%=Title %></h2>
            <controls:Button ID="btnBuild" Text="Build report" CssClass="plain_button build-button" runat="server" IsSubmit="true" />
        </div>
        <div class="report-panel__content">
            <asp:UpdatePanel ID="updatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" class="sidebar">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnBuild" />
                </Triggers>
                <ContentTemplate>
                    <div class="sidebar__item">
                        <header class="flex-panel flex-panel-row">
                            <h4>
                                <controls:SvgImage runat="server" ImageName="assignment_turned_in" Title="<%$CPResource:Survey%>" />
                                <%=Strings.Survey%></h4>
                            <controls:ImageButton ID="btnSurvey" Text="Select..." runat="server" IsSubmit="false" ImageName="edit" />
                        </header>
                        <div class="flex-panel flex-panel-column">
                        </div>
                    </div>
                    <div class="sidebar__item">
                        <header class="flex-panel flex-panel-row">
                            <h4>
                                <controls:SvgImage runat="server" ImageName="filter_1" Title="<%$CPResource:Status%>" />
                                <%=Strings.Status%></h4>
                            <controls:ImageButton ID="btnITS" runat="server" IsSubmit="false" Text="Select..." ImageName="edit" />
                        </header>
                        <div class="flex-panel flex-panel-column">
                            <controls:CheckBox ID="cbxITS" runat="server" Text="Status filter" AutoPostBack="True"
                                Checked="false" TextAlign="Right" />
                        </div>
                    </div>
                    <div class="sidebar__item" id="byQuestion" runat="server">
                        <header class="flex-panel flex-panel-row">
                            <h4>
                                <controls:SvgImage runat="server" ImageName="filter_1" Title="<%$CPResource:Question%>" />
                                <%=Strings.Question%></h4>
                        </header>
                        <div class="flex-panel flex-panel-column">
                            <div class="setting-item">
                                <controls:DropDownList ID="ddlByQuestion" runat="server" Width="120px" AutoPostBack="false"></controls:DropDownList>
                            </div>

                        </div>
                    </div>
                    <div class="sidebar__item">
                        <header class="flex-panel flex-panel-row">
                            <h4>
                                <controls:SvgImage runat="server" ImageName="filter_list" Title="Misc" />
                                Misc</h4>
                        </header>
                        <div class="flex-panel flex-panel-column">
                            <div class="setting-item">
                                <div class="setting-item">
                                    <controls:CheckBox ID="cbxShowScheduled" runat="server" Text="<%$CPResource:ShowScheduledCalls%>" AutoPostBack="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="flex-panel flex-panel-column">
                <controls:ItsSelect ID="itsSelect" runat="server" AutoPostBack="false" />
            </div>
            <div class="report-panel__telerik-area">
                <controls:Hint ID="reportHint" runat="server" Text="<%$CPResource:SampleStatusSummaryByQuestionReportHint %>" />
                <%--parent div is used to centre pnlReport one--%>
                <asp:Panel ID="pnlReport" ClientIDMode="Static" runat="server" CssClass="crystalReportsPanel" Style="height: 100%; overflow: hidden">
                    <tlr:ReportViewer ID="reportViewer" runat="server" ShowHistoryButtons="False" Style="border: 1px solid #ccc;" Visible="False" Height="100%" Width="99%" />
                </asp:Panel>
            </div>
        </div>
    </div>    
</asp:Content>
