<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Hint.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.Hint" %>
<div class="attention <%=CssClass %>">
    <div class="attention_icon">
        <controls:SvgImage runat="server" ImageName="info" ID="attentionIcon" />
    </div>
    <asp:Panel runat="server" ID="pnlWarning" CssClass="attention__content">
        <asp:Label runat="server" ID="lblHint" />
    </asp:Panel>
</div>
