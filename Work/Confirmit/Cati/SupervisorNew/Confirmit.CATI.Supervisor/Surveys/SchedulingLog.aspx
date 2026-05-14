<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SchedulingLog.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.SchedulingLog" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Register TagPrefix="controls" TagName="Hint" Src="./../Controls/Hint.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        body {
            background-color: white;
        }
    </style>

    <div class="flex-panel flex-panel-column" style="height: 100%;">
        <controls:Hint ID="gridHint" runat="server" />

        <div id="data" runat="server" class="flex-panel flex-panel-column" style="height: 100%; overflow: auto; margin: 20px; font-family: Consolas, monospace; font-size: 13px">
            <controls:ScrollableDiv runat="server" ID="txtlogDiv">
            </controls:ScrollableDiv>
        </div>
    </div>
</asp:Content>
