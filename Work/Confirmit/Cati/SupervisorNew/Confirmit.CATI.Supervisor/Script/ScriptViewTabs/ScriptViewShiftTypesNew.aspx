<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ScriptViewShiftTypesNew.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptViewTabs.ScriptViewShiftTypesNew" %>
<%@ Register TagPrefix="Controls" TagName="ShiftTypes" Src="~/Script/Controls/ShiftTypesNewControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    
    <Controls:ShiftTypes runat="server"/>

</asp:Content>
