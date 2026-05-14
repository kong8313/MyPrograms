<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialerParameterControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.DialerParameterControl" %>
<div class="settings-table__row">
    <div class="settings-table__label">
        <asp:Label ID="ParameterName" runat="server" />
    </div>
    <div class="settings-table__value">
        <div id="ParameterValue" runat="server" />
        <controls:HelpTextViewer runat="server" ID="ibParameterHelp"></controls:HelpTextViewer>
    </div>
    <div class="settings-table__error-message">
        <asp:Label ID="errorMessage" runat="server" EnableViewState="false" ForeColor="Red" />
    </div>
</div>
