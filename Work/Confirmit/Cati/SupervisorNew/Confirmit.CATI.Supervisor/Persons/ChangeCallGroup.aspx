<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangeCallGroup.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.ChangeCallGroup" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="Save" />
        <Content>
            <div class="content-panel flex-panel-column">
                <controls:Hint ID="ChangeCallGroupHint" Text="<%$CPResource:ChangeCallGroupOfInterviewersWarning%>" runat="server" />
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblCallGroup" runat="server" Text="<%$CPResource:CallGroup%>" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlCallGroup" runat="server" AutoPostBack="False" Width="100%" />
                        </td>
                    </tr>
                </table>

            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
