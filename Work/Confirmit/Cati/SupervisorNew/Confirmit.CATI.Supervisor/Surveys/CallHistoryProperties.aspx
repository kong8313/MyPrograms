<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CallHistoryProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.CallHistoryProperties" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel">
                <div class="flex-panel flex-panel-row flex-panel-row--justify">
                    <asp:Label ID="lblSelectITS" Text="<%$CPResource:SelectITS%>" runat="server" />
                    <controls:DropDownList ID="ddlITS" runat="server" Width="300px" />
                </div>
                <div class="flex-panel flex-panel-row flex-panel-row--justify">
                    <asp:Label ID="lblTelNumber" Text="<%$CPResource:TelNumber%>" runat="server" />
                    <controls:TextBox ID="tbTelephoneNumber" runat="server" Width="300px" />
                </div>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>