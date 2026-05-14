<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.master"
    CodeBehind="SwitchCallCenter.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.SwitchCallCenter" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style>
        table, td
        {
            padding: 3px;
        }
    </style>
    <controls:Dialog ID="dialog" runat="server" HideHeader="True">
        <OKButton Visible="False" />
        <SaveButton Visible="True" OnClick="Switch" />
        <Content>
            <table style="width: 99%; border: 0; margin-top: 6px">
                <tr>
                    <td nowrap="nowrap" style="width: 130px">
                        <asp:Label runat="server" Font-Bold="True" Text="<%$CPResource:CallCenterSwitch_User%>"></asp:Label>
                    </td>
                    <td nowrap="nowrap">
                        <asp:Label ID="lblUserLogin" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Font-Bold="True" Text="<%$CPResource:CallCenterSwitch_CallCenter%>"></asp:Label>
                    </td>
                    <td>
                        <controls:DropDownList ID="ddlCallCenter" Width="100%" runat="server">
                            <Items>
                                <asp:ListItem Value="1" Text="Call Center 1"> </asp:ListItem>
                                <asp:ListItem Value="2" Text="Call Center 2"> </asp:ListItem>
                            </Items>
                        </controls:DropDownList>
                    </td>
                </tr>
            </table>
        </Content>
    </controls:Dialog>
</asp:Content>
