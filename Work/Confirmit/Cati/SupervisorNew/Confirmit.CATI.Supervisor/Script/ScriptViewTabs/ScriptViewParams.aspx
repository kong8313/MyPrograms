<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ScriptViewParams.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptViewTabs.ScriptViewParams" %>
<%@ Register TagPrefix="Controls" TagName="SchedulingParams" Src="~/Script/Controls/SchedulingParamsControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Controls:SchedulingParams ID="ctrlParams" runat="server" />
</asp:Content>
