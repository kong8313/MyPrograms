<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ScriptViewShiftsNew.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptViewTabs.ScriptViewShiftsNew" %>
<%@ Register TagPrefix="Controls" TagName="Shifts" Src="~/Script/Controls/ShiftsNewControl.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Shifts runat="server" />
</asp:Content>
