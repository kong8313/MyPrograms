<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AsyncOperationSpecificParameters.aspx.cs" Inherits="Confirmit.CATI.Supervisor.AsyncOperations.AsyncOperationSpecificParameters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">

    <controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal" HideButtons="True">
        <OKButton Visible="false" />
        <CancelButton />
        <Content>
            <main class="content-panel">
                <div class="content-panel__scroll-pane">
                    <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                        <asp:Repeater ID="repeater" OnItemDataBound="repeater_ItemDataBound" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblName" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblValue" runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                        <tr id="trError" runat="server" visible="False">
                            <td class="boldLabel">Error
                            </td>
                            <td>
                                <controls:TextBox Rows="25" TextMode="MultiLine" Width="100%" Height="140px" ID="tbErrorValue" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
