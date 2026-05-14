<%@ Page language="c#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" Codebehind="ScriptsList.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Script.ScriptsList" %>
<%@ Register TagPrefix="Controls" TagName="ScriptsList" Src="~/Script/Controls/ScriptsList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" Runat="Server">
		<Controls:ScriptsList runat=server ID="ScrList" />
</asp:Content>
