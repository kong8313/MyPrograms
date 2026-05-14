<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppointmentProperties.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.AppointmentProperties" %>

<table class="settings-table settings-table--default-columns settings-table--no-min-width">
    <tr>
        <td>
            <asp:Label ID="lblTimeToCall" runat="server" Text="<%$CPResource:TimeToCall%>" />
            <span class="settings-table__help">
                <controls:HelpTextViewer runat="server" ID="helpTimeToCall" HelpTextId="EditTimeToCallHelpText"
                                         TitleTextId="TimeToCall"></controls:HelpTextViewer>
            </span>
        </td>
        <td>
            <controls:DateTimeEdit ID="dteAppointmentTime" runat="server" />
        </td>
        <td>
            <controls:CheckBox ID="cbxTimeNow" runat="server" Text="<%$CPResource:SetToNow%>"
                Checked="False" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTimeToExpire" runat="server" Text="<%$CPResource:TimeToExpire%>" />
            <span class="settings-table__help">
                <controls:HelpTextViewer runat="server" ID="helpTimeToExpire" HelpTextId="EditTimeToExpireHelpText"
                                         TitleTextId="TimeToExpire"></controls:HelpTextViewer>
            </span>
        </td>
        <td>
            <controls:DateTimeEdit ID="dteTimeToExpire" runat="server" />
        </td>
        <td>
            <controls:CheckBox ID="cbxTimeToExpire" runat="server" Text="<%$CPResource:SetToNever%>"
                Checked="true" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" Text="<%$CPResource:ContactName%>" />
        </td>
        <td colspan="2">
            <controls:TextBox ID="tbContactName" runat="server">
            </controls:TextBox>
        </td>
    </tr>
</table>

