<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToggleSelector.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.ToggleSelector" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Controls" %>

<style type="text/css">
    .toggle-label {
        vertical-align: top;
        padding-left: 10px;
        font-size: 15px;
    }
</style>

<div runat="server" id="divMain">
    <div runat="server" id="divToggle" class="comd-button-toggle">
        <input type="hidden" runat="server" ID="toggleValue" value="False" />
        <input type="checkbox" id="modeChangerToggle" runat="server" class="comd-button-toggle__checkbox" />
        <label
            class="comd-button-toggle__label"
            data-checked-value="On"
            data-unchecked-value="Off">
        </label>
    </div>
    <asp:Label ID="lblToggleText" runat="server" class="toggle-label" />
    <div class="divInline" style="padding-left: 5px;" runat="server" id="divHelp">
        <controls:HelpTextViewer ID="HelpTextEdit" runat="server"  />
    </div>
</div>