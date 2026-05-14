<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="CallGroupChangePriority.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.CallGroupChangePriority" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" Text="Change priority" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="hintChangePriority" Text="<%$CPResource:ChangeCallGroupExtendedStatusHint %>"
                    runat="server" />
                <table class="settings-table">
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="lblLimit" runat="server" Text="<%$CPResource:SelectPriority%>" Font-Bold="true" />
                        </td>
                        <td style="width: 100%">
                            <controls:NumericEdit ID="nePriority" runat="server" Width="100%" Nullable="False"
                                ValueText="1" MinValue="0">
                                <Buttons SpinButtonsDisplay="OnRight">
                                </Buttons>
                            </controls:NumericEdit>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
