<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DialerFeatureEdit.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DialerFeatureEdit" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">

    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="Save" Text="<%$CPResource:Save%>" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="featureHint" runat="server" />
                <table cellpadding="3" cellspacing="1" class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label Text="<%$CPResource:FeatureName%>" runat="server" />
                        </td>
                        <td><%#HttpUtility.HtmlEncode(FeatureName)%></td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label Text="<%$CPResource:DialerValue%>" runat="server" />
                        </td>
                        <td><%#HttpUtility.HtmlEncode(FeatureDefaultValue)%></td>
                    </tr>

                    <tr runat="server" id="trOverridenValue">
                        <td>
                            <asp:Label Text="<%$CPResource:OverridenValue%>" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlOverridenValue" runat="server" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>

</asp:Content>
