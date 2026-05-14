<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
CodeBehind="CustomTimezonesView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.CustomTimezonesView" %>

<%@ Register TagPrefix="Controls" TagName="CustomTimezonesList" Src="~/CustomTimezone/Controls/CustomTimezonesList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div class="tab-content custom-timezones-list">
        <Controls:CustomTimezonesList runat="server" ID="CustomTimezones1" />
    </div>
</asp:Content>
