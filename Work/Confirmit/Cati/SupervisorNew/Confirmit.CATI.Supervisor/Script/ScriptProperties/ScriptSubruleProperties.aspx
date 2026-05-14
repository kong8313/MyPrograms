<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptSubruleProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptSubruleProperties" %>
<%@ Register TagPrefix="Controls" TagName="CodeEditor" Src="~/Script/Controls/CodeEditor.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel" style="overflow: auto">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--dropdown-auto-width">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" Text="Filter enabled" runat="server" />
                        </td>
                        <td>
                            <controls:CheckBox ID="cbFilterEnabled" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblFilter" Text="Filter" runat="server" />
                        </td>
                        <td style="height: 100px; width: 500px" class="textarea-framed">
                            <Controls:CodeEditor runat="server" ID="codeEditorFilter" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblITS" Text="<%$CPResource:ExtendedStatus%>" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlITS" runat="server" columnKey="ItsName" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblShiftType" Text="<%$CPResource:ShiftType%>" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlShiftType" runat="server" columnKey="ShiftTypeName" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDescription" Text="Description" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbxDescription" Rows="6" TextMode="MultiLine" runat="server" columnKey="Description" Width="100%"  CssClass="textarea-framed"/>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
