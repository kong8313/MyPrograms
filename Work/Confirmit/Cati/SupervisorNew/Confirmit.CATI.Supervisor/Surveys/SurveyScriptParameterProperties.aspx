<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveyScriptParameterProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyScriptParameterProperties" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" runat="server" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--controls-100percent">
                    <tr>
                        <td>
                            <asp:Label ID="lblParamName" Text="<%$CPResource:Name%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbParamName" runat="server" Width="100%" Enabled="False" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ldlDescription" Text="<%$CPResource:Description%>" runat="server" />
                        </td>
                        <td style="text-align: left; width: 80%;">
                            <controls:TextBox ID="tbDescription" runat="server" Width="100%" Enabled="False" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblType" Text="<%$CPResource:ParamType%>" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlType" runat="server" Width="100%" Enabled="False">
                            </controls:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDefaultValue" Text="<%$CPResource:Value%>" runat="server" />
                        </td>
                        <td>
                            <controls:NumericEdit ID="neDefaultValue" runat="server" class="plain_textbox" EnableAjaxViewState="true"
                                Width="100%" HorizontalAlign="left" MaxValue="999999999"
                                MinValue="-999999999" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
