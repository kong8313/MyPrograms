<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectTaskChoicePermissions.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.SelectTaskChoicePermissions" %>
<section style="margin-left: 20px;">
<table class="settings-table settings-table--default-columns settings-table--no-min-width" style="width: 50%;">
    <tr>
        <td style="width: 180px;">
            <asp:Label ID="Label1" runat="server" Text="<%$CPResource:TaskChoiceAutomatic%>" />
        </td>
        <td>
            <controls:CheckBox runat="server" ID="cbAutomaticSelection" onchange="MarkAsChanged();" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label2" runat="server" Text="<%$CPResource:TaskChoiceManualSelection%>" />
        </td>
        <td>
            <controls:CheckBox runat="server" ID="cbManualSelection" onchange="MarkAsChanged();" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label3" runat="server" Text="<%$CPResource:TaskChoiceSurveySelection%>" />
        </td>
        <td>
            <controls:CheckBox runat="server" ID="cbSurveySelection" onchange="MarkAsChanged();" />
        </td>
    </tr>
</table>
</section>
<script type="text/javascript">
    function MarkAsChanged() {
        if (window.StateChecker)
            window.StateChecker.MarkAsChanged();
    }
</script>
