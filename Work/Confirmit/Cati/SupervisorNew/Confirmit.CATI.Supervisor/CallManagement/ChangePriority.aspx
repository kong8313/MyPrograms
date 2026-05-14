<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangePriority.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.ChangePriority" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" Text="Change priority" />
        <Content>
            <main class="content-panel">
                <asp:Label ID="lblSelectPriority" Text="Select new call priority:" runat="server" />
                <controls:NumericEdit ID="wnePriority" runat="server" Width="100%" Nullable="False"
                    NullValue="1" MinValue="1" Style="margin-top: 10px;">
                    <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                </controls:NumericEdit>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
