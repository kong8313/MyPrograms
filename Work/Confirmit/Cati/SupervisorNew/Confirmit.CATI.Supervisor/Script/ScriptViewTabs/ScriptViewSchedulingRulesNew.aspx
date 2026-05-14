<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ScriptViewSchedulingRulesNew.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptViewTabs.ScriptViewSchedulingRulesNew" %>
<%@ Register TagPrefix="Controls" TagName="SchedulingRules" Src="~/Script/Controls/SchedulingRulesNewControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    
	<Controls:SchedulingRules runat="server" />

</asp:Content>
