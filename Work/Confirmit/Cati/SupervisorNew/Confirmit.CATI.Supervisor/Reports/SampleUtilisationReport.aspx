<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SampleUtilisationReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.SampleUtilisationReport"
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
                        </div>
                    </div>
                    <div class="sidebar__item">
                        <header class="flex-panel flex-panel-row">
                            <h4>
                                <controls:SvgImage runat="server" ImageName="date_range" Title="<%$CPResource:DatesRange%>" />
                                <%=Strings.DatesRange%></h4>
                        </header>
                        <div class="flex-panel flex-panel-column">
                            <controls:DateTimeRangeSelect ID="dtrsDates" runat="server" AutoPostBack="false" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="flex-panel flex-panel-column">
                <controls:ItsSelect ID="itsSelect" runat="server" AutoPostBack="false" />
            </div>
            <div class="report-panel__telerik-area">
                <controls:Hint ID="reportHint" runat="server" Text="<%$CPResource:SampleUtilisationReportHint %>" />
                <%--parent div is used to centre pnlReport one--%>
                <asp:Panel ID="pnlReport" ClientIDMode="Static" runat="server" CssClass="crystalReportsPanel" Style="height: 100%; overflow: hidden">
                    <tlr:ReportViewer ID="reportViewer" runat="server" ShowHistoryButtons="False" Style="border: 1px solid #ccc;" Visible="False" Height="100%" Width="99%" />
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
