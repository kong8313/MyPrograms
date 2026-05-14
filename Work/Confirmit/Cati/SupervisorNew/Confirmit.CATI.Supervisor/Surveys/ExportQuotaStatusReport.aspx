<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ExportQuotaStatusReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.ExportQuotaStatusReport" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton Text="Generate report" OnClick="btnExportClick" />
        <Content>
            <main class="content-panel">
                <controls:Hint runat="server" ID="hintExportQuotaStatusReportHint" />
                <div id="divExportQuotaStatusReportHelp" class="plain_text" runat="server" />
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
