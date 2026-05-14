<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialerState.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.DialerState" %>
<style>
    .state {

    }

    .state-container {
        background: #E3EFFF url(../images/attention_small.gif) no-repeat 6px 6px;
        padding: 6px 3px 6px 26px;
        border: 1px solid #C1D2EE;
        font-weight: bold;
    }
</style>
<div class="state-container" runat="server" ID="StateContainer">
    <asp:Label runat="server" ID="lblState" CssClass="state" />
</div>