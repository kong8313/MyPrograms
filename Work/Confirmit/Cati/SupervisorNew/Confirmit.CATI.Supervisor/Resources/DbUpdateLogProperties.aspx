<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DbUpdateLogProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DbUpdateLogProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style type="text/css">
        textarea {
            resize: vertical
        }
    </style>
    <main class="content-panel">
        <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--fixed-labels-200px">
            <tr id="trId" runat="server">
                <td><%=Strings.ID%></td>
                <td><%#BvVersionHistoryEntity.Id%></td>
            </tr>

            <tr id="trMajor" runat="server">
                <td><%=Strings.Major%></td>
                <td><%#BvVersionHistoryEntity.Major%></td>
            </tr>

            <tr id="trMinor" runat="server">
                <td><%=Strings.Minor%></td>
                <td><%#BvVersionHistoryEntity.Minor%></td>
            </tr>

            <tr id="trBranchName" runat="server">
                <td><%=Strings.BranchName%></td>
                <td><%#BvVersionHistoryEntity.BranchName%></td>
            </tr>

            <tr id="trScriptNumber" runat="server">
                <td><%=Strings.ScriptNumber%></td>
                <td><%#BvVersionHistoryEntity.ScriptNumber%></td>
            </tr>

            <tr id="trDescription" runat="server">
                <td><%=Strings.Description%></td>
                <td><%#BvVersionHistoryEntity.Description%></td>
            </tr>

            <tr id="trScriptAppliedDate" runat="server">
                <td><%=Strings.ScriptAppliedDate%></td>
                <td><%#BvVersionHistoryEntity.ScriptAppliedDate.ToString("dd.MM.yyyy HH:mm:ss.") + BvVersionHistoryEntity.ScriptAppliedDate.Millisecond%></td>
            </tr>

            <tr id="trDuration" runat="server">
                <td><%=Strings.Duration%></td>
                <td><%#BvVersionHistoryEntity.Duration%></td>
            </tr>

            <tr id="trScriptText" runat="server">
                <td><%=Strings.ScriptText%></td>
                <td>
                    <textarea readonly="readonly" rows="10" style="width: 100%;"><%#BvVersionHistoryEntity.ScriptText%></textarea></td>
            </tr>

            <tr id="trScriptOutput" runat="server">
                <td><%=Strings.ScriptOutput%></td>
                <td>
                    <textarea readonly="readonly" rows="10" style="width: 100%;"><%#BvVersionHistoryEntity.ScriptOutput%></textarea></td>
            </tr>

            <tr id="trIsAppliedDuringDBCreation" runat="server">
                <td><%=Strings.IsAppliedDuringDBCreation%></td>
                <td><%#BvVersionHistoryEntity.IsAppliedDuringDBCreation ? "Yes" : "No"%></td>
            </tr>

            <tr id="trDbUpateUtilityVersion" runat="server">
                <td><%=Strings.DbUpdateUtilityVersion%></td>
                <td><%#BvVersionHistoryEntity.DbUpateUtilityVersion%></td>
            </tr>

            <tr id="trActiveUser" runat="server">
                <td><%=Strings.ActiveUser%></td>
                <td><%#BvVersionHistoryEntity.ActiveUser%></td>
            </tr>
        </table>
    </main>
</asp:Content>
