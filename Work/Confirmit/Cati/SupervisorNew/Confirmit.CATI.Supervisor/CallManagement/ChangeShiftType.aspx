<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangeShiftType.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.ChangeShiftType" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="SaveButtonClick" Text="Change shift type" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--controls-100percent">
                    <tr>
                        <td nowrap>
                            <asp:Label ID="lblShiftType" runat="server" Text="<%$CPResource:ShiftTypeName%>"
                                Font-Bold="true" />
                        </td>
                        <td>
                            <controls:ShiftTypesDropDown ID="ddlShiftType" runat="server" AutoPostBack="false" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>

</asp:Content>
