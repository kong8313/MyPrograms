<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralToolbar.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.GeneralToolbar" %>
<div class="XpMenu clearfix" id="<%=ClientID %>">
    <div id="labelDiv" style="float: left" runat="server">
        <asp:Label ID="leftLabel" runat="server" Text="" CssClass="boldLabel" />
    </div>
    <div id="leftMenuDiv" runat="server" class="leftMenu">
        <controls:XpMenu ID="leftMenu" runat="server" BorderWidth="0">
        </controls:XpMenu>
    </div>
    <div runat="server" ID="rightMenuDiv">
        <controls:XpMenu ID="rightMenu" runat="server" BorderWidth="0">
        </controls:XpMenu>
    </div>
</div>