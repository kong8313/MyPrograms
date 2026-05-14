<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ExportCallHistoryData.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.ExportCallHistoryData" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton Text="Export" OnClick="ExportClick" />
        <Content>
            <controls:Hint runat="server" ID="limitsHint" />
            <main class="content-panel">
                <div class="scrollable-container">
                    <table class="settings-table settings-table--default-columns">
                        <tr>
                            <td colspan="2">
                                <controls:CheckBox ID="cbxIncludeDataFromAllSurveys" runat="server" Text="<%$CPResource:CallExportHistoryIncludeDataFromAllSurveys%>"
                                    Checked="false" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="flex-panel flex-panel-column">
                                    <asp:Label ID="lblTimeToCall" runat="server" Text="<%$CPResource:StartTime%>" />
                                    <controls:DateTimeEdit ID="dteStartTime" runat="server" />
                                </div>
                            </td>
                            <td>
                                <div class="flex-panel flex-panel-column settings-table__label" style="min-width: 0;">
                                    <asp:Label ID="lblTimeToExpire" runat="server" Text="<%$CPResource:EndTime%>" />
                                    <controls:DateTimeEdit ID="dteEndTime" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <controls:CheckBox ID="cbxSelectAll" runat="server" Text="<%$CPResource:AllData%>"
                                    Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <controls:CheckBox ID="cbxIncludeBreaks" runat="server" Text="<%$CPResource:IncludeBreaks%>"
                                    Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <controls:CheckBox ID="cbxIncludeLoginLogout" runat="server" Text="<%$CPResource:IncludeLoginLogout%>"
                                    Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbxIncludeReplicatedVariables" runat="server" Text="<%$CPResource:IncludeReplicatedVariables%>"
                                    Checked="false" onclick="OnIncludeReplicatedVariablesEnabled(this)" />
                            </td>
                            <td>
                                <controls:TextBox ID="ReplicatedVariablesTextBox" runat="server" Width="300px" Style="padding: 0;" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <controls:CheckBox ID="cbxIncludeColumnHeadings" runat="server" Text="<%$CPResource:IncludeColumnHeadings%>"
                                    Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div id="divExportCallHistoryDataHelp" class="plain_text description-container" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </main>
        </Content>
    </controls:Dialog>

    <script type="text/javascript">

        function OnIncludeReplicatedVariablesEnabled(checkBox) {
            var checkboxNode = Y.one(checkBox);
            var row = checkboxNode.ancestor("tr");
            row.all("input[type='text'], select").set("disabled", !checkBox.checked);
        }

        Y.on('load', function () {
            OnIncludeReplicatedVariablesEnabled(document.getElementById("<%=cbxIncludeReplicatedVariables.ClientID%>"));
        });

    </script>
</asp:Content>
