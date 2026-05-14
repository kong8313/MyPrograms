<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true" CodeBehind="PriorityGroupsPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.PriorityGroupsPage" %>
<%@ Register TagPrefix="controls" tagName="CallGroupsList" src="Controls/CallGroupsList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <controls:CallGroupsList ID="pgList" runat="server"/>    
</asp:Content>