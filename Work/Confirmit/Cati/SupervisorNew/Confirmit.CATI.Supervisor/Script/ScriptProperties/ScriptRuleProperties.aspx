<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptRuleProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptRuleProperties" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--fixed-labels-100px settings-table--dropdown-auto-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblDescription" Text="<%$CPResource:Description%>" runat="server" />
                        </td>
                        <td></td>
                        <td>
                            <controls:TextBox ID="tbxDescripton" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblSampleUpdate" Text="<%$CPResource:SampleUpdate%>" AssociatedControlID="cbSampleUpdate" runat="server" />
                        </td>
                        <td>
                            <controls:HelpTextViewer ID="HelpTextViewer1" runat="server" HelpTextId="SampleUpdateHelpViewer" />
                        </td>
                        <td>
                            <controls:CheckBox ID="cbSampleUpdate" runat="server" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
