<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ScriptViewCustom.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptViewTabs.ScriptViewCustom" ValidateRequest="false" %>
<%@ Register TagPrefix="Controls" TagName="CustomScript" Src="~/Script/Controls/CustomScriptPage.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:CustomScript id="ctrlCustomScript" runat="server" />
</asp:Content>
